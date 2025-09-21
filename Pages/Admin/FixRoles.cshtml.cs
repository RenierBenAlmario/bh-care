using Barangay.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Barangay.Pages.Admin
{
    public class FixRolesModel : PageModel
    {
        private readonly IFixUserRolesService _fixUserRolesService;

        public FixRolesModel(IFixUserRolesService fixUserRolesService)
        {
            _fixUserRolesService = fixUserRolesService;
        }

        [TempData]
        public int FixCount { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            FixCount = await _fixUserRolesService.FixUserRolesAsync();
            return Page();
        }
    }
}
