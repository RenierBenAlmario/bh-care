# Token-Based URL Protection System

This document explains how to use the comprehensive token-based URL protection system implemented for the BHCARE application.

## Overview

The token system ensures that all sensitive pages (User, Nurse, Doctor, Admin) are protected with secure tokens that:
- Hide direct URLs from unauthorized access
- Provide time-limited access (default 24 hours)
- Track usage and prevent token reuse
- Log all access attempts for security auditing

## Components

### 1. UrlToken Model
- Stores token information in the database
- Tracks resource type, resource ID, expiration, and usage
- Includes IP address and user agent logging

### 2. TokenService
- `GenerateTokenAsync()` - Creates secure tokens
- `ValidateTokenAsync()` - Validates token authenticity
- `GetResourceIdAsync()` - Extracts resource ID from token
- `MarkTokenAsUsedAsync()` - Prevents token reuse
- `CleanupExpiredTokensAsync()` - Removes expired tokens

### 3. TokenValidationMiddleware
- Automatically validates tokens for protected routes
- Handles unauthorized access gracefully
- Supports both web and API requests
- Logs all access attempts

### 4. ProtectedUrlService
- Generates protected URLs with embedded tokens
- Supports both Razor Pages and Controllers
- Handles different resource types (User, Nurse, Doctor, Admin)

## Usage Examples

### 1. Generating Protected URLs in Controllers

```csharp
[HttpGet]
public async Task<IActionResult> GetProtectedUrl()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (int.TryParse(userId, out var resourceId))
    {
        var resourceType = DetermineResourceType(User);
        var protectedUrl = await _protectedUrlService.GenerateProtectedPageUrlAsync(
            resourceType, resourceId, "/Doctor/PatientList");
        
        return Json(new { protectedUrl });
    }
    return BadRequest("Invalid user");
}
```

### 2. Using Protected URLs in Views

```html
@inject IProtectedUrlService ProtectedUrlService

@{
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    var resourceType = User.IsInRole("Doctor") ? "Doctor" : "User";
    var protectedUrl = await ProtectedUrlService.GenerateProtectedPageUrlAsync(
        resourceType, userId, "/Doctor/PatientList");
}

<a href="@protectedUrl" class="btn btn-primary">
    Access Patient List
</a>
```

### 3. Using Extension Methods

```csharp
// In a controller
var protectedUrl = await Url.ProtectedActionAsync("PatientList", "Doctor");

// In a Razor Page
var protectedUrl = await Url.ProtectedPageAsync("/Doctor/PatientList");
```

### 4. Navigation Component

```html
@await Component.InvokeAsync("ProtectedNavigation")
```

This automatically generates protected URLs for all navigation links based on user roles.

## Protected Routes

The following routes are automatically protected by the middleware:

- `/User/*` - All user pages
- `/Nurse/*` - All nurse pages  
- `/Doctor/*` - All doctor pages
- `/Admin/*` - All admin pages

## Token Parameters

### Query String
```
https://localhost:5003/Doctor/PatientList?token=abc123def456
```

### Header
```
X-Access-Token: abc123def456
```

### Cookie
```
access_token=abc123def456
```

## Security Features

### 1. Token Expiration
- Default: 24 hours
- Configurable per token
- Automatic cleanup of expired tokens

### 2. Single Use
- Tokens are marked as used after first access
- Prevents token replay attacks
- Tracks usage timestamp and IP address

### 3. Role-Based Access
- Tokens are validated against user roles
- Admin tokens can access all areas
- Doctor tokens can access doctor and user areas
- Nurse tokens can access nurse and user areas
- User tokens can only access user areas

### 4. Audit Logging
- All token generation logged
- All access attempts logged
- Failed access attempts logged
- IP address and user agent tracking

## Configuration

### Program.cs Registration
```csharp
// Register services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IProtectedUrlService, ProtectedUrlService>();
builder.Services.AddHostedService<TokenCleanupService>();

// Add middleware
app.UseMiddleware<TokenValidationMiddleware>();
```

### Database Migration
```bash
dotnet ef migrations add AddUrlTokensTable
dotnet ef database update
```

## Error Handling

### Unauthorized Access
- Returns 401 for API requests
- Redirects to AccessDenied page for web requests
- Logs all unauthorized access attempts

### Invalid Tokens
- Returns appropriate error messages
- Logs token validation failures
- Provides debugging information in development

## Best Practices

### 1. Token Generation
- Always generate tokens server-side
- Use appropriate expiration times
- Include resource type and ID

### 2. URL Handling
- Never expose unprotected URLs in client-side code
- Use the ProtectedUrlService for all sensitive links
- Implement proper error handling

### 3. Security
- Monitor token usage patterns
- Implement rate limiting if needed
- Regular cleanup of expired tokens
- Audit logs regularly

## API Endpoints

### Generate Protected URLs
- `GET /ProtectedUrl/GenerateDoctorPatientListUrl`
- `GET /ProtectedUrl/GenerateNurseDashboardUrl`
- `GET /ProtectedUrl/GenerateAdminUserManagementUrl`
- `GET /ProtectedUrl/GenerateUserDashboardUrl`

### Token Management
- `GET /ProtectedUrl/ValidateToken?token={token}`
- `GET /ProtectedUrl/GetTokenStatistics` (Admin only)

## Troubleshooting

### Common Issues

1. **Token Not Found**
   - Check if token was generated correctly
   - Verify token hasn't expired
   - Ensure token hasn't been used already

2. **Access Denied**
   - Verify user has appropriate role
   - Check if user is authenticated
   - Ensure token matches resource type

3. **Database Errors**
   - Run database migration
   - Check connection string
   - Verify UrlTokens table exists

### Debugging

Enable detailed logging in development:
```csharp
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

Check token validation in middleware logs for detailed error information.

## Migration from Existing System

1. Update all navigation links to use ProtectedUrlService
2. Replace direct URL references with protected URLs
3. Update any client-side URL generation
4. Test all protected routes thoroughly
5. Monitor logs for any issues

This system ensures complete URL protection while maintaining usability and providing comprehensive security auditing.
