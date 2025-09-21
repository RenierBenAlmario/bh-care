using System;
using System.Threading.Tasks;
using System.Linq;
using Barangay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Barangay.Data;
using Barangay.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Barangay.Extensions;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse,Head Nurse")]
    public class CreateHEEADSSSAssessmentModel : PageModel
    {
        private readonly EncryptedDbContext _context;
        private readonly ILogger<CreateHEEADSSSAssessmentModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataEncryptionService _encryptionService;

        public CreateHEEADSSSAssessmentModel(
            EncryptedDbContext context,
            ILogger<CreateHEEADSSSAssessmentModel> logger,
            UserManager<ApplicationUser> userManager,
            IDataEncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _encryptionService = encryptionService;
            Assessment = new HEEADSSSAssessment();
        }

        [BindProperty]
        public HEEADSSSAssessment Assessment { get; set; }
        
        public string PatientName { get; set; }
        public string PatientAddress { get; set; }
        public string PatientGender { get; set; }
        public string PatientPhone { get; set; }
        public int PatientAge { get; set; }
        public string HealthFacility { get; set; } = "Baesa Health Center";
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
                // Since AppointmentId is encrypted, we need to check all records and decrypt them
                var allHEEADSSSAssessments = await _context.HEEADSSSAssessments
                    .AsNoTracking()
                    .ToListAsync();

                bool assessmentExists = false;

                foreach (var assessment in allHEEADSSSAssessments)
                {
                    try
                    {
                        // Decrypt the AppointmentId to check if it matches
                        var decryptedAppointmentId = _encryptionService.DecryptForUser(assessment.AppointmentId, User);
                        if (decryptedAppointmentId == appointmentId.ToString())
                        {
                            assessmentExists = true;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to decrypt AppointmentId for HEEADSSS assessment {Id}", assessment.Id);
                        // Continue checking other assessments
                    }
                }

                if (assessmentExists)
                {
                    _logger.LogWarning("Assessment already exists for appointment ID {AppointmentId}", appointmentId);
                    StatusMessage = "An assessment already exists for this appointment.";
                    return RedirectToPage("/Nurse/AppointmentDetails", new { id = appointmentId });
                }

                // Initialize assessment with appointment data
                Assessment.AppointmentId = appointmentId.Value.ToString();
                
                if (appointment.Patient != null)
                {
                    // Decrypt patient data for display
                    appointment.Patient.DecryptSensitiveData(_encryptionService, User);
                    
                    PatientName = appointment.Patient.FullName;
                    Assessment.UserId = appointment.Patient.UserId;
                    PatientAddress = appointment.Patient.Address;
                    PatientPhone = appointment.Patient.ContactNumber;
                    PatientGender = appointment.Patient.Gender;
                    Assessment.Gender = appointment.Patient.Gender;
                    
                    // Calculate age
                    PatientAge = appointment.Patient.Age;
                    Assessment.Age = PatientAge.ToString();

                    // Get family number from associated user if available
                    if (appointment.Patient.User != null)
                    {
                        FamilyNo = await GetOrGenerateFamilyNumber(appointment.Patient.User);
                    }

                    // Set some default values based on common responses
                    Assessment.FeelsSafeAtHome = "True";
                    Assessment.FeelsSafeAtSchool = "True";
                }
                else
                {
                    PatientName = appointment.PatientName ?? "Unknown";
                    Assessment.UserId = appointment.PatientId;
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
                    _logger.LogWarning("Invalid model state in CreateHEEADSSSAssessment OnPost");
                    var errors = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    _logger.LogWarning("Validation errors: {Errors}", errors);
                    return Page();
                }

                // Set timestamps
                Assessment.CreatedAt = DateTime.Now;
                Assessment.UpdatedAt = DateTime.Now;

                // Get appointment
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .FirstOrDefaultAsync(a => a.Id == int.Parse(Assessment.AppointmentId));

                if (appointment == null)
                {
                    _logger.LogWarning("Appointment not found when saving assessment");
                    StatusMessage = "Appointment not found.";
                    return RedirectToPage("/Nurse/Appointments");
                }

                // Set substance type to null if substance use is not checked
                if (Assessment.SubstanceUse != "True")
                {
                    Assessment.SubstanceType = null;
                }

                // Check for critical responses that might need immediate attention
                bool hasCriticalResponses = Assessment.SuicidalThoughts == "True" || Assessment.SelfHarmBehavior == "True";
                
                // Encrypt sensitive data before saving
                Assessment.EncryptSensitiveData(_encryptionService);
                
                // Save assessment
                _context.HEEADSSSAssessments.Add(Assessment);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Successfully saved HEEADSSS Assessment for appointment ID: {Id}", Assessment.AppointmentId);
                
                if (hasCriticalResponses)
                {
                    StatusMessage = "HEEADSSS Assessment saved successfully. ATTENTION: Critical responses detected that may require immediate action!";
                }
                else
                {
                    StatusMessage = "HEEADSSS Assessment saved successfully.";
                }
                
                return RedirectToPage("/Nurse/AppointmentDetails", new { id = Assessment.AppointmentId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving HEEADSSS Assessment");
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
            var existingAssessment = await _context.HEEADSSSAssessments
                .Where(a => a.UserId == user.Id)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            if (existingAssessment != null && !string.IsNullOrEmpty(existingAssessment.FamilyNo))
            {
                // Decrypt the existing family number
                existingAssessment.DecryptSensitiveData(_encryptionService, User);
                return existingAssessment.FamilyNo;
            }

            // Try to get family number from NCD assessment
            var ncdAssessment = await _context.NCDRiskAssessments
                .Where(a => a.UserId == user.Id)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            if (ncdAssessment != null && !string.IsNullOrEmpty(ncdAssessment.FamilyNo))
            {
                // Decrypt the existing family number
                ncdAssessment.DecryptSensitiveData(_encryptionService, User);
                return ncdAssessment.FamilyNo;
            }

            // Generate a new family number based on last name initial
            var lastName = user.LastName ?? user.FullName?.Split(' ').LastOrDefault() ?? "X";
            var firstLetter = lastName.Substring(0, 1).ToUpper();

            // Get the highest sequence number for this letter
            var lastNumber = await _context.HEEADSSSAssessments
                .Where(a => a.FamilyNo != null && a.FamilyNo.StartsWith(firstLetter + "-"))
                .Select(a => a.FamilyNo.Substring(2))
                .Where(n => n.All(char.IsDigit))
                .Select(n => int.Parse(n))
                .DefaultIfEmpty(0)
                .MaxAsync();
            
            // Generate new family number
            var newSequence = lastNumber + 1;
            return $"{firstLetter}-{newSequence:D3}"; // Format: X-001, X-002, etc.
        }
    }
} 