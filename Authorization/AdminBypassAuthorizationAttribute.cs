using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Barangay.Authorization
{
    public class AdminBypassAuthorizationAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public AdminBypassAuthorizationAttribute()
        {
            Policy = "AdminBypassPolicy";
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var loggerFactory = context.HttpContext.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
            var logger = loggerFactory?.CreateLogger<AdminBypassAuthorizationAttribute>();

            var user = context.HttpContext.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                logger?.LogInformation($"User {user.Identity.Name} attempting to access admin-protected resource");
                logger?.LogInformation($"User roles: {string.Join(", ", user.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(c => c.Value))}");
            }
        }
    }
} 