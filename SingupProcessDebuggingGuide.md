# Barangay Health Center Sign-Up Process Debugging Guide

This guide provides a step-by-step approach to diagnose and fix issues with the sign-up process in the Barangay Health Center application. The guide addresses the specific issue where sign-up appears to work but data is not being saved to the `AspNetUsers` and `UserDocuments` tables.

## Overview of the Issue

Based on diagnostic tests:
- Database connection is successful (as shown by api/DatabaseTest)
- The system has 1 user but 0 pending users in the database 
- No documents are being saved to UserDocuments table
- Sign-up appears to complete successfully from the user's perspective

## 1. Tracing the Sign-Up Flow

### 1.1. Controller/Page Analysis

First, locate the controller or page model handling sign-up:

```csharp
// For Razor Pages (typically Account/Register.cshtml.cs)
public class RegisterModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RegisterModel> _logger;
    
    // Add enhanced detailed logging
    public async Task<IActionResult> OnPostAsync()
    {
        try {
            _logger.LogInformation("Starting user registration process");
            
            // Detailed logging of each step
            _logger.LogInformation("Creating new user: {Email}", Input.Email);
            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                Status = "Pending", // Ensure Status is set
                CreatedAt = DateTime.UtcNow
                // Other properties...
            };
            
            _logger.LogInformation("Attempting to create user in database");
            var result = await _userManager.CreateAsync(user, Input.Password);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("User created successfully. UserId: {UserId}", user.Id);
                
                // Check if file was uploaded
                if (Input.ResidencyProof != null)
                {
                    _logger.LogInformation("Processing residency proof document");
                    // Process file upload and save to UserDocuments
                    var userDocument = new UserDocument
                    {
                        UserId = user.Id,
                        FileName = Input.ResidencyProof.FileName,
                        ContentType = Input.ResidencyProof.ContentType,
                        // Other properties...
                    };
                    
                    _context.UserDocuments.Add(userDocument);
                    _logger.LogInformation("Added document to context, attempting to save changes");
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Document saved successfully: {DocId}", userDocument.Id);
                }
                else
                {
                    _logger.LogWarning("No residency proof document was provided");
                }
                
                // Further processing...
                return RedirectToPage("RegisterConfirmation");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError("User creation error: {Code} - {Description}", 
                        error.Code, error.Description);
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }
        catch (Exception ex)
        {
            // Log detailed exception information
            _logger.LogError(ex, "Exception during user registration process");
            
            // Additional detailed logging for specific exception types
            if (ex is DbUpdateException dbEx)
            {
                _logger.LogError("Database update error: {Message}", dbEx.InnerException?.Message ?? dbEx.Message);
            }
            else if (ex is SqlException sqlEx)
            {
                _logger.LogError("SQL error number {Number}: {Message}", sqlEx.Number, sqlEx.Message);
            }
            
            ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
            return Page();
        }
    }
}
```

### 1.2. Adding Debug Controller

Create a debug controller to query and test database operations directly:

```csharp
[ApiController]
[Route("api/[controller]")]
public class RegistrationDebugController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RegistrationDebugController> _logger;
    
    public RegistrationDebugController(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        ILogger<RegistrationDebugController> logger)
    {
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }
    
    [HttpGet("test-create-user")]
    public async Task<IActionResult> TestCreateUser()
    {
        try
        {
            // Create test user
            var testUser = new ApplicationUser
            {
                UserName = $"test_{Guid.NewGuid()}@example.com",
                Email = $"test_{Guid.NewGuid()}@example.com",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };
            
            var result = await _userManager.CreateAsync(testUser, "Test@123");
            
            if (result.Succeeded)
            {
                // Create test document
                var testDoc = new UserDocument
                {
                    UserId = testUser.Id,
                    FileName = "test-document.pdf",
                    ContentType = "application/pdf",
                    CreatedAt = DateTime.UtcNow
                };
                
                _context.UserDocuments.Add(testDoc);
                await _context.SaveChangesAsync();
                
                return Ok(new { 
                    success = true, 
                    userId = testUser.Id,
                    documentId = testDoc.Id,
                    message = "Test user and document created successfully" 
                });
            }
            
            return BadRequest(new { 
                success = false, 
                errors = result.Errors.Select(e => new { e.Code, e.Description }) 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in test user creation");
            return StatusCode(500, new { 
                success = false, 
                error = ex.Message,
                innerError = ex.InnerException?.Message,
                stackTrace = ex.StackTrace
            });
        }
    }
    
    [HttpGet("connection-info")]
    public IActionResult GetConnectionInfo()
    {
        try
        {
            var connection = _context.Database.GetDbConnection();
            bool canConnect = _context.Database.CanConnect();
            
            return Ok(new {
                canConnect,
                databaseName = connection.Database,
                dataSource = connection.DataSource,
                connectionString = HidePasswordInConnectionString(connection.ConnectionString)
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    private string HidePasswordInConnectionString(string connectionString)
    {
        // Hide password in connection string for security
        if (string.IsNullOrEmpty(connectionString)) return null;
        
        var regex = new Regex("Password=([^;]*)");
        return regex.Replace(connectionString, "Password=*****");
    }
}
```

