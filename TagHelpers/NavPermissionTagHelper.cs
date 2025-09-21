using Barangay.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Barangay.TagHelpers
{
    /// <summary>
    /// Tag helper to conditionally render navigation items based on user permissions
    /// </summary>
    /// <example>
    /// <li nav-permission="ManageAppointments">
    ///     <a class="nav-link" href="/Appointments">Appointments</a>
    /// </li>
    /// </example>
    [HtmlTargetElement(Attributes = "nav-permission")]
    public class NavPermissionTagHelper : TagHelper
    {
        private readonly IPermissionService _permissionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NavPermissionTagHelper(IPermissionService permissionService, IHttpContextAccessor httpContextAccessor)
        {
            _permissionService = permissionService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// The permission required to display this navigation item
        /// </summary>
        [HtmlAttributeName("nav-permission")]
        public string NavPermission { get; set; }

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

            // Check if user has the required permission
            if (!string.IsNullOrEmpty(NavPermission))
            {
                var hasPermission = await _permissionService.UserHasPermissionAsync(userId, NavPermission);
                if (!hasPermission)
                {
                    output.SuppressOutput();
                    return;
                }
            }
        }
    }
} 