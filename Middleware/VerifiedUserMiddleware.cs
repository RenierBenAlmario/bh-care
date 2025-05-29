using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Barangay.Models;
using Barangay.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Barangay.Middleware
{
    public class VerifiedUserMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<VerifiedUserMiddleware> _logger;

        public VerifiedUserMiddleware(RequestDelegate next, ILogger<VerifiedUserMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            UserManager<ApplicationUser> userManager,
            IUserVerificationService userVerificationService)
        {
            // Skip middleware for non-authenticated users or specific paths
            if (!context.User.Identity.IsAuthenticated ||
                context.Request.Path.StartsWithSegments("/Account") ||
                context.Request.Path.StartsWithSegments("/Admin") ||
                context.Request.Path.StartsWithSegments("/lib") ||
                context.Request.Path.StartsWithSegments("/css") ||
                context.Request.Path.StartsWithSegments("/js"))
            {
                await _next(context);
                return;
            }

            var user = await userManager.GetUserAsync(context.User);
            if (user == null)
            {
                await _next(context);
                return;
            }

            // Check if user is in admin roles
            bool isAdmin = await userManager.IsInRoleAsync(user, "Admin") ||
                         await userManager.IsInRoleAsync(user, "System Administrator") ||
                         await userManager.IsInRoleAsync(user, "Admin Staff") ||
                         await userManager.IsInRoleAsync(user, "Doctor") ||
                         await userManager.IsInRoleAsync(user, "Nurse");

            if (isAdmin)
            {
                // Ensure admin users are always verified and active
                if (user.Status != "Verified" || !user.IsActive)
                {
                    user.Status = "Verified";
                    user.IsActive = true;
                    await userManager.UpdateAsync(user);
                    _logger.LogInformation($"Admin user {user.Email} status updated to Verified and Active");
                }
                await _next(context);
                return;
            }

            // For regular users, check verification status
            if (user.Status == "Verified" && user.IsActive)
            {
                // Ensure user has the User role if verified
                await userVerificationService.EnsureUserRoleAssignedAsync(user.Id);
                await _next(context);
                return;
            }

            // Redirect unverified users to access denied page
            if (!context.Request.Path.StartsWithSegments("/Account/AccessDenied"))
            {
                _logger.LogWarning($"Unverified user {user.Email} attempted to access {context.Request.Path}");
                context.Response.Redirect($"/Account/AccessDenied?ReturnUrl={context.Request.Path}");
                return;
            }

            await _next(context);
        }
    }

    public static class VerifiedUserMiddlewareExtensions
    {
        public static IApplicationBuilder UseVerifiedUser(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<VerifiedUserMiddleware>();
        }
    }
} 