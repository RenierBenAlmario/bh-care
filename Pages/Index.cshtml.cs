using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Barangay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Barangay.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<IndexModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me")]
            public bool RememberMe { get; set; }
        }

        public void OnGet()
        {
            // This method is called when the page is initially loaded
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Ensure Email and Password are not null before attempting sign-in
                    if (string.IsNullOrEmpty(Input.Email) || string.IsNullOrEmpty(Input.Password))
                    {
                        ErrorMessage = "Email or password cannot be empty.";
                        return Page();
                    }

                    // Try to find the user by email first
                    var user = await _userManager.FindByEmailAsync(Input.Email);
                    
                    // If not found by email, try by username
                    if (user == null)
                    {
                        _logger.LogInformation($"User not found by email {Input.Email}, trying username lookup");
                        user = await _userManager.FindByNameAsync(Input.Email);
                    }
                    
                    if (user == null)
                    {
                        // Log the issue but show generic error to user
                        _logger.LogWarning($"Login attempt failed: User with email/username {Input.Email} not found.");
                        ErrorMessage = "Invalid email or password.";
                        return Page();
                    }

                    // Enhanced logging to diagnose user account state
                    _logger.LogInformation(
                        $"User found: ID={user.Id}, Email={user.Email}, UserName={user.UserName}, " +
                        $"NormalizedEmail={user.NormalizedEmail}, NormalizedUserName={user.NormalizedUserName}, " +
                        $"Status={user.Status}, EncryptedStatus={user.EncryptedStatus}, " +
                        $"EmailConfirmed={user.EmailConfirmed}, LockoutEnabled={user.LockoutEnabled}");

                    // Check if user account is approved - check both Status and EncryptedStatus
                    // First check if user is admin
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        // Admin users bypass approval check
                        _logger.LogInformation($"Admin user {user.Email} bypassing approval check");
                    }
                    else if (user.Status == "Pending" || user.EncryptedStatus == "Pending")
                    {
                        ErrorMessage = "Your account is pending approval by an administrator. Please check back later.";
                        return Page();
                    }

                    // First try direct password verification
                    var passwordCheck = await _userManager.CheckPasswordAsync(user, Input.Password);
                    _logger.LogInformation($"Direct password check result: {passwordCheck}");

                    if (!passwordCheck)
                    {
                        _logger.LogWarning($"Password verification failed for user {user.Email}");
                        ErrorMessage = "Invalid email or password.";
                        return Page();
                    }

                    // Since password is correct, ensure the user can log in
                    if (user.LockoutEnd != null && user.LockoutEnd > System.DateTimeOffset.Now)
                    {
                        _logger.LogWarning($"Removing lockout for user {user.Email}");
                        await _userManager.SetLockoutEndDateAsync(user, null);
                        await _userManager.ResetAccessFailedCountAsync(user);
                    }

                    // Try to sign in with username first
                    var signInIdentifier = user.UserName;
                    
                    _logger.LogInformation($"Attempting sign in with identifier: {signInIdentifier}");
                    var result = await _signInManager.PasswordSignInAsync(signInIdentifier, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                    
                    if (!result.Succeeded && !string.IsNullOrEmpty(user.Email))
                    {
                        _logger.LogInformation($"Username sign-in failed, trying email: {user.Email}");
                        result = await _signInManager.PasswordSignInAsync(user.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                    }

                    _logger.LogInformation($"Final sign-in result: {result.Succeeded}");

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in successfully.");
                        
                        // Redirect based on role
                        if (await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            return RedirectToPage("/Admin/AdminDashboard");
                        }
                        else if (await _userManager.IsInRoleAsync(user, "Admin Staff"))
                        {
                            _logger.LogInformation("Redirecting Admin Staff user to dashboard");
                            return RedirectToPage("/AdminStaff/Dashboard");
                        }
                        else if (await _userManager.IsInRoleAsync(user, "Doctor"))
                        {
                            return RedirectToPage("/Doctor/DoctorDashboard");
                        }
                        else if (await _userManager.IsInRoleAsync(user, "Nurse"))
                        {
                            _logger.LogInformation("Redirecting Nurse to Dashboard");
                            return RedirectToPage("/Nurse/NurseDashboard");
                        }
                        else 
                        {
                            return RedirectToPage("/User/UserDashboard");
                        }
                    }
                    
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("/Account/LoginWith2fa", new { RememberMe = Input.RememberMe });
                    }
                    
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToPage("/Account/Lockout");
                    }

                    // If we get here, something went wrong with the sign in process
                    _logger.LogWarning($"Login failed for {user.Email} with correct password but sign-in failed");
                    ErrorMessage = "Login failed. Please try again.";
                    return Page();
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "Error during login process");
                    ErrorMessage = "An error occurred during login. Please try again later.";
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}

