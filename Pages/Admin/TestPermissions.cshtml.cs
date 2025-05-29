using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barangay.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class TestPermissionsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly PermissionService _permissionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TestPermissionsModel(
            ApplicationDbContext context,
            PermissionService permissionService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _permissionService = permissionService;
            _userManager = userManager;
        }

        [TempData]
        public string Message { get; set; }

        [TempData]
        public bool IsError { get; set; }

        [BindProperty]
        public string UserId { get; set; }

        [BindProperty]
        public string PermissionName { get; set; }

        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public List<Permission> Permissions { get; set; } = new List<Permission>();
        public bool? TestResult { get; set; }
        public string SelectedUserName { get; set; }
        public List<string> UserPermissions { get; set; } = new List<string>();

        public async Task OnGetAsync()
        {
            // Load users
            Users = await _userManager.Users.ToListAsync();

            // Load permissions
            Permissions = await _context.Permissions.OrderBy(p => p.Category).ThenBy(p => p.Name).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(PermissionName))
            {
                IsError = true;
                Message = "Please select both a user and a permission to test.";
                return RedirectToPage();
            }

            // Get user
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                IsError = true;
                Message = "Selected user not found.";
                return RedirectToPage();
            }

            SelectedUserName = user.FullName ?? user.UserName;

            // Test permission
            TestResult = await _permissionService.UserHasPermissionAsync(UserId, PermissionName);

            // Get all user permissions for display
            UserPermissions = await _permissionService.GetUserPermissionsAsync(UserId);

            Message = $"Permission test for {SelectedUserName}: {(TestResult.Value ? "GRANTED" : "DENIED")}";
            IsError = false;

            return RedirectToPage();
        }
    }
} 