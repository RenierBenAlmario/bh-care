using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Barangay.Services;
using Barangay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Barangay.Pages
{
    public class TestLoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IDataEncryptionService _encryptionService;
        private readonly ILogger<TestLoginModel> _logger;

        public TestLoginModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IDataEncryptionService encryptionService,
            ILogger<TestLoginModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _encryptionService = encryptionService;
            _logger = logger;
        }

        [BindProperty]
        public string Email { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        [BindProperty]
        public string SearchEmail { get; set; } = "";

        public string Result { get; set; } = "";
        public ApplicationUser? FoundUser { get; set; }
        public string DecryptedEmail { get; set; } = "";

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                Result = "Email and password are required.";
                return Page();
            }

            try
            {
                _logger.LogInformation($"Testing login for: {Email}");

                // Find user using the same method as the main login
                var user = await FindUserAsync(Email);
                
                if (user == null)
                {
                    Result = $"User not found for email: {Email}";
                    return Page();
                }

                _logger.LogInformation($"User found: {user.UserName}, Status: {user.Status}");

                // Check password
                var passwordCheck = await _userManager.CheckPasswordAsync(user, Password);
                if (!passwordCheck)
                {
                    Result = "Invalid password.";
                    return Page();
                }

                // Try to sign in
                var signInResult = await _signInManager.PasswordSignInAsync(user, Password, false, lockoutOnFailure: false);
                
                if (signInResult.Succeeded)
                {
                    Result = $"Login successful! Redirecting to dashboard...";
                    return RedirectToPage("/User/Dashboard");
                }
                else
                {
                    Result = $"Sign-in failed: {string.Join(", ", signInResult.GetType().GetProperties().Select(p => $"{p.Name}={p.GetValue(signInResult)}"))}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during test login");
                Result = $"Error: {ex.Message}";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSearchUserAsync()
        {
            if (string.IsNullOrEmpty(SearchEmail))
            {
                Result = "Search email is required.";
                return Page();
            }

            try
            {
                var user = await FindUserAsync(SearchEmail);
                
                if (user != null)
                {
                    FoundUser = user;
                    
                    // Try to decrypt the email
                    if (!string.IsNullOrEmpty(user.Email) && _encryptionService.IsEncrypted(user.Email))
                    {
                        try
                        {
                            DecryptedEmail = _encryptionService.Decrypt(user.Email);
                        }
                        catch (Exception ex)
                        {
                            DecryptedEmail = $"Decryption failed: {ex.Message}";
                        }
                    }
                    else
                    {
                        DecryptedEmail = user.Email ?? "No email";
                    }
                    
                    Result = "User found!";
                }
                else
                {
                    Result = "User not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user search");
                Result = $"Error: {ex.Message}";
            }

            return Page();
        }

        private async Task<ApplicationUser?> FindUserAsync(string emailOrUsername)
        {
            // First try the standard methods
            var user = await _userManager.FindByEmailAsync(emailOrUsername);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(emailOrUsername);
            }
            
            // If still not found, try searching through all users for encrypted emails or direct username match
            if (user == null)
            {
                _logger.LogInformation($"User not found by standard methods, searching through all users for: {emailOrUsername}");
                
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
                            _logger.LogInformation($"Found user by direct username match: {candidateUser.Id}");
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
                                        _logger.LogWarning(ex, $"Error decrypting Email for user {candidateUser.Id}");
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
                                        _logger.LogWarning(ex, $"Error decrypting NormalizedEmail for user {candidateUser.Id}");
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
                                _logger.LogInformation($"Found user by encrypted email match: {candidateUser.Id}");
                                user = candidateUser;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Error processing user {candidateUser.Id}");
                        // Continue to next user
                    }
                }
            }
            
            return user;
        }
    }
}
