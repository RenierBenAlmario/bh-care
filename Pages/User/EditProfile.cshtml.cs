using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using Barangay.Models;
using Microsoft.AspNetCore.Http;
using Barangay.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using Barangay.Services;
using Barangay.Extensions;

namespace Barangay.Pages.User
{
    public class EditProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<EditProfileModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IDataEncryptionService _encryptionService;

        public EditProfileModel(
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment,
            ILogger<EditProfileModel> logger,
            ApplicationDbContext context,
            IDataEncryptionService encryptionService)
        {
            _userManager = userManager;
            _environment = environment;
            _logger = logger;
            _context = context;
            _encryptionService = encryptionService;
            CurrentUser = new ApplicationUser();
        }

        [BindProperty]
        public ApplicationUser CurrentUser { get; set; }
        
        [BindProperty]
        public IFormFile? ProfileImage { get; set; }
        
        [BindProperty]
        [Display(Name = "PhilHealth ID")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "PhilHealth ID must be 12 digits")]
        public string? PhilHealthId { get; set; }
        
        [BindProperty]
        [Display(Name = "Emergency Contact Person")]
        [Required(ErrorMessage = "Emergency contact person is required")]
        [StringLength(100, ErrorMessage = "Emergency contact name cannot exceed 100 characters")]
        public string EmergencyContact { get; set; } = string.Empty;
        
        [BindProperty]
        [Display(Name = "Emergency Contact Number")]
        [Required(ErrorMessage = "Emergency contact number is required")]
        [RegularExpression(@"^(\+63|0)\d{10}$", ErrorMessage = "Please enter a valid Philippine phone number")]
        public string EmergencyContactNumber { get; set; } = string.Empty;
        
        [BindProperty]
        [Display(Name = "Allergies")]
        public string? Allergies { get; set; }
        
        [BindProperty]
        [Display(Name = "Medical History")]
        public string? MedicalHistory { get; set; }
        
        [BindProperty]
        [Display(Name = "Current Medications")]
        public string? CurrentMedications { get; set; }

        [BindProperty]
        [Display(Name = "Height (cm)")]
        [Range(1, 300, ErrorMessage = "Please enter a valid height between 1 and 300 cm")]
        public decimal? Height { get; set; }

        [BindProperty]
        [Display(Name = "Blood Type")]
        [RegularExpression(@"^(A|B|AB|O)[+-]$", ErrorMessage = "Please enter a valid blood type (A+, A-, B+, B-, AB+, AB-, O+, O-)")]
        public string? BloodType { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not found");
                    return NotFound("User not found");
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

                CurrentUser = new ApplicationUser
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    BirthDate = user.BirthDate,
                    Gender = user.Gender
                };

                PhilHealthId = user.PhilHealthId;
                
                // Ensure default profile image exists
                EnsureDefaultProfileImage();
                
                // Get patient data if it exists
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.UserId == user.Id);
                
                if (patient != null)
                {
                    EmergencyContact = patient.EmergencyContact ?? string.Empty;
                    EmergencyContactNumber = patient.EmergencyContactNumber ?? string.Empty;
                    Allergies = patient.Allergies;
                    MedicalHistory = patient.MedicalHistory;
                    CurrentMedications = patient.CurrentMedications;
                    Height = patient.Height;
                    BloodType = patient.BloodType;
                }
                
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EditProfile GET");
                ErrorMessage = "An error occurred while loading your profile. Please try again.";
                return RedirectToPage("/User/Profile");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                user.FirstName = CurrentUser.FirstName;
                user.LastName = CurrentUser.LastName;
                user.PhoneNumber = CurrentUser.PhoneNumber;
                user.Address = CurrentUser.Address;
                user.BirthDate = CurrentUser.BirthDate;
                user.Gender = CurrentUser.Gender;
                user.PhilHealthId = PhilHealthId;

                // Handle profile picture upload
                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(ProfileImage.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("ProfileImage", "Only .jpg, .jpeg and .png files are allowed.");
                        return Page();
                    }

                    if (ProfileImage.Length > 5 * 1024 * 1024) // 5MB limit
                    {
                        ModelState.AddModelError("ProfileImage", "File size cannot exceed 5MB.");
                        return Page();
                    }

                    try
                    {
                        string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "profiles");
                        Directory.CreateDirectory(uploadsFolder); // Ensure directory exists

                        string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(ProfileImage.FileName)}";
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ProfileImage.CopyToAsync(fileStream);
                        }

                        // Delete old profile picture if exists
                        if (!string.IsNullOrEmpty(user.ProfileImage) && 
                            user.ProfileImage != "/images/default-profile.png" &&
                            System.IO.File.Exists(Path.Combine(_environment.WebRootPath, user.ProfileImage.TrimStart('/'))))
                        {
                            System.IO.File.Delete(Path.Combine(_environment.WebRootPath, user.ProfileImage.TrimStart('/')));
                        }

                        user.ProfileImage = "/images/profiles/" + uniqueFileName;
                        user.ProfilePicture = user.ProfileImage;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error uploading profile image");
                        ModelState.AddModelError("ProfileImage", "Error uploading profile image. Please try again.");
                        return Page();
                    }
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    SuccessMessage = "Profile updated successfully.";

                    // Update or create patient record
                    var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);
                    if (patient == null)
                    {
                        patient = new Patient
                        {
                            UserId = user.Id,
                            Status = "Active",
                            CreatedAt = DateTime.Now
                        };
                        _context.Patients.Add(patient);
                    }

                    // Update patient information
                    patient.Name = user.FullName ?? string.Empty;
                    patient.Gender = user.Gender ?? string.Empty;
                    patient.Email = user.Email;
                    patient.Address = user.Address;
                    patient.ContactNumber = user.PhoneNumber;
                    
                    patient.BirthDate = DateTime.TryParse(user.BirthDate, out var parsedPatientBirthDate) ? parsedPatientBirthDate : DateTime.MinValue;
                    
                    patient.EmergencyContact = EmergencyContact;
                    patient.EmergencyContactNumber = EmergencyContactNumber;
                    patient.Allergies = Allergies;
                    patient.MedicalHistory = MedicalHistory;
                    patient.CurrentMedications = CurrentMedications;
                    patient.Height = Height;
                    patient.BloodType = BloodType;
                    patient.UpdatedAt = DateTime.Now;

                    // Save changes to database
                    await _context.SaveChangesAsync();

                    // Update appointments if they exist
                    var appointments = await _context.Appointments
                        .Where(a => a.PatientId == user.Id)
                        .ToListAsync();

                    if (appointments.Any())
                    {
                        foreach (var appointment in appointments)
                        {
                            appointment.PatientName = user.FullName ?? string.Empty;
                            appointment.ContactNumber = user.PhoneNumber;
                            appointment.DateOfBirth = DateTime.TryParse(user.BirthDate, out var parsedAppointmentBirthDate) ? parsedAppointmentBirthDate : DateTime.MinValue;
                            appointment.Address = user.Address;
                            appointment.EmergencyContact = EmergencyContact;
                            appointment.EmergencyContactNumber = EmergencyContactNumber;
                            appointment.Allergies = Allergies;
                            appointment.MedicalHistory = MedicalHistory;
                            appointment.CurrentMedications = CurrentMedications;
                            appointment.UpdatedAt = DateTime.Now;
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToPage("/User/Profile");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EditProfile POST");
                ModelState.AddModelError("", "An error occurred while saving your profile. Please try again.");
                return Page();
            }
        }

        private void EnsureDefaultProfileImage()
        {
            string defaultImagePath = Path.Combine(_environment.WebRootPath, "images", "default-profile.png");
            if (!System.IO.File.Exists(defaultImagePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(defaultImagePath)!);
                using var stream = System.IO.File.Create(defaultImagePath);
                // You would need to add default image content here
            }
        }
    }
}