## 2. Checking HTML Form Configuration

Inspect the registration form to ensure it's correctly configured:

```html
<!-- Check the form attributes -->
<form method="post" enctype="multipart/form-data" id="registrationForm">
    <div class="form-group">
        <!-- Inspect input fields to ensure they match model properties -->
        <label asp-for="Input.Email">Email</label>
        <input asp-for="Input.Email" class="form-control" />
        <span asp-validation-for="Input.Email" class="text-danger"></span>
    </div>
    
    <!-- Check file upload field -->
    <div class="form-group">
        <label asp-for="Input.ResidencyProof">Residency Proof</label>
        <input type="file" asp-for="Input.ResidencyProof" class="form-control" />
        <span asp-validation-for="Input.ResidencyProof" class="text-danger"></span>
    </div>
    
    <!-- Check submit button -->
    <button type="submit" class="btn btn-primary">Register Account</button>
</form>
```

### 2.1. Debugging Form Submission with JavaScript

Add the following script to monitor form submission:

```javascript
// Add this to register.html or .cshtml
document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('registrationForm');
    
    if (form) {
        console.log('Registration form found, adding submission monitor');
        
        form.addEventListener('submit', function(event) {
            console.log('Form submission initiated');
            
            // Optional: stop the submission to inspect
            // event.preventDefault();
            
            // Log form data
            const formData = new FormData(form);
            console.log('Form data entries:');
            for (let [key, value] of formData.entries()) {
                if (key.toLowerCase().includes('password')) {
                    console.log(`${key}: (value hidden for security)`);
                } else if (value instanceof File) {
                    console.log(`${key}: File - ${value.name} (${value.size} bytes)`);
                } else {
                    console.log(`${key}: ${value}`);
                }
            }
            
            // Continue with submission (remove preventDefault above)
            console.log('Continuing with form submission');
        });
    } else {
        console.error('Registration form not found in DOM');
    }
});
```

## 3. Checking Database Configuration

Verify the database configuration in the application:

```csharp
// In Program.cs or Startup.cs
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Check connection string configuration
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        Console.WriteLine($"Using connection string: {connectionString}");
        
        // Add detailed logging for database operations
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options
                .UseSqlServer(connectionString)
                .EnableSensitiveDataLogging() // For development only
                .LogTo(Console.WriteLine, LogLevel.Information));
        
        // Configure identity with explicit settings
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
            options.SignIn.RequireConfirmedAccount = false; // Adjust based on your requirements
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
        
        // ...
    }
}
```

## 4. Common Issues and Solutions

### 4.1. Transaction Issues

**Issue**: Transaction not being completed

**Solution**: Ensure transactions are explicitly managed when needed:

```csharp
// In Register.cshtml.cs or RegisterController.cs
using (var transaction = await _context.Database.BeginTransactionAsync())
{
    try
    {
        // Create user with UserManager
        var result = await _userManager.CreateAsync(user, Input.Password);
        
        if (result.Succeeded)
        {
            // Process document upload
            // ...
            
            await _context.SaveChangesAsync();
            
            // Commit transaction only if everything succeeds
            await transaction.CommitAsync();
            _logger.LogInformation("Transaction committed successfully");
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error during registration, rolling back transaction");
        await transaction.RollbackAsync();
        throw;
    }
}
```

### 4.2. Status Field Issues

