using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Barangay.Models;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Barangay.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        [Required(ErrorMessage = "Email or Username is required")]
        [Display(Name = "Email or Username")]
        public string EmailOrUsername { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public bool RememberMe { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Ensure Email and Password are not null before attempting sign-in
                if (string.IsNullOrEmpty(EmailOrUsername) || string.IsNullOrEmpty(Password))
                {
                    ModelState.AddModelError(string.Empty, "Email or password cannot be empty.");
                    return Page();
                }

                // Try to find the user by email first
                var user = await _userManager.FindByEmailAsync(EmailOrUsername);
                
                // If not found by email, try by username
                if (user == null)
                {
                    _logger.LogInformation($"User not found by email {EmailOrUsername}, trying username lookup");
                    user = await _userManager.FindByNameAsync(EmailOrUsername);
                }
                
                if (user == null)
                {
                    // Log the issue but show generic error to user
                    _logger.LogWarning($"Login attempt failed: User with email/username {EmailOrUsername} not found.");
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return Page();
                }

                // Enhanced logging to diagnose user account state
                _logger.LogInformation(
                    $"User found: ID={user.Id}, Email={user.Email}, UserName={user.UserName}, " +
                    $"NormalizedEmail={user.NormalizedEmail}, NormalizedUserName={user.NormalizedUserName}, " +
                    $"Status={user.Status}, EncryptedStatus={user.EncryptedStatus}, " +
                    $"EmailConfirmed={user.EmailConfirmed}, LockoutEnabled={user.LockoutEnabled}, " +
                    $"LockoutEnd={user.LockoutEnd}, AccessFailedCount={user.AccessFailedCount}");

                // Check if user account is approved - check both Status and EncryptedStatus
                // First check if user is admin
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    // Admin users bypass approval check
                    _logger.LogInformation($"Admin user {user.Email} bypassing approval check");
                }
                else if (user.Status == "Pending" || user.EncryptedStatus == "Pending")
                {
                    ModelState.AddModelError(string.Empty, "Your account is pending approval by an administrator. Please check back later.");
                    return Page();
                }

                // First try direct password verification
                var passwordCheck = await _userManager.CheckPasswordAsync(user, Password);
                _logger.LogInformation($"Direct password check result: {passwordCheck}");

                if (!passwordCheck)
                {
                    _logger.LogWarning($"Password verification failed for user {user.Email}");
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return Page();
                }

                // Since password is correct, ensure the user can log in
                if (user.LockoutEnd != null && user.LockoutEnd > System.DateTimeOffset.Now)
                {
                    _logger.LogWarning($"Removing lockout for user {user.Email}");
                    await _userManager.SetLockoutEndDateAsync(user, null);
                    await _userManager.ResetAccessFailedCountAsync(user);
                }

                if (!user.EmailConfirmed && (user.Status == "Approved" || user.EncryptedStatus == "Approved"))
                {
                    _logger.LogInformation($"Auto-confirming email for approved user {user.Email}");
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                }

                // Try to sign in with email if username doesn't match email
                var signInIdentifier = user.UserName;
                
                _logger.LogInformation($"Attempting sign in with identifier: {signInIdentifier}");
                var result = await _signInManager.PasswordSignInAsync(signInIdentifier, Password, RememberMe, lockoutOnFailure: false);
                
                if (!result.Succeeded && !string.IsNullOrEmpty(user.Email))
                {
                    _logger.LogInformation($"Username sign-in failed, trying email: {user.Email}");
                    result = await _signInManager.PasswordSignInAsync(user.Email, Password, RememberMe, lockoutOnFailure: false);
                }

                _logger.LogInformation($"Final sign-in result: {result.Succeeded}");

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in successfully.");
                    
                    var roles = await _userManager.GetRolesAsync(user);
                    _logger.LogInformation($"User roles: {string.Join(", ", roles)}");
                    
                    var claims = new List<Claim>
                    {
                        new Claim("UserId", user.Id)
                    };

                    await _userManager.AddClaimsAsync(user, claims);
                    
                    if (roles.Contains("Admin"))
                    {
                        return RedirectToPage("/Admin/AdminDashboard");
                    }
                    else if (roles.Contains("Admin Staff"))
                    {
                        _logger.LogInformation("Redirecting Admin Staff user to dashboard");
                        return RedirectToPage("/AdminStaff/Dashboard");
                    }
                    else if (roles.Contains("Doctor"))
                    {
                        return RedirectToPage("/Doctor/DoctorDashboard");
                    }
                    else if (roles.Contains("Nurse"))
                    {
                        _logger.LogInformation("Redirecting Nurse to Dashboard");
                        return RedirectToPage("/Nurse/NurseDashboard");
                    }
                    else if (roles.Contains("User"))
                    {
                        return RedirectToPage("/User/UserDashboard");
                    }
                    else
                    {
                        await _signInManager.SignOutAsync();
                        ModelState.AddModelError(string.Empty, "The account has not have user role.");
                        return Page();
                    }
                }
                
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { RememberMe = RememberMe });
                }
                
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }

                // If we get here, something went wrong with the sign in process
                _logger.LogWarning($"Login failed for {user.Email} with correct password but sign-in failed");
                ModelState.AddModelError(string.Empty, "Login failed. Please try again.");
                return Page();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error during login process");
                ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again later.");
                return Page();
            }
        }
    }
}