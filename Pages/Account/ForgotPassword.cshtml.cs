using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Barangay.Models;
using Barangay.Data;
using Barangay.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Barangay.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ForgotPasswordModel> _logger;

        public ForgotPasswordModel(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IEmailSender emailSender,
            ILogger<ForgotPasswordModel> logger)
        {
            _userManager = userManager;
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required(ErrorMessage = "Email address is required")]
            [EmailAddress(ErrorMessage = "Please enter a valid email address")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnPostSendOTPAsync(string email)
        {
            try
            {
                // Find user by email
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new JsonResult(new { success = false, error = "User not found" });
                }

                // Generate OTP
                var otp = GenerateOTP();
                var otpExpiry = DateTime.UtcNow.AddMinutes(10); // OTP expires in 10 minutes

                // Save OTP to database
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

                // Send OTP email
                await SendOTPEmailAsync(user.Email!, otp);

                _logger.LogInformation($"OTP sent to {email} for password reset");

                return new JsonResult(new { success = true, message = "OTP sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending OTP to {Email}", email);
                return new JsonResult(new { success = false, error = "Failed to send OTP" });
            }
        }

        public async Task<IActionResult> OnPostResetPasswordAsync(string email, string newPassword, string otpCode)
        {
            try
            {
                // Find user by email
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new JsonResult(new { success = false, error = "User not found" });
                }

                // Verify OTP
                var otpRecord = await _context.PasswordResetOTPs
                    .Where(o => o.Email == email && o.OTP == otpCode && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
                    .OrderByDescending(o => o.CreatedAt)
                    .FirstOrDefaultAsync();

                if (otpRecord == null)
                {
                    return new JsonResult(new { success = false, error = "Invalid or expired verification code" });
                }

                // Reset the password
                var result = await _userManager.ResetPasswordAsync(user, await _userManager.GeneratePasswordResetTokenAsync(user), newPassword);

                if (result.Succeeded)
                {
                    // Mark OTP as used
                    otpRecord.IsUsed = true;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Password reset successfully for user {email}");

                    return new JsonResult(new { success = true, message = "Password reset successfully" });
                }
                else
                {
                    return new JsonResult(new { success = false, error = string.Join(", ", result.Errors.Select(e => e.Description)) });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for {Email}", email);
                return new JsonResult(new { success = false, error = "Failed to reset password" });
            }
        }

        private string GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // 6-digit OTP
        }

        private async Task SendOTPEmailAsync(string email, string otp)
        {
            var subject = "Password Reset Verification Code - BHCare";
            var message = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 10px 10px 0 0; text-align: center;'>
                            <h1 style='margin: 0; font-size: 24px;'>🏥 BHCare</h1>
                            <p style='margin: 5px 0 0 0; font-size: 14px;'>Baesa Health Center</p>
                        </div>
                        
                        <div style='background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px;'>
                            <h2 style='color: #333; margin-top: 0;'>Password Reset Verification</h2>
                            
                            <p>Hello,</p>
                            
                            <p>You have requested to reset your password. Please use the following verification code:</p>
                            
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
