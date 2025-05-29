using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using Barangay;
using Barangay.Middleware;
using Barangay.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// ✅ Configure Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add DatabaseDebugService
builder.Services.AddScoped<IDatabaseDebugService, DatabaseDebugService>();

// ✅ Configure Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuthenticatedUser", policy => policy.RequireAuthenticatedUser());

    // Role-based policies
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
    options.AddPolicy("RequireDoctorRole", policy => policy.RequireRole("Doctor"));
    options.AddPolicy("RequireNurseRole", policy => policy.RequireRole("Nurse"));
    options.AddPolicy("RequirePatientRole", policy => policy.RequireRole("Patient"));
    
    // Admin access policies
    options.AddPolicy("AccessAdminDashboard", policy => 
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") || 
            context.User.IsInRole("System Administrator")));

    options.AddPolicy("AdminBypassPolicy", policy => 
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") || 
            context.User.IsInRole("System Administrator")));
    
    // Staff-specific policies that exclude admin
    options.AddPolicy("StaffOnlyPolicy", policy =>
        policy.RequireRole("Doctor", "Nurse", "Staff"));

    // Add permission-based policies
    options.AddPolicy("AccessDoctorDashboard", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") || 
            context.User.IsInRole("System Administrator") ||
            context.User.HasClaim(c => c.Type == "Permission" && c.Value == "Access Doctor Dashboard")));
    
    options.AddPolicy("AccessNurseDashboard", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") || 
            context.User.IsInRole("System Administrator") ||
            context.User.HasClaim(c => c.Type == "Permission" && c.Value == "Access Nurse Dashboard")));
    
    options.AddPolicy("AccessAdminDashboard", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") || 
            context.User.IsInRole("System Administrator") ||
            context.User.HasClaim(c => c.Type == "Permission" && c.Value == "Access Admin Dashboard")));
    
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
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/SignUp");
    options.Conventions.AllowAnonymousToPage("/Account/ResetUserPassword");
    options.Conventions.AllowAnonymousToPage("/Account/AccessDenied");
    options.Conventions.AllowAnonymousToPage("/Account/WaitingForApproval");

    // Admin pages - use AccessAdminDashboard policy
    options.Conventions.AuthorizeFolder("/Admin", "AccessAdminDashboard");
    options.Conventions.AuthorizeFolder("/AdminStaff", "AccessAdminDashboard");
    options.Conventions.AuthorizeFolder("/Account/UserManagement", "AccessAdminDashboard");
    
    // Role-specific pages
    options.Conventions.AuthorizeFolder("/User", "RequireUserRole");
    options.Conventions.AuthorizeFolder("/Doctor", "RequireDoctorRole");
    options.Conventions.AuthorizeFolder("/Nurse", "AccessNurseDashboard");
});

// ✅ Email Sender
builder.Services.AddSingleton<Barangay.Services.IEmailSender>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var smtpHost = config["Smtp:Host"] ?? "smtp.gmail.com";
    var smtpPort = int.TryParse(config["Smtp:Port"], out var port) ? port : 587;
    var smtpUser = config["Smtp:User"] ?? "your-email@gmail.com";
    var smtpPass = config["Smtp:Pass"] ?? "your-app-password";

    return new Barangay.Services.EmailSender(smtpHost, smtpPort, smtpUser, smtpPass);
});

// ✅ Forwarded Headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// ✅ Logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

// ✅ Encryption Service
builder.Services.AddScoped<IEncryptionService, EncryptionService>();

// ✅ Email Service
builder.Services.AddScoped<IEmailService, EmailService>();

// Add services to the container
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

// ✅ Register DateService
builder.Services.AddSingleton<IDateService, DateService>();

// Register custom services
builder.Services.AddScoped<INotificationService, NotificationService>();

// Register User Verification Service
builder.Services.AddScoped<IUserVerificationService, UserVerificationService>();

// Register RBAC Permission Service
builder.Services.AddScoped<PermissionService>();

// Register Permission Fix Service
builder.Services.AddScoped<PermissionFixService>();

var app = builder.Build();

// ✅ Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        // Seed roles first
        await RoleSeeder.SeedRoles(services);
        logger.LogInformation("Roles seeded successfully");

        // Then seed admin user with both Admin and System Administrator roles
        await SeedUserWithRoleAsync(userManager, context, "admin@example.com", "System Administrator", "Admin@123", "System Administrator");
        var adminUser = await userManager.FindByEmailAsync("admin@example.com");
        if (adminUser != null)
        {
            // Ensure admin user has both roles
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            logger.LogInformation("Admin user seeded successfully with both roles");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the database");
    }
}

// ✅ Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        const int durationInSeconds = 60 * 60 * 24; // 24 hours
        ctx.Context.Response.Headers["Cache-Control"] = 
            "public,max-age=" + durationInSeconds;
    }
});
app.UseRouting();

// Add session middleware before authentication
app.UseSession();

app.UseAuthentication(); // Important!
app.UseAuthorization();

// Add the VerifiedUserMiddleware after authentication and authorization
app.UseVerifiedUserMiddleware();

