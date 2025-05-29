using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace Barangay.Pages.User
{
    [Authorize]
    public class RecordsModel : PageModel
    {
        public void OnGet()
        {
            // In a real application, this would load the user's medical records
        }
    }
} 