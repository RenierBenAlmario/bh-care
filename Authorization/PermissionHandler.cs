using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using System.Threading.Tasks;

namespace Barangay.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ApplicationDbContext _context;

        public PermissionHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if (context.User == null)
            {
                return;
            }

            var userId = context.User.FindFirst(c => c.Type == "sub")?.Value ??
                        context.User.FindFirst(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            // Check if user has the required permission
            var hasPermission = await _context.StaffPermissions
                .Include(sp => sp.Permission)
                .AnyAsync(sp => sp.StaffMemberId.ToString() == userId && 
                               sp.Permission.Name == requirement.Permission);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
} 