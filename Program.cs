using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using Barangay;
using Barangay.Middleware;
using Barangay.Authorization;
using Barangay.Helpers;
using Barangay.Migrations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net.Mail;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.IIS;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Configure logging based on environment
if (builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();
    builder.Logging.SetMinimumLevel(LogLevel.Information);
    
    // Configure EF Core logging for development
    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information);
    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Warning);
}
else
{
    // Production logging configuration
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Error);
}

// Configure Kestrel to handle large uploads
builder.WebHost.ConfigureKestrel(options =>
{
    // Increase request timeout to 5 minutes
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5);
    // Increase max request body size to 10MB (for file uploads)
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB
});

// Configure form options for larger file uploads
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
    options.ValueLengthLimit = 10 * 1024 * 1024; // 10 MB
    options.MultipartHeadersLengthLimit = 10 * 1024 * 1024; // 10 MB
});

// Add data protection
builder.Services.AddDataProtection()
    .SetApplicationName("Barangay")
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys")));

// Add in-memory cache for app-wide caching (e.g., permission cache)
builder.Services.AddMemoryCache();

// Add session with a longer timeout
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// ✅ Configure DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
    
    // Enable sensitive data logging only in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
    
    // Ensure required SET options are always enabled for the connection and every command
    options.AddInterceptors(new QuotedIdentifierConnectionInterceptor(),
                           new SetSessionOptionsCommandInterceptor());
});

// ✅ Configure EncryptedDbContext for custom services
builder.Services.AddDbContext<EncryptedDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
    
    // Enable sensitive data logging only in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
    
    // Ensure required SET options are always enabled for the connection and every command
    options.AddInterceptors(new QuotedIdentifierConnectionInterceptor(),
                           new SetSessionOptionsCommandInterceptor());
});

// ✅ Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ✅ Configure Identity Application Cookie (avoid dual-cookie mismatch)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;

    // Return proper HTTP status codes for AJAX/fetch requests instead of redirects
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            // For API/AJAX requests, send 401 to allow frontend to handle
            if (IsAjaxOrApiRequest(context.Request))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = context =>
        {
            // For API/AJAX requests, send 403 to allow frontend to handle
            if (IsAjaxOrApiRequest(context.Request))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        }
    };
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add DatabaseDebugService
builder.Services.AddScoped<IDatabaseDebugService, DatabaseDebugService>();

// Add DatabaseSchemaFixService
builder.Services.AddScoped<IDatabaseSchemaFixService, DatabaseSchemaFixService>();

// ✅ Configure Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuthenticatedUser", policy => policy.RequireAuthenticatedUser());

    // Role-based policies
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    // Backward-compatibility for existing attributes
    options.AddPolicy("AccessAdminDashboard", policy => policy.RequireRole("Admin", "Admin Staff"));
    options.AddPolicy("AccessDashboard", policy => policy.RequireRole("Admin", "Admin Staff"));
    // Allow Admins to access Doctor/Nurse protected areas as supervisors
    options.AddPolicy("RequireDoctorRole", policy => policy.RequireRole("Doctor", "Admin"));
    options.AddPolicy("RequireNurseRole", policy => policy.RequireRole("Nurse", "Head Nurse", "Admin"));
    options.AddPolicy("RequireAdminStaffRole", policy => policy.RequireRole("Admin Staff"));
    options.AddPolicy("RequireSystemAdministratorRole", policy => policy.RequireRole("System Administrator"));

    // Combined role policies
    options.AddPolicy("AdminOrSystemAdmin", policy =>
        policy.RequireRole("Admin", "System Administrator"));

    // Permission-based policies
    options.AddPolicy("AccessUserDashboard", policy =>
        policy.AddRequirements(new Barangay.Authorization.PermissionRequirement("Access Dashboard")));

    // Doctor policies (simplified + legacy dashboard policy kept for compatibility)
    options.AddPolicy("AccessDoctorDashboard", policy =>
        policy.AddRequirements(new Barangay.Authorization.PermissionRequirement("Access Doctor Dashboard")));
    options.AddPolicy("DoctorDashboard", policy =>
        policy.AddRequirements(new PermissionRequirement("DoctorDashboard")));
    options.AddPolicy("Consultation", policy =>
        policy.AddRequirements(new PermissionRequirement("Consultation")));
    options.AddPolicy("DoctorPatientList", policy =>
        policy.AddRequirements(new PermissionRequirement("PatientList")));
    options.AddPolicy("DoctorReports", policy =>
        policy.AddRequirements(new PermissionRequirement("Reports")));

    // Simplified Nurse permissions
    options.AddPolicy("NurseDashboard", policy =>
        policy.AddRequirements(new PermissionRequirement("NurseDashboard")));
    options.AddPolicy("VitalSigns", policy =>
        policy.AddRequirements(new PermissionRequirement("VitalSigns")));
    options.AddPolicy("PatientList", policy =>
        policy.AddRequirements(new PermissionRequirement("PatientList")));
    options.AddPolicy("Appointments", policy =>
        policy.AddRequirements(new PermissionRequirement("Appointments")));
    options.AddPolicy("PatientQueue", policy =>
        policy.AddRequirements(new PermissionRequirement("PatientQueue")));

    // Policy to manage permissions
    options.AddPolicy("ManagePermissions", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") || 
            context.User.IsInRole("System Administrator") ||
            context.User.HasClaim(c => c.Type == "Permission" && c.Value == "ManagePermissions")));
});

