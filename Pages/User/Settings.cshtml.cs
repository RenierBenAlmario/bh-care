using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Barangay.Models;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Services;
using Barangay.Extensions;

namespace Barangay.Pages.User
{
    [Authorize]
    public class SettingsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<SettingsModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IDataEncryptionService _encryptionService;

        public SettingsModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment environment,
            ILogger<SettingsModel> logger,
            ApplicationDbContext context,
            IDataEncryptionService encryptionService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
            _logger = logger;
            _context = context;
            _encryptionService = encryptionService;
        }

        [BindProperty]
        public UserProfileViewModel UserProfile { get; set; } = new UserProfileViewModel();

        [BindProperty]
        public ChangePasswordViewModel PasswordModel { get; set; } = new ChangePasswordViewModel();

        [BindProperty]
        public IFormFile? ProfilePicture { get; set; }

        [BindProperty]
        public NotificationSettingsViewModel NotificationSettings { get; set; } = new NotificationSettingsViewModel();

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Decrypt user data for authorized users
            user = user.DecryptSensitiveData(_encryptionService, User);
            
            // Manually decrypt Email and PhoneNumber since they're not marked with [Encrypted] attribute
            if (!string.IsNullOrEmpty(user.Email) && _encryptionService.IsEncrypted(user.Email))
            {
                user.Email = user.Email.DecryptForUser(_encryptionService, User);
            }
            if (!string.IsNullOrEmpty(user.PhoneNumber) && _encryptionService.IsEncrypted(user.PhoneNumber))
            {
                user.PhoneNumber = user.PhoneNumber.DecryptForUser(_encryptionService, User);
            }

            UserProfile = new UserProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                DateOfBirth = DateTime.TryParse(user.BirthDate, out var parsedBirthDate) ? parsedBirthDate : DateTime.MinValue,
                Gender = user.Gender
            };

            NotificationSettings = new NotificationSettingsViewModel
            {
                AppointmentReminders = user.AppointmentReminders,
                PrescriptionAlerts = user.PrescriptionAlerts,
                HealthTips = user.HealthTips
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Ensure password fields from the Security tab don't affect Profile save
            // Remove both prefixed and unprefixed keys (some tag helpers/binders may add both)
            string[] pwdKeys = new[] {
                "PasswordModel.OldPassword", "PasswordModel.NewPassword", "PasswordModel.ConfirmPassword",
                "OldPassword", "NewPassword", "ConfirmPassword", nameof(PasswordModel)
            };
            foreach (var k in pwdKeys)
            {
                if (ModelState.ContainsKey(k)) ModelState.Remove(k);
            }

            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    foreach(var error in ModelState[key].Errors)
                    {
                        _logger.LogWarning("ModelState Error in Settings Page - Key: {Key}, Error: {ErrorMessage}", key, error.ErrorMessage);
                    }
                }
                StatusMessage = "Error: Could not save profile. Please check your inputs.";
                return Page();
            }

            // Apply email change (username follows email in this app)
            var incomingEmail = (UserProfile.Email ?? string.Empty).Trim();
            if (!string.IsNullOrEmpty(incomingEmail) && !string.Equals(user.Email, incomingEmail, StringComparison.OrdinalIgnoreCase))
            {
                var setEmail = await _userManager.SetEmailAsync(user, incomingEmail);
                if (!setEmail.Succeeded)
                {
                    foreach (var err in setEmail.Errors)
                        ModelState.AddModelError(string.Empty, err.Description);
                    StatusMessage = "Error: Could not update email.";
                    return Page();
                }

                var setUserName = await _userManager.SetUserNameAsync(user, incomingEmail);
                if (!setUserName.Succeeded)
                {
                    foreach (var err in setUserName.Errors)
                        ModelState.AddModelError(string.Empty, err.Description);
                    StatusMessage = "Error: Could not update username.";
                    return Page();
                }
            }

            // Update other profile fields
            user.FirstName = UserProfile.FirstName;
            user.LastName = UserProfile.LastName;
            user.PhoneNumber = UserProfile.PhoneNumber;
            user.Address = UserProfile.Address;
            user.BirthDate = UserProfile.DateOfBirth.ToString("yyyy-MM-dd");
            user.Gender = UserProfile.Gender;
            user.FullName = $"{UserProfile.FirstName} {UserProfile.LastName}";
            user.UpdatedAt = DateTime.Now;

            // Handle profile picture upload
            if (ProfilePicture != null && ProfilePicture.Length > 0)
            {
                // Optional: Delete old picture if it exists
                if (!string.IsNullOrEmpty(user.ProfileImage))
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, "images", "profiles", user.ProfileImage);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "profiles");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + ProfilePicture.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfilePicture.CopyToAsync(fileStream);
                }
                user.ProfileImage = uniqueFileName;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Sync to Patient record if it exists
                try
                {
                    var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);
                    if (patient != null)
                    {
                        patient.FullName = user.FullName ?? $"{user.FirstName} {user.LastName}".Trim();
                        if (!string.IsNullOrWhiteSpace(user.Gender)) patient.Gender = user.Gender;
                        if (DateTime.TryParse(user.BirthDate, out var parsedBirthDate) && parsedBirthDate != DateTime.MinValue) 
                            patient.BirthDate = parsedBirthDate;
                        if (!string.IsNullOrWhiteSpace(user.Address)) patient.Address = user.Address;
                        if (!string.IsNullOrWhiteSpace(user.PhoneNumber)) patient.ContactNumber = user.PhoneNumber;
                        if (!string.IsNullOrWhiteSpace(user.Email)) patient.Email = user.Email;
                        patient.UpdatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error syncing patient record for user {UserId}", user.Id);
                    // Do not fail the request; just log.
                }

                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your profile has been updated";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            // Only validate the PasswordModel for this handler
            ModelState.Clear();
            TryValidateModel(PasswordModel, nameof(PasswordModel));
            if (!ModelState.IsValid) return Page();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, 
                PasswordModel.OldPassword, PasswordModel.NewPassword);
            
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your password has been changed.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSaveNotificationSettingsAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Update notification settings in user profile
            user.AppointmentReminders = NotificationSettings.AppointmentReminders;
            user.PrescriptionAlerts = NotificationSettings.PrescriptionAlerts;
            user.HealthTips = NotificationSettings.HealthTips;
            user.UpdatedAt = DateTime.Now;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                StatusMessage = "Your notification preferences have been saved.";
            }
            else
            {
                StatusMessage = "Error: Could not save notification preferences.";
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return RedirectToPage();
        }
    }

    public class UserProfileViewModel
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class NotificationSettingsViewModel
    {
        [Display(Name = "Appointment Reminders")]
        public bool AppointmentReminders { get; set; } = true;

        [Display(Name = "Prescription Alerts")]
        public bool PrescriptionAlerts { get; set; } = true;

        [Display(Name = "Health Tips & Updates")]
        public bool HealthTips { get; set; } = false;
    }
} 