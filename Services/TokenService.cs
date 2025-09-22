using Barangay.Data;
using Barangay.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Barangay.Services
{
    public class TokenService : ITokenService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TokenService> _logger;
        private readonly IConfiguration _configuration;

        public TokenService(ApplicationDbContext context, ILogger<TokenService> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<string> GenerateTokenAsync(string resourceType, string resourceId, string originalUrl, int expirationHours = 24)
        {
            try
            {
                // Generate a secure random token
                var tokenBytes = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(tokenBytes);
                }
                var token = Convert.ToBase64String(tokenBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");

                // Create token record
                var urlToken = new UrlToken
                {
                    Token = token,
                    ResourceType = resourceType,
                    ResourceId = resourceId,
                    OriginalUrl = originalUrl,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(expirationHours),
                    IsUsed = false
                };

                _context.UrlTokens.Add(urlToken);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Generated token for {ResourceType} ID {ResourceId} with URL {OriginalUrl}", 
                    resourceType, resourceId, originalUrl);

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating token for {ResourceType} ID {ResourceId}", resourceType, resourceId);
                throw;
            }
        }

        public async Task<string?> GetResourceIdAsync(string token)
        {
            try
            {
                var urlToken = await ValidateTokenAsync(token);
                return urlToken?.ResourceId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting resource ID for token");
                return null;
            }
        }

        public async Task<UrlToken?> ValidateTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Token validation failed: Token is null or empty");
                    return null;
                }

                var urlToken = await _context.UrlTokens
                    .FirstOrDefaultAsync(t => t.Token == token && !t.IsUsed);

                if (urlToken == null)
                {
                    _logger.LogWarning("Token validation failed: Token not found or already used");
                    return null;
                }

                if (urlToken.ExpiresAt < DateTime.UtcNow)
                {
                    _logger.LogWarning("Token validation failed: Token expired at {ExpiresAt}", urlToken.ExpiresAt);
                    return null;
                }

                _logger.LogInformation("Token validation successful for {ResourceType} ID {ResourceId}", 
                    urlToken.ResourceType, urlToken.ResourceId);

                return urlToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return null;
            }
        }

        public async Task<bool> MarkTokenAsUsedAsync(string token, string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                var urlToken = await _context.UrlTokens
                    .FirstOrDefaultAsync(t => t.Token == token);

                if (urlToken == null)
                {
                    _logger.LogWarning("Cannot mark token as used: Token not found");
                    return false;
                }

                urlToken.IsUsed = true;
                urlToken.UsedAt = DateTime.UtcNow;
                urlToken.ClientIp = ipAddress;
                urlToken.UserAgent = userAgent;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Token marked as used for {ResourceType} ID {ResourceId}", 
                    urlToken.ResourceType, urlToken.ResourceId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking token as used");
                return false;
            }
        }

        public async Task<int> CleanupExpiredTokensAsync()
        {
            try
            {
                var expiredTokens = await _context.UrlTokens
                    .Where(t => t.ExpiresAt < DateTime.UtcNow)
                    .ToListAsync();

                if (expiredTokens.Any())
                {
                    _context.UrlTokens.RemoveRange(expiredTokens);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Cleaned up {Count} expired tokens", expiredTokens.Count);
                }

                return expiredTokens.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired tokens");
                return 0;
            }
        }
    }
}
