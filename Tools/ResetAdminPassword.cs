using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Barangay.Models;

namespace Barangay.Tools
{
    public class ResetAdminPassword
    {
        public static async Task ResetPassword(IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                
                // Find admin user
                var adminUser = await userManager.FindByEmailAsync("admin@example.com");
                
                if (adminUser == null)
                {
                    // Create admin user if it doesn't exist
                    adminUser = new ApplicationUser
                    {
                        UserName = "admin@example.com",
                        Email = "admin@example.com",
                        EmailConfirmed = true,
                        FullName = "System Administrator",
                        Status = "Verified"
                    };
                    
                    var result = await userManager.CreateAsync(adminUser, "Admin@123");
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors)}");
                    }
                    
                    // Add to admin role
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    // Reset password for existing admin
                    var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
                    var result = await userManager.ResetPasswordAsync(adminUser, token, "Admin@123");
                    
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to reset admin password: {string.Join(", ", result.Errors)}");
                    }
                }
            }
        }
    }
} 