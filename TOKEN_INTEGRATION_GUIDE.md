# Token Protection Integration Guide

## Quick Integration Steps

### 1. Add Protected URL Service to Your Pages

Add this to the top of any Razor Page (.cshtml):

```csharp
@inject Barangay.Services.IProtectedUrlService ProtectedUrlService
```

### 2. Generate Protected URLs

In your page code section:

```csharp
@{
    var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
    var resourceType = User.IsInRole("Admin") ? "Admin" : 
                     User.IsInRole("Doctor") ? "Doctor" : 
                     User.IsInRole("Nurse") ? "Nurse" : "User";
    
    var protectedUrl = await ProtectedUrlService.GenerateProtectedPageUrlAsync(resourceType, userId, "/Your/Page/Path");
}
```

### 3. Use Protected URLs in Links

Replace regular links with protected ones:

```html
<!-- Before -->
<a href="/Doctor/PatientList">Patient List</a>

<!-- After -->
<a href="@protectedUrl">Patient List</a>
```

### 4. Navigation Component (Automatic)

Add this to your layout files for automatic protected navigation:

```html
@await Component.InvokeAsync("ProtectedNavigation")
```

## Example: Doctor PatientList Integration

### Current PatientList.cshtml
```html
<a href="/Doctor/PatientList" class="btn btn-primary">Patient List</a>
```

### Protected PatientList.cshtml
```html
@inject Barangay.Services.IProtectedUrlService ProtectedUrlService

@{
    var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
    var protectedUrl = await ProtectedUrlService.GenerateProtectedPageUrlAsync("Doctor", userId, "/Doctor/PatientList");
}

<a href="@protectedUrl" class="btn btn-primary">Patient List</a>
```

## Example: User Dashboard Integration

### Current UserDashboard.cshtml
```html
<a href="/User/UserDashboard" class="nav-link">Dashboard</a>
```

### Protected UserDashboard.cshtml
```html
@inject Barangay.Services.IProtectedUrlService ProtectedUrlService

@{
    var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
    var protectedUrl = await ProtectedUrlService.GenerateProtectedPageUrlAsync("User", userId, "/User/UserDashboard");
}

<a href="@protectedUrl" class="nav-link">Dashboard</a>
```

## Testing Your Integration

1. **Visit the test page**: `/TestTokenSystem`
2. **Click on generated URLs** to verify they work
3. **Try accessing URLs without tokens** - should be blocked
4. **Check application logs** for token generation and validation

## Security Benefits

✅ **URLs are hidden** - Direct access is blocked
✅ **Time-limited access** - Tokens expire in 24 hours
✅ **Single use** - Tokens can only be used once
✅ **Role-based** - Tokens are validated against user roles
✅ **Audit logging** - All access attempts are logged
✅ **IP tracking** - Records user location and device

## Troubleshooting

### Common Issues

1. **"Token required" error**
   - Make sure you're using the protected URL
   - Check if the token has expired

2. **"Invalid token" error**
   - Token may have been used already
   - Check if user has proper role permissions

3. **"Access denied" error**
   - User doesn't have permission for the resource type
   - Check user roles and permissions

### Debug Information

Check the application logs for detailed token information:
- Token generation
- Token validation
- Access attempts
- Error details

## Next Steps

1. **Run database migration**: `dotnet ef database update`
2. **Start the application**: `dotnet run`
3. **Test the system**: Visit `/TestTokenSystem`
4. **Integrate into your pages**: Follow the examples above
5. **Monitor logs**: Check for any issues

The token protection system is now ready to secure all your sensitive pages!
