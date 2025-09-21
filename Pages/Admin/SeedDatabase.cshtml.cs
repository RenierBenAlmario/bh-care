using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Barangay.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class SeedDatabaseModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
