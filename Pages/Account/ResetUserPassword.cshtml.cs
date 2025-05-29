using System;
using System.Threading.Tasks;
using Barangay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Barangay.Pages.Account
{
    public class ResetUserPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ResetUserPasswordModel> _logger;

        public ResetUserPasswordModel(
            UserManager<ApplicationUser> userManager,
            ILogger<ResetUserPasswordModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        public bool Success { get; set; } = false;

        public void OnGet()
        {
            // Default values
            Username = "Ben";
            NewPassword = "P@ssword123";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(NewPassword))
            {
                ModelState.AddModelError(string.Empty, "Username and password are required");
                return Page();
            }

            try
            {
                // Find user by username
                var user = await _userManager.FindByNameAsync(Username);
                if (user == null)
                {
                    // Try by email
                    user = await _userManager.FindByEmailAsync(Username);
                }

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User not found");
                    return Page();
                }

                // Remove existing password
                var removeResult = await _userManager.RemovePasswordAsync(user);
                if (!removeResult.Succeeded)
                {
                    foreach (var error in removeResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, $"Error removing old password: {error.Description}");
                    }
                    return Page();
                }

                // Add new password
                var addResult = await _userManager.AddPasswordAsync(user, NewPassword);
                if (!addResult.Succeeded)
                {
                    foreach (var error in addResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, $"Error setting new password: {error.Description}");
                    }
                    return Page();
                }

                // Set user status to "Approved" if it's not already
                if (user.Status != "Approved")
                {
                    user.Status = "Approved";
                    await _userManager.UpdateAsync(user);
                }

                // Log success
                _logger.LogInformation($"Password reset successfully for user {Username}");
                Success = true;
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error resetting password for user {Username}");
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }
    }
} 