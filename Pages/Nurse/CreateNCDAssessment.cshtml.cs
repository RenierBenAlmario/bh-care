using System;
using System.Threading.Tasks;
using System.Linq;
using Barangay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Barangay.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Barangay.Services;
using Barangay.Extensions;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse,Head Nurse")]
    public class CreateNCDAssessmentModel : PageModel
    {
        private readonly EncryptedDbContext _context;
        private readonly ILogger<CreateNCDAssessmentModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataEncryptionService _encryptionService;

        public CreateNCDAssessmentModel(
            EncryptedDbContext context,
            ILogger<CreateNCDAssessmentModel> logger,
            UserManager<ApplicationUser> userManager,
            IDataEncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _encryptionService = encryptionService;
            Assessment = new NCDRiskAssessment();
        }

        [BindProperty]
        public NCDRiskAssessment Assessment { get; set; }
        
        public string PatientName { get; set; }
        public string PatientAddress { get; set; }
        public string PatientBarangay { get; set; }
        public string PatientPhone { get; set; }
        public int PatientAge { get; set; }
        public string HealthFacility { get; set; } = "Barangay Health Center 161";
        public string FamilyNo { get; set; } = "C-001";

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? appointmentId)
        {
            try
            {
                if (appointmentId == null)
                {
                    _logger.LogWarning("Appointment ID not provided");
                    return NotFound("Appointment ID must be provided");
                }

                _logger.LogInformation("Loading data for appointment: {AppointmentId}", appointmentId);
                
                // Find the appointment
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                    .FirstOrDefaultAsync(a => a.Id == appointmentId);

                if (appointment == null)
                {
                    _logger.LogWarning("Appointment with ID {Id} not found", appointmentId);
                    return NotFound("Appointment not found");
                }

                // Check if assessment already exists
                var existingAssessment = await _context.NCDRiskAssessments
                    .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

                if (existingAssessment != null)
                {
                    _logger.LogInformation("Existing assessment found for appointment ID {AppointmentId}. Loading for editing.", appointmentId);
                    // Decrypt and load into bound model for editing
                    try
                    {
                        _logger.LogInformation("Attempting to decrypt assessment data for editing");
                        existingAssessment.DecryptSensitiveData(_encryptionService, User);
                        _logger.LogInformation("Assessment data decryption completed successfully");
                    }
                    catch (Exception decryptEx)
                    {
                        _logger.LogError(decryptEx, "Failed to decrypt assessment data for appointment {AppointmentId}", appointmentId);
                        // Continue with encrypted data rather than failing completely
                        _logger.LogWarning("Continuing with encrypted data due to decryption failure");
                    }
                    Assessment = existingAssessment;
                }
                else
                {
                    _logger.LogInformation("No existing NCD assessment for appointment {AppointmentId}. Creating new assessment.", appointmentId);
                    // Initialize new assessment with appointment data
                    Assessment = new NCDRiskAssessment
                    {
                        AppointmentId = appointmentId.Value,
                        UserId = appointment.PatientId,
                        HealthFacility = "Barangay Health Center 161",
                        DateOfAssessment = DateTime.Now.ToString("yyyy-MM-dd"),
                        AppointmentType = "NCD Risk Assessment"
                    };

                    // Set patient information from appointment
                    if (appointment.Patient?.User != null)
                    {
                        Assessment.FirstName = appointment.Patient.User.FirstName;
                        Assessment.LastName = appointment.Patient.User.LastName;
                        Assessment.MiddleName = appointment.Patient.User.MiddleName;
                        Assessment.Address = appointment.Patient.User.Address;
                        Assessment.Barangay = appointment.Patient.User.Barangay;
                        Assessment.Telepono = appointment.Patient.User.PhoneNumber;
                        Assessment.Birthday = appointment.Patient.User.BirthDate?.ToString("yyyy-MM-dd") ?? DateTime.Today.ToString("yyyy-MM-dd");
                        Assessment.Edad = appointment.AgeValue.ToString();
                        Assessment.Kasarian = appointment.Patient.User.Gender;
                        Assessment.Relihiyon = appointment.Patient.User.Religion;
                        Assessment.CivilStatus = appointment.Patient.User.CivilStatus;
                        Assessment.Occupation = appointment.Patient.User.Occupation;
                    }

                    // Generate family number
                    Assessment.FamilyNo = await GetOrGenerateFamilyNumber(appointment.Patient.User);
                }

                // Set patient information for display
                if (appointment.Patient?.User != null)
                {
                    PatientName = appointment.Patient.User.FullName ?? $"{appointment.Patient.User.FirstName} {appointment.Patient.User.LastName}";
                    PatientAddress = appointment.Patient.User.Address ?? "";
                    PatientBarangay = appointment.Patient.User.Barangay ?? "";
                    PatientPhone = appointment.Patient.User.PhoneNumber ?? "";
                    PatientAge = appointment.AgeValue;
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading appointment data for ID: {Id}", appointmentId);
                StatusMessage = "Error loading appointment data. Please try again later.";
                return RedirectToPage("/Nurse/Appointments");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state in CreateNCDAssessment OnPost");
                    var errors = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    _logger.LogWarning("Validation errors: {Errors}", errors);
                    return Page();
                }

                // Set timestamps
                Assessment.CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Assessment.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Get appointment
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .FirstOrDefaultAsync(a => a.Id == Assessment.AppointmentId);

                if (appointment == null)
                {
                    _logger.LogWarning("Appointment not found when saving assessment");
                    StatusMessage = "Appointment not found.";
                    return RedirectToPage("/Nurse/Appointments");
                }

                // Make sure cancer type is only set when cancer is checked
                if (Assessment.HasCancer != "true")
                {
                    Assessment.CancerType = null;
                }

                // Set all missing fields to default values if not provided
                Assessment.UserId = appointment.PatientId;
                Assessment.AppointmentId = appointment.Id;
                
                // Set default values for missing fields
                if (string.IsNullOrEmpty(Assessment.HealthFacility))
                    Assessment.HealthFacility = "Barangay Health Center 161";
                if (string.IsNullOrEmpty(Assessment.FamilyNo))
                    Assessment.FamilyNo = await GetOrGenerateFamilyNumber(appointment.Patient.User);
                if (string.IsNullOrEmpty(Assessment.DateOfAssessment))
                    Assessment.DateOfAssessment = DateTime.Now.ToString("yyyy-MM-dd");
                if (string.IsNullOrEmpty(Assessment.AppointmentType))
                    Assessment.AppointmentType = "NCD Risk Assessment";
                
                // Set personal information from appointment
                if (appointment.Patient?.User != null)
                {
                    if (string.IsNullOrEmpty(Assessment.FirstName))
                        Assessment.FirstName = appointment.Patient.User.FirstName;
                    if (string.IsNullOrEmpty(Assessment.LastName))
                        Assessment.LastName = appointment.Patient.User.LastName;
                    if (string.IsNullOrEmpty(Assessment.MiddleName))
                        Assessment.MiddleName = appointment.Patient.User.MiddleName;
                    if (string.IsNullOrEmpty(Assessment.Address))
                        Assessment.Address = appointment.Patient.User.Address;
                    if (string.IsNullOrEmpty(Assessment.Barangay))
                        Assessment.Barangay = appointment.Patient.User.Barangay;
                    if (string.IsNullOrEmpty(Assessment.Telepono))
                        Assessment.Telepono = appointment.Patient.User.PhoneNumber;
                    if (string.IsNullOrEmpty(Assessment.Birthday))
                        Assessment.Birthday = appointment.Patient.User.BirthDate?.ToString("yyyy-MM-dd") ?? DateTime.Today.ToString("yyyy-MM-dd");
                    if (string.IsNullOrEmpty(Assessment.Edad))
                        Assessment.Edad = appointment.AgeValue.ToString();
                    if (string.IsNullOrEmpty(Assessment.Kasarian))
                        Assessment.Kasarian = appointment.Patient.User.Gender;
                    if (string.IsNullOrEmpty(Assessment.Relihiyon))
                        Assessment.Relihiyon = appointment.Patient.User.Religion;
                    if (string.IsNullOrEmpty(Assessment.CivilStatus))
                        Assessment.CivilStatus = appointment.Patient.User.CivilStatus;
                    if (string.IsNullOrEmpty(Assessment.Occupation))
                        Assessment.Occupation = appointment.Patient.User.Occupation;
                }

                // Encrypt sensitive data before saving
                Assessment.EncryptSensitiveData(_encryptionService);

                // Save assessment
                // Upsert logic: update if exists, otherwise create
var existing = await _context.NCDRiskAssessments.FirstOrDefaultAsync(a => a.AppointmentId == Assessment.AppointmentId);
if (existing != null)
{
    // Map posted values onto existing entity
    _context.Entry(existing).CurrentValues.SetValues(Assessment);
    // Preserve original CreatedAt if any, update UpdatedAt
    if (string.IsNullOrEmpty(existing.CreatedAt))
        existing.CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    existing.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    // Ensure cancer type cleared when not applicable
    if (existing.HasCancer != "true") { existing.CancerType = null; }
    // Encrypt before saving
    existing.EncryptSensitiveData(_encryptionService);
}
else
{
    Assessment.CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    Assessment.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    if (Assessment.HasCancer != "true") { Assessment.CancerType = null; }
    Assessment.EncryptSensitiveData(_encryptionService);
    _context.NCDRiskAssessments.Add(Assessment);
}
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Successfully saved NCD Risk Assessment for appointment ID: {Id}", Assessment.AppointmentId);
                StatusMessage = "NCD Risk Assessment saved successfully.";
                
                return RedirectToPage("/Nurse/AppointmentDetails", new { id = Assessment.AppointmentId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving NCD Risk Assessment");
                StatusMessage = "Error saving assessment. Please try again later.";
                return Page();
            }
        }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            
            if (birthDate.Date > today.AddYears(-age))
            {
                age--;
            }
            
            return age;
        }

        private async Task<string> GetOrGenerateFamilyNumber(ApplicationUser user)
        {
            // Check if user already has a family number in previous assessments
            var existingAssessment = await _context.NCDRiskAssessments
                .Where(a => a.UserId == user.Id)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            if (existingAssessment != null && !string.IsNullOrEmpty(existingAssessment.FamilyNo))
            {
                return existingAssessment.FamilyNo;
            }

            // Generate a new family number based on last name initial
            var lastName = user.LastName ?? user.FullName?.Split(' ').LastOrDefault() ?? "X";
            var firstLetter = lastName.Substring(0, 1).ToUpper();

            // Get the highest sequence number for this letter
            // Since FamilyNo is encrypted, we need to decrypt all records to check for existing numbers
            var allAssessments = await _context.NCDRiskAssessments
                .Where(a => !string.IsNullOrEmpty(a.FamilyNo))
                .ToListAsync();

            int nextNumber = 1;
            var existingNumbers = new List<int>();

            foreach (var assessment in allAssessments)
            {
                try
                {
                    // Decrypt the family number if possible
                    string decryptedFamilyNo = assessment.FamilyNo;
                    if (_encryptionService.CanUserDecrypt(User))
                    {
                        decryptedFamilyNo = _encryptionService.Decrypt(assessment.FamilyNo);
                    }

                    // Check if this family number starts with our letter
                    if (!string.IsNullOrEmpty(decryptedFamilyNo) && decryptedFamilyNo.StartsWith($"{firstLetter}-"))
                    {
                        var parts = decryptedFamilyNo.Split('-');
                        if (parts.Length == 2 && int.TryParse(parts[1], out int num))
                        {
                            existingNumbers.Add(num);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to decrypt FamilyNo for NCD assessment {AssessmentId}", assessment.Id);
                    // Skip this record if decryption fails
                }
            }

            if (existingNumbers.Any())
            {
                nextNumber = existingNumbers.Max() + 1;
            }
            
            // Generate new family number
            return $"{firstLetter}-{nextNumber:D3}"; // Format: X-001, X-002, etc.
        }
    }
} 
