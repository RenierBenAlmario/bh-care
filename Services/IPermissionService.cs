using Barangay.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Barangay.Services
{
    public interface IPermissionService
    {
        /// <summary>
        /// Checks if a user has a specific permission
        /// </summary>
        /// <param name="userId">The user ID to check</param>
        /// <param name="permissionName">The permission name to check for</param>
        /// <returns>True if the user has the permission, false otherwise</returns>
        Task<bool> UserHasPermissionAsync(string userId, string permissionName);

        /// <summary>
        /// Checks if a user has a specific permission
        /// </summary>
        /// <param name="user">The ClaimsPrincipal user to check</param>
        /// <param name="permissionName">The permission name to check for</param>
        /// <returns>True if the user has the permission, false otherwise</returns>
        Task<bool> UserHasPermissionAsync(ClaimsPrincipal user, string permissionName);

        /// <summary>
        /// Checks if a user has any of the specified permissions
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="permissionNames">The list of permission names to check</param>
        /// <returns>True if the user has any of the specified permissions, false otherwise</returns>
        Task<bool> UserHasAnyPermissionAsync(string userId, IEnumerable<string> permissionNames);

        /// <summary>
        /// Checks if a user has any of the specified permissions
        /// </summary>
        /// <param name="user">The ClaimsPrincipal user to check</param>
        /// <param name="permissionNames">The list of permission names to check</param>
        /// <returns>True if the user has any of the specified permissions, false otherwise</returns>
        Task<bool> UserHasAnyPermissionAsync(ClaimsPrincipal user, IEnumerable<string> permissionNames);

        /// <summary>
        /// Checks if a user has a specific permission (alias for UserHasPermissionAsync)
        /// </summary>
        /// <param name="permissionName">The permission name to check for</param>
        /// <returns>True if the user has the permission, false otherwise</returns>
        Task<bool> HasPermissionAsync(string permissionName);

        /// <summary>
        /// Gets all permissions for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>A list of permission names</returns>
        Task<IEnumerable<string>> GetUserPermissionsAsync(string userId);

        /// <summary>
        /// Gets all permissions for a user
        /// </summary>
        /// <param name="user">The ClaimsPrincipal user</param>
        /// <returns>A list of permission names</returns>
        Task<IEnumerable<string>> GetUserPermissionsAsync(ClaimsPrincipal user);

        /// <summary>
        /// Clears cached permissions for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        void ClearCachedPermissions(string userId);

        /// <summary>
        /// Clears cached permissions for a staff member
        /// </summary>
        /// <param name="staffMemberId">The staff member ID</param>
        Task ClearCachedPermissionsForStaffMemberAsync(int staffMemberId);

        /// <summary>
        /// Refreshes all permission caches in the system
        /// </summary>
        Task RefreshAllPermissionCachesAsync();

        /// <summary>
        /// Gets all available permissions
        /// </summary>
        /// <returns>A list of all permissions</returns>
        Task<List<Permission>> GetAllPermissionsAsync();

        /// <summary>
        /// Adds a permission to a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="permissionName">The permission name to add</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> AddPermissionToUserAsync(string userId, string permissionName);

        /// <summary>
        /// Removes a permission from a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="permissionName">The permission name to remove</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> RemovePermissionFromUserAsync(string userId, string permissionName);
    }
} 