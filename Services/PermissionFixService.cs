using Barangay.Data;
using Barangay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barangay.Services
{
    public class PermissionFixService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PermissionFixService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PermissionFixService(
            ApplicationDbContext context,
            ILogger<PermissionFixService> logger,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task FixPermissionsAsync()
        {
            try
            {
                _logger.LogInformation("Starting permission fix process");

                // Ensure all essential permissions exist
                await EnsurePermissionsExistAsync();

                // Grant essential permissions to users based on their roles
                await GrantEssentialPermissionsAsync();

                _logger.LogInformation("Permission fix process completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during permission fix process");
                throw;
            }
        }

        public async Task EnsurePermissionsExistAsync()
        {
            _logger.LogInformation("Checking for missing permissions in the database");
            
            // Define all required permissions
            var requiredPermissions = new[]
            {
                new { Name = "ApproveUsers", Description = "Ability to approve user registrations", Category = "User Management" },
                new { Name = "DeleteUsers", Description = "Ability to delete users from the system", Category = "User Management" },
                new { Name = "ViewReports", Description = "Ability to view system reports", Category = "Reports" },
                new { Name = "ManageUsers", Description = "Ability to manage user accounts", Category = "User Management" },
                new { Name = "ManageAppointments", Description = "Ability to manage appointments", Category = "Appointments" },
                new { Name = "ManageMedicalRecords", Description = "Ability to manage medical records", Category = "Medical Records" },
                new { Name = "Access Admin Dashboard", Description = "Ability to access the admin dashboard", Category = "Dashboard" },
                new { Name = "Access Doctor Dashboard", Description = "Ability to access the doctor dashboard", Category = "Dashboard" },
                new { Name = "Access Nurse Dashboard", Description = "Ability to access the nurse dashboard", Category = "Dashboard" },
                new { Name = "View Appointments", Description = "Ability to view appointments", Category = "Appointments" },
                new { Name = "Create Appointments", Description = "Ability to create new appointments", Category = "Appointments" }
            };
            
            // Get existing permissions
            var existingPermissions = await _context.Permissions.ToListAsync();
            var existingPermissionNames = existingPermissions.Select(p => p.Name).ToHashSet();
            
            // Add missing permissions
            var missingPermissions = new List<Permission>();
            foreach (var permission in requiredPermissions)
            {
                if (!existingPermissionNames.Contains(permission.Name))
                {
                    missingPermissions.Add(new Permission
                    {
                        Name = permission.Name,
                        Description = permission.Description,
                        Category = permission.Category
                    });
                }
            }
            
            if (missingPermissions.Any())
            {
                _logger.LogInformation("Adding {Count} missing permissions", missingPermissions.Count);
                await _context.Permissions.AddRangeAsync(missingPermissions);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogInformation("No missing permissions found");
            }
        }

        private async Task GrantEssentialPermissionsAsync()
        {
            _logger.LogInformation("Granting essential permissions to users based on roles");

            try
            {
            // Define role-specific essential permissions
            var rolePermissions = new Dictionary<string, List<string>>
            {
                {
                    "Admin", new List<string>
                    {
                        "Manage Users", "Access Admin Dashboard", "View Reports", 
                        "Approve Users", "Manage Appointments", "Manage Medical Records", 
                        "Manage Permissions"
                    }
                },
                {
                    "Doctor", new List<string>
                    {
                        "Access Doctor Dashboard", "Manage Appointments", "Manage Medical Records", 
                        "View Patient History", "Create Prescriptions", "View Reports"
                    }
                },
                {
                    "Nurse", new List<string>
                    {
                        "ManageAppointments", "Access Nurse Dashboard", "Record Vital Signs", 
                        "View Patient History", "Manage Medical Records"
                    }
                }
            };

            // Get all permissions
            var allPermissions = await _context.Permissions.ToListAsync();

            // Process each role
            foreach (var roleEntry in rolePermissions)
            {
                var roleName = roleEntry.Key;
                var permissionNames = roleEntry.Value;
                
                // Get users with this role
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    _logger.LogWarning("Role {RoleName} not found", roleName);
                    continue;
                }
                
                    // Get users in role directly from database
                    var userIds = await _context.UserRoles
                        .Where(ur => ur.RoleId == role.Id)
                        .Select(ur => ur.UserId)
                        .ToListAsync();
                        
                    var usersInRole = await _context.Set<ApplicationUser>()
                        .Where(u => userIds.Contains(u.Id))
                        .Select(u => new ApplicationUser
                        {
                            Id = u.Id,
                            FirstName = u.FirstName ?? string.Empty,
                            MiddleName = u.MiddleName ?? string.Empty,
                            LastName = u.LastName ?? string.Empty,
                            Email = u.Email ?? string.Empty,
                            UserName = u.UserName ?? string.Empty,
                            PhoneNumber = u.PhoneNumber,
                            EmailConfirmed = u.EmailConfirmed,
                            PhoneNumberConfirmed = u.PhoneNumberConfirmed,
                            TwoFactorEnabled = u.TwoFactorEnabled,
                            LockoutEnabled = u.LockoutEnabled,
                            AccessFailedCount = u.AccessFailedCount
                        })
                        .ToListAsync();
                
                    _logger.LogInformation($"Found {usersInRole.Count} users with role {roleName}");

                // Get permission IDs for this role
                var permissionIds = allPermissions
                    .Where(p => permissionNames.Contains(p.Name))
                    .Select(p => p.Id)
                    .ToList();

                // Grant permissions to each user
                foreach (var user in usersInRole)
                {
                        if (string.IsNullOrEmpty(user.Id))
                        {
                            _logger.LogWarning("Skipping user with null ID");
                            continue;
                        }

                    // Get existing permission IDs for this user
                    var existingPermissionIds = await _context.UserPermissions
                        .Where(up => up.UserId == user.Id)
                        .Select(up => up.PermissionId)
                        .ToListAsync();

                    // Add missing permissions
                        foreach (var permissionId in permissionIds.Where(pid => !existingPermissionIds.Contains(pid)))
                        {
                            _context.UserPermissions.Add(new UserPermission
                            {
                                UserId = user.Id,
                                PermissionId = permissionId,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            });
                        }
                    }
                }

            await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully granted essential permissions to users");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error granting essential permissions to users");
                throw;
            }
        }

        /// <summary>
        /// Ensures a user has the specified permissions
        /// </summary>
        public async Task EnsureUserHasPermissionsAsync(string userId, IEnumerable<string> permissionNames)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Cannot ensure permissions for null or empty user ID");
                return;
            }
            
            // Get permissions by name
            var permissions = await _context.Permissions
                .Where(p => permissionNames.Contains(p.Name))
                .ToListAsync();
                
            if (permissions.Count < permissionNames.Count())
            {
                var foundNames = permissions.Select(p => p.Name).ToHashSet();
                var missingNames = permissionNames.Where(n => !foundNames.Contains(n));
                _logger.LogWarning("Some permissions not found: {MissingPermissions}", 
                    string.Join(", ", missingNames));
            }
            
            // Get user's existing permissions
            var existingPermissionIds = await _context.UserPermissions
                .Where(up => up.UserId == userId)
                .Select(up => up.PermissionId)
                .ToListAsync();
                
            // Add missing permissions
            var permissionsToAdd = permissions
                .Where(p => !existingPermissionIds.Contains(p.Id))
                .ToList();
                
            if (permissionsToAdd.Any())
            {
                _logger.LogInformation("Adding {Count} permissions to user {UserId}", 
                    permissionsToAdd.Count, userId);
                    
                var userPermissions = permissionsToAdd.Select(p => new UserPermission
                {
                    UserId = userId,
                    PermissionId = p.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                
                await _context.UserPermissions.AddRangeAsync(userPermissions);
                await _context.SaveChangesAsync();
            }
        }
        
        /// <summary>
        /// Removes specified permissions from a user
        /// </summary>
        public async Task RemoveUserPermissionsAsync(string userId, IEnumerable<string> permissionNames)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Cannot remove permissions for null or empty user ID");
                return;
            }
            
            // Get permissions by name
            var permissions = await _context.Permissions
                .Where(p => permissionNames.Contains(p.Name))
                .ToListAsync();
                
            if (!permissions.Any())
            {
                _logger.LogWarning("No permissions found to remove");
                return;
            }
            
            // Get user permissions to remove
            var permissionIds = permissions.Select(p => p.Id).ToList();
            var userPermissionsToRemove = await _context.UserPermissions
                .Where(up => up.UserId == userId && permissionIds.Contains(up.PermissionId))
                .ToListAsync();
                
            if (userPermissionsToRemove.Any())
            {
                _logger.LogInformation("Removing {Count} permissions from user {UserId}", 
                    userPermissionsToRemove.Count, userId);
                    
                _context.UserPermissions.RemoveRange(userPermissionsToRemove);
                await _context.SaveChangesAsync();
            }
        }
        
        /// <summary>
        /// Verifies that a user's permissions match the specified set
        /// </summary>
        public async Task<bool> VerifyUserPermissionsAsync(string userId, IEnumerable<string> expectedPermissionNames)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Cannot verify permissions for null or empty user ID");
                return false;
            }
            
            // Get expected permission IDs
            var expectedPermissions = await _context.Permissions
                .Where(p => expectedPermissionNames.Contains(p.Name))
                .ToListAsync();
                
            var expectedPermissionIds = expectedPermissions.Select(p => p.Id).ToHashSet();
            
            // Get user's actual permissions
            var actualPermissionIds = await _context.UserPermissions
                .Where(up => up.UserId == userId)
                .Select(up => up.PermissionId)
                .ToListAsync();
                
            // Check if all expected permissions are present and no extras exist
            var hasAllExpected = expectedPermissionIds.All(id => actualPermissionIds.Contains(id));
            var hasNoExtras = actualPermissionIds.All(id => expectedPermissionIds.Contains(id));
            
            return hasAllExpected && hasNoExtras;
        }
    }
} 