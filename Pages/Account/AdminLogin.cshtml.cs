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
using Barangay.Services;
using Barangay.Extensions;

namespace Barangay.Pages.Account
{
    public class AdminLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminLoginModel> _logger;
        private readonly IOTPService _otpService;
        private readonly IEmailService _emailService;
        private readonly IDataEncryptionService _encryptionService;

        public AdminLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AdminLoginModel> logger,
            IOTPService otpService,
            IEmailService emailService,
            IDataEncryptionService encryptionService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _otpService = otpService;
            _emailService = emailService;
            _encryptionService = encryptionService;
        }

        [BindProperty]
        [Required(ErrorMessage = "Admin Email or Username is required")]
        [Display(Name = "Admin Email or Username")]
        public string EmailOrUsername { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Admin Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Admin Password")]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public bool RememberMe { get; set; }

        [BindProperty]
        [Display(Name = "OTP Code")]
        public string? OTPCode { get; set; }

        [BindProperty]
        public bool ShowOTPField { get; set; } = false;

        [BindProperty]
        public bool OTPRequired { get; set; } = false;

        [BindProperty]
        public string? UserEmail { get; set; }

        private async Task<ApplicationUser?> FindUserAsync(string emailOrUsername)
        {
            // First try the standard methods
            var user = await _userManager.FindByEmailAsync(emailOrUsername);
            if (user == null)
            {
                _logger.LogInformation($"Admin user not found by email {emailOrUsername}, trying username lookup");
                user = await _userManager.FindByNameAsync(emailOrUsername);
            }
            
            // If still not found, try searching through all users for encrypted emails or direct username match
            if (user == null)
            {
                _logger.LogInformation($"Admin user not found by standard methods, searching through all users for: {emailOrUsername}");
                
                // Normalize the email for comparison
                var normalizedEmail = emailOrUsername.ToUpperInvariant();
                
                // Get all users and check their encrypted emails
                var allUsers = _userManager.Users.ToList();
                foreach (var candidateUser in allUsers)
                {
                    try
                    {
                        // First try direct username match
                        if (string.Equals(candidateUser.UserName, emailOrUsername, StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogInformation($"Found admin user by direct username match: {candidateUser.Id}");
                            user = candidateUser;
                            break;
                        }
                        
                        // If it looks like an email, check encrypted emails
                        if (emailOrUsername.Contains("@"))
                        {
                            bool emailMatch = false;
                            
                            // Check Email field
                            if (!string.IsNullOrEmpty(candidateUser.Email))
                            {
                                if (_encryptionService.IsEncrypted(candidateUser.Email))
                                {
                                    try
                                    {
                                        var decryptedEmail = _encryptionService.Decrypt(candidateUser.Email);
                                        emailMatch = string.Equals(decryptedEmail, emailOrUsername, StringComparison.OrdinalIgnoreCase);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogWarning(ex, $"Error decrypting Email for admin user {candidateUser.Id}");
                                    }
                                }
                                else
                                {
                                    // Email is not encrypted, compare directly
                                    emailMatch = string.Equals(candidateUser.Email, emailOrUsername, StringComparison.OrdinalIgnoreCase);
                                }
                            }
                            
                            // Check NormalizedEmail field if Email didn't match
                            if (!emailMatch && !string.IsNullOrEmpty(candidateUser.NormalizedEmail))
                            {
                                if (_encryptionService.IsEncrypted(candidateUser.NormalizedEmail))
                                {
                                    try
                                    {
                                        var decryptedNormalizedEmail = _encryptionService.Decrypt(candidateUser.NormalizedEmail);
                                        emailMatch = string.Equals(decryptedNormalizedEmail, normalizedEmail, StringComparison.OrdinalIgnoreCase);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogWarning(ex, $"Error decrypting NormalizedEmail for admin user {candidateUser.Id}");
                                    }
                                }
                                else
                                {
                                    // NormalizedEmail is not encrypted, compare directly
                                    emailMatch = string.Equals(candidateUser.NormalizedEmail, normalizedEmail, StringComparison.OrdinalIgnoreCase);
                                }
                            }
                            
                            if (emailMatch)
                            {
                                _logger.LogInformation($"Found admin user by encrypted email match: {candidateUser.Id}");
                                user = candidateUser;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Error processing admin user {candidateUser.Id}");
                        // Continue to next user
                    }
                }
            }
            
            return user;
        }

        private IActionResult? GetAdminDashboardRedirect(ApplicationUser user, IList<string> roles)
        {
            // Only allow Admin and Admin Staff roles through admin login
            if (roles.Contains("Admin"))
            {
                _logger.LogInformation($"Admin user {user.Email} logging in via admin portal");
                return RedirectToPage("/Admin/AdminDashboard");
            }
            if (roles.Contains("Admin Staff"))
            {
                _logger.LogInformation($"Admin Staff user {user.Email} logging in via admin portal");
                return RedirectToPage("/AdminStaff/Dashboard");
            }
            
            // If user doesn't have admin roles, deny access
            _logger.LogWarning($"Non-admin user {user.Email} attempted to access admin login");
            return null;
        }

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
                    ModelState.AddModelError(string.Empty, "Admin email or password cannot be empty.");
                    return Page();
                }

                var user = await FindUserAsync(EmailOrUsername);
                
                if (user == null)
                {
                    // Log the issue but show generic error to user
                    _logger.LogWarning($"Admin login attempt failed: User with email/username {EmailOrUsername} not found.");
                    ModelState.AddModelError(string.Empty, "Invalid admin credentials.");
                    return Page();
                }

                // Check if user has admin roles
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Admin") && !roles.Contains("Admin Staff"))
                {
                    _logger.LogWarning($"Non-admin user {user.Email} attempted admin login");
                    ModelState.AddModelError(string.Empty, "Access denied. Admin credentials required.");
                    return Page();
                }

                // Enhanced logging to diagnose user account state
                _logger.LogInformation(
                    $"Admin user found: ID={user.Id}, Email={user.Email}, UserName={user.UserName}, " +
                    $"NormalizedEmail={user.NormalizedEmail}, NormalizedUserName={user.NormalizedUserName}, " +
                    $"Status={user.Status}, EncryptedStatus={user.EncryptedStatus}, " +
                    $"EmailConfirmed={user.EmailConfirmed}, LockoutEnabled={user.LockoutEnabled}, " +
                    $"LockoutEnd={user.LockoutEnd}, AccessFailedCount={user.AccessFailedCount}");

                // Admin users bypass approval check
                _logger.LogInformation($"Admin user {user.Email} bypassing approval check");

                // First try direct password verification
                var passwordCheck = await _userManager.CheckPasswordAsync(user, Password);
                _logger.LogInformation($"Admin direct password check result: {passwordCheck}");

                if (!passwordCheck)
                {
                    _logger.LogWarning($"Admin password verification failed for user {user.Email}");
                    ModelState.AddModelError(string.Empty, "Invalid admin credentials.");
                    return Page();
                }

                // Check if OTP is required for this admin user
                var userEmail = EmailOrUsername;
                var isOTPRequired = await _otpService.IsOTPRequiredAsync(userEmail);
                
                if (isOTPRequired)
                {
                    // If OTP is required but not provided, show OTP field
                    if (string.IsNullOrEmpty(OTPCode))
                    {
                        _logger.LogInformation($"OTP required for admin Gmail user: {userEmail}");
                        
                        // Generate and send OTP
                        var otp = await _otpService.GenerateOTPAsync(userEmail);
                        var emailSent = await _emailService.SendOTPEmailAsync(userEmail, otp);
                        
                        if (emailSent)
                        {
                            // Redirect to OTP verification page with admin flag
                            return RedirectToPage("/Account/OTPVerification", new { 
                                email = userEmail, 
                                password = Password, 
                                rememberMe = RememberMe,
                                isAdmin = true
                            });
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Failed to send OTP. Please try again later.");
                            return Page();
                        }
                    }
                }

                // Since password is correct, ensure the user can log in
                if (user.LockoutEnd != null && user.LockoutEnd > System.DateTimeOffset.Now)
                {
                    _logger.LogWarning($"Removing lockout for admin user {user.Email}");
                    await _userManager.SetLockoutEndDateAsync(user, null);
                    await _userManager.ResetAccessFailedCountAsync(user);
                }

                if (!user.EmailConfirmed)
                {
                    _logger.LogInformation($"Auto-confirming email for admin user {user.Email}");
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                }

                // Try to sign in with email if username doesn't match email
                var signInIdentifier = user.UserName;
                
                _logger.LogInformation($"Attempting admin sign in with identifier: {signInIdentifier}");
                var result = await _signInManager.PasswordSignInAsync(signInIdentifier, Password, RememberMe, lockoutOnFailure: false);
                
                if (!result.Succeeded && !string.IsNullOrEmpty(user.Email))
                {
                    _logger.LogInformation($"Admin username sign-in failed, trying email: {user.Email}");
                    result = await _signInManager.PasswordSignInAsync(user.Email, Password, RememberMe, lockoutOnFailure: false);
                }

                _logger.LogInformation($"Final admin sign-in result: {result.Succeeded}");

                if (result.Succeeded)
                {
                    _logger.LogInformation("Admin user logged in successfully.");
                    
                    _logger.LogInformation($"Admin user roles: {string.Join(", ", roles)}");

                    var claims = new List<Claim>
                    {
                        new Claim("UserId", user.Id),
                        new Claim("Status", user.Status),
                        new Claim("IsActive", user.IsActive.ToString()),
                        new Claim("IsAdminLogin", "true")
                    };

                    var existingClaims = await _userManager.GetClaimsAsync(user);
                    await _userManager.RemoveClaimsAsync(user, existingClaims);
                    await _userManager.AddClaimsAsync(user, claims);

                    var redirectResult = GetAdminDashboardRedirect(user, roles);
                    if (redirectResult != null)
                    {
                        return redirectResult;
                    }

                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError(string.Empty, "The admin account does not have proper admin roles assigned.");
                    return Page();
                }
                
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { RememberMe = RememberMe });
                }
                
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Admin user account locked out.");
                    return RedirectToPage("./Lockout");
                }

                // If we get here, something went wrong with the sign in process
                _logger.LogWarning($"Admin login failed for {user.Email} with correct password but sign-in failed");
                ModelState.AddModelError(string.Empty, "Admin login failed. Please try again.");
                return Page();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error during admin login process");
                ModelState.AddModelError(string.Empty, "An error occurred during admin login. Please try again later.");
                return Page();
            }
        }
    }
}
