using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Barangay.Models;
using Barangay.Data;
using Barangay.Services;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Pages.Account
{
    public class VerifyOTPModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<VerifyOTPModel> _logger;

        public VerifyOTPModel(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IEmailSender emailSender,
            ILogger<VerifyOTPModel> logger)
        {
            _userManager = userManager;
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string Email { get; set; } = string.Empty;

        public class InputModel
        {
            [Required(ErrorMessage = "Verification code is required")]
            [StringLength(6, MinimumLength = 6, ErrorMessage = "Verification code must be 6 digits")]
            [RegularExpression(@"^\d{6}$", ErrorMessage = "Verification code must contain only numbers")]
            [Display(Name = "Verification Code")]
            public string OTP { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Invalid request. Please start the password reset process again.";
                return RedirectToPage("./ForgotPassword");
            }

            Email = email;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Get email from query parameter
                var email = Request.Query["email"].FirstOrDefault();
                if (string.IsNullOrEmpty(email))
                {
                    TempData["ErrorMessage"] = "Invalid request. Please start the password reset process again.";
                    return RedirectToPage("./ForgotPassword");
                }

                Email = email;

                // Find the OTP record
                var otpRecord = await _context.PasswordResetOTPs
                    .Where(o => o.Email == email && o.OTP == Input.OTP && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
                    .OrderByDescending(o => o.CreatedAt)
                    .FirstOrDefaultAsync();

                if (otpRecord == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid or expired verification code. Please try again.");
                    return Page();
                }

                // Mark OTP as used
                otpRecord.IsUsed = true;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"OTP verified successfully for {email}");

                // Redirect to reset password page with token
                TempData["SuccessMessage"] = "Code verified successfully! You can now reset your password.";
                return RedirectToPage("./ResetPassword", new { email = email, token = otpRecord.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for {Email}", Email);
                TempData["ErrorMessage"] = "An error occurred. Please try again later.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostResendOTPAsync()
        {
            try
            {
                var email = Request.Query["email"].FirstOrDefault();
                if (string.IsNullOrEmpty(email))
                {
                    return new JsonResult(new { success = false, error = "Invalid email" });
                }

                // Find user
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new JsonResult(new { success = false, error = "User not found" });
                }

                // Generate new OTP
                var otp = GenerateOTP();
                var otpExpiry = DateTime.UtcNow.AddMinutes(10);

                // Save new OTP
                var otpRecord = new PasswordResetOTP
                {
                    UserId = user.Id,
                    Email = user.Email!,
                    OTP = otp,
                    ExpiresAt = otpExpiry,
                    CreatedAt = DateTime.UtcNow,
                    IsUsed = false
                };

                _context.PasswordResetOTPs.Add(otpRecord);
                await _context.SaveChangesAsync();

                // Send new OTP email
                await SendOTPEmailAsync(user.Email!, otp);

                _logger.LogInformation($"OTP resent to {email}");

                return new JsonResult(new { success = true, message = "New verification code sent!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending OTP to {Email}", Request.Query["email"].FirstOrDefault());
                return new JsonResult(new { success = false, error = "Failed to resend code" });
            }
        }

        private string GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // 6-digit OTP
        }

        private async Task SendOTPEmailAsync(string email, string otp)
        {
            var subject = "New Password Reset Verification Code - BHCare";
            var message = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 10px 10px 0 0; text-align: center;'>
                            <h1 style='margin: 0; font-size: 24px;'>🏥 BHCare</h1>
                            <p style='margin: 5px 0 0 0; font-size: 14px;'>Baesa Health Center</p>
                        </div>
                        
                        <div style='background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px;'>
                            <h2 style='color: #333; margin-top: 0;'>New Verification Code</h2>
                            
                            <p>Hello,</p>
                            
                            <p>You have requested a new verification code. Please use the following code:</p>
                            
                            <div style='background: white; border: 2px dashed #667eea; border-radius: 10px; padding: 20px; text-align: center; margin: 20px 0;'>
                                <h1 style='color: #667eea; font-size: 36px; margin: 0; letter-spacing: 5px;'>{otp}</h1>
                            </div>
                            
                            <p><strong>Important:</strong></p>
                            <ul>
                                <li>This code will expire in <strong>10 minutes</strong></li>
                                <li>Enter this code exactly as shown</li>
                                <li>If you didn't request this, please ignore this email</li>
                            </ul>
                            
                            <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;'>
                            
                            <p style='font-size: 12px; color: #666; margin-bottom: 0;'>
                                This is an automated message from BHCare. Please do not reply to this email.
                            </p>
                        </div>
                    </div>
                </body>
                </html>";

            await _emailSender.SendEmailAsync(email, subject, message);
        }
    }
}
