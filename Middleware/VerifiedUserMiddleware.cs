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

            // Get user roles
            var roles = await userManager.GetRolesAsync(user);
            
            // Allow doctors and nurses to access User pages without verification check
            if (roles.Any(r => r == "Doctor") && 
               (context.Request.Path.StartsWithSegments("/User") || 
                context.Request.Path.StartsWithSegments("/Doctor")))
            {
                _logger.LogInformation($"Doctor {user.Email} accessing page: {context.Request.Path}");
                await _next(context);
                return;
            }

            // Allow nurses to access User and Nurse pages
            if (roles.Any(r => r == "Nurse") && 
               (context.Request.Path.StartsWithSegments("/User") || 
                context.Request.Path.StartsWithSegments("/Nurse")))
            {
                _logger.LogInformation($"Nurse {user.Email} accessing page: {context.Request.Path}");
                await _next(context);
                return;
            }

            // Admin users and staff bypass verification check
            if (roles.Any(r => r == "Admin" || r == "System Administrator" || r == "Admin Staff"))
            {
                await _next(context);
                return;
            }

            // Only check verification status for user dashboard access
            if (context.Request.Path.StartsWithSegments("/User"))
            {
                if (user.Status == "Verified" && user.IsActive)
                {
                    // Ensure user has the User role if verified
                    if (!roles.Contains("User"))
                    {
                        await userVerificationService.EnsureUserRoleAssignedAsync(user.Id);
                    }
                    await _next(context);
                    return;
                }

                // Redirect unverified users trying to access user pages
                _logger.LogWarning($"Unverified user {user.Email} attempted to access {context.Request.Path}");
                context.Response.Redirect("/Account/WaitingForApproval");
                return;
            }

            // Allow access to other pages
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