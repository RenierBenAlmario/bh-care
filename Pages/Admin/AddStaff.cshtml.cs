using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Barangay.Pages.Admin
{
    [Authorize(Policy = "AccessAdminDashboard")]
    public class AddStaffModel : PageModel
    {
        public void OnGet()
        {
            // This page redirects to AddStaffMember
        }
    }
} 