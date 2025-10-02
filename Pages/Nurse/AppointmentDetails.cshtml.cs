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

        private string SafeDecrypt(string encryptedValue)
        {
            if (string.IsNullOrEmpty(encryptedValue) || !_encryptionService.IsEncrypted(encryptedValue))
                return encryptedValue;

            try
            {
                return _encryptionService.DecryptForUser(encryptedValue, User);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to decrypt value, returning original");
                return encryptedValue; // Return original value if decryption fails
            }
        }

        public AppointmentsModel.AppointmentViewModel Appointment { get; set; }
        public NCDRiskAssessment NCDRiskAssessment { get; set; }
        public HEEADSSSAssessment HEEADSSSAssessment { get; set; }
        public AdolescentHealthInfo AdolescentHealthInfo { get; set; }
        public int PatientAge { get; set; }
        public bool HasNCDAssessment { get; set; }
        public bool HasHEEADSSSAssessment { get; set; }
        public bool HasAdolescentHealthInfo { get; set; }

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
                _logger.LogInformation("=== Starting appointment details loading for ID: {Id} ===", id);

                // Step 1: User Authentication Check
                _logger.LogInformation("Step 1: User authentication check");
                _logger.LogInformation("User authentication: {IsAuthenticated}, User roles: {Roles}", 
                    User?.Identity?.IsAuthenticated, 
                    string.Join(", ", User?.Claims?.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Select(c => c.Value) ?? new string[0]));
                _logger.LogInformation("CanUserDecrypt: {CanDecrypt}", _encryptionService.CanUserDecrypt(User));

                // Step 2: Database Query
                _logger.LogInformation("Step 2: Database query for appointment");
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (appointment == null)
                {
                    _logger.LogWarning("Appointment with ID {Id} not found", id);
                    return NotFound("Appointment not found");
                }

                _logger.LogInformation("Step 2 Complete: Appointment found with Status={Status}, PatientId={PatientId}", 
                    appointment.Status, appointment.PatientId);

                // Step 3: Decrypt doctor name if available
                string doctorName = "Not Assigned";
                if (appointment.Doctor != null)
                {
                    try
                    {
                        appointment.Doctor.DecryptSensitiveData(_encryptionService, User);
                        doctorName = appointment.Doctor.FullName;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to decrypt doctor data for appointment ID {Id}", id);
                        doctorName = "Not Assigned";
                    }
                }

                // Step 4: Decrypt patient name
                string patientName = "Unknown";
                if (!string.IsNullOrEmpty(appointment.PatientName))
                {
                    if (_encryptionService.IsEncrypted(appointment.PatientName))
                    {
                        patientName = _encryptionService.DecryptForUser(appointment.PatientName, User);
                    }
                    else
                    {
                        patientName = appointment.PatientName;
                    }
                }
                else if (appointment.Patient != null)
                {
                    patientName = appointment.Patient.FullName;
                }

                // Step 5: Convert to View Model
                _logger.LogInformation("Step 5: Converting to view model");
                Appointment = new AppointmentsModel.AppointmentViewModel
                {
                    Id = appointment.Id,
                    PatientId = appointment.PatientId,
                    PatientName = patientName,
                    AppointmentDate = appointment.AppointmentDate,
                    AppointmentTime = appointment.AppointmentTime,
                    DoctorId = appointment.DoctorId,
                    DoctorName = doctorName,
                    Status = appointment.Status,
                    Type = appointment.Type ?? "General",
                    Description = appointment.Description
                };

                _logger.LogInformation("Step 5 Complete: View model created");

                // Step 6: Patient Data Loading and Decryption
                _logger.LogInformation("Step 6: Loading patient details");
                if (appointment.Patient != null)
                {
                    _logger.LogInformation("Patient found: UserId={UserId}", appointment.Patient.UserId);
                    try
                    {
                        // Try to decrypt patient data first
                        _logger.LogInformation("Attempting patient data decryption");
                        appointment.Patient.DecryptSensitiveData(_encryptionService, User);
                        
                        // Use safe decryption for critical patient fields
                        appointment.Patient.FullName = SafeDecrypt(appointment.Patient.FullName);
                        appointment.Patient.Address = SafeDecrypt(appointment.Patient.Address);
                        appointment.Patient.ContactNumber = SafeDecrypt(appointment.Patient.ContactNumber);
                        
                        _logger.LogInformation("Successfully decrypted patient data for appointment ID {Id}", id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to decrypt patient data for appointment ID {Id}, continuing without decryption", id);
                        // Continue without throwing to avoid breaking the entire page
                    }
                    
                    // Use the age from the appointment (age at booking time) instead of current age
                    PatientAge = appointment.AgeValue > 0 ? appointment.AgeValue : appointment.Patient.Age;
                    _logger.LogInformation("PatientAge set to: {PatientAge}", PatientAge);
                }
                else
                {
                    _logger.LogInformation("No patient data found for appointment ID {Id}", id);
                }

                _logger.LogInformation("Step 4 Complete: Patient data processing");

                // Step 5: NCD Risk Assessment Loading
                _logger.LogInformation("Step 5: Checking for NCD Risk Assessment existence");
                HasNCDAssessment = await _context.NCDRiskAssessments
                    .AnyAsync(a => a.AppointmentId == id);
                _logger.LogInformation("NCD Risk Assessment exists: {HasNCDAssessment}", HasNCDAssessment);

                // Load NCD Risk Assessment if it exists
                if (HasNCDAssessment)
                {
                    try {
                        _logger.LogInformation("Loading NCD Risk Assessment from database");
                        NCDRiskAssessment = await _context.NCDRiskAssessments
                            .Where(a => a.AppointmentId == id)
                            .AsNoTracking()
                            .FirstOrDefaultAsync();
                        
                        if (NCDRiskAssessment != null)
                        {
                            _logger.LogInformation("NCD Risk Assessment loaded from database, attempting decryption");
                        }
                        
                        // Decrypt NCD Risk Assessment data
                        if (NCDRiskAssessment != null)
                        {
                            try
                            {
                                NCDRiskAssessment.DecryptSensitiveData(_encryptionService, User);
                                
                                // Manual decryption fallback for critical NCD fields using safe decryption
                                NCDRiskAssessment.FirstName = SafeDecrypt(NCDRiskAssessment.FirstName);
                                NCDRiskAssessment.MiddleName = SafeDecrypt(NCDRiskAssessment.MiddleName);
                                NCDRiskAssessment.LastName = SafeDecrypt(NCDRiskAssessment.LastName);
                                NCDRiskAssessment.Edad = SafeDecrypt(NCDRiskAssessment.Edad);
                                NCDRiskAssessment.Kasarian = SafeDecrypt(NCDRiskAssessment.Kasarian);
                                NCDRiskAssessment.Address = SafeDecrypt(NCDRiskAssessment.Address);
                                NCDRiskAssessment.Barangay = SafeDecrypt(NCDRiskAssessment.Barangay);
                                NCDRiskAssessment.Telepono = SafeDecrypt(NCDRiskAssessment.Telepono);
                                NCDRiskAssessment.SmokingStatus = SafeDecrypt(NCDRiskAssessment.SmokingStatus);
                                NCDRiskAssessment.AlcoholFrequency = SafeDecrypt(NCDRiskAssessment.AlcoholFrequency);
                                NCDRiskAssessment.HighSaltIntake = SafeDecrypt(NCDRiskAssessment.HighSaltIntake);
                                NCDRiskAssessment.ExerciseDuration = SafeDecrypt(NCDRiskAssessment.ExerciseDuration);
                                NCDRiskAssessment.RiskStatus = SafeDecrypt(NCDRiskAssessment.RiskStatus);
                                // CreatedAt and UpdatedAt are now DateTime, no decryption needed
                                
                                _logger.LogInformation("Successfully loaded and decrypted NCDRiskAssessment data for appointment ID {Id}", id);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Failed to decrypt NCD Risk Assessment data for appointment ID {Id}", id);
                                // Continue without throwing to avoid breaking the entire page
                            }
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
                _logger.LogInformation("Step 5 Complete: NCD Risk Assessment processing");

                // Check for HEEADSSS Assessment existence based on UserId
                _logger.LogInformation("Checking for HEEADSSS Assessment for appointment ID: {AppointmentId}", id);
                
                // Check if HEEADSSS assessment exists for this patient
                HasHEEADSSSAssessment = false;
                HEEADSSSAssessment = null;

                if (appointment.Patient != null)
                {
                    try
                    {
                        // Decrypt patient data first (this might already be done above, but ensure it's done safely)
                        appointment.Patient.DecryptSensitiveData(_encryptionService, User);
                        
                        // Additional safe decryption for patient critical fields
                        appointment.Patient.FullName = SafeDecrypt(appointment.Patient.FullName);
                        appointment.Patient.Address = SafeDecrypt(appointment.Patient.Address);
                        appointment.Patient.ContactNumber = SafeDecrypt(appointment.Patient.ContactNumber);
                        
                        _logger.LogInformation("Looking for HEEADSSS Assessment for UserId: {UserId}", appointment.Patient.UserId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to decrypt patient data for HEEADSSS lookup, continuing anyway");
                    }
                    
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
                            
                            // Decrypt the HEEADSSS assessment data
                            heeadsssAssessment.DecryptSensitiveData(_encryptionService, User);
                            
                            // Manual decryption fallback for critical fields using safe decryption
                            heeadsssAssessment.FullName = SafeDecrypt(heeadsssAssessment.FullName);
                            heeadsssAssessment.Age = SafeDecrypt(heeadsssAssessment.Age);
                            heeadsssAssessment.Gender = SafeDecrypt(heeadsssAssessment.Gender);
                            heeadsssAssessment.Address = SafeDecrypt(heeadsssAssessment.Address);
                            heeadsssAssessment.ContactNumber = SafeDecrypt(heeadsssAssessment.ContactNumber);
                            heeadsssAssessment.HomeEnvironment = SafeDecrypt(heeadsssAssessment.HomeEnvironment);
                            heeadsssAssessment.FamilyRelationship = SafeDecrypt(heeadsssAssessment.FamilyRelationship);
                            heeadsssAssessment.HomeFamilyProblems = SafeDecrypt(heeadsssAssessment.HomeFamilyProblems);
                            heeadsssAssessment.HomeParentalListening = SafeDecrypt(heeadsssAssessment.HomeParentalListening);
                            heeadsssAssessment.SchoolPerformance = SafeDecrypt(heeadsssAssessment.SchoolPerformance);
                            // AttendanceIssues is now a boolean field, no need to decrypt
                            heeadsssAssessment.CareerPlans = SafeDecrypt(heeadsssAssessment.CareerPlans);
                            heeadsssAssessment.EducationCurrentlyStudying = SafeDecrypt(heeadsssAssessment.EducationCurrentlyStudying);
                            heeadsssAssessment.Hobbies = SafeDecrypt(heeadsssAssessment.Hobbies);
                            heeadsssAssessment.PhysicalActivity = SafeDecrypt(heeadsssAssessment.PhysicalActivity);
                            heeadsssAssessment.ScreenTime = SafeDecrypt(heeadsssAssessment.ScreenTime);
                            heeadsssAssessment.ActivitiesRegularExercise = SafeDecrypt(heeadsssAssessment.ActivitiesRegularExercise);
                            
                            _logger.LogInformation("Successfully loaded and decrypted HEEADSSS Assessment data for appointment ID {Id}", id);
                            
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

                // Step 7: Load Adolescent Health Information
                _logger.LogInformation("Step 7: Loading Adolescent Health Information");
                HasAdolescentHealthInfo = false;
                AdolescentHealthInfo = null;

                if (appointment.Patient != null)
                {
                    var adolescentHealthInfo = await _context.AdolescentHealthInfo
                        .Where(a => a.UserId == appointment.Patient.UserId)
                        .OrderByDescending(a => a.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (adolescentHealthInfo != null)
                    {
                        try
                        {
                            adolescentHealthInfo.DecryptSensitiveData(_encryptionService, User);
                            HasAdolescentHealthInfo = true;
                            AdolescentHealthInfo = adolescentHealthInfo;
                            _logger.LogInformation("Adolescent Health Information loaded for patient: {PatientName}", adolescentHealthInfo.PatientName);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to decrypt Adolescent Health Information {Id}", adolescentHealthInfo.Id);
                            HasAdolescentHealthInfo = false;
                        }
                    }
                }

                _logger.LogInformation("Adolescent Health Info found: {HasAdolescentHealthInfo}", HasAdolescentHealthInfo);
                _logger.LogInformation("Step 7 Complete: Adolescent Health Information processing");

                // Step 8: Complete
                _logger.LogInformation("Step 8: All processing complete, returning page");
                _logger.LogInformation("=== Successfully completed appointment details loading for ID: {Id} ===", id);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading appointment details for ID: {Id}. Exception message: {Message}. Stack trace: {StackTrace}", 
                    id, ex.Message, ex.StackTrace);
                StatusMessage = $"Error loading appointment details: {ex.Message}. Please try again later.";
                return RedirectToPage("/Nurse/Appointments");
            }
        }
    }
} 