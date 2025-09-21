using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Barangay.Services
{
    // Adds additional role claims to Admins so [Authorize(Roles="...")] passes universally
    public class AdminRoleClaimsTransformation : IClaimsTransformation
    {
        private static readonly string[] ExtraRolesForAdmin = new[] { "Doctor", "Nurse", "Admin Staff" };

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var p = principal ?? new ClaimsPrincipal();

            // If user is Admin, ensure they also have Doctor/Nurse/Admin Staff role claims
            if (p.IsInRole("Admin"))
            {
                var identity = p.Identities.FirstOrDefault(i => i.IsAuthenticated) ?? p.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    foreach (var role in ExtraRolesForAdmin)
                    {
                        // Add role claim if not present
                        if (!p.IsInRole(role))
                        {
                            identity.AddClaim(new Claim(identity.RoleClaimType ?? ClaimTypes.Role, role));
                        }
                    }
                }
            }

            return Task.FromResult(p);
        }
    }
}
