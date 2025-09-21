using Barangay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Barangay.Services;
using Barangay.Extensions;

namespace Barangay.Pages.User
{
    public class MedicalRecordDetailModel : PageModel
    {
        private readonly Barangay.Data.ApplicationDbContext _context;
        private readonly IDataEncryptionService _encryptionService;

        public MedicalRecordDetailModel(Barangay.Data.ApplicationDbContext context, IDataEncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
        }

        public MedicalRecord MedicalRecord { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            MedicalRecord = await _context.MedicalRecords
                .Include(m => m.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (MedicalRecord == null)
            {
                return NotFound();
            }

            // Decrypt medical record data for display
            MedicalRecord.DecryptSensitiveData(_encryptionService, User);

            return Page();
        }
    }
}
