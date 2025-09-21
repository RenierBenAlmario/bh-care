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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace Barangay.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<PermissionService> _logger;
        private readonly IMemoryCache _cache;

        public PermissionService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<PermissionService> logger,
            IMemoryCache cache)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _cache = cache;
        }

        /// <summary>
        /// Checks if a user has a specific permission
        /// </summary>
        /// <param name="userId">The user ID to check</param>
        /// <param name="permissionName">The permission name to check for</param>
        /// <returns>True if the user has the permission, false otherwise</returns>
        public async Task<bool> UserHasPermissionAsync(string userId, string permissionName)
        {
            _logger.LogInformation("Checking permission '{PermissionName}' for user '{UserId}'.", permissionName, userId);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(permissionName))
            {
                _logger.LogWarning("User ID or Permission Name is null or empty. Returning false.");
                return false;
            }

            // Check application cache first
            var cacheKey = $"user_permissions_{userId}";
            if (_cache.TryGetValue(cacheKey, out List<string> userPermissions))
            {
                _logger.LogInformation("Found {PermissionCount} permissions in cache for user '{UserId}'.", userPermissions?.Count ?? 0, userId);

                if (userPermissions != null)
                {
                    // Perform case-insensitive check against cached permissions
                    if (userPermissions.Contains(permissionName, StringComparer.OrdinalIgnoreCase))
                    {
                        _logger.LogInformation("Permission '{PermissionName}' found in cache for user '{UserId}'. Access granted.", permissionName, userId);
                        return true;
                    }

                    // If the input is a simple name (no category), check if it matches the name part of any cached permission
                    if (!permissionName.Contains(":"))
                    {
                        if (userPermissions.Any(p => p.EndsWith($":{permissionName}", StringComparison.OrdinalIgnoreCase)))
                        {
                            _logger.LogInformation("Permission '{PermissionName}' found in cache (as part of a category) for user '{UserId}'. Access granted.", permissionName, userId);
                            return true;
                        }
                    }
                    _logger.LogInformation("Permission '{PermissionName}' not found in cache for user '{UserId}'. Proceeding to database check.", permissionName, userId);
                }
            }

            _logger.LogInformation("Checking database for permission '{PermissionName}'.", permissionName);
            Models.Permission? permission = null;
            if (permissionName.Contains(":"))
            {
                var parts = permissionName.Split(':');
                if (parts.Length == 2)
                {
                    var category = parts[0];
                    var name = parts[1];
                    permission = await _context.Permissions
                        .FirstOrDefaultAsync(p => p.Category.ToLower() == category.ToLower() && p.Name.ToLower() == name.ToLower());
                }
            }
            else
            {
                permission = await _context.Permissions
                    .FirstOrDefaultAsync(p => p.Name.ToLower() == permissionName.ToLower());
            }

            if (permission == null)
            {
                _logger.LogWarning("Permission '{PermissionName}' does not exist in the database. Access denied.", permissionName);
                return false; // Permission doesn't exist at all
            }
            _logger.LogInformation("Permission '{PermissionName}' found in database with ID {PermissionId}.", permissionName, permission.Id);

            // Get user and their roles
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) 
            {
                _logger.LogWarning("Could not find user with ID '{UserId}'. Access denied.", userId);
                return false;
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            _logger.LogInformation("User '{UserId}' has the following roles: {UserRoles}", userId, string.Join(", ", userRoles));

            // Check for direct user permission
            _logger.LogInformation("Checking for direct permission assignment for user '{UserId}'.", userId);
            var hasDirectPermission = await _context.UserPermissions
                .AnyAsync(up => up.UserId == userId && up.PermissionId == permission.Id);

            if (hasDirectPermission)
            {
                _logger.LogInformation("User '{UserId}' has direct permission. Access granted.", userId);
                return true;
            }
            _logger.LogInformation("User '{UserId}' does not have direct permission. Checking roles.", userId);

            // Check for staff-specific permissions (StaffPermissions table)
            _logger.LogInformation("Checking for staff-specific permission assignment for user '{UserId}'.", userId);
            var staffMember = await _context.StaffMembers
                .FirstOrDefaultAsync(sm => sm.UserId == userId);

            if (staffMember != null)
            {
                // Staff membership always overrides roles: only allow if staff or direct user assignment grants it.
                var hasStaffPermission = await _context.StaffPermissions
                    .AnyAsync(sp => sp.StaffMemberId == staffMember.Id && sp.PermissionId == permission.Id);

                if (hasStaffPermission)
                {
                    _logger.LogInformation("User '{UserId}' has permission via staff-specific assignment. Access granted.", userId);
                    return true;
                }

                _logger.LogWarning("User '{UserId}' is staff but missing '{PermissionName}' in StaffPermissions. Denying access (ignoring roles).", userId, permissionName);
                return false;
            }

            // Non-staff: check for permission through roles
            var hasRolePermission = await _context.RolePermissions
                .AnyAsync(rp => userRoles.Contains(rp.Role.Name) && rp.PermissionId == permission.Id);

            if (hasRolePermission)
            {
                _logger.LogInformation("User '{UserId}' has permission via a role. Access granted.", userId);
            }
            else
            {
                _logger.LogWarning("User '{UserId}' does not have permission, either directly or via a role. Access denied.", userId);
            }

            return hasRolePermission;
        }

        /// <summary>
        /// Checks if a user has a specific permission using ClaimsPrincipal
        /// </summary>
        /// <param name="user">The ClaimsPrincipal user to check</param>
        /// <param name="permissionName">The permission name to check for</param>
        /// <returns>True if the user has the permission, false otherwise</returns>
        public async Task<bool> UserHasPermissionAsync(ClaimsPrincipal user, string permissionName)
        {
            if (user == null || string.IsNullOrEmpty(permissionName))
                return false;

            // Get the user ID from the claims
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return false;

            return await HasPermissionAsync(userId, permissionName);
        }

        /// <summary>
        /// Gets all permissions for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>A list of permission names</returns>
        public async Task<IEnumerable<string>> GetUserPermissionsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<string>();

            _logger.LogInformation($"Getting permissions for user {userId}");

            // Check application cache first
            var cacheKey = $"user_permissions_{userId}";
            if (_cache.TryGetValue(cacheKey, out List<string> cachedPermissions))
            {
                _logger.LogInformation($"Found {cachedPermissions.Count} cached permissions for user {userId}");
                return cachedPermissions;
            }

            // Get the user's roles
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"User {userId} not found");
                return new List<string>();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            _logger.LogInformation($"User {userId} has roles: {string.Join(", ", userRoles)}");

            var permissions = new List<string>();

            // Check for staff permissions first (highest priority)
            var staffMember = await _context.StaffMembers
                .FirstOrDefaultAsync(sm => sm.UserId == userId);

            if (staffMember != null)
            {
                _logger.LogInformation($"User {userId} is staff member {staffMember.Id} ({staffMember.Name})");
                
                var staffPermissionEntities = await _context.StaffPermissions
                    .Where(sp => sp.StaffMemberId == staffMember.Id)
                    .Include(sp => sp.Permission)
                    .ToListAsync();
                
                // Extract actual permission names from entities
                var staffPermissionNames = staffPermissionEntities
                    .Where(sp => sp.Permission != null)
                    .Select(sp => sp.Permission.Name)
                    .ToList();
                
                _logger.LogInformation($"Found {staffPermissionNames.Count} staff permissions: {string.Join(", ", staffPermissionNames)}");
                permissions.AddRange(staffPermissionNames);
                
                // Also add category:name format
                var staffPermissionFullNames = staffPermissionEntities
                    .Where(sp => sp.Permission != null && !string.IsNullOrEmpty(sp.Permission.Category))
                    .Select(sp => $"{sp.Permission.Category}:{sp.Permission.Name}")
                    .ToList();
                
                permissions.AddRange(staffPermissionFullNames);
            }

            // Add direct user permissions ONLY for non-staff users (staff permissions override user permissions)
            if (staffMember == null)
            {
                var userPermissionIds = await _context.UserPermissions
                    .Where(up => up.UserId == userId)
                    .Select(up => up.PermissionId)
                    .ToListAsync();

                if (userPermissionIds.Any())
                {
                    var userPermissions = await _context.Permissions
                        .Where(p => userPermissionIds.Contains(p.Id))
                        .Select(p => p.Name)
                        .ToListAsync();
                    
                    _logger.LogInformation($"Found {userPermissions.Count} direct user permissions");
                    permissions.AddRange(userPermissions);
                    
                    // Also add with category
                    var userPermissionsWithCategory = await _context.Permissions
                        .Where(p => userPermissionIds.Contains(p.Id) && !string.IsNullOrEmpty(p.Category))
                        .Select(p => $"{p.Category}:{p.Name}")
                        .ToListAsync();
                    
                    permissions.AddRange(userPermissionsWithCategory);
                }
            }
            else
            {
                _logger.LogInformation($"User {userId} is staff member - skipping user permissions, using only staff permissions");
            }

            // Add role-based permissions only for non-staff users (staff overrides roles)
            if (staffMember == null && userRoles.Any())
            {
                var roleIds = await _context.Roles
                    .Where(r => userRoles.Contains(r.Name))
                    .Select(r => r.Id)
                    .ToListAsync();
                
                var rolePermissionIds = await _context.RolePermissions
                    .Where(rp => roleIds.Contains(rp.RoleId))
                    .Select(rp => rp.PermissionId)
                    .ToListAsync();
                
                if (rolePermissionIds.Any())
                {
                    var rolePermissions = await _context.Permissions
                        .Where(p => rolePermissionIds.Contains(p.Id))
                        .Select(p => p.Name)
                        .ToListAsync();
                    
                    _logger.LogInformation($"Found {rolePermissions.Count} role-based permissions");
                    permissions.AddRange(rolePermissions);
                    
                    // Also add with category
                    var rolePermissionsWithCategory = await _context.Permissions
                        .Where(p => rolePermissionIds.Contains(p.Id) && !string.IsNullOrEmpty(p.Category))
                        .Select(p => $"{p.Category}:{p.Name}")
                        .ToListAsync();
                    
                    permissions.AddRange(rolePermissionsWithCategory);
                }
            }

            // Special handling for Admin roles
            if (userRoles.Contains("Admin") || userRoles.Contains("System Administrator"))
            {
                _logger.LogInformation($"User {userId} is an admin, granting all permissions");
                
                // Add all existing permissions
                var allPermissions = await _context.Permissions
                    .Select(p => p.Name)
                    .ToListAsync();
                
                permissions.AddRange(allPermissions);
                
                // Also add with category
                var allPermissionsWithCategory = await _context.Permissions
                    .Where(p => !string.IsNullOrEmpty(p.Category))
                    .Select(p => $"{p.Category}:{p.Name}")
                    .ToListAsync();
                
                permissions.AddRange(allPermissionsWithCategory);
            }

            // Deduplicate permissions
            var distinctPermissions = permissions.Distinct().ToList();
            _logger.LogInformation($"Total distinct permissions for user {userId}: {distinctPermissions.Count}");

            // Save to cache
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20),
                SlidingExpiration = TimeSpan.FromMinutes(5)
            };
            _cache.Set(cacheKey, distinctPermissions, cacheOptions);

            return distinctPermissions;
        }

        /// <summary>
        /// Clears cached permissions for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        public void ClearCachedPermissions(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return;

            var cacheKey = $"user_permissions_{userId}";
            _cache.Remove(cacheKey);
        }

        public async Task ClearCachedPermissionsForStaffMemberAsync(int staffMemberId)
        {
            var staffMember = await _context.StaffMembers
                .FirstOrDefaultAsync(sm => sm.Id == staffMemberId);
            
            if (staffMember != null)
            {
                ClearCachedPermissions(staffMember.UserId);
                _logger.LogInformation($"Cleared cached permissions for staff member {staffMemberId} (user {staffMember.UserId})");
            }
        }

        /// <summary>
        /// Refreshes all permission caches in the system
        /// </summary>
        public async Task RefreshAllPermissionCachesAsync()
        {
            // Get all users with roles
            var usersWithRoles = await _context.UserRoles
                .Join(_context.Users,
                    ur => ur.UserId,
                    u => u.Id,
                    (ur, u) => new { UserId = u.Id })
                .Distinct()
                .ToListAsync();

            foreach (var user in usersWithRoles)
            {
                ClearCachedPermissions(user.UserId);
                // Force reload of permissions
                await GetUserPermissionsAsync(user.UserId);
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

        /// <summary>
        /// Checks if a user has a specific permission (alias for UserHasPermissionAsync)
        /// </summary>
        /// <param name="permissionName">The permission name to check for</param>
        /// <returns>True if the user has the permission, false otherwise</returns>
        public async Task<bool> HasPermissionAsync(string permissionName)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
                return false;

            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return false;

            return await UserHasPermissionAsync(userId, permissionName);
        }

        /// <summary>
        /// Checks if a user has any of the specified permissions
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="permissionNames">The list of permission names to check</param>
        /// <returns>True if the user has any of the specified permissions, false otherwise</returns>
        public async Task<bool> UserHasAnyPermissionAsync(string userId, IEnumerable<string> permissionNames)
        {
            if (string.IsNullOrEmpty(userId) || permissionNames == null || !permissionNames.Any())
                return false;

            foreach (var permissionName in permissionNames)
            {
                if (await UserHasPermissionAsync(userId, permissionName))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a user has any of the specified permissions
        /// </summary>
        /// <param name="user">The ClaimsPrincipal user to check</param>
        /// <param name="permissionNames">The list of permission names to check</param>
        /// <returns>True if the user has any of the specified permissions, false otherwise</returns>
        public async Task<bool> UserHasAnyPermissionAsync(ClaimsPrincipal user, IEnumerable<string> permissionNames)
        {
            if (user == null || permissionNames == null || !permissionNames.Any())
                return false;

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return false;

            return await UserHasAnyPermissionAsync(userId, permissionNames);
        }

        /// <summary>
        /// Gets all permissions for a user using ClaimsPrincipal
        /// </summary>
        /// <param name="user">The ClaimsPrincipal user</param>
        /// <returns>A list of permission names</returns>
        public async Task<IEnumerable<string>> GetUserPermissionsAsync(ClaimsPrincipal user)
        {
            if (user == null)
                return new List<string>();

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return new List<string>();

            return await GetUserPermissionsAsync(userId);
        }

        /// <summary>
        /// Checks if a user has a specific permission
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="permissionName">The permission name to check for</param>
        /// <returns>True if the user has the permission, false otherwise</returns>
        private async Task<bool> HasPermissionAsync(string userId, string permissionName)
        {
            return await UserHasPermissionAsync(userId, permissionName);
        }
    }
} 