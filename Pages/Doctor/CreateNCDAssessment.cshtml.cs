using System;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
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

namespace Barangay.Pages.Doctor
{
    [Authorize(Roles = "Nurse,Head Nurse,Doctor,Head Doctor")]
    public class CreateNCDAssessmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateNCDAssessmentModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataEncryptionService _encryptionService;

        public CreateNCDAssessmentModel(
            ApplicationDbContext context,
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

                // Populate patient information
                PatientName = $"{appointment.Patient.User.FirstName} {appointment.Patient.User.LastName}";
                PatientAddress = appointment.Patient.User.Address ?? "";
                PatientBarangay = appointment.Patient.User.Barangay ?? "";
                PatientPhone = appointment.Patient.User.PhoneNumber ?? "";
                var birthDate = appointment.Patient.User.BirthDate ?? DateTime.Now.AddYears(-30);
                PatientAge = CalculateAge(birthDate);

                // Set assessment properties
                Assessment.AppointmentId = appointmentId.Value;
                Assessment.UserId = appointment.Patient.UserId;
                Assessment.Birthday = birthDate.ToString("yyyy-MM-dd");
                Assessment.Edad = PatientAge.ToString();

                // Check if assessment already exists
                var existingAssessment = await _context.NCDRiskAssessments
                    .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

                if (existingAssessment != null)
                {
                    _logger.LogInformation("Existing assessment found for appointment ID {AppointmentId}. Loading for editing.", appointmentId);
                    // Decrypt and load into bound model for editing
                    _logger.LogInformation("Before decryption - HealthFacility: {HealthFacility}", existingAssessment.HealthFacility);
                    _logger.LogInformation("Before decryption - Weight: {Weight}", existingAssessment.Weight);
                    _logger.LogInformation("Before decryption - Height: {Height}", existingAssessment.Height);
                    
                    // Test decryption service directly
                    _logger.LogInformation("Testing decryption service - CanUserDecrypt: {CanDecrypt}", _encryptionService.CanUserDecrypt(User));
                    if (!string.IsNullOrEmpty(existingAssessment.Weight))
                    {
                        _logger.LogInformation("Testing Weight decryption - IsEncrypted: {IsEncrypted}", _encryptionService.IsEncrypted(existingAssessment.Weight));
                        try
                        {
                            var testDecrypt = _encryptionService.DecryptForUser(existingAssessment.Weight, User);
                            _logger.LogInformation("Test decryption result: {Result}", testDecrypt);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Test decryption failed");
                        }
                    }
                    
                    existingAssessment.DecryptSensitiveData(_encryptionService, User);
                    _logger.LogInformation("After decryption - HealthFacility: {HealthFacility}", existingAssessment.HealthFacility);
                    _logger.LogInformation("After decryption - Weight: {Weight}", existingAssessment.Weight);
                    _logger.LogInformation("After decryption - Height: {Height}", existingAssessment.Height);
                    
                    // Manually decrypt all encrypted fields for display
                    DecryptAllFields(existingAssessment);
                    
                    Assessment = existingAssessment;
                    
                    // Convert string boolean values to proper booleans for Razor binding
                    ConvertStringBooleansToBool(Assessment);
                }
                else
                {
                    _logger.LogInformation("Creating new assessment for appointment {AppointmentId}", appointmentId);
                    // Initialize new assessment with patient data
                    Assessment.FirstName = appointment.Patient.User.FirstName ?? "";
                    Assessment.LastName = appointment.Patient.User.LastName ?? "";
                    Assessment.MiddleName = appointment.Patient.User.MiddleName ?? "";
                    Assessment.Address = PatientAddress;
                    Assessment.Barangay = PatientBarangay;
                    Assessment.Telepono = PatientPhone;
                    Assessment.Kasarian = appointment.Patient.User.Gender ?? "";
                    Assessment.DateOfAssessment = DateTime.Now.ToString("yyyy-MM-dd");
                    
                    // Generate family number
                    Assessment.FamilyNo = await GetOrGenerateFamilyNumber(appointment.Patient.User);
                    
                    // Convert string boolean values to proper booleans for Razor binding
                    ConvertStringBooleansToBool(Assessment);
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading appointment data for ID: {Id}", appointmentId);
                StatusMessage = "Error loading appointment data. Please try again later.";
                return RedirectToPage("/Doctor/Appointments");
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
                    return RedirectToPage("/Doctor/Appointments");
                }

                // Make sure cancer type is only set when cancer is checked
                if (Assessment.HasCancer != "true")
                {
                    Assessment.CancerType = null;
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
    existing.CreatedAt = string.IsNullOrWhiteSpace(existing.CreatedAt) ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : existing.CreatedAt;
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
                
                return RedirectToPage("/Doctor/Consultation", new { id = Assessment.AppointmentId });
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

        private void DecryptAllFields(NCDRiskAssessment assessment)
        {
            // Get all properties with [Encrypted] attribute
            var properties = typeof(NCDRiskAssessment).GetProperties()
                .Where(p => p.GetCustomAttribute<Barangay.Attributes.EncryptedAttribute>() != null)
                .ToList();

            foreach (var property in properties)
            {
                if (property.CanWrite && property.PropertyType == typeof(string))
                {
                    var value = property.GetValue(assessment)?.ToString();
                    if (!string.IsNullOrEmpty(value) && _encryptionService.IsEncrypted(value))
                    {
                        try
                        {
                            var decryptedValue = _encryptionService.DecryptForUser(value, User);
                            if (decryptedValue != value && !decryptedValue.Contains("[ACCESS DENIED]"))
                            {
                                property.SetValue(assessment, decryptedValue);
                                _logger.LogInformation("Decrypted {PropertyName}: {DecryptedValue}", property.Name, decryptedValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to decrypt {PropertyName}", property.Name);
                        }
                    }
                }
            }
        }

        private void ConvertStringBooleansToBool(NCDRiskAssessment assessment)
        {
            // Convert encrypted string boolean values to "true"/"false" for Razor checkbox binding
            // This is needed because encrypted boolean values are stored as encrypted strings
            // Only include properties that are actually used in the Razor page as checkboxes
            var booleanProperties = new[]
            {
                nameof(assessment.HasDiabetes),
                nameof(assessment.HasHypertension),
                nameof(assessment.HasCancer),
                nameof(assessment.HasCOPD),
                nameof(assessment.HasLungDisease),
                nameof(assessment.FamilyHasHypertension),
                nameof(assessment.FamilyHasStroke),
                nameof(assessment.FamilyHasDiabetes),
                nameof(assessment.FamilyHasCancer),
                nameof(assessment.FamilyHasKidneyDisease),
                nameof(assessment.FamilyHasOtherDisease),
                nameof(assessment.HasHighSaltIntake),
                nameof(assessment.HasPolyuria),
                nameof(assessment.HasPolydipsia),
                nameof(assessment.HasPolyphagia),
                nameof(assessment.HasWeightLoss),
                nameof(assessment.HasUrineProtein),
                nameof(assessment.HasUrineKetones),
                nameof(assessment.BreastCancerScreened),
                nameof(assessment.CervicalCancerScreened),
                nameof(assessment.HasChestPain),
                nameof(assessment.ChestPainSpreadsToArm),
                nameof(assessment.NumbnessWhenWalkingFast),
                nameof(assessment.PainRelievedWithRest),
                nameof(assessment.LossOfConsciousnessLessThan10Min),
                nameof(assessment.PainLastsMoreThan30Min),
                nameof(assessment.SeeDoctorIfYes),
                nameof(assessment.EatsVegetablesDaily),
                nameof(assessment.EatsFruitsDaily),
                nameof(assessment.EatsFishDaily),
                nameof(assessment.EatsMeatDaily),
                nameof(assessment.HasUnhealthyDiet),
                nameof(assessment.EatsFattyFoodMoreThan2TimesPerWeek),
                nameof(assessment.EatsSweetFoodMoreThan2TimesPerWeek),
                nameof(assessment.EatsOilyFoodMoreThan2TimesPerWeek),
                nameof(assessment.DrinksAlcohol),
                nameof(assessment.DrinksBeer),
                nameof(assessment.DrinksWine),
                nameof(assessment.DrinksWhiskyGinBrandy),
                nameof(assessment.AlcoholAmount1Bottle320ml),
                nameof(assessment.AlcoholAmount2Bottle640ml),
                nameof(assessment.AlcoholAmount3to4WineGlasses300ml),
                nameof(assessment.AlcoholAmountLessThan3Shot45ml),
                nameof(assessment.AlcoholAmountMoreThan4Shots75ml),
                nameof(assessment.AlcoholFrequency1to3TimesPerWeek),
                nameof(assessment.AlcoholFrequencyMoreThan4TimesPerWeek),
                nameof(assessment.IsBingeDrinker),
                nameof(assessment.ModerateIntensityExercise),
                nameof(assessment.VigorousIntensityExercise),
                nameof(assessment.CombinationExercise),
                nameof(assessment.InsufficientPhysicalActivity),
                nameof(assessment.FormerSmoker),
                nameof(assessment.NeverSmokedButExposedToSmoke),
                nameof(assessment.HasHistoryOfSmoking),
                nameof(assessment.HasStress)
            };

            foreach (var propName in booleanProperties)
            {
                var property = typeof(NCDRiskAssessment).GetProperty(propName);
                if (property != null && property.PropertyType == typeof(string))
                {
                    var value = property.GetValue(assessment)?.ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        // After manual decryption, values should be plain text
                        if (value.Equals("true", StringComparison.OrdinalIgnoreCase))
                        {
                            property.SetValue(assessment, "true");
                        }
                        else
                        {
                            property.SetValue(assessment, "false");
                        }
                    }
                    else
                    {
                        // Set default to "false" if null or empty
                        property.SetValue(assessment, "false");
                    }
                }
            }
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
            var lastNumber = await _context.NCDRiskAssessments
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
