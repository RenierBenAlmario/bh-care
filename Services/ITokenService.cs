using Barangay.Models;

namespace Barangay.Services
{
    public interface ITokenService
    {
        /// <summary>
        /// Generates a secure token for accessing a specific resource
        /// </summary>
        /// <param name="resourceType">Type of resource (User, Nurse, Doctor, Admin)</param>
        /// <param name="resourceId">ID of the resource</param>
        /// <param name="originalUrl">Original URL being protected</param>
        /// <param name="expirationHours">Token expiration in hours (default: 24)</param>
        /// <returns>Generated token</returns>
        Task<string> GenerateTokenAsync(string resourceType, string resourceId, string originalUrl, int expirationHours = 24);

        /// <summary>
        /// Gets the resource ID associated with a token
        /// </summary>
        /// <param name="token">Token to validate</param>
        /// <returns>Resource ID if token is valid, null otherwise</returns>
        Task<string?> GetResourceIdAsync(string token);

        /// <summary>
        /// Validates a token and returns token information
        /// </summary>
        /// <param name="token">Token to validate</param>
        /// <returns>Token information if valid, null otherwise</returns>
        Task<UrlToken?> ValidateTokenAsync(string token);

        /// <summary>
        /// Marks a token as used
        /// </summary>
        /// <param name="token">Token to mark as used</param>
        /// <param name="ipAddress">IP address of the request</param>
        /// <param name="userAgent">User agent of the request</param>
        /// <returns>True if successful</returns>
        Task<bool> MarkTokenAsUsedAsync(string token, string? ipAddress = null, string? userAgent = null);

        /// <summary>
        /// Cleans up expired tokens
        /// </summary>
        /// <returns>Number of tokens cleaned up</returns>
        Task<int> CleanupExpiredTokensAsync();
    }
}
