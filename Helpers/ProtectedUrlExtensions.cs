using Barangay.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Barangay.Helpers
{
    public static class ProtectedUrlExtensions
    {
        /// <summary>
        /// Generates a protected URL with token for the current user
        /// </summary>
        public static async Task<string> ProtectedActionAsync(this IUrlHelper urlHelper, string action, string controller, object? routeValues = null, int expirationHours = 24)
        {
            var httpContext = urlHelper.ActionContext.HttpContext;
            var protectedUrlService = httpContext.RequestServices.GetRequiredService<IProtectedUrlService>();
            
            if (!httpContext.User.Identity?.IsAuthenticated ?? true)
            {
                throw new UnauthorizedAccessException("User must be authenticated to generate protected URLs");
            }

            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID");
            }

            // Determine resource type based on user roles
            var resourceType = DetermineResourceType(httpContext.User);

            return await protectedUrlService.GenerateProtectedUrlAsync(resourceType, userId, action, controller, routeValues, expirationHours);
        }

        /// <summary>
        /// Generates a protected URL for Razor Pages with token for the current user
        /// </summary>
        public static async Task<string> ProtectedPageAsync(this IUrlHelper urlHelper, string pageName, object? routeValues = null, int expirationHours = 24)
        {
            var httpContext = urlHelper.ActionContext.HttpContext;
            var protectedUrlService = httpContext.RequestServices.GetRequiredService<IProtectedUrlService>();
            
            if (!httpContext.User.Identity?.IsAuthenticated ?? true)
            {
                throw new UnauthorizedAccessException("User must be authenticated to generate protected URLs");
            }

            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID");
            }

            // Determine resource type based on user roles
            var resourceType = DetermineResourceType(httpContext.User);

            return await protectedUrlService.GenerateProtectedPageUrlAsync(resourceType, userId, pageName, routeValues, expirationHours);
        }

        /// <summary>
        /// Generates a protected URL with token for a specific user
        /// </summary>
        public static async Task<string> ProtectedActionForUserAsync(this IUrlHelper urlHelper, string action, string controller, string userId, object? routeValues = null, int expirationHours = 24)
        {
            var httpContext = urlHelper.ActionContext.HttpContext;
            var protectedUrlService = httpContext.RequestServices.GetRequiredService<IProtectedUrlService>();
            
            // Determine resource type based on user roles
            var resourceType = DetermineResourceType(httpContext.User);

            return await protectedUrlService.GenerateProtectedUrlAsync(resourceType, userId, action, controller, routeValues, expirationHours);
        }

        private static string DetermineResourceType(ClaimsPrincipal user)
        {
            if (user.IsInRole("Admin") || user.IsInRole("System Administrator"))
                return "Admin";
            if (user.IsInRole("Doctor"))
                return "Doctor";
            if (user.IsInRole("Nurse") || user.IsInRole("Head Nurse"))
                return "Nurse";
            if (user.IsInRole("User"))
                return "User";
            
            return "User"; // Default fallback
        }
    }
}
