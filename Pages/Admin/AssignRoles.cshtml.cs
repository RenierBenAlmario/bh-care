using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barangay.Pages.Admin
{
    [Authorize(Roles = "Admin,System Administrator")]
    public class AssignRolesModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AssignRolesModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public class UserWithRoles
        {
            public string Id { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public List<string> Roles { get; set; } = new List<string>();
        }

        public List<UserWithRoles> Users { get; set; } = new List<UserWithRoles>();

        [TempData]
        public string StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Users.Add(new UserWithRoles
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName ?? "N/A",
                    Roles = roles.ToList()
                });
            }
        }

        public async Task<IActionResult> OnPostAssignRoleAsync(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            {
                StatusMessage = "User ID and role name are required.";
                return RedirectToPage();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                StatusMessage = "User not found.";
                return RedirectToPage();
            }

            // Ensure role exists
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // Check if user already has this role
            if (await _userManager.IsInRoleAsync(user, roleName))
            {
                StatusMessage = $"User {user.Email} already has the {roleName} role.";
                return RedirectToPage();
            }

            // Assign role to user
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                StatusMessage = $"Successfully assigned {roleName} role to {user.Email}.";
            }
            else
            {
                StatusMessage = $"Failed to assign role: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }

            return RedirectToPage();
        }
    }
}
