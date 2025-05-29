using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Barangay.Models;
using System.Threading.Tasks;
using Barangay.Data;
using Microsoft.EntityFrameworkCore;

namespace Barangay.Pages.User
{
    public class ProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ProfileModel(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        // Mark CurrentUser as nullable
        public ApplicationUser? CurrentUser { get; set; }
        
        public Patient? PatientData { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            CurrentUser = user;
            
            // Get patient data if it exists
            PatientData = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == user.Id);
            
            return Page();
        }
    }
}