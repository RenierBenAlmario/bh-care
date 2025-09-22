using Microsoft.AspNetCore.Mvc;
using Barangay.Services;

namespace Barangay.Controllers
{
    public class TokenTestController : Controller
    {
        private readonly ITokenService _tokenService;
        private readonly IProtectedUrlService _protectedUrlService;

        public TokenTestController(ITokenService tokenService, IProtectedUrlService protectedUrlService)
        {
            _tokenService = tokenService;
            _protectedUrlService = protectedUrlService;
        }

        public async Task<IActionResult> TestTokenCreation()
        {
            try
            {
                // Test token creation
                var testUserId = "test-user-123";
                var testUrl = "/User/UserDashboard";
                var token = await _tokenService.GenerateTokenAsync("User", testUserId, testUrl, 1); // 1 hour expiration
                
                return Json(new { 
                    success = true, 
                    message = "Token created successfully",
                    token = token,
                    userId = testUserId,
                    originalUrl = testUrl
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "Error creating token: " + ex.Message,
                    error = ex.ToString()
                });
            }
        }

        public async Task<IActionResult> TestTokenValidation(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "Token parameter is required" });
                }

                var urlToken = await _tokenService.ValidateTokenAsync(token);
                
                if (urlToken != null)
                {
                    return Json(new { 
                        success = true, 
                        message = "Token is valid",
                        tokenData = new {
                            id = urlToken.Id,
                            resourceType = urlToken.ResourceType,
                            resourceId = urlToken.ResourceId,
                            originalUrl = urlToken.OriginalUrl,
                            createdAt = urlToken.CreatedAt,
                            expiresAt = urlToken.ExpiresAt,
                            isUsed = urlToken.IsUsed
                        }
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Token is invalid or expired" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "Error validating token: " + ex.Message,
                    error = ex.ToString()
                });
            }
        }

        public async Task<IActionResult> TestProtectedUrl()
        {
            try
            {
                var testUserId = "test-user-123";
                var protectedUrl = await _protectedUrlService.GenerateProtectedPageUrlAsync("User", testUserId, "/User/UserDashboard", null, 1);
                
                return Json(new { 
                    success = true, 
                    message = "Protected URL created successfully",
                    protectedUrl = protectedUrl,
                    userId = testUserId
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "Error creating protected URL: " + ex.Message,
                    error = ex.ToString()
                });
            }
        }
    }
}
