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
using Barangay.Extensions;

namespace Barangay.Pages.Nurse
{
    public class EditHEEADSSSAssessmentModel : PageModel
    {
        private readonly EncryptedDbContext _context;
        private readonly ILogger<EditHEEADSSSAssessmentModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataEncryptionService _encryptionService;

        public EditHEEADSSSAssessmentModel(
            EncryptedDbContext context,
            ILogger<EditHEEADSSSAssessmentModel> logger,
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

        public async Task<IActionResult> OnGetAsync(int appointmentId)
        {
            try
            {
                // Get the appointment
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .FirstOrDefaultAsync(a => a.Id == appointmentId);

                if (appointment == null)
                {
                    TempData["StatusMessage"] = "Error: Appointment not found.";
                    return RedirectToPage("/Nurse/Appointments");
                }

                // Get existing HEEADSSS assessment
                // Since AppointmentId is encrypted, we need to check all records and decrypt them
                var allHEEADSSSAssessments = await _context.HEEADSSSAssessments
                    .AsNoTracking()
                    .ToListAsync();

                HEEADSSSAssessment existingAssessment = null;

                foreach (var assessment in allHEEADSSSAssessments)
                {
                    try
                    {
                        // Decrypt the AppointmentId to check if it matches
                        var decryptedAppointmentId = _encryptionService.DecryptForUser(assessment.AppointmentId, User);
                        if (decryptedAppointmentId == appointmentId.ToString())
                        {
                            existingAssessment = assessment;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to decrypt AppointmentId for HEEADSSS assessment {Id}", assessment.Id);
                        // Continue checking other assessments
                    }
                }

                if (existingAssessment == null)
                {
                    TempData["StatusMessage"] = "Error: HEEADSSS assessment not found.";
                    return RedirectToPage("/Nurse/AppointmentDetails", new { id = appointmentId });
                }

                // Decrypt existing assessment data for editing
                existingAssessment.DecryptSensitiveData(_encryptionService, User);

                // Map to ViewModel
                Assessment = new HEEADSSSAssessmentViewModel
                {
                    AppointmentId = appointmentId,
                    UserId = appointment.PatientId,
                    FullName = appointment.Patient?.FullName,
                    Age = appointment.AgeValue,
                    Birthday = appointment.DateOfBirth ?? DateTime.Today.AddYears(-19),
                    HealthFacility = "Baesa Health Center",
                    FamilyNo = "C-001",
                    
                    // Map all the assessment fields
                    HomeFamilyProblems = existingAssessment.HomeFamilyProblems,
                    HomeParentalListening = existingAssessment.HomeParentalListening,
                    HomeParentalBlame = existingAssessment.HomeParentalBlame,
                    HomeFamilyChanges = existingAssessment.HomeFamilyChanges,
                    
                    EducationCurrentlyStudying = existingAssessment.EducationCurrentlyStudying,
                    EducationWorking = existingAssessment.EducationWorking,
                    EducationSchoolWorkProblems = existingAssessment.EducationSchoolWorkProblems,
                    
                    EatingBodyImageSatisfaction = existingAssessment.EatingBodyImageSatisfaction,
                    EatingDisorderedEatingBehaviors = existingAssessment.EatingDisorderedEatingBehaviors,
                    
                    ActivitiesParticipation = existingAssessment.ActivitiesParticipation,
                    ActivitiesRegularExercise = existingAssessment.ActivitiesRegularExercise,
                    
                    DrugsTobaccoUse = existingAssessment.DrugsTobaccoUse,
                    DrugsAlcoholUse = existingAssessment.DrugsAlcoholUse,
                    
                    SexualityIntimateRelationships = existingAssessment.SexualityIntimateRelationships,
                    SexualityProtection = existingAssessment.SexualityProtection,
                    
                    SafetyPhysicalAbuse = existingAssessment.SafetyPhysicalAbuse,
                    SafetyRelationshipViolence = existingAssessment.SafetyRelationshipViolence,
                    
                    SuicideDepressionFeelings = existingAssessment.SuicideDepressionFeelings,
                    SuicideSelfHarmThoughts = existingAssessment.SuicideSelfHarmThoughts,
                    
                    AssessedBy = existingAssessment.AssessedBy,
                    Notes = existingAssessment.Notes
                };

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading HEEADSSS assessment for appointment {AppointmentId}", appointmentId);
                TempData["StatusMessage"] = "Error: Unable to load assessment.";
                return RedirectToPage("/Nurse/Appointments");
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

                // Get existing assessment
                // Since AppointmentId is encrypted, we need to check all records and decrypt them
                var allHEEADSSSAssessments = await _context.HEEADSSSAssessments
                    .AsNoTracking()
                    .ToListAsync();

                HEEADSSSAssessment existingAssessment = null;

                foreach (var assessment in allHEEADSSSAssessments)
                {
                    try
                    {
                        // Decrypt the AppointmentId to check if it matches
                        var decryptedAppointmentId = _encryptionService.DecryptForUser(assessment.AppointmentId, User);
                        if (decryptedAppointmentId == Assessment.AppointmentId.ToString())
                        {
                            existingAssessment = assessment;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to decrypt AppointmentId for HEEADSSS assessment {Id}", assessment.Id);
                        // Continue checking other assessments
                    }
                }

                if (existingAssessment == null)
                {
                    TempData["StatusMessage"] = "Error: Assessment not found.";
                    return RedirectToPage("/Nurse/Appointments");
                }

                // Update the assessment
                existingAssessment.HomeFamilyProblems = Assessment.HomeFamilyProblems;
                existingAssessment.HomeParentalListening = Assessment.HomeParentalListening;
                existingAssessment.HomeParentalBlame = Assessment.HomeParentalBlame;
                existingAssessment.HomeFamilyChanges = Assessment.HomeFamilyChanges;
                
                existingAssessment.EducationCurrentlyStudying = Assessment.EducationCurrentlyStudying;
                existingAssessment.EducationWorking = Assessment.EducationWorking;
                existingAssessment.EducationSchoolWorkProblems = Assessment.EducationSchoolWorkProblems;
                
                existingAssessment.EatingBodyImageSatisfaction = Assessment.EatingBodyImageSatisfaction;
                existingAssessment.EatingDisorderedEatingBehaviors = Assessment.EatingDisorderedEatingBehaviors;
                
                existingAssessment.ActivitiesParticipation = Assessment.ActivitiesParticipation;
                existingAssessment.ActivitiesRegularExercise = Assessment.ActivitiesRegularExercise;
                
                existingAssessment.DrugsTobaccoUse = Assessment.DrugsTobaccoUse;
                existingAssessment.DrugsAlcoholUse = Assessment.DrugsAlcoholUse;
                
                existingAssessment.SexualityIntimateRelationships = Assessment.SexualityIntimateRelationships;
                existingAssessment.SexualityProtection = Assessment.SexualityProtection;
                
                existingAssessment.SafetyPhysicalAbuse = Assessment.SafetyPhysicalAbuse;
                existingAssessment.SafetyRelationshipViolence = Assessment.SafetyRelationshipViolence;
                
                existingAssessment.SuicideDepressionFeelings = Assessment.SuicideDepressionFeelings;
                existingAssessment.SuicideSelfHarmThoughts = Assessment.SuicideSelfHarmThoughts;
                
                existingAssessment.AssessedBy = Assessment.AssessedBy;
                existingAssessment.Notes = Assessment.Notes;
                existingAssessment.UpdatedAt = DateTime.Now;

                // Encrypt sensitive data before saving
                existingAssessment.EncryptSensitiveData(_encryptionService);

                await _context.SaveChangesAsync();

                _logger.LogInformation("HEEADSSS assessment updated successfully for appointment {AppointmentId}", Assessment.AppointmentId);
                TempData["StatusMessage"] = "HEEADSSS assessment updated successfully.";
                
                return RedirectToPage("/Nurse/AppointmentDetails", new { id = Assessment.AppointmentId ?? 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating HEEADSSS assessment for appointment {AppointmentId}", Assessment.AppointmentId);
                TempData["StatusMessage"] = "Error: Unable to update assessment.";
                return Page();
            }
        }
    }
}
