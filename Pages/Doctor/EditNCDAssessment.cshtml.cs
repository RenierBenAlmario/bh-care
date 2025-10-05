using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Barangay.Pages.Doctor
{
    [Authorize(Roles = "Doctor,Head Doctor")]
    public class EditNCDAssessmentModel : PageModel
    {
        public IActionResult OnGet(int appointmentId)
        {
            // Redirect to the nurse edit page with proper layout handling
            return RedirectToPage("/Nurse/EditNCDAssessment", new { appointmentId = appointmentId });
        }
    }
}
