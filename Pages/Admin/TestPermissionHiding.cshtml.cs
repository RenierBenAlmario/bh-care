using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Barangay.Pages.Admin
{
    [Authorize(Policy = "RequireAdminRole")]
    public class TestPermissionHidingModel : PageModel
    {
        public void OnGet()
        {
            // Display the test page
        }
    }
}
