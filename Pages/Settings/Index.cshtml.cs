using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Barangay.Models;
using Barangay.Data;

namespace Barangay.Pages.Settings
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public string FirstName { get; set; } = "Juan";
        public string LastName { get; set; } = "Dela Cruz";
        public string Email { get; set; } = "juan.delacruz@example.com";
        public string PhoneNumber { get; set; } = "+63 912 345 6789";
        public DateTime? DateOfBirth { get; set; } = new DateTime(1990, 5, 15);
        public string Gender { get; set; } = "Male";
        public string Address { get; set; } = "123 Rizal St.";
        public string City { get; set; } = "Manila";
        public string Province { get; set; } = "Metro Manila";
        public string PostalCode { get; set; } = "1000";
        public string EmergencyContactName { get; set; } = "Maria Dela Cruz";
        public string EmergencyContactPhone { get; set; } = "+63 912 987 6543";
        public bool TwoFactorEnabled { get; set; } = false;
        public DateTime LastLogin { get; set; } = DateTime.Now.AddDays(-2);
        public DateTime LastPasswordChange { get; set; } = DateTime.Now.AddMonths(-3);

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // In a real application, you would populate these properties from the user's data
            // For now, we're using sample data defined in the properties above

            return Page();
        }
    }
} 