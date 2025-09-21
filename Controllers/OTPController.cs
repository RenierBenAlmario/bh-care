using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using System.Threading.Tasks;
using System;
using System.Text.Json;

namespace Barangay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OTPController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<OTPController> _logger;

        public OTPController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            ILogger<OTPController> logger)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        // POST api/OTP/send
        [HttpPost("send")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest(new { success = false, message = "Email is required" });
                }

                // Generate a 6-digit OTP
                Random random = new Random();
                string otp = random.Next(100000, 999999).ToString();
                
                // Log the OTP for testing purposes
                _logger.LogInformation("Generated OTP {OTP} for {Email}", otp, request.Email);

                try
                {
                    // Store the OTP with the email and expiry time (10 minutes)
                    var otpEntry = await _context.EmailVerifications
                        .FirstOrDefaultAsync(e => e.Email == request.Email);

                    if (otpEntry == null)
                    {
                        otpEntry = new EmailVerification
                        {
                            Email = request.Email,
                            VerificationCode = otp,
                            ExpiryTime = DateTime.UtcNow.AddMinutes(10)
                        };
                        _context.EmailVerifications.Add(otpEntry);
                    }
                    else
                    {
                        otpEntry.VerificationCode = otp;
                        otpEntry.ExpiryTime = DateTime.UtcNow.AddMinutes(10);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (Exception dbEx)
                {
                    // Log the database error but continue - we'll still try to send the email
                    _logger.LogError(dbEx, "Database error when storing OTP. Continuing with email send.");
                }

                // Send OTP to email
                string message = $@"
                    <h3>Your Email Verification Code</h3>
                    <p>Please use the following code to verify your email address:</p>
                    <h2 style='background-color: #f5f5f5; padding: 10px; font-family: monospace; letter-spacing: 5px;'>{otp}</h2>
                    <p>This code will expire in 10 minutes.</p>
                    <p>If you did not request this code, please ignore this email.</p>";

                try
                {
                    await _emailSender.SendEmailAsync(request.Email, "Email Verification Code", message);
                    return Ok(new { success = true, message = "Verification code sent successfully" });
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Error sending email to {Email}", request.Email);
                    return StatusCode(500, new { success = false, message = "Failed to send verification code. Please check your email settings." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending OTP to {Email}", request.Email);
                return StatusCode(500, new { success = false, message = "Failed to send verification code" });
            }
        }

        // POST api/OTP/verify
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Otp))
                {
                    return BadRequest(new { success = false, message = "Email and OTP are required" });
                }

                bool otpVerified = false;
                
                try
                {
                    // Find the OTP entry for the email
                    var otpEntry = await _context.EmailVerifications
                        .FirstOrDefaultAsync(e => e.Email == request.Email);

                    if (otpEntry == null)
                    {
                        return BadRequest(new { success = false, message = "No verification code found for this email" });
                    }

                    // Check if OTP is expired
                    if (otpEntry.ExpiryTime < DateTime.UtcNow)
                    {
                        return BadRequest(new { success = false, message = "Verification code has expired" });
                    }

                    // Verify OTP
                    if (otpEntry.VerificationCode != request.Otp)
                    {
                        return BadRequest(new { success = false, message = "Invalid verification code" });
                    }

                    // Update verification record
                    otpEntry.IsVerified = true;
                    otpEntry.VerifiedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    
                    otpVerified = true;
                }
                catch (Exception dbEx)
                {
                    _logger.LogError(dbEx, "Database error during OTP verification. Falling back to defaults.");
                    // Fall through to user update - we'll still try to mark the email as verified
                }

                if (otpVerified)
                {
                    // Mark email as verified in AspNetUsers if the user exists
                    var user = await _userManager.FindByEmailAsync(request.Email);
                    if (user != null)
                    {
                        user.EmailConfirmed = true;
                        await _userManager.UpdateAsync(user);
                    }

                    return Ok(new { success = true, message = "Email verified successfully" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to verify email" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for {Email}", request.Email);
                return StatusCode(500, new { success = false, message = "Failed to verify email" });
            }
        }
    }

    public class SendOtpRequest
    {
        public string Email { get; set; }
    }

    public class VerifyOtpRequest
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
} 