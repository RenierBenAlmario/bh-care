
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using System.Threading.Tasks;

namespace Barangay.Pages.User
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Prescription Prescription { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Prescription = await _context.Prescriptions
                .Include(p => p.Doctor)
                .Include(p => p.PrescriptionMedicines)
                    .ThenInclude(pm => pm.Medication)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Prescription == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
