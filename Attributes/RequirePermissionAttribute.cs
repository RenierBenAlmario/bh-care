using Barangay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Barangay.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _permissionName;

        public RequirePermissionAttribute(string permissionName)
        {
            _permissionName = permissionName;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Skip authorization if action is decorated with [AllowAnonymous]
            if (context.ActionDescriptor.EndpointMetadata.Any(em => em is AllowAnonymousAttribute))
            {
                return;
            }

            // Ensure user is authenticated
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new ChallengeResult();
                return;
            }

            // Get the current user ID
            var userId = context.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new ForbidResult();
                return;
            }

            // Get permission service from DI
            var permissionService = context.HttpContext.RequestServices.GetService(typeof(IPermissionService)) as IPermissionService;
            if (permissionService == null)
            {
                throw new InvalidOperationException("IPermissionService is not registered in the service collection.");
            }

            // Check if user has the required permission
            if (!await permissionService.UserHasPermissionAsync(userId, _permissionName))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
} 