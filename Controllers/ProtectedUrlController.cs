using Barangay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Barangay.Controllers
{
    [Authorize]
    public class ProtectedUrlController : Controller
    {
        private readonly IProtectedUrlService _protectedUrlService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<ProtectedUrlController> _logger;

        public ProtectedUrlController(
            IProtectedUrlService protectedUrlService,
            ITokenService tokenService,
            ILogger<ProtectedUrlController> logger)
        {
            _protectedUrlService = protectedUrlService;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// Example: Generate a protected URL for Doctor PatientList
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GenerateDoctorPatientListUrl()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid user ID");
                }

                var resourceType = DetermineResourceType(User);
                var protectedUrl = await _protectedUrlService.GenerateProtectedPageUrlAsync(
                    resourceType, userId, "/Doctor/PatientList");

                return Json(new { protectedUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating protected URL for Doctor PatientList");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Example: Generate a protected URL for Nurse Dashboard
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GenerateNurseDashboardUrl()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid user ID");
                }

                var resourceType = DetermineResourceType(User);
                var protectedUrl = await _protectedUrlService.GenerateProtectedPageUrlAsync(
                    resourceType, userId, "/Nurse/Dashboard");

                return Json(new { protectedUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating protected URL for Nurse Dashboard");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Example: Generate a protected URL for Admin User Management
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GenerateAdminUserManagementUrl()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid user ID");
                }

                var resourceType = DetermineResourceType(User);
                var protectedUrl = await _protectedUrlService.GenerateProtectedPageUrlAsync(
                    resourceType, userId, "/Admin/UserManagement");

                return Json(new { protectedUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating protected URL for Admin User Management");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Example: Generate a protected URL for User Dashboard
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GenerateUserDashboardUrl()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid user ID");
                }

                var resourceType = DetermineResourceType(User);
                var protectedUrl = await _protectedUrlService.GenerateProtectedPageUrlAsync(
                    resourceType, userId, "/User/UserDashboard");

                return Json(new { protectedUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating protected URL for User Dashboard");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Example: Generate a protected URL for a specific user (Admin functionality)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,System Administrator")]
        public async Task<IActionResult> GenerateProtectedUrlForUser(string userId, string pageName)
        {
            try
            {
                var resourceType = "User"; // Default for user-specific URLs
                var protectedUrl = await _protectedUrlService.GenerateProtectedPageUrlAsync(
                    resourceType, userId, pageName);

                return Json(new { protectedUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating protected URL for user {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Example: Validate a token and get resource information
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ValidateToken(string token)
        {
            try
            {
                var urlToken = await _tokenService.ValidateTokenAsync(token);
                if (urlToken == null)
                {
                    return Json(new { valid = false, message = "Invalid or expired token" });
                }

                return Json(new
                {
                    valid = true,
                    resourceType = urlToken.ResourceType,
                    resourceId = urlToken.ResourceId,
                    originalUrl = urlToken.OriginalUrl,
                    expiresAt = urlToken.ExpiresAt,
                    createdAt = urlToken.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Example: Get token statistics (Admin functionality)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,System Administrator")]
        public IActionResult GetTokenStatistics()
        {
            try
            {
                // This would require additional methods in TokenService
                // For now, return a placeholder response
                return Json(new
                {
                    totalTokens = 0,
                    activeTokens = 0,
                    expiredTokens = 0,
                    usedTokens = 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting token statistics");
                return StatusCode(500, "Internal server error");
            }
        }

        private static string DetermineResourceType(ClaimsPrincipal user)
        {
            if (user.IsInRole("Admin") || user.IsInRole("System Administrator"))
                return "Admin";
            if (user.IsInRole("Doctor"))
                return "Doctor";
            if (user.IsInRole("Nurse") || user.IsInRole("Head Nurse"))
                return "Nurse";
            if (user.IsInRole("User"))
                return "User";
            
            return "User"; // Default fallback
        }
    }
}
