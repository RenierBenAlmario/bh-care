using Barangay.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Barangay.Services
{
    public interface IProtectedUrlService
    {
        /// <summary>
        /// Generates a protected URL with token for the specified resource
        /// </summary>
        /// <param name="resourceType">Type of resource (User, Nurse, Doctor, Admin)</param>
        /// <param name="resourceId">ID of the resource</param>
        /// <param name="action">Action name</param>
        /// <param name="controller">Controller name</param>
        /// <param name="routeValues">Route values</param>
        /// <param name="expirationHours">Token expiration in hours</param>
        /// <returns>Protected URL with token</returns>
        Task<string> GenerateProtectedUrlAsync(string resourceType, string resourceId, string action, string controller, object? routeValues = null, int expirationHours = 24);

        /// <summary>
        /// Generates a protected URL for Razor Pages
        /// </summary>
        /// <param name="resourceType">Type of resource</param>
        /// <param name="resourceId">ID of the resource</param>
        /// <param name="pageName">Page name</param>
        /// <param name="routeValues">Route values</param>
        /// <param name="expirationHours">Token expiration in hours</param>
        /// <returns>Protected URL with token</returns>
        Task<string> GenerateProtectedPageUrlAsync(string resourceType, string resourceId, string pageName, object? routeValues = null, int expirationHours = 24);
    }

    public class ProtectedUrlService : IProtectedUrlService
    {
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ProtectedUrlService> _logger;

        public ProtectedUrlService(
            ITokenService tokenService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ProtectedUrlService> logger)
        {
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<string> GenerateProtectedUrlAsync(string resourceType, string resourceId, string action, string controller, object? routeValues = null, int expirationHours = 24)
        {
            try
            {
                // Generate the original URL manually
                var baseUrl = _httpContextAccessor.HttpContext?.Request.Scheme + "://" + 
                             _httpContextAccessor.HttpContext?.Request.Host;
                var originalUrl = $"{baseUrl}/{controller}/{action}";
                
                // Generate token
                var token = await _tokenService.GenerateTokenAsync(resourceType, resourceId, originalUrl, expirationHours);
                
                // Create protected URL with token
                var protectedUrl = $"{originalUrl}?token={token}";
                
                _logger.LogInformation("Generated protected URL for {ResourceType} ID {ResourceId}: {Action}/{Controller}", 
                    resourceType, resourceId, action, controller);
                
                return protectedUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating protected URL for {ResourceType} ID {ResourceId}", resourceType, resourceId);
                throw;
            }
        }

        public async Task<string> GenerateProtectedPageUrlAsync(string resourceType, string resourceId, string pageName, object? routeValues = null, int expirationHours = 24)
        {
            try
            {
                // Generate the original URL manually
                var baseUrl = _httpContextAccessor.HttpContext?.Request.Scheme + "://" + 
                             _httpContextAccessor.HttpContext?.Request.Host;
                var originalUrl = $"{baseUrl}{pageName}";
                
                // Generate token
                var token = await _tokenService.GenerateTokenAsync(resourceType, resourceId, originalUrl, expirationHours);
                
                // Create protected URL with token
                var protectedUrl = $"{originalUrl}?token={token}";
                
                _logger.LogInformation("Generated protected page URL for {ResourceType} ID {ResourceId}: {PageName}", 
                    resourceType, resourceId, pageName);
                
                return protectedUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating protected page URL for {ResourceType} ID {ResourceId}", resourceType, resourceId);
                throw;
            }
        }
    }
}
