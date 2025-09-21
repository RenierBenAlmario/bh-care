using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Barangay.Services;
using Barangay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Barangay.Pages
{
    public class TestEncryptionModel : PageModel
    {
        private readonly IDataEncryptionService _encryptionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TestEncryptionModel(IDataEncryptionService encryptionService, UserManager<ApplicationUser> userManager)
        {
            _encryptionService = encryptionService;
            _userManager = userManager;
        }

        [BindProperty]
        public string TestText { get; set; } = "";

        public string EncryptedResult { get; set; } = "";
        public string DecryptedResult { get; set; } = "";
        public bool IsEncrypted { get; set; }

        public List<UserEncryptionInfo> Users { get; set; } = new List<UserEncryptionInfo>();

        public async Task OnGetAsync()
        {
            await LoadUsers();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!string.IsNullOrEmpty(TestText))
            {
                EncryptedResult = _encryptionService.Encrypt(TestText);
                DecryptedResult = _encryptionService.Decrypt(EncryptedResult);
                IsEncrypted = _encryptionService.IsEncrypted(EncryptedResult);
            }

            await LoadUsers();
            return Page();
        }

        private async Task LoadUsers()
        {
            var allUsers = await _userManager.Users.ToListAsync();
            Users = allUsers.Select(user => new UserEncryptionInfo
            {
                UserName = user.UserName,
                Email = user.Email,
                DecryptedEmail = !string.IsNullOrEmpty(user.Email) && _encryptionService.IsEncrypted(user.Email) 
                    ? _encryptionService.Decrypt(user.Email) 
                    : user.Email,
                IsEmailEncrypted = !string.IsNullOrEmpty(user.Email) && _encryptionService.IsEncrypted(user.Email)
            }).ToList();
        }
    }

    public class UserEncryptionInfo
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string DecryptedEmail { get; set; }
        public bool IsEmailEncrypted { get; set; }
    }
}
