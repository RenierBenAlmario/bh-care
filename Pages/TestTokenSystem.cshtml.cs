using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Barangay.Pages
{
    [AllowAnonymous]
    public class TestTokenSystemModel : PageModel
    {
        public void OnGet()
        {
            // This page is accessible to everyone for testing purposes
        }
    }
}
