using Barangay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Barangay.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Find the Doctor role
            var doctorRole = await roleManager.FindByNameAsync("Doctor");
            if (doctorRole == null)
            {
                // Log or handle the case where the role doesn't exist
                return;
            }

            // Find the AccessDoctorDashboard permission
            var permission = await context.Permissions.FirstOrDefaultAsync(p => p.Name == "AccessDoctorDashboard");
            if (permission == null)
            {
                // Log or handle the case where the permission doesn't exist
                return;
            }

            // Check if the role already has the permission
            var roleHasPermission = await context.RolePermissions
                .AnyAsync(rp => rp.RoleId == doctorRole.Id && rp.PermissionId == permission.Id);

            if (!roleHasPermission)
            {
                // Assign the permission to the role
                context.RolePermissions.Add(new RolePermission { RoleId = doctorRole.Id, PermissionId = permission.Id });
                await context.SaveChangesAsync();
            }
        }
    }
}