**Issue**: Status field not set or improperly cased

**Solution**: Explicitly set status field for new users:

```csharp
var user = new ApplicationUser
{
    // Other properties...
    Status = "Pending" // Ensure the exact casing matches filter expectations
};
```

### 4.3. Silent Validation Failures

**Issue**: Model validation failing silently

**Solution**: Explicitly check and log model state:

```csharp
public async Task<IActionResult> OnPostAsync()
{
    _logger.LogInformation("Registration form posted");
    
    if (!ModelState.IsValid)
    {
        _logger.LogWarning("Model state is invalid:");
        foreach (var key in ModelState.Keys)
        {
            var errors = ModelState[key].Errors;
            foreach (var error in errors)
            {
                _logger.LogWarning($"  - {key}: {error.ErrorMessage}");
            }
        }
        return Page();
    }
    
    // Proceed with registration...
}
```

### 4.4. File Upload Issues

**Issue**: File upload not being processed correctly

**Solution**: Explicitly check file content and save correctly:

```csharp
if (Input.ResidencyProof != null && Input.ResidencyProof.Length > 0)
{
    _logger.LogInformation($"Processing file: {Input.ResidencyProof.FileName}, Size: {Input.ResidencyProof.Length} bytes");
    
    // Save file to disk if needed
    var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
    Directory.CreateDirectory(uploadsFolder); // Ensure folder exists
    
    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(Input.ResidencyProof.FileName)}";
    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
    
    using (var fileStream = new FileStream(filePath, FileMode.Create))
    {
        await Input.ResidencyProof.CopyToAsync(fileStream);
    }
    
    // Save reference in database
    var userDocument = new UserDocument
    {
        UserId = user.Id,
        FileName = Input.ResidencyProof.FileName,
        FilePath = $"/uploads/{uniqueFileName}",
        ContentType = Input.ResidencyProof.ContentType,
        FileSize = Input.ResidencyProof.Length,
        UploadedAt = DateTime.UtcNow
    };
    
    _context.UserDocuments.Add(userDocument);
    await _context.SaveChangesAsync();
}
```

## 5. Testing Process

1. Enable detailed logging in `appsettings.Development.json`:
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning",
         "Microsoft.EntityFrameworkCore.Database.Command": "Information"
       }
     }
   }
   ```

2. Run the test route: `/api/RegistrationDebug/test-create-user`

3. Check the logs for errors during the registration process

4. Verify the database after a test registration:
   ```sql
   -- Run this in SQL Server Management Studio
   SELECT TOP 10 * FROM AspNetUsers ORDER BY CreatedAt DESC;
   SELECT TOP 10 * FROM UserDocuments ORDER BY CreatedAt DESC;
   ```

## 6. Additional Debugging Tools

### Database Migration Status Check

Run this in the Package Manager Console to verify migrations:

```
Add-Migration CheckMigrationStatus
```

If it says "no migrations," the database is up to date.

### Direct Database Verification

Use this SQL query to check the database schema:

```sql
IF OBJECT_ID('AspNetUsers', 'U') IS NOT NULL
BEGIN
    PRINT 'AspNetUsers table exists'
    IF COL_LENGTH('AspNetUsers', 'Status') IS NOT NULL
    BEGIN
        PRINT 'Status column exists in AspNetUsers'
        SELECT DISTINCT Status FROM AspNetUsers;
    END
    ELSE
    BEGIN
        PRINT 'Status column does NOT exist in AspNetUsers'
    END
END
ELSE
BEGIN
    PRINT 'AspNetUsers table does NOT exist'
END

IF OBJECT_ID('UserDocuments', 'U') IS NOT NULL
BEGIN
    PRINT 'UserDocuments table exists'
END
ELSE
BEGIN
    PRINT 'UserDocuments table does NOT exist'
END
```

## Conclusion

By methodically analyzing and testing each component of the sign-up process, you should be able to identify and fix the issues preventing user data from being saved to the database. The most common culprits are:

1. Database connection or transaction issues
2. Silent form validation failures
3. Incorrect model binding or file handling
4. Status field case sensitivity issues
5. Exceptions being caught but not properly logged

After implementing these debugging steps and fixes, test the sign-up process thoroughly to ensure users are successfully registered and their data appears in the User Management page. 