using Barangay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Barangay.Services;
using Barangay.Extensions;

namespace Barangay.Pages.User
{
    public class VitalDetailModel : PageModel
    {
        private readonly Barangay.Data.ApplicationDbContext _context;
        private readonly IDataEncryptionService _encryptionService;

        public VitalDetailModel(Barangay.Data.ApplicationDbContext context, IDataEncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
        }

        public VitalSign VitalSign { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            VitalSign = await _context.VitalSigns.FindAsync(id);

            if (VitalSign == null)
            {
                return NotFound();
            }

            // Decrypt vital signs data before displaying
            VitalSign.DecryptVitalSignData(_encryptionService, User);

            return Page();
        }
    }
}
