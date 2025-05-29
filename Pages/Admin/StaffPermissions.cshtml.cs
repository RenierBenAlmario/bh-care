using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Barangay.Data;
using Barangay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Barangay.Pages.Admin
{
    [Authorize(Policy = "AccessAdminDashboard")]
    public class StaffPermissionsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StaffPermissionsModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public StaffPermissionsModel(
            ApplicationDbContext context,
            ILogger<StaffPermissionsModel> logger,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public int? StaffId { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public List<StaffMember> StaffMembers { get; set; } = new();
        public StaffMember? SelectedStaff { get; set; }
        public List<Permission> Permissions { get; set; } = new();
        public List<StaffPermission> StaffPermissions { get; set; } = new();
        public Dictionary<string, List<Permission>> PermissionsByCategory { get; set; } = new();

        [BindProperty]
        public List<int> SelectedPermissions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _logger.LogInformation("Loading staff permissions page");

                // Load all staff members
                StaffMembers = await _context.StaffMembers
                    .Include(s => s.User)
                    .OrderBy(s => s.Name)
                    .ToListAsync();

                // Load all permissions
                Permissions = await _context.Permissions
                    .OrderBy(p => p.Category)
                    .ThenBy(p => p.Name)
                    .ToListAsync();

                // Group permissions by category
                PermissionsByCategory = Permissions
                    .GroupBy(p => !string.IsNullOrEmpty(p.Category) ? p.Category : "General")
                    .ToDictionary(g => g.Key, g => g.ToList());

                // If a staff ID is provided, load that staff member and their permissions
                if (StaffId.HasValue)
                {
                    _logger.LogInformation("Loading permissions for staff ID: {StaffId}", StaffId);

                    SelectedStaff = await _context.StaffMembers
                        .Include(s => s.User)
                        .FirstOrDefaultAsync(s => s.Id == StaffId);

                    if (SelectedStaff != null)
                    {
                        // Load current permissions for this staff member
                        StaffPermissions = await _context.StaffPermissions
                            .Where(sp => sp.StaffMemberId == StaffId)
                            .ToListAsync();

                        _logger.LogInformation("Found {Count} permissions for staff member {Name}", 
                            StaffPermissions.Count, SelectedStaff.Name);
                    }
                    else
                    {
                        _logger.LogWarning("Staff member not found for ID: {StaffId}", StaffId);
                        ErrorMessage = "Staff member not found.";
                        return RedirectToPage("/Admin/AdminDashboard");
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading staff permissions page");
                ErrorMessage = "Error loading permissions. Please try again later.";
                return RedirectToPage("/Admin/AdminDashboard");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!StaffId.HasValue)
                {
                    ErrorMessage = "No staff member selected.";
                    return RedirectToPage();
                }

                // Verify staff member exists
                var staff = await _context.StaffMembers
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == StaffId);

                if (staff == null)
                {
                    ErrorMessage = "Staff member not found.";
                    return RedirectToPage();
                }

                if (staff.User == null)
                {
                    ErrorMessage = "Staff member has no associated user account.";
                    return RedirectToPage();
                }

                _logger.LogInformation("Updating permissions for staff member {Name} (ID: {Id})", 
                    staff.Name, staff.Id);

                // Start a transaction
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Get current permissions
                    var currentPermissions = await _context.StaffPermissions
                        .Where(sp => sp.StaffMemberId == StaffId)
                        .Include(sp => sp.Permission)
                        .ToListAsync();

                    // Get all selected permissions with their details
                    var selectedPermissionDetails = await _context.Permissions
                        .Where(p => SelectedPermissions.Contains(p.Id))
                        .ToListAsync();

                    // Remove permissions that are no longer selected
                    var permissionsToRemove = currentPermissions
                        .Where(cp => !SelectedPermissions.Contains(cp.PermissionId))
                        .ToList();

                    if (permissionsToRemove.Any())
                    {
                        _context.StaffPermissions.RemoveRange(permissionsToRemove);
                        _logger.LogInformation("Removing {Count} permissions for staff member {Name}", 
                            permissionsToRemove.Count, staff.Name);
                    }

                    // Add newly selected permissions
                    var existingPermissionIds = currentPermissions.Select(p => p.PermissionId);
                    var newPermissionIds = SelectedPermissions
                        .Except(existingPermissionIds)
                        .ToList();

                    if (newPermissionIds.Any())
                    {
                        var newPermissions = newPermissionIds.Select(id => new StaffPermission
                        {
                            StaffMemberId = StaffId.Value,
                            PermissionId = id,
                            GrantedAt = DateTime.UtcNow
                        });

                        await _context.StaffPermissions.AddRangeAsync(newPermissions);
                        _logger.LogInformation("Adding {Count} new permissions for staff member {Name}", 
                            newPermissionIds.Count, staff.Name);
                    }

                    // Save changes to StaffPermissions
                    await _context.SaveChangesAsync();

                    // Update user claims
                    var user = staff.User;
                    
                    // Remove all existing permission claims
                    var existingClaims = await _userManager.GetClaimsAsync(user);
                    var permissionClaims = existingClaims.Where(c => c.Type == "Permission").ToList();
                    foreach (var claim in permissionClaims)
                    {
                        await _userManager.RemoveClaimAsync(user, claim);
                    }

                    // Add new permission claims
                    var newClaims = selectedPermissionDetails.Select(p => 
                        new Claim("Permission", p.Name)
                    ).ToList();

                    foreach (var claim in newClaims)
                    {
                        await _userManager.AddClaimAsync(user, claim);
                    }

                    // Commit transaction
                    await transaction.CommitAsync();

                    _logger.LogInformation("Successfully updated permissions and claims for staff member {Name}", staff.Name);
                    SuccessMessage = $"Permissions updated successfully for {staff.Name}.";

                    return RedirectToPage(new { staffId = StaffId });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Error saving permissions changes", ex);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating staff permissions");
                ErrorMessage = "Error updating permissions. Please try again later.";
                return RedirectToPage(new { staffId = StaffId });
            }
        }
    }
} 