using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Barangay.Data;
using Barangay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Barangay.Pages.Admin
{
    public class EditStaffMemberModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EditStaffMemberModel> _logger;

        public EditStaffMemberModel(ApplicationDbContext context, ILogger<EditStaffMemberModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        [BindProperty]
        public StaffInputModel Input { get; set; }

        public List<SelectListItem> Departments { get; set; }
        public List<SelectListItem> Positions { get; set; }
        public List<SelectListItem> Roles { get; set; }

        public class StaffInputModel
        {
            [Required]
            [Display(Name = "Full Name")]
            public string Name { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Department { get; set; }

            [Required]
            public string Position { get; set; }

            public string Specialization { get; set; }

            public string LicenseNumber { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Contact Number")]
            public string ContactNumber { get; set; }

            [Required]
            [Display(Name = "Working Days")]
            public string WorkingDays { get; set; }

            [Required]
            [Display(Name = "Working Hours")]
            public string WorkingHours { get; set; }

            public int MaxDailyPatients { get; set; } = 20;

            public bool IsActive { get; set; } = true;

            [Required]
            public string Role { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Staff member ID is null or empty");
                return NotFound();
            }

            _logger.LogInformation($"Attempting to find staff member with ID: {id}");

            // Check if the ID is a user ID (GUID) or a staff member ID (int)
            StaffMember staffMember = null;
            
            // First try to find by staff ID (numeric)
            if (int.TryParse(id, out int staffId))
            {
                _logger.LogInformation($"Looking up staff member by numeric ID: {staffId}");
                staffMember = await _context.StaffMembers.FindAsync(staffId);
            }
            
            // If not found and it looks like a GUID, try to find by UserId
            if (staffMember == null && Guid.TryParse(id, out _))
            {
                _logger.LogInformation($"Looking up staff member by User ID (GUID): {id}");
                
                // Get all staff members to debug
                var allStaffMembers = await _context.StaffMembers.ToListAsync();
                _logger.LogInformation($"Total staff members in database: {allStaffMembers.Count}");
                foreach (var staff in allStaffMembers)
                {
                    _logger.LogInformation($"Staff ID: {staff.Id}, User ID: {staff.UserId}, Name: {staff.Name}");
                }
                
                staffMember = await _context.StaffMembers
                    .FirstOrDefaultAsync(m => m.UserId == id);
                    
                // If still not found, try with case insensitive comparison
                if (staffMember == null)
                {
                    _logger.LogInformation($"Trying case-insensitive User ID lookup for: {id}");
                    staffMember = await _context.StaffMembers
                        .FirstOrDefaultAsync(m => m.UserId.ToLower() == id.ToLower());
                }
            }

            if (staffMember == null)
            {
                _logger.LogWarning($"Staff member not found: {id}");
                TempData["ErrorMessage"] = $"Staff member with ID {id} could not be found. The staff member may have been deleted or the ID is invalid.";
                return RedirectToPage("/Admin/StaffList");
            }

            _logger.LogInformation($"Found staff member: {staffMember.Id}, Name: {staffMember.Name}");

            // Populate the input model
            Input = new StaffInputModel
            {
                Name = staffMember.Name,
                Email = staffMember.Email,
                Department = staffMember.Department ?? "General",
                Position = staffMember.Position ?? "Staff",
                Specialization = staffMember.Specialization ?? "",
                LicenseNumber = staffMember.LicenseNumber ?? "",
                ContactNumber = staffMember.ContactNumber ?? "",
                WorkingDays = staffMember.WorkingDays ?? "Monday-Friday",
                WorkingHours = staffMember.WorkingHours ?? "9:00 AM - 5:00 PM",
                MaxDailyPatients = staffMember.MaxDailyPatients,
                IsActive = staffMember.IsActive,
                Role = staffMember.Role
            };

            // Populate dropdown lists
            PopulateDropdowns();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return Page();
            }

            // Use the same improved lookup logic as in OnGetAsync
            StaffMember staffMember = null;
            
            // First try to find by staff ID (numeric)
            if (int.TryParse(Id, out int staffId))
            {
                staffMember = await _context.StaffMembers.FindAsync(staffId);
            }
            
            // If not found and it looks like a GUID, try to find by UserId
            if (staffMember == null && Guid.TryParse(Id, out _))
            {
                staffMember = await _context.StaffMembers
                    .FirstOrDefaultAsync(m => m.UserId == Id);
                    
                // If still not found, try with case insensitive comparison
                if (staffMember == null)
                {
                    staffMember = await _context.StaffMembers
                        .FirstOrDefaultAsync(m => m.UserId.ToLower() == Id.ToLower());
                }
            }

            if (staffMember == null)
            {
                _logger.LogWarning($"Staff member not found: {Id}");
                TempData["ErrorMessage"] = $"Staff member with ID {Id} could not be found. The staff member may have been deleted or the ID is invalid.";
                return RedirectToPage("/Admin/StaffList");
            }

            // Update staff member properties
            staffMember.Name = Input.Name;
            staffMember.Email = Input.Email;
            staffMember.Department = Input.Department;
            staffMember.Position = Input.Position;
            staffMember.Specialization = Input.Specialization;
            staffMember.LicenseNumber = Input.LicenseNumber;
            staffMember.ContactNumber = Input.ContactNumber;
            staffMember.WorkingDays = Input.WorkingDays;
            staffMember.WorkingHours = Input.WorkingHours;
            staffMember.MaxDailyPatients = Input.MaxDailyPatients;
            staffMember.IsActive = Input.IsActive;
            staffMember.Role = Input.Role;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Staff member updated: {staffMember.Id}");
                TempData["SuccessMessage"] = "Staff member updated successfully.";
                return RedirectToPage("/Admin/StaffList");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating staff member: {Id}");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the staff member. Please try again.");
                PopulateDropdowns();
                return Page();
            }
        }

        private void PopulateDropdowns()
        {
            // Populate departments
            Departments = new List<SelectListItem>
            {
                new SelectListItem { Value = "General", Text = "General" },
                new SelectListItem { Value = "Pediatrics", Text = "Pediatrics" },
                new SelectListItem { Value = "Internal Medicine", Text = "Internal Medicine" },
                new SelectListItem { Value = "OB-GYN", Text = "OB-GYN" },
                new SelectListItem { Value = "Surgery", Text = "Surgery" },
                new SelectListItem { Value = "Emergency", Text = "Emergency" }
            };

            // Populate positions
            Positions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Doctor", Text = "Doctor" },
                new SelectListItem { Value = "Nurse", Text = "Nurse" },
                new SelectListItem { Value = "Staff", Text = "Staff" },
                new SelectListItem { Value = "Administrator", Text = "Administrator" }
            };

            // Populate roles
            Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Doctor", Text = "Doctor" },
                new SelectListItem { Value = "Nurse", Text = "Nurse" },
                new SelectListItem { Value = "Staff", Text = "Staff" },
                new SelectListItem { Value = "Admin", Text = "Admin" },
                new SelectListItem { Value = "User", Text = "User" }
            };
        }
    }
} 