// Register the permission handler
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();



// ✅ Require Authorization on Razor Pages
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToPage("/Privacy");
    options.Conventions.AllowAnonymousToPage("/Terms");
    options.Conventions.AllowAnonymousToPage("/DataPrivacy");
    options.Conventions.AllowAnonymousToPage("/About");
    options.Conventions.AllowAnonymousToPage("/Contact");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/Logout");
    options.Conventions.AllowAnonymousToPage("/Account/Register");
    options.Conventions.AllowAnonymousToPage("/Account/AccessDenied");
    options.Conventions.AllowAnonymousToFolder("/Account");

    // Role-specific authorization
    options.Conventions.AuthorizeFolder("/Admin", "RequireAdminRole");
    // Gate doctor area by role and enforce per-page simplified policies
    options.Conventions.AuthorizeFolder("/Doctor", "RequireDoctorRole");
    options.Conventions.AuthorizeFolder("/Nurse", "RequireNurseRole");
});

// ✅ Email Sender
builder.Services.AddSingleton<Barangay.Services.IEmailSender>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var smtpHost = config["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
    var smtpPort = int.TryParse(config["EmailSettings:SmtpPort"], out var port) ? port : 587;
    var smtpUser = config["EmailSettings:SmtpUsername"];
    var smtpPassword = config["EmailSettings:SmtpPassword"];

    if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPassword))
    {
        // Fallback or logging for missing credentials
        // In a real app, you might use a null pattern object or log a warning.
        // For now, we'll return a dummy sender that does nothing.
        return new Barangay.Services.EmailSender(null, 0, null, null); // This will likely fail if used, which is intended if not configured.
    }

    return new Barangay.Services.EmailSender(smtpHost, smtpPort, smtpUser, smtpPassword);
});

// ✅ Add other services
builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
builder.Services.AddScoped<IConsultationPdfService, ConsultationPdfService>();
builder.Services.AddScoped<IPrescriptionPdfService, PrescriptionPdfService>();
builder.Services.AddScoped<IPasswordHistoryService, PasswordHistoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IOTPService, OTPService>();
builder.Services.AddScoped<IAppointmentReminderService, AppointmentReminderService>();
builder.Services.AddScoped<IImmunizationReminderService, ImmunizationReminderService>();

// Register encryption services
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IDataEncryptionService, DataEncryptionService>();
builder.Services.AddScoped<IEncryptedDataService, EncryptedDataService>();
builder.Services.AddScoped<EncryptExistingDataService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Register the custom model binder provider
builder.Services.AddControllersWithViews(options =>
{
        options.ModelBinderProviders.Insert(0, new TrimModelBinderProvider());
});

// Add hosted services
builder.Services.AddHostedService<SessionCleanupService>();
builder.Services.AddHostedService<AppointmentReminderBackgroundService>();

// Register Notification Service
builder.Services.AddScoped<INotificationService, NotificationService>();

// Register User Verification Service
builder.Services.AddScoped<IUserVerificationService, UserVerificationService>();

// Register RBAC Permission Service
builder.Services.AddScoped<IPermissionService, PermissionService>();

// Register Database Seeder
builder.Services.AddScoped<DatabaseSeeder>();

// Register Permission Fix Service
builder.Services.AddScoped<PermissionFixService>();

// Register UserNumber Fix Service
// builder.Services.AddHostedService<UserNumberFixService>();

var app = builder.Build();

// Check for pending migrations in Development environment
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var pendingMigrations = dbContext.Database.GetPendingMigrations();
        if (pendingMigrations.Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"""
            ****************************************************************************************
            WARNING: There are {pendingMigrations.Count()} pending database migrations.
            Run 'dotnet ef database update' to apply them.
            Pending migrations: {string.Join(", ", pendingMigrations)}
            ****************************************************************************************
            """);
            Console.ResetColor();
        }
    }
}

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var seeder = services.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAllAsync();
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Database seeding completed successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database seeding.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Add the custom middleware
app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseRouting();

// Add session middleware
app.UseSession();

app.UseAuthentication();
// Ensure verified user checks run for authenticated requests (returns 403 JSON for AJAX)
app.UseVerifiedUserMiddleware();
app.UseMiddleware<DataEncryptionMiddleware>();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();

// Helper to detect AJAX/fetch or API requests
static bool IsAjaxOrApiRequest(HttpRequest request)
{
    var accept = request.Headers["Accept"].ToString();
    var requestedWith = request.Headers["X-Requested-With"].ToString();
    var secFetchMode = request.Headers["Sec-Fetch-Mode"].ToString();

    return (!string.IsNullOrEmpty(requestedWith) && requestedWith == "XMLHttpRequest")
           || (!string.IsNullOrEmpty(accept) && (accept.Contains("application/json") || accept.Contains("text/json")))
           || (!string.IsNullOrEmpty(secFetchMode) && (secFetchMode == "cors" || secFetchMode == "same-origin"));
}