using Barangay.Data;
using Barangay.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Barangay.Services
{
    public class PermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PermissionService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Checks if a user has a specific permission
        /// </summary>
        /// <param name="userId">The user ID to check</param>
        /// <param name="permissionName">The permission name to check for</param>
        /// <returns>True if the user has the permission, false otherwise</returns>
        public async Task<bool> UserHasPermissionAsync(string userId, string permissionName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(permissionName))
                return false;

            // Check session cache first
            var session = _httpContextAccessor.HttpContext?.Session;
            var cacheKey = $"user_permissions_{userId}";
            
            List<string>? userPermissions = null;
            
            if (session != null)
            {
                var cachedPermissions = session.GetString(cacheKey);
                if (!string.IsNullOrEmpty(cachedPermissions))
                {
                    userPermissions = System.Text.Json.JsonSerializer.Deserialize<List<string>>(cachedPermissions);
                    if (userPermissions != null)
                    {
                        return userPermissions.Contains(permissionName);
                    }
                }
            }

            // Find the permission by name
            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == permissionName);

            if (permission == null)
                return false;

            // Check if the user has this permission
            var hasPermission = await _context.UserPermissions
                .AnyAsync(up => up.UserId == userId && up.PermissionId == permission.Id);
                
            // Cache the result if session is available
            if (session != null && hasPermission)
            {
                // Get all user permissions and cache them
                userPermissions = await GetUserPermissionsAsync(userId);
                if (userPermissions != null)
                {
                    var permissionsJson = System.Text.Json.JsonSerializer.Serialize(userPermissions);
                    session.SetString(cacheKey, permissionsJson);
                }
            }
            
            return hasPermission;
        }

        /// <summary>
        /// Gets all permissions for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>A list of permission names</returns>
        public async Task<List<string>> GetUserPermissionsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<string>();

            // Check session cache first
            var session = _httpContextAccessor.HttpContext?.Session;
            var cacheKey = $"user_permissions_{userId}";
            
            if (session != null)
            {
                var cachedPermissions = session.GetString(cacheKey);
                if (!string.IsNullOrEmpty(cachedPermissions))
                {
                    return System.Text.Json.JsonSerializer.Deserialize<List<string>>(cachedPermissions);
                }
            }

            // Get all permission IDs for the user
            var userPermissionIds = await _context.UserPermissions
                .Where(up => up.UserId == userId)
                .Select(up => up.PermissionId)
                .ToListAsync();

            // Get the permission names
            var permissions = await _context.Permissions
                .Where(p => userPermissionIds.Contains(p.Id))
                .Select(p => p.Name)
                .ToListAsync();

            // Cache the result if session is available
            if (session != null)
            {
                var permissionsJson = System.Text.Json.JsonSerializer.Serialize(permissions);
                session.SetString(cacheKey, permissionsJson);
            }

            return permissions;
        }

        /// <summary>
        /// Clears cached permissions for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        public void ClearCachedPermissions(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return;
                
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                var cacheKey = $"user_permissions_{userId}";
                session.Remove(cacheKey);
            }
        }

        /// <summary>
        /// Gets all available permissions
        /// </summary>
        /// <returns>A list of all permissions</returns>
        public async Task<List<Permission>> GetAllPermissionsAsync()
        {
            return await _context.Permissions.ToListAsync();
        }

        /// <summary>
        /// Adds a permission to a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="permissionName">The permission name</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> AddPermissionToUserAsync(string userId, string permissionName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(permissionName))
                return false;

            // Find the permission by name
            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == permissionName);

            if (permission == null)
                return false;

            // Check if the user already has this permission
            var userHasPermission = await _context.UserPermissions
                .AnyAsync(up => up.UserId == userId && up.PermissionId == permission.Id);

            if (userHasPermission)
                return true; // Already has the permission

            // Add the permission
            var userPermission = new UserPermission
            {
                UserId = userId,
                PermissionId = permission.Id
            };

            await _context.UserPermissions.AddAsync(userPermission);
            await _context.SaveChangesAsync();
            
            // Clear cached permissions
            ClearCachedPermissions(userId);

            return true;
        }

        /// <summary>
        /// Removes a permission from a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="permissionName">The permission name</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> RemovePermissionFromUserAsync(string userId, string permissionName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(permissionName))
                return false;

            // Find the permission by name
            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == permissionName);

            if (permission == null)
                return false;

            // Find the user permission
            var userPermission = await _context.UserPermissions
                .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permission.Id);

            if (userPermission == null)
                return true; // User doesn't have this permission

            // Remove the permission
            _context.UserPermissions.Remove(userPermission);
            await _context.SaveChangesAsync();
            
            // Clear cached permissions
            ClearCachedPermissions(userId);

            return true;
        }

        public async Task<bool> HasPermissionAsync(string userId, string permissionName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Any())
            {
                return false;
            }

            var roleIds = await _roleManager.Roles
                .Where(r => roles.Contains(r.Name ?? string.Empty))
                .Select(r => r.Id)
                .ToListAsync();

            var hasPermission = await _context.RolePermissions
                .AnyAsync(rp => roleIds.Contains(rp.RoleId) && rp.Permission.Name == permissionName);

            return hasPermission;
        }
    }
} 