// Run database migrations and ensure UserDocuments table structure
try
{
    await app.MigrateDatabaseAsync();
    
    // Initialize and seed the Permissions table
    var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
    var permissionsLogger = loggerFactory.CreateLogger("PermissionsSeeder");
    await DbInitializer.Initialize(app.Services, permissionsLogger);
    
    // Fix permissions
    using (var scope = app.Services.CreateScope())
    {
        var permissionFixService = scope.ServiceProvider.GetRequiredService<PermissionFixService>();
        await permissionFixService.FixPermissionsAsync();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error during database migration: {ex.Message}");
}

// Get the connection string
var configuration = app.Services.GetRequiredService<IConfiguration>();
var connectionString = configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrEmpty(connectionString))
{
    try
    {
        // Fix the database using the new comprehensive DatabaseMigration class
        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        var migrationLogger = loggerFactory.CreateLogger<DatabaseMigration>();
        var databaseMigration = new DatabaseMigration(configuration, migrationLogger);
        await databaseMigration.MigrateAsync();
        app.Logger.LogInformation("Comprehensive database migration completed successfully.");

        // Also run the existing database fix for backward compatibility
        await Barangay.DatabaseFix.FixDatabase(connectionString);
        app.Logger.LogInformation("Legacy database fix completed successfully.");

        // Check database tables
        await Barangay.CheckDatabase.CheckTables(connectionString);
        app.Logger.LogInformation("Database check completed.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error fixing database.");
    }
}

// Reset admin password during startup
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            await Barangay.Tools.ResetAdminPassword.ResetPassword(app.Services);
            Console.WriteLine("Admin password has been reset to: Admin@123");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resetting admin password: {ex.Message}");
        }
    }
}

app.MapControllers();
app.MapRazorPages();
app.Run();


// ✅ Helper: Seed user and optionally patient
async Task SeedUserWithRoleAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context, string email, string fullName, string password, string role, bool isPatient = false)
{
    try
    {
        // Exit early if email is null or empty
        if (string.IsNullOrEmpty(email))
        {
            Console.WriteLine($"❌ Cannot seed user with empty email for role {role}.");
            return;
        }

        // First, ensure no NULL values are in the database
        await context.Database.ExecuteSqlRawAsync(@"
            UPDATE [AspNetUsers] 
            SET 
                [UserName] = COALESCE([UserName], [Email]),
                [NormalizedUserName] = COALESCE([NormalizedUserName], UPPER([Email])),
                [Email] = COALESCE([Email], [UserName]),
                [NormalizedEmail] = COALESCE([NormalizedEmail], UPPER([Email]))
            WHERE 
                [Email] IS NOT NULL OR [UserName] IS NOT NULL;");

        ApplicationUser? user = null;
        try
        {
            // Try to find the user using UserManager instead of context.Users
            user = await userManager.FindByEmailAsync(email);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error finding user by email: {ex.Message}");
            // Continue with creation, don't rethrow
        }

        // Set verified status for Admin users automatically
        bool isAdmin = role.Equals("Admin", StringComparison.OrdinalIgnoreCase);

        if (user == null)
        {
            var tempUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                FirstName = fullName.Split(' ').FirstOrDefault() ?? "",
                MiddleName = "",
                LastName = fullName.Split(' ').LastOrDefault() ?? "",
                Suffix = "",
                PhilHealthId = "",
                Address = "",
                Gender = "",
                ProfilePicture = "",
                EncryptedFullName = "",
                EncryptedStatus = "",
                WorkingHours = "",
                Specialization = "",
                JoinDate = DateTime.Now,
                LastActive = DateTime.Now,
                PhoneNumber = "",
                EmailConfirmed = true,
                // Set verified status and active for Admin users
                Status = isAdmin ? "Verified" : "Pending",
                IsActive = isAdmin
            };

            try
            {
                var result = await userManager.CreateAsync(tempUser, password);
                if (result.Succeeded)
                {
                    user = tempUser;
                    Console.WriteLine($"✅ User created: {email}");
                }
                else
                {
                    Console.WriteLine($"❌ Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in user creation: {ex.Message}");
                return;
            }
        }
        else
        {
            // If user exists and is admin, ensure it's verified and active
            if (isAdmin && (user.Status != "Verified" || !user.IsActive))
            {
                user.Status = "Verified";
                user.IsActive = true;
                await userManager.UpdateAsync(user);
                Console.WriteLine($"✅ Admin user status updated to Verified and Active: {email}");
            }
            Console.WriteLine($"✓ User already exists: {email}");
        }

        // Check and assign role
        try
        {
            if (!string.IsNullOrEmpty(role) && !(await userManager.IsInRoleAsync(user, role)))
            {
                var roleResult = await userManager.AddToRoleAsync(user, role);
                if (roleResult.Succeeded)
                {
                    Console.WriteLine($"✅ Role assigned: {role}");
                }
                else
                {
                    Console.WriteLine($"❌ Failed to assign role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in role assignment: {ex.Message}");
        }

        // Create patient record if needed
        if (isPatient)
        {
            try
            {
                var existingPatient = await context.Patients.FindAsync(user.Id);
                if (existingPatient == null)
                {
                    var patient = new Patient
                    {
                        UserId = user.Id,
                        User = user,
                        BirthDate = DateTime.Now.AddYears(-25),
                        Gender = "Prefer not to say",
                        BloodType = "Unknown",
                        Allergies = "",
                        MedicalHistory = ""
                    };

                    context.Patients.Add(patient);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"✅ Patient record created for {email}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creating patient record: {ex.Message}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error in SeedUserWithRoleAsync: {ex.Message}");
        Console.WriteLine($"Exception details: {ex}");
    }
}
