using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace Barangay.Pages.User
{
    [Authorize]
    public class PrescriptionsModel : PageModel
    {
        public void OnGet()
        {
            // In a real application, this would load the user's prescriptions
        }
    }
} 