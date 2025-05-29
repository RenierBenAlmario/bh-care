using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Barangay.Data;
using Barangay.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse")]
    public class SettingsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SettingsModel> _logger;

        public SettingsModel(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<SettingsModel> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
            Settings = new UserSettingsViewModel();
        }

        [BindProperty]
        public UserSettingsViewModel Settings { get; set; }

        public class UserSettingsViewModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; }

            [Phone]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Email Notifications")]
            public bool EmailNotifications { get; set; } = true;

            [Display(Name = "Appointment Reminders")]
            public bool AppointmentReminders { get; set; } = true;

            [Display(Name = "Patient Updates")]
            public bool PatientUpdates { get; set; } = true;

            [Display(Name = "Theme")]
            public string Theme { get; set; } = "light";

            [Display(Name = "Records Per Page")]
            public int RecordsPerPage { get; set; } = 25;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                // Load user settings
                Settings.Email = user.Email;
                Settings.FullName = user.FullName;
                Settings.PhoneNumber = user.PhoneNumber;

                // In a real implementation, you would load user preferences from the database
                // For now, we'll use default values
                Settings.EmailNotifications = true;
                Settings.AppointmentReminders = true;
                Settings.PatientUpdates = true;
                Settings.Theme = "light";
                Settings.RecordsPerPage = 25;

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user settings");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                // Update user information
                var emailChanged = user.Email != Settings.Email;
                if (emailChanged)
                {
                    user.Email = Settings.Email;
                    user.UserName = Settings.Email; // Assuming username is the same as email
                    user.NormalizedEmail = Settings.Email.ToUpper();
                    user.NormalizedUserName = Settings.Email.ToUpper();
                }

                user.FullName = Settings.FullName;
                user.PhoneNumber = Settings.PhoneNumber;

                // Update user in the database
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }

                // Handle password change if provided
                var currentPassword = Request.Form["currentPassword"];
                var newPassword = Request.Form["newPassword"];
                var confirmPassword = Request.Form["confirmPassword"];

                if (!string.IsNullOrEmpty(currentPassword) && !string.IsNullOrEmpty(newPassword))
                {
                    if (newPassword != confirmPassword)
                    {
                        ModelState.AddModelError(string.Empty, "The new password and confirmation password do not match.");
                        return Page();
                    }

                    var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                    if (!changePasswordResult.Succeeded)
                    {
                        foreach (var error in changePasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return Page();
                    }
                }

                // In a real implementation, you would save user preferences to the database
                // For now, we'll just show a success message

                TempData["SuccessMessage"] = "Your settings have been updated successfully.";
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user settings");
                ModelState.AddModelError(string.Empty, "An error occurred while saving your settings. Please try again.");
                return Page();
            }
        }
    }
} 