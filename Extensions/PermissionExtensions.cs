using Barangay.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Barangay.Extensions
{
    /// <summary>
    /// Extension methods for working with permissions in views
    /// </summary>
    public static class PermissionExtensions
    {
        /// <summary>
        /// Checks if the current user has the specified permission
        /// </summary>
        /// <param name="user">The ClaimsPrincipal representing the current user</param>
        /// <param name="permissionName">The name of the permission to check</param>
        /// <param name="permissionService">The permission service</param>
        /// <param name="httpContextAccessor">The HTTP context accessor</param>
        /// <returns>True if the user has the permission, false otherwise</returns>
        public static async Task<bool> HasPermissionAsync(
            this ClaimsPrincipal user, 
            string permissionName,
            PermissionService permissionService,
            IHttpContextAccessor httpContextAccessor)
        {
            if (!user.Identity.IsAuthenticated)
                return false;
                
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return false;
                
            return await permissionService.UserHasPermissionAsync(userId, permissionName);
        }
    }
} 