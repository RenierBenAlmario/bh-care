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
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Barangay.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Barangay.Pages.User
{
    [Authorize]
    public class HEEADSSSAssessmentModel : PageModel
    {
        private readonly EncryptedDbContext _context;
        private readonly ILogger<HEEADSSSAssessmentModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataEncryptionService _encryptionService;

        public HEEADSSSAssessmentModel(
            EncryptedDbContext context,
            ILogger<HEEADSSSAssessmentModel> logger,
            UserManager<ApplicationUser> userManager,
            IDataEncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _encryptionService = encryptionService;
        }

        [BindProperty]
        public HEEADSSSAssessmentViewModel Assessment { get; set; }

        [BindProperty]
        public string HealthFacility { get; set; } = "Barangay Health Center";

        [BindProperty]
        public string FamilyNo { get; set; } = "C-001";

        [BindProperty]
        public int? AppointmentId { get; set; }

        [BindProperty]
        public string UserId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Decrypt user data for authorized users
            user = user.DecryptSensitiveData(_encryptionService, User);
            
            // Manually decrypt PhoneNumber since it's not marked with [Encrypted] attribute
            if (!string.IsNullOrEmpty(user.PhoneNumber) && _encryptionService.IsEncrypted(user.PhoneNumber))
            {
                user.PhoneNumber = user.PhoneNumber.DecryptForUser(_encryptionService, User);
            }

            // Set default values
            HealthFacility = "Baesa Health Center";
            UserId = user.Id;
            FamilyNo = "C-001"; // Set default family number
            
            // Set ViewData for the view
            ViewData["HealthFacility"] = HealthFacility;
            ViewData["FamilyNo"] = FamilyNo;
            
            _logger.LogInformation("OnGetAsync - Setting default values: UserId={UserId}, HealthFacility={HealthFacility}, FamilyNo={FamilyNo}",
                UserId, HealthFacility, FamilyNo);
            
            // Check if appointmentId is provided in the query string
            string? appointmentIdStr = Request.Query["appointmentId"];
            if (!string.IsNullOrEmpty(appointmentIdStr) && int.TryParse(appointmentIdStr, out int appointmentId))
            {
                AppointmentId = appointmentId;
                _logger.LogInformation("OnGetAsync - AppointmentId set from query string: {AppointmentId}", AppointmentId);
                
                // Get appointment details to use patient information
                var appointment = await _context.Appointments.FindAsync(appointmentId);
                if (appointment != null)
                {
                    // Use appointment patient information
                    ViewData["PatientName"] = appointment.PatientName ?? user.FullName;
                    ViewData["PatientAge"] = appointment.AgeValue > 0 ? appointment.AgeValue : 19;
                    ViewData["PatientPhone"] = appointment.ContactNumber ?? user.PhoneNumber ?? string.Empty;
                    ViewData["PatientBirthdate"] = appointment.DateOfBirth?.ToString("yyyy-MM-dd") ?? DateTime.Today.AddYears(-19).ToString("yyyy-MM-dd");
                    
                    _logger.LogInformation($"Using appointment patient info: Name={appointment.PatientName}, Age={appointment.AgeValue}, Phone={appointment.ContactNumber}");
                }
                else
                {
                    // Fallback to logged-in user info
                    DateTime birthDate = DateTime.Today.AddYears(-19);
                    int age = 19;
                    ViewData["PatientName"] = user.FullName;
                    ViewData["PatientAge"] = age;
                    ViewData["PatientPhone"] = user.PhoneNumber ?? string.Empty;
                    ViewData["PatientBirthdate"] = birthDate.ToString("yyyy-MM-dd");
                    
                    _logger.LogInformation($"Appointment not found, using logged-in user info: Name={user.FullName}, Age={age}");
                }
            }
            else
            {
                // No appointment ID, use logged-in user info
                DateTime birthDate = DateTime.Today.AddYears(-19);
                int age = 19;
                ViewData["PatientName"] = user.FullName;
                ViewData["PatientAge"] = age;
                ViewData["PatientPhone"] = user.PhoneNumber ?? string.Empty;
                ViewData["PatientBirthdate"] = birthDate.ToString("yyyy-MM-dd");
                
                _logger.LogInformation($"No appointment ID, using logged-in user info: Name={user.FullName}, Age={age}");
            }

            // Initialize Assessment if not already initialized
            if (Assessment == null)
            {
                Assessment = new HEEADSSSAssessmentViewModel();
            }
            
            // Always set these required fields
            Assessment.UserId = user.Id;
            Assessment.HealthFacility = HealthFacility;
            Assessment.FamilyNo = FamilyNo;
            Assessment.AppointmentId = AppointmentId;
            Assessment.Age = (ViewData["PatientAge"] as int? ?? 19); // Use patient age from appointment
            Assessment.Birthday = DateTime.Parse(ViewData["PatientBirthdate"] as string ?? DateTime.Today.AddYears(-19).ToString("yyyy-MM-dd")); // Use patient birthdate from appointment
            Assessment.FullName = ViewData["PatientName"] as string ?? user.FullName; // Use patient name from appointment
            
            _logger.LogInformation("OnGetAsync - Assessment initialized with UserId={UserId}, HealthFacility={HealthFacility}, FamilyNo={FamilyNo}, AppointmentId={AppointmentId}, Age={Age}",
                Assessment.UserId, Assessment.HealthFacility, Assessment.FamilyNo, Assessment.AppointmentId, Assessment.Age);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                _logger.LogInformation("Starting HEEADSSS assessment submission");
                _logger.LogInformation("Request method: {Method}, Content-Type: {ContentType}", Request.Method, Request.ContentType);
                
                // Log all form values to help diagnose issues
                _logger.LogInformation("Form values received:");
                foreach (var key in Request.Form.Keys)
                {
                    _logger.LogInformation("- {Key}: {Value}", key, Request.Form[key].ToString());
                }
                
                // Log specific important fields
                _logger.LogInformation("Key form fields - UserId: '{UserId}', HealthFacility: '{HealthFacility}', FamilyNo: '{FamilyNo}', AppointmentId: '{AppointmentId}', FullName: '{FullName}'",
                    Request.Form["Assessment.UserId"].ToString(),
                    Request.Form["Assessment.HealthFacility"].ToString(),
                    Request.Form["Assessment.FamilyNo"].ToString(),
                    Request.Form["Assessment.AppointmentId"].ToString(),
                    Request.Form["Assessment.FullName"].ToString());
                
                // Get current user
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not found during HEEADSSS assessment submission");
                    return new JsonResult(new { success = false, error = "User not found" });
                }

                // Get required fields from form
                string userId = Request.Form["UserId"].ToString();
                string healthFacility = Request.Form["HealthFacility"].ToString();
                string familyNo = Request.Form["FamilyNo"].ToString();
                
                // Get appointmentId if provided
                string appointmentId = null;
                var appointmentIdFormValue = Request.Form["Assessment.AppointmentId"].ToString();
                _logger.LogInformation("Received AppointmentId from form: '{AppointmentIdFormValue}'", appointmentIdFormValue);
                
                if (!string.IsNullOrEmpty(appointmentIdFormValue))
                {
                    appointmentId = appointmentIdFormValue;
                    _logger.LogInformation("Successfully set AppointmentId: {AppointmentId}", appointmentId);
                }
                else
                {
                    _logger.LogWarning("AppointmentId is empty from form value: '{AppointmentIdFormValue}'", appointmentIdFormValue);
                }
                
                // Get patient data from appointment if available
                string patientName = user.FullName ?? "Unknown";
                DateTime birthday = DateTime.TryParse(user.BirthDate, out var parsedBirthDate) ? parsedBirthDate : DateTime.MinValue;
                string patientGender = user.Gender ?? "Not specified";
                string patientAddress = user.Address ?? "Not specified";
                string patientPhone = user.PhoneNumber ?? "Not specified";
                
                if (!string.IsNullOrEmpty(appointmentId) && int.TryParse(appointmentId, out int parsedAppointmentId))
                {
                    var appointment = await _context.Appointments.FindAsync(parsedAppointmentId);
                    if (appointment != null)
                    {
                        patientName = appointment.PatientName ?? user.FullName ?? "Unknown";
                        birthday = appointment.DateOfBirth ?? (DateTime.TryParse(user.BirthDate, out var parsedUserBirthDate) ? parsedUserBirthDate : DateTime.MinValue);
                        patientGender = appointment.Gender ?? user.Gender ?? "Not specified";
                        patientAddress = appointment.Address ?? user.Address ?? "Not specified";
                        patientPhone = appointment.ContactNumber ?? user.PhoneNumber ?? "Not specified";
                        
                        _logger.LogInformation($"Using appointment patient data: Name={patientName}, Birthday={birthday.ToShortDateString()}, Gender={patientGender}");
                    }
                    else
                    {
                        _logger.LogWarning("Appointment {AppointmentId} not found in database", appointmentId);
                    }
                }
                else
                {
                    _logger.LogWarning("No valid AppointmentId provided, using logged-in user data");
                }
                
                // Override with form data if provided
                if (DateTime.TryParse(Request.Form["Birthday"].ToString(), out DateTime parsedBirthday))
                {
                    birthday = parsedBirthday;
                    _logger.LogInformation($"Using birthday from form: {birthday.ToShortDateString()}");
                }
                
                int age = CalculateAge(birthday);
                _logger.LogInformation($"Calculated age from birthday {birthday.ToShortDateString()}: {age} years");
                
                // Log the direct form values
                _logger.LogInformation("Form values - UserId: {UserId}, HealthFacility: {HealthFacility}, FamilyNo: {FamilyNo}, AppointmentId: {AppointmentId}, Age: {Age}",
                    userId, healthFacility, familyNo, appointmentId, age);

                // Use fallback values if needed
                if (string.IsNullOrEmpty(userId))
                {
                    userId = user.Id;
                    _logger.LogInformation("Using fallback UserId: {UserId}", userId);
                }
                
                if (string.IsNullOrEmpty(healthFacility))
                {
                    healthFacility = "Baesa Health Center";
                    _logger.LogInformation("Using fallback HealthFacility: {HealthFacility}", healthFacility);
                }
                
                if (string.IsNullOrEmpty(familyNo))
                {
                    familyNo = "C-001";
                    _logger.LogInformation("Using fallback FamilyNo: {FamilyNo}", familyNo);
                }

                // Note: Duplicate check removed temporarily due to encryption complexity
                // TODO: Implement proper duplicate check that handles encrypted data
                
                // Create assessment object
                var assessment = new HEEADSSSAssessment
                {
                    // Required fields
                    UserId = userId,
                    HealthFacility = healthFacility,
                    FamilyNo = familyNo,
                    AppointmentId = appointmentId,
                    
                    // Patient information (from appointment or form)
                    FullName = patientName,
                    Birthday = birthday,
                    Age = age.ToString(),
                    Gender = patientGender,
                    Address = patientAddress,
                    ContactNumber = patientPhone,
                    
                    // Form fields - provide default values for non-nullable fields
                    HomeFamilyProblems = GetFormValueOrDefault("Assessment.HomeFamilyProblems"),
                    HomeParentalListening = GetFormValueOrDefault("Assessment.HomeParentalListening"),
                    HomeParentalBlame = GetFormValueOrDefault("Assessment.HomeParentalBlame"),
                    HomeFamilyChanges = GetFormValueOrDefault("Assessment.HomeFamilyChanges"),
                    HomeEnvironment = GetFormValueOrDefault("Assessment.HomeEnvironment", "Not assessed"),
                    FamilyRelationship = GetFormValueOrDefault("Assessment.FamilyRelationship", "Not assessed"),
                    
                    EducationCurrentlyStudying = GetFormValueOrDefault("Assessment.EducationCurrentlyStudying"),
                    EducationWorking = GetFormValueOrDefault("Assessment.EducationWorking"),
                    EducationSchoolWorkProblems = GetFormValueOrDefault("Assessment.EducationSchoolWorkProblems"),
                    EducationBullying = GetFormValueOrDefault("Assessment.EducationBullying"),
                    SchoolPerformance = GetFormValueOrDefault("Assessment.SchoolPerformance", "Not assessed"),
                    AttendanceIssues = "False", // Default to false for boolean fields
                    CareerPlans = GetFormValueOrDefault("Assessment.CareerPlans", "Not assessed"),
                    EducationEmployment = GetFormValueOrDefault("Assessment.EducationEmployment", "Not assessed"),
                    
                    EatingBodyImageSatisfaction = GetFormValueOrDefault("Assessment.EatingBodyImageSatisfaction"),
                    EatingDisorderedEatingBehaviors = GetFormValueOrDefault("Assessment.EatingDisorderedEatingBehaviors"),
                    EatingWeightComments = GetFormValueOrDefault("Assessment.EatingWeightComments"),
                    DietDescription = GetFormValueOrDefault("Assessment.DietDescription", "Not assessed"),
                    WeightConcerns = "False", // Default to false for boolean fields
                    EatingDisorderSymptoms = "False", // Default to false for boolean fields
                    
                    ActivitiesParticipation = GetFormValueOrDefault("Assessment.ActivitiesParticipation"),
                    ActivitiesRegularExercise = GetFormValueOrDefault("Assessment.ActivitiesRegularExercise"),
                    ActivitiesScreenTime = GetFormValueOrDefault("Assessment.ActivitiesScreenTime"),
                    Hobbies = GetFormValueOrDefault("Assessment.Hobbies", "Not assessed"),
                    PhysicalActivity = GetFormValueOrDefault("Assessment.PhysicalActivity", "Not assessed"),
                    ScreenTime = GetFormValueOrDefault("Assessment.ScreenTime", "Not assessed"),
                    
                    DrugsTobaccoUse = GetFormValueOrDefault("Assessment.DrugsTobaccoUse"),
                    DrugsAlcoholUse = GetFormValueOrDefault("Assessment.DrugsAlcoholUse"),
                    DrugsIllicitDrugUse = GetFormValueOrDefault("Assessment.DrugsIllicitDrugUse"),
                    SubstanceUse = "False", // Default to false for boolean fields
                    SubstanceType = GetFormValueOrDefault("Assessment.SubstanceType", "Not assessed"),
                    
                    SexualityBodyConcerns = GetFormValueOrDefault("Assessment.SexualityBodyConcerns"),
                    SexualityIntimateRelationships = GetFormValueOrDefault("Assessment.SexualityIntimateRelationships"),
                    SexualityPartners = GetFormValueOrDefault("Assessment.SexualityPartners"),
                    SexualitySexualOrientation = GetFormValueOrDefault("Assessment.SexualitySexualOrientation"),
                    SexualityPregnancy = GetFormValueOrDefault("Assessment.SexualityPregnancy"),
                    SexualitySTI = GetFormValueOrDefault("Assessment.SexualitySTI"),
                    SexualityProtection = GetFormValueOrDefault("Assessment.SexualityProtection"),
                    DatingRelationships = GetFormValueOrDefault("Assessment.DatingRelationships", "Not assessed"),
                    SexualActivity = "False", // Default to false for boolean fields
                    SexualOrientation = GetFormValueOrDefault("Assessment.SexualOrientation", "Not assessed"),
                    
                    SafetyPhysicalAbuse = GetFormValueOrDefault("Assessment.SafetyPhysicalAbuse"),
                    SafetyRelationshipViolence = GetFormValueOrDefault("Assessment.SafetyRelationshipViolence"),
                    SafetyProtectiveGear = GetFormValueOrDefault("Assessment.SafetyProtectiveGear"),
                    SafetyGunsAtHome = GetFormValueOrDefault("Assessment.SafetyGunsAtHome"),
                    FeelsSafeAtHome = "True", // Default to true for boolean fields
                    FeelsSafeAtSchool = "True", // Default to true for boolean fields
                    ExperiencedBullying = "False", // Default to false for boolean fields
                    
                    SuicideDepressionFeelings = GetFormValueOrDefault("Assessment.SuicideDepressionFeelings"),
                    SuicideSelfHarmThoughts = GetFormValueOrDefault("Assessment.SuicideSelfHarmThoughts"),
                    SuicideFamilyHistory = GetFormValueOrDefault("Assessment.SuicideFamilyHistory"),
                    MoodChanges = "False", // Default to false for boolean fields
                    SuicidalThoughts = "False", // Default to false for boolean fields
                    SelfHarmBehavior = "False", // Default to false for boolean fields
                    
                    // Strengths section
                    PersonalStrengths = GetFormValueOrDefault("Assessment.PersonalStrengths", "Not assessed"),
                    SupportSystems = GetFormValueOrDefault("Assessment.SupportSystems", "Not assessed"),
                    CopingMechanisms = GetFormValueOrDefault("Assessment.CopingMechanisms", "Not assessed"),
                    
                    // Non-nullable fields with default values
                    Notes = GetFormValueOrDefault("Assessment.Notes"),
                    AssessedBy = GetFormValueOrDefault("Assessment.AssessedBy"),
                    AssessmentNotes = GetFormValueOrDefault("Assessment.AssessmentNotes", "No assessment notes provided"),
                    RecommendedActions = GetFormValueOrDefault("Assessment.RecommendedActions", "No actions recommended"),
                    FollowUpPlan = GetFormValueOrDefault("Assessment.FollowUpPlan", "No follow-up plan specified"),
                    
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Encrypt sensitive data before saving
                try
                {
                    assessment.EncryptSensitiveData(_encryptionService);
                    _logger.LogInformation("Assessment data encrypted successfully");
                }
                catch (Exception encEx)
                {
                    _logger.LogError(encEx, "Encryption failed: {Error}", encEx.Message);
                    return new JsonResult(new { success = false, error = "Data encryption failed. Please try again." });
                }
                
                _context.HEEADSSSAssessments.Add(assessment);
                
                // Update appointment status if AppointmentId is provided
                if (!string.IsNullOrEmpty(appointmentId) && int.TryParse(appointmentId, out int parsedAppointmentIdForStatus) && parsedAppointmentIdForStatus > 0)
                {
                    _logger.LogInformation("Attempting to update appointment {AppointmentId} status", parsedAppointmentIdForStatus);
                    var appointment = await _context.Appointments.FindAsync(parsedAppointmentIdForStatus);
                    if (appointment != null)
                    {
                        _logger.LogInformation("Found appointment {AppointmentId}, current status: {Status}", parsedAppointmentIdForStatus, appointment.Status);
                        appointment.Status = Barangay.Models.AppointmentStatus.InProgress;
                        appointment.UpdatedAt = DateTime.UtcNow;
                        _logger.LogInformation("Updated appointment {AppointmentId} status from Draft to InProgress", parsedAppointmentIdForStatus);
                    }
                    else
                    {
                        _logger.LogWarning("Appointment {AppointmentId} not found in database", parsedAppointmentIdForStatus);
                    }
                }
                else
                {
                    _logger.LogWarning("No valid AppointmentId provided for status update");
                }
                
                // Save to database with better error handling
                try
                {
                    var rowsAffected = await _context.SaveChangesAsync();
                    _logger.LogInformation("HEEADSSS assessment saved successfully with ID: {Id}, rows affected: {RowsAffected}", assessment.Id, rowsAffected);
                    
                    if (rowsAffected > 0)
                    {
                        return new JsonResult(new { success = true });
                    }
                    else
                    {
                        _logger.LogWarning("No rows were affected during save operation");
                        return new JsonResult(new { success = false, error = "Failed to save assessment. Please try again." });
                    }
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "Database update error: {Message}", dbEx.Message);
                    return new JsonResult(new { success = false, error = "Database error occurred. Please check your data and try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving HEEADSSS assessment");
                return new JsonResult(new { success = false, error = "An error occurred while saving the assessment. Please try again." });
            }
        }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            
            if (today < birthDate.AddYears(age))
            {
                age--;
            }
            
            _logger.LogInformation($"Calculated age for birthdate {birthDate.ToShortDateString()}: {age} years (using current date: {today.ToShortDateString()})");
            return age;
        }

        private async Task<string> GetOrGenerateFamilyNumber(ApplicationUser user)
        {
            // Check if user already has a family number
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

            // Get the first letter of the user's last name
            var lastName = user.LastName ?? user.FullName?.Split(' ').LastOrDefault() ?? "X";
            var firstLetter = lastName.Substring(0, 1).ToUpper();

            // Get the highest sequence number for this letter
            var lastNumber = await _context.HEEADSSSAssessments
                .Where(a => a.FamilyNo.StartsWith(firstLetter + "-"))
                .Select(a => a.FamilyNo.Substring(2))
                .Where(n => n.All(char.IsDigit))
                .Select(n => int.Parse(n))
                    .DefaultIfEmpty(0)
                .MaxAsync();
                
            // Generate new family number
            var newSequence = lastNumber + 1;
            return $"{firstLetter}-{newSequence:D3}"; // Format: X-001, X-002, etc.
        }

        // Helper method to get form value with default
        private string GetFormValueOrDefault(string key, string defaultValue = "Not provided")
        {
            if (Request.Form.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value.ToString()))
            {
                return value.ToString();
            }
            return defaultValue;
        }
        
        // AJAX endpoint for calculating age from birthdate
        public IActionResult OnPostCalculateAgeAsync(string birthdate)
        {
            _logger.LogInformation($"Calculating age for birthdate: {birthdate}");
            
            try
            {
                if (DateTime.TryParse(birthdate, out DateTime parsedDate))
                {
                    int age = CalculateAge(parsedDate);
                    _logger.LogInformation($"Server calculated age: {age}");
                    return new JsonResult(new { success = true, age = age });
                }
                else
                {
                    _logger.LogWarning($"Could not parse birthdate: {birthdate}");
                    return new JsonResult(new { success = false, error = "Invalid date format" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calculating age for birthdate: {birthdate}");
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }

        // AJAX endpoint to get next sequential family number
        public async Task<IActionResult> OnGetNextFamilyNumberAsync(string letterPrefix)
        {
            try
            {
                if (string.IsNullOrEmpty(letterPrefix) || letterPrefix.Length == 0)
                {
                    return new JsonResult(new { success = false, error = "Letter prefix is required" });
                }

                string firstLetter = letterPrefix.Substring(0, 1).ToUpper();
                
                // Get the highest sequence number for this letter from both assessment types
                int lastNCDNumber = await _context.NCDRiskAssessments
                    .Where(a => a.FamilyNo != null && a.FamilyNo.StartsWith(firstLetter + "-"))
                    .Select(a => a.FamilyNo.Substring(2))
                    .Where(n => n.All(char.IsDigit))
                    .Select(n => int.Parse(n))
                    .DefaultIfEmpty(0)
                    .MaxAsync();
                    
                int lastHEEADSSSNumber = await _context.HEEADSSSAssessments
                    .Where(a => a.FamilyNo != null && a.FamilyNo.StartsWith(firstLetter + "-"))
                    .Select(a => a.FamilyNo.Substring(2))
                    .Where(n => n.All(char.IsDigit))
                    .Select(n => int.Parse(n))
                    .DefaultIfEmpty(0)
                    .MaxAsync();
                    
                // Take the highest of the two numbers
                int lastNumber = Math.Max(lastNCDNumber, lastHEEADSSSNumber);
                
                // Generate new family number
                int newSequence = lastNumber + 1;
                string newFamilyNo = $"{firstLetter}-{newSequence:D3}"; // Format: X-001, X-002, etc.
                
                return new JsonResult(new { 
                    success = true, 
                    familyNo = newFamilyNo,
                    lastNumber = lastNumber,
                    newSequence = newSequence
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating next family number");
                return new JsonResult(new { success = false, error = "Error generating next family number" });
            }
        }

    }
} 