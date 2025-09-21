using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Barangay.Models;
using System.Threading.Tasks;
using Barangay.Data;
using Microsoft.EntityFrameworkCore;
using Barangay.Services;
using Barangay.Extensions;

namespace Barangay.Pages.User
{
    public class ProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IDataEncryptionService _encryptionService;

        public ProfileModel(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IDataEncryptionService encryptionService)
        {
            _userManager = userManager;
            _context = context;
            _encryptionService = encryptionService;
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

            // Decrypt user data for authorized users
            user = user.DecryptSensitiveData(_encryptionService, User);
            
            // Manually decrypt Email and PhoneNumber since they're not marked with [Encrypted] attribute
            if (!string.IsNullOrEmpty(user.Email) && _encryptionService.IsEncrypted(user.Email))
            {
                user.Email = user.Email.DecryptForUser(_encryptionService, User);
            }
            if (!string.IsNullOrEmpty(user.PhoneNumber) && _encryptionService.IsEncrypted(user.PhoneNumber))
            {
                user.PhoneNumber = user.PhoneNumber.DecryptForUser(_encryptionService, User);
            }

            CurrentUser = user;
            
            // Get patient data if it exists
            PatientData = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == user.Id);
            
            return Page();
        }
    }
}