using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using Barangay.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barangay.Pages.Admin
{
    [Authorize(Policy = "AccessAdminDashboard")]
    public class ManagePermissionsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly PermissionService _permissionService;
        private readonly PermissionFixService _permissionFixService;
        private readonly ILogger<ManagePermissionsModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManagePermissionsModel(
            ApplicationDbContext context,
            PermissionService permissionService,
            PermissionFixService permissionFixService,
            ILogger<ManagePermissionsModel> logger,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _permissionService = permissionService;
            _permissionFixService = permissionFixService;
            _logger = logger;
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        [TempData]
        public bool IsError { get; set; } = false;

        [BindProperty(SupportsGet = true)]
        public string SelectedUserId { get; set; } = string.Empty;

        public string SelectedUserName { get; set; } = string.Empty;

        [BindProperty]
        public List<int> SelectedPermissions { get; set; } = new List<int>();

        public List<int> UserPermissions { get; set; } = new List<int>();

        public List<StaffMember> StaffMembers { get; set; } = new List<StaffMember>();

        public Dictionary<string, List<Permission>> PermissionsByCategory { get; set; } = new Dictionary<string, List<Permission>>();

        public async Task OnGetAsync(string userId)
        {
            // Ensure all necessary permissions exist in the database
            await _permissionFixService.EnsurePermissionsExistAsync();

            // Load staff members for the sidebar
            StaffMembers = await _context.StaffMembers
                .OrderBy(s => s.Name)
                .ToListAsync();

            // Load permissions grouped by category
            var permissions = await _context.Permissions
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .ToListAsync();

            PermissionsByCategory = permissions
                .GroupBy(p => p.Category)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Handle user selection from query string
            if (!string.IsNullOrEmpty(userId))
            {
                SelectedUserId = userId;
                await LoadUserPermissions();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                _logger.LogInformation("Starting permission update for user {UserId}", SelectedUserId);
                
                if (string.IsNullOrEmpty(SelectedUserId))
                {
                    IsError = true;
                    StatusMessage = "No user was selected.";
                    _logger.LogWarning("Attempted to update permissions without selecting a user");
                    return RedirectToPage();
                }

                // Verify the user exists using UserManager instead of DbContext
                var user = await _userManager.FindByIdAsync(SelectedUserId);
                if (user == null)
                {
                    IsError = true;
                    StatusMessage = "Selected user does not exist.";
                    _logger.LogWarning("Attempted to update permissions for non-existent user {UserId}", SelectedUserId);
                    return RedirectToPage();
                }

                // Get all available permissions
                var allPermissions = await _context.Permissions.ToListAsync();
                var allPermissionIds = allPermissions.Select(p => p.Id).ToList();
                
                // Ensure SelectedPermissions is not null
                if (SelectedPermissions == null)
                {
                    SelectedPermissions = new List<int>();
                    _logger.LogInformation("SelectedPermissions was null, initialized to empty list");
                }

                // Get current user permissions
                var currentUserPermissions = await _context.UserPermissions
                    .Where(up => up.UserId == SelectedUserId)
                    .ToListAsync();
                
                var currentPermissionIds = currentUserPermissions.Select(up => up.PermissionId).ToList();

                _logger.LogInformation("User {UserId} currently has {PermissionCount} permissions", 
                    SelectedUserId, currentPermissionIds.Count);
                _logger.LogInformation("User {UserId} will have {PermissionCount} permissions after update", 
                    SelectedUserId, SelectedPermissions.Count);

                // IMPORTANT: Remove ALL existing permissions first to ensure unchecked permissions are removed
                if (currentUserPermissions.Any())
                {
                    _context.UserPermissions.RemoveRange(currentUserPermissions);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Removed all existing permissions for user {UserId}", SelectedUserId);
                }

                // Add selected permissions
                if (SelectedPermissions.Any())
                {
                    var newPermissions = SelectedPermissions
                        .Where(p => allPermissionIds.Contains(p)) // Validate against existing permissions
                        .Select(p => new UserPermission
                        {
                            UserId = SelectedUserId,
                            PermissionId = p
                        })
                        .ToList();

                    await _context.UserPermissions.AddRangeAsync(newPermissions);
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("Added {Count} permissions for user {UserId}", 
                        newPermissions.Count, SelectedUserId);
                }

                // Get the staff name for the message
                var staffName = await _context.StaffMembers
                    .Where(s => s.UserId == SelectedUserId)
                    .Select(s => s.Name)
                    .FirstOrDefaultAsync() ?? "User";

                // Log the permission names that were added/removed
                var addedPermissionNames = await _context.Permissions
                    .Where(p => SelectedPermissions.Contains(p.Id) && !currentPermissionIds.Contains(p.Id))
                    .Select(p => p.Name)
                    .ToListAsync();
                
                var removedPermissionNames = await _context.Permissions
                    .Where(p => !SelectedPermissions.Contains(p.Id) && currentPermissionIds.Contains(p.Id))
                    .Select(p => p.Name)
                    .ToListAsync();
                
                if (addedPermissionNames.Any() || removedPermissionNames.Any())
                {
                    _logger.LogInformation("For user {UserId}: Added permissions: {AddedPermissions}, Removed permissions: {RemovedPermissions}", 
                        SelectedUserId, string.Join(", ", addedPermissionNames), string.Join(", ", removedPermissionNames));
                }

                IsError = false;
                StatusMessage = $"Permissions for {staffName} have been updated successfully.";
                _logger.LogInformation("Successfully updated permissions for {UserId} ({StaffName})", SelectedUserId, staffName);

                // Clear any cached permissions
                _permissionService.ClearCachedPermissions(SelectedUserId);

                return RedirectToPage(new { userId = SelectedUserId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating permissions for user {UserId}", SelectedUserId);
                IsError = true;
                StatusMessage = $"An error occurred: {ex.Message}";
                return RedirectToPage(new { userId = SelectedUserId });
            }
        }

        private async Task LoadUserPermissions()
        {
            // Get user name
            var staff = await _context.StaffMembers
                .FirstOrDefaultAsync(s => s.UserId == SelectedUserId);

            if (staff != null)
            {
                SelectedUserName = staff.Name;
            }

            // Get current user permissions
            UserPermissions = await _context.UserPermissions
                .Where(up => up.UserId == SelectedUserId)
                .Select(up => up.PermissionId)
                .ToListAsync();

            // Pre-select existing permissions
            SelectedPermissions = UserPermissions.ToList();
        }
    }
} 