using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using Barangay.Extensions;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Collections.Generic; // Added for List

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse,Head Nurse")]
    public class AppointmentDetailsModel : PageModel
    {
        private readonly EncryptedDbContext _context;
        private readonly ILogger<AppointmentDetailsModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataEncryptionService _encryptionService;

        public AppointmentDetailsModel(
            EncryptedDbContext context,
            ILogger<AppointmentDetailsModel> logger,
            UserManager<ApplicationUser> userManager,
            IDataEncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _encryptionService = encryptionService;
        }

        public AppointmentsModel.AppointmentViewModel Appointment { get; set; }
        public NCDRiskAssessment NCDRiskAssessment { get; set; }
        public HEEADSSSAssessment HEEADSSSAssessment { get; set; }
        public int PatientAge { get; set; }
        public bool HasNCDAssessment { get; set; }
        public bool HasHEEADSSSAssessment { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Appointment ID not provided");
                return NotFound("Appointment ID must be provided");
            }

            try
            {
                _logger.LogInformation("Loading appointment details for ID: {Id}", id);

                // Get the appointment
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (appointment == null)
                {
                    _logger.LogWarning("Appointment with ID {Id} not found", id);
                    return NotFound("Appointment not found");
                }

                // Convert to view model
                Appointment = new AppointmentsModel.AppointmentViewModel
                {
                    Id = appointment.Id,
                    PatientId = appointment.PatientId,
                    PatientName = string.IsNullOrEmpty(appointment.PatientName) ?
                        (appointment.Patient != null ? appointment.Patient.FullName : "Unknown") : appointment.PatientName,
                    AppointmentDate = appointment.AppointmentDate,
                    AppointmentTime = appointment.AppointmentTime,
                    DoctorId = appointment.DoctorId,
                    DoctorName = appointment.Doctor?.FullName ?? "Not Assigned",
                    Status = appointment.Status,
                    Type = appointment.Type ?? "General",
                    Description = appointment.Description
                };

                // Load the patient details
                if (appointment.Patient != null)
                {
                    // Use the age from the appointment (age at booking time) instead of current age
                    PatientAge = appointment.AgeValue > 0 ? appointment.AgeValue : appointment.Patient.Age;
                }

                // Check for NCD Risk Assessment existence
                HasNCDAssessment = await _context.NCDRiskAssessments
                    .AnyAsync(a => a.AppointmentId == id);

                // Load NCD Risk Assessment if it exists
                if (HasNCDAssessment)
                {
                    try {
                        NCDRiskAssessment = await _context.NCDRiskAssessments
                            .Where(a => a.AppointmentId == id)
                            .AsNoTracking()
                            .FirstOrDefaultAsync();
                        
                        // Decrypt NCD Risk Assessment data
                        if (NCDRiskAssessment != null)
                        {
                            NCDRiskAssessment.DecryptSensitiveData(_encryptionService, User);
                            
                            // Manual decryption fallback for critical NCD fields
                            if (!string.IsNullOrEmpty(NCDRiskAssessment.FirstName) && _encryptionService.IsEncrypted(NCDRiskAssessment.FirstName))
                            {
                                NCDRiskAssessment.FirstName = _encryptionService.DecryptForUser(NCDRiskAssessment.FirstName, User);
                            }
                            if (!string.IsNullOrEmpty(NCDRiskAssessment.MiddleName) && _encryptionService.IsEncrypted(NCDRiskAssessment.MiddleName))
                            {
                                NCDRiskAssessment.MiddleName = _encryptionService.DecryptForUser(NCDRiskAssessment.MiddleName, User);
                            }
                            if (!string.IsNullOrEmpty(NCDRiskAssessment.LastName) && _encryptionService.IsEncrypted(NCDRiskAssessment.LastName))
                            {
                                NCDRiskAssessment.LastName = _encryptionService.DecryptForUser(NCDRiskAssessment.LastName, User);
                            }
                            if (!string.IsNullOrEmpty(NCDRiskAssessment.Edad) && _encryptionService.IsEncrypted(NCDRiskAssessment.Edad))
                            {
                                NCDRiskAssessment.Edad = _encryptionService.DecryptForUser(NCDRiskAssessment.Edad, User);
                            }
                            if (!string.IsNullOrEmpty(NCDRiskAssessment.Kasarian) && _encryptionService.IsEncrypted(NCDRiskAssessment.Kasarian))
                            {
                                NCDRiskAssessment.Kasarian = _encryptionService.DecryptForUser(NCDRiskAssessment.Kasarian, User);
                            }
                            if (!string.IsNullOrEmpty(NCDRiskAssessment.Address) && _encryptionService.IsEncrypted(NCDRiskAssessment.Address))
                            {
                                NCDRiskAssessment.Address = _encryptionService.DecryptForUser(NCDRiskAssessment.Address, User);
                            }
                            if (!string.IsNullOrEmpty(NCDRiskAssessment.Barangay) && _encryptionService.IsEncrypted(NCDRiskAssessment.Barangay))
                            {
                                NCDRiskAssessment.Barangay = _encryptionService.DecryptForUser(NCDRiskAssessment.Barangay, User);
                            }
                            if (!string.IsNullOrEmpty(NCDRiskAssessment.Telepono) && _encryptionService.IsEncrypted(NCDRiskAssessment.Telepono))
                            {
                                NCDRiskAssessment.Telepono = _encryptionService.DecryptForUser(NCDRiskAssessment.Telepono, User);
                            }
                            if (!string.IsNullOrEmpty(NCDRiskAssessment.SmokingStatus) && _encryptionService.IsEncrypted(NCDRiskAssessment.SmokingStatus))
                            {
                                NCDRiskAssessment.SmokingStatus = _encryptionService.DecryptForUser(NCDRiskAssessment.SmokingStatus, User);
                            }
                            if (!string.IsNullOrEmpty(NCDRiskAssessment.AlcoholFrequency) && _encryptionService.IsEncrypted(NCDRiskAssessment.AlcoholFrequency))
                            {
                                NCDRiskAssessment.AlcoholFrequency = _encryptionService.DecryptForUser(NCDRiskAssessment.AlcoholFrequency, User);
                            }
                            if (!string.IsNullOrEmpty(NCDRiskAssessment.HighSaltIntake) && _encryptionService.IsEncrypted(NCDRiskAssessment.HighSaltIntake))
                            {
                                NCDRiskAssessment.HighSaltIntake = _encryptionService.DecryptForUser(NCDRiskAssessment.HighSaltIntake, User);
                            }
                            if (!string.IsNullOrEmpty(NCDRiskAssessment.ExerciseDuration) && _encryptionService.IsEncrypted(NCDRiskAssessment.ExerciseDuration))
                            {
                                NCDRiskAssessment.ExerciseDuration = _encryptionService.DecryptForUser(NCDRiskAssessment.ExerciseDuration, User);
                            }
                            if (!string.IsNullOrEmpty(NCDRiskAssessment.RiskStatus) && _encryptionService.IsEncrypted(NCDRiskAssessment.RiskStatus))
                            {
                                NCDRiskAssessment.RiskStatus = _encryptionService.DecryptForUser(NCDRiskAssessment.RiskStatus, User);
                            }
                            if (!string.IsNullOrEmpty(NCDRiskAssessment.CreatedAt) && _encryptionService.IsEncrypted(NCDRiskAssessment.CreatedAt))
                            {
                                NCDRiskAssessment.CreatedAt = _encryptionService.DecryptForUser(NCDRiskAssessment.CreatedAt, User);
                            }
                            if (!string.IsNullOrEmpty(NCDRiskAssessment.UpdatedAt) && _encryptionService.IsEncrypted(NCDRiskAssessment.UpdatedAt))
                            {
                                NCDRiskAssessment.UpdatedAt = _encryptionService.DecryptForUser(NCDRiskAssessment.UpdatedAt, User);
                            }
                            
                            _logger.LogInformation("Successfully loaded and decrypted NCDRiskAssessment data for appointment ID {Id}", id);
                        }
                        else
                        {
                            _logger.LogWarning("NCDRiskAssessment is null for appointment ID {Id}", id);
                            HasNCDAssessment = false;
                        }
                    }
                    catch (Exception ex) {
                        _logger.LogError(ex, "Error loading NCD Risk Assessment data for appointment ID {Id}", id);
                        HasNCDAssessment = false;
                    }
                }

                // Check for HEEADSSS Assessment existence based on UserId
                _logger.LogInformation("Checking for HEEADSSS Assessment for appointment ID: {AppointmentId}", id);
                
                // Check if HEEADSSS assessment exists for this patient
                HasHEEADSSSAssessment = false;
                HEEADSSSAssessment = null;

                if (appointment.Patient != null)
                {
                    // Decrypt patient data first
                    appointment.Patient.DecryptSensitiveData(_encryptionService, User);
                    
                    _logger.LogInformation("Looking for HEEADSSS Assessment for UserId: {UserId}", appointment.Patient.UserId);
                    
                    // Look for HEEADSSS assessment by UserId
                    var heeadsssAssessment = await _context.HEEADSSSAssessments
                        .Where(a => a.UserId == appointment.Patient.UserId)
                        .OrderByDescending(a => a.CreatedAt)
                        .FirstOrDefaultAsync();
                    
                    _logger.LogInformation("HEEADSSS Assessment query result: {AssessmentFound}", heeadsssAssessment != null);

                    if (heeadsssAssessment != null)
                    {
                        try
                        {
                            _logger.LogInformation("Found HEEADSSS Assessment {Id} for UserId: {UserId}", heeadsssAssessment.Id, appointment.Patient.UserId);
                            
                            // Log some encrypted values before decryption
                            _logger.LogInformation("Before decryption - FullName: {FullName}, Age: {Age}, Gender: {Gender}", 
                                heeadsssAssessment.FullName?.Substring(0, Math.Min(20, heeadsssAssessment.FullName?.Length ?? 0)) + "...",
                                heeadsssAssessment.Age?.Substring(0, Math.Min(20, heeadsssAssessment.Age?.Length ?? 0)) + "...",
                                heeadsssAssessment.Gender?.Substring(0, Math.Min(20, heeadsssAssessment.Gender?.Length ?? 0)) + "...");
                            
                            // Test encryption service first
                            _logger.LogInformation("Testing encryption service - CanUserDecrypt: {CanDecrypt}, User: {User}", 
                                _encryptionService.CanUserDecrypt(User), User?.Identity?.Name);
                            
                            // Test manual decryption of one field
                            if (!string.IsNullOrEmpty(heeadsssAssessment.FullName))
                            {
                                var testDecrypted = _encryptionService.DecryptForUser(heeadsssAssessment.FullName, User);
                                _logger.LogInformation("Manual decryption test - Original: {Original}, Decrypted: {Decrypted}", 
                                    heeadsssAssessment.FullName.Substring(0, Math.Min(20, heeadsssAssessment.FullName.Length)) + "...",
                                    testDecrypted?.Substring(0, Math.Min(20, testDecrypted?.Length ?? 0)) + "...");
                            }
                            
                            // Decrypt the HEEADSSS assessment data
                            heeadsssAssessment.DecryptSensitiveData(_encryptionService, User);
                            
                            // Manual decryption fallback for critical fields
                            if (!string.IsNullOrEmpty(heeadsssAssessment.FullName) && _encryptionService.IsEncrypted(heeadsssAssessment.FullName))
                            {
                                heeadsssAssessment.FullName = _encryptionService.DecryptForUser(heeadsssAssessment.FullName, User);
                            }
                            if (!string.IsNullOrEmpty(heeadsssAssessment.Age) && _encryptionService.IsEncrypted(heeadsssAssessment.Age))
                            {
                                heeadsssAssessment.Age = _encryptionService.DecryptForUser(heeadsssAssessment.Age, User);
                            }
                            if (!string.IsNullOrEmpty(heeadsssAssessment.Gender) && _encryptionService.IsEncrypted(heeadsssAssessment.Gender))
                            {
                                heeadsssAssessment.Gender = _encryptionService.DecryptForUser(heeadsssAssessment.Gender, User);
                            }
                            if (!string.IsNullOrEmpty(heeadsssAssessment.Address) && _encryptionService.IsEncrypted(heeadsssAssessment.Address))
                            {
                                heeadsssAssessment.Address = _encryptionService.DecryptForUser(heeadsssAssessment.Address, User);
                            }
                            if (!string.IsNullOrEmpty(heeadsssAssessment.ContactNumber) && _encryptionService.IsEncrypted(heeadsssAssessment.ContactNumber))
                            {
                                heeadsssAssessment.ContactNumber = _encryptionService.DecryptForUser(heeadsssAssessment.ContactNumber, User);
                            }
                            if (!string.IsNullOrEmpty(heeadsssAssessment.HomeEnvironment) && _encryptionService.IsEncrypted(heeadsssAssessment.HomeEnvironment))
                            {
                                heeadsssAssessment.HomeEnvironment = _encryptionService.DecryptForUser(heeadsssAssessment.HomeEnvironment, User);
                            }
                            if (!string.IsNullOrEmpty(heeadsssAssessment.FamilyRelationship) && _encryptionService.IsEncrypted(heeadsssAssessment.FamilyRelationship))
                            {
                                heeadsssAssessment.FamilyRelationship = _encryptionService.DecryptForUser(heeadsssAssessment.FamilyRelationship, User);
                            }
                            if (!string.IsNullOrEmpty(heeadsssAssessment.HomeFamilyProblems) && _encryptionService.IsEncrypted(heeadsssAssessment.HomeFamilyProblems))
                            {
                                heeadsssAssessment.HomeFamilyProblems = _encryptionService.DecryptForUser(heeadsssAssessment.HomeFamilyProblems, User);
                            }
                            if (!string.IsNullOrEmpty(heeadsssAssessment.HomeParentalListening) && _encryptionService.IsEncrypted(heeadsssAssessment.HomeParentalListening))
                            {
                                heeadsssAssessment.HomeParentalListening = _encryptionService.DecryptForUser(heeadsssAssessment.HomeParentalListening, User);
                            }
                            if (!string.IsNullOrEmpty(heeadsssAssessment.SchoolPerformance) && _encryptionService.IsEncrypted(heeadsssAssessment.SchoolPerformance))
                            {
                                heeadsssAssessment.SchoolPerformance = _encryptionService.DecryptForUser(heeadsssAssessment.SchoolPerformance, User);
                            }
                            if (!string.IsNullOrEmpty(heeadsssAssessment.AttendanceIssues) && _encryptionService.IsEncrypted(heeadsssAssessment.AttendanceIssues))
                            {
                                heeadsssAssessment.AttendanceIssues = _encryptionService.DecryptForUser(heeadsssAssessment.AttendanceIssues, User);
                            }
                            if (!string.IsNullOrEmpty(heeadsssAssessment.CareerPlans) && _encryptionService.IsEncrypted(heeadsssAssessment.CareerPlans))
                            {
                                heeadsssAssessment.CareerPlans = _encryptionService.DecryptForUser(heeadsssAssessment.CareerPlans, User);
                            }
                            if (!string.IsNullOrEmpty(heeadsssAssessment.EducationCurrentlyStudying) && _encryptionService.IsEncrypted(heeadsssAssessment.EducationCurrentlyStudying))
                            {
                                heeadsssAssessment.EducationCurrentlyStudying = _encryptionService.DecryptForUser(heeadsssAssessment.EducationCurrentlyStudying, User);
                            }
                            if (!string.IsNullOrEmpty(heeadsssAssessment.Hobbies) && _encryptionService.IsEncrypted(heeadsssAssessment.Hobbies))
                            {
                                heeadsssAssessment.Hobbies = _encryptionService.DecryptForUser(heeadsssAssessment.Hobbies, User);
                            }
                            if (!string.IsNullOrEmpty(heeadsssAssessment.PhysicalActivity) && _encryptionService.IsEncrypted(heeadsssAssessment.PhysicalActivity))
                            {
                                heeadsssAssessment.PhysicalActivity = _encryptionService.DecryptForUser(heeadsssAssessment.PhysicalActivity, User);
                            }
                            if (!string.IsNullOrEmpty(heeadsssAssessment.ScreenTime) && _encryptionService.IsEncrypted(heeadsssAssessment.ScreenTime))
                            {
                                heeadsssAssessment.ScreenTime = _encryptionService.DecryptForUser(heeadsssAssessment.ScreenTime, User);
                            }
                            if (!string.IsNullOrEmpty(heeadsssAssessment.ActivitiesRegularExercise) && _encryptionService.IsEncrypted(heeadsssAssessment.ActivitiesRegularExercise))
                            {
                                heeadsssAssessment.ActivitiesRegularExercise = _encryptionService.DecryptForUser(heeadsssAssessment.ActivitiesRegularExercise, User);
                            }
                            
                            // Log some decrypted values after decryption
                            _logger.LogInformation("After decryption - FullName: {FullName}, Age: {Age}, Gender: {Gender}", 
                                heeadsssAssessment.FullName?.Substring(0, Math.Min(20, heeadsssAssessment.FullName?.Length ?? 0)) + "...",
                                heeadsssAssessment.Age?.Substring(0, Math.Min(20, heeadsssAssessment.Age?.Length ?? 0)) + "...",
                                heeadsssAssessment.Gender?.Substring(0, Math.Min(20, heeadsssAssessment.Gender?.Length ?? 0)) + "...");
                            
                            HasHEEADSSSAssessment = true;
                            HEEADSSSAssessment = heeadsssAssessment;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to decrypt HEEADSSS assessment {Id}", heeadsssAssessment.Id);
                            HasHEEADSSSAssessment = false;
                        }
                    }
                }

                _logger.LogInformation("HEEADSSS Assessment found: {HasAssessment}", HasHEEADSSSAssessment);

                // HEEADSSS Assessment data is already decrypted above
                if (HasHEEADSSSAssessment && HEEADSSSAssessment != null)
                {
                    _logger.LogInformation("HEEADSSS Assessment loaded and decrypted. FullName: {FullName}, Age: {Age}, Gender: {Gender}", 
                        HEEADSSSAssessment.FullName, HEEADSSSAssessment.Age, HEEADSSSAssessment.Gender);
                }
                
                _logger.LogInformation("Assessment flags - NCD: {HasNCD}, HEEADSSS: {HasHEEADSSS}", HasNCDAssessment, HasHEEADSSSAssessment);

                // Add additional properties to track history
                if (NCDRiskAssessment != null)
                {
                    _logger.LogInformation("NCD Risk Assessment creation date: {Date}", NCDRiskAssessment.CreatedAt);
                }

                if (HEEADSSSAssessment != null)
                {
                    _logger.LogInformation("HEEADSSS Assessment creation date: {Date}", HEEADSSSAssessment.CreatedAt);
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading appointment details for ID: {Id}", id);
                StatusMessage = "Error loading appointment details. Please try again later.";
                return RedirectToPage("/Nurse/Appointments");
            }
        }
    }
} 