using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse,Head Nurse")]
    public class ReportsModel : PageModel
    {
        public void OnGet()
        {
            // This will be populated with actual report data in a real implementation
        }
    }
} 