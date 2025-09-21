using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Barangay.Services
{
    public interface IOTPService
    {
        Task<string> GenerateOTPAsync(string email);
        Task<bool> ValidateOTPAsync(string email, string otp);
        Task<bool> IsOTPRequiredAsync(string email);
    }

    public class OTPService : IOTPService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<OTPService> _logger;
        private readonly Random _random = new Random();

        public OTPService(IMemoryCache cache, ILogger<OTPService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<string> GenerateOTPAsync(string email)
        {
            try
            {
                // Generate truly random 6-digit OTP using cryptographic random
                var randomBytes = new byte[4];
                using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomBytes);
                }
                
                // Convert to 6-digit number (100000 to 999999)
                var randomNumber = Math.Abs(BitConverter.ToInt32(randomBytes, 0));
                var otp = (100000 + (randomNumber % 900000)).ToString();
                
                // Store OTP in cache with 5-minute expiration
                var cacheKey = $"otp_{email.ToLower()}";
                _cache.Set(cacheKey, otp, TimeSpan.FromMinutes(5));
                
                _logger.LogInformation($"Random OTP generated for {email}: {otp}");
                
                return Task.FromResult(otp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating OTP for {email}");
                throw;
            }
        }

        public Task<bool> ValidateOTPAsync(string email, string otp)
        {
            try
            {
                var cacheKey = $"otp_{email.ToLower()}";
                _logger.LogInformation($"Looking for OTP in cache with key: {cacheKey}");
                
                if (_cache.TryGetValue(cacheKey, out string? storedOTP))
                {
                    _logger.LogInformation($"Found stored OTP: {storedOTP}, Provided OTP: {otp}");
                    var isValid = storedOTP == otp;
                    
                    if (isValid)
                    {
                        // Remove OTP from cache after successful validation
                        _cache.Remove(cacheKey);
                        _logger.LogInformation($"OTP validated successfully for {email}");
                    }
                    else
                    {
                        _logger.LogWarning($"Invalid OTP provided for {email}. Expected: {storedOTP}, Got: {otp}");
                    }
                    
                    return Task.FromResult(isValid);
                }
                
                _logger.LogWarning($"No OTP found in cache for {email} with key: {cacheKey}");
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating OTP for {email}");
                return Task.FromResult(false);
            }
        }

        public Task<bool> IsOTPRequiredAsync(string email)
        {
            try
            {
                // Check if email is provided
                if (string.IsNullOrEmpty(email))
                    return Task.FromResult(false);

                var emailLower = email.ToLower();
                
                // Test accounts are exempt from OTP requirement
                var exemptAccounts = new List<string>
                {
                    "admin@example.com",
                    "doctor@example.com",
                    "nurse@example.com"
                };

                foreach (var account in exemptAccounts)
                {
                    if (emailLower == account)
                    {
                        _logger.LogInformation($"Test account exempt from OTP: {email}");
                        return Task.FromResult(false);
                    }
                }
                
                // Gmail accounts require OTP
                var gmailDomains = new List<string>
                {
                    "@gmail.com",
                    "@googlemail.com"
                };

                foreach (var domain in gmailDomains)
                {
                    if (emailLower.EndsWith(domain))
                    {
                        _logger.LogInformation($"OTP required for Gmail account: {email}");
                        return Task.FromResult(true);
                    }
                }

                _logger.LogInformation($"OTP not required for account: {email}");
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking OTP requirement for {email}");
                return Task.FromResult(false);
            }
        }
    }
}
