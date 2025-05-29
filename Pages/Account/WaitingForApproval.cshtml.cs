using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Barangay.Models;
using System.Threading.Tasks;

namespace Barangay.Pages.Account
{
    public class WaitingForApprovalModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<WaitingForApprovalModel> _logger;

        public WaitingForApprovalModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<WaitingForApprovalModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // If user is already verified, redirect to home
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null && user.Status == "Verified" && user.IsActive)
                {
                    return RedirectToPage("/Index");
                }
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Sign out the user
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            
            return RedirectToPage("/Index");
        }
    }
} 