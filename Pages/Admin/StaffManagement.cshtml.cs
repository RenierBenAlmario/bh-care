using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Barangay.Pages.Admin
{
    [Authorize(Policy = "AccessAdminDashboard")]
    public class StaffManagementModel : PageModel
    {
        public void OnGet()
        {
            // This page redirects to AdminDashboard
        }
    }
} 