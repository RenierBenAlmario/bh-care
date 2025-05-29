using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Barangay.Data;
using Barangay.Models;
using System.Threading.Tasks;

namespace Barangay.Pages.Admin
{
    public class DeleteStaffMemberModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeleteStaffMemberModel> _logger;

        public DeleteStaffMemberModel(ApplicationDbContext context, ILogger<DeleteStaffMemberModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string? StaffMemberId { get; set; }  // Make nullable

        // Add the missing StaffMember property
        public StaffMember? StaffMember { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Staff member ID is null or empty");
                return NotFound();
            }

            // Convert string id to int
            if (!int.TryParse(id, out int staffMemberId))
            {
                _logger.LogWarning($"Invalid staff member ID format: {id}");
                return NotFound();
            }

            var staffMember = await _context.StaffMembers.FindAsync(staffMemberId);

            if (staffMember == null)
            {
                _logger.LogWarning($"Staff member not found: {id}");
                return NotFound();
            }

            // Set the staff member to the view
            StaffMember = staffMember;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Staff member ID is null or empty");
                return NotFound();
            }

            // Convert string id to int
            if (!int.TryParse(id, out int staffMemberId))
            {
                _logger.LogWarning($"Invalid staff member ID format: {id}");
                return NotFound();
            }

            var staffMember = await _context.StaffMembers.FindAsync(staffMemberId);

            if (staffMember == null)
            {
                _logger.LogWarning($"Staff member not found: {id}");
                return NotFound();
            }

            // Delete the staff member
            _context.StaffMembers.Remove(staffMember);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Staff member deleted: {id}");
            // Fix: Update the redirect path to the correct page
            return RedirectToPage("/Admin/StaffMembers"); // or whatever the correct page name is
        }
    }
}