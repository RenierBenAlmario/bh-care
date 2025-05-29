using Barangay.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Barangay.TagHelpers
{
    /// <summary>
    /// Tag helper to conditionally render content based on user permissions
    /// </summary>
    /// <example>
    /// <div permission="DeleteUsers">This will only show if user has DeleteUsers permission</div>
    /// <div permission-not="DeleteUsers">This will only show if user does NOT have DeleteUsers permission</div>
    /// </example>
    [HtmlTargetElement(Attributes = "permission")]
    [HtmlTargetElement(Attributes = "permission-not")]
    public class PermissionTagHelper : TagHelper
    {
        private readonly PermissionService _permissionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionTagHelper(PermissionService permissionService, IHttpContextAccessor httpContextAccessor)
        {
            _permissionService = permissionService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Show content if user has this permission
        /// </summary>
        [HtmlAttributeName("permission")]
        public string Permission { get; set; }

        /// <summary>
        /// Show content if user does NOT have this permission
        /// </summary>
        [HtmlAttributeName("permission-not")]
        public string ExcludePermission { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated != true)
            {
                output.SuppressOutput();
                return;
            }

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                output.SuppressOutput();
                return;
            }

            bool hasPermission = false;
            
            // Check for required permission
            if (!string.IsNullOrEmpty(Permission))
            {
                hasPermission = await _permissionService.UserHasPermissionAsync(userId, Permission);
                if (!hasPermission)
                {
                    output.SuppressOutput();
                    return;
                }
            }

            // Check for excluded permission
            if (!string.IsNullOrEmpty(ExcludePermission))
            {
                hasPermission = await _permissionService.UserHasPermissionAsync(userId, ExcludePermission);
                if (hasPermission)
                {
                    output.SuppressOutput();
                    return;
                }
            }
        }
    }
} 