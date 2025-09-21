using Barangay.Data;
using Barangay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Barangay.Services
{
    public class FixUserRolesService : IFixUserRolesService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<FixUserRolesService> _logger;

        public FixUserRolesService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<FixUserRolesService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<int> FixUserRolesAsync()
        {
            var fixedUserCount = 0;
            var verifiedUsers = await _userManager.Users.Where(u => u.Status == "Verified").ToListAsync();

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            foreach (var user in verifiedUsers)
            {
                if (!await _userManager.IsInRoleAsync(user, "User"))
                {
                    var result = await _userManager.AddToRoleAsync(user, "User");
                    if (result.Succeeded)
                    {
                        fixedUserCount++;
                        _logger.LogInformation($"Assigned 'User' role to {user.Email}");
                    }
                    else
                    {
                        _logger.LogError($"Failed to assign 'User' role to {user.Email}");
                    }
                }
            }
            return fixedUserCount;
        }
    }
}