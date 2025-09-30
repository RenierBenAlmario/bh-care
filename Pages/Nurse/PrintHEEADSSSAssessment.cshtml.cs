using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Barangay.Data;
using Barangay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Barangay.Services;
using System.Security.Claims;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse,Head Nurse")]
    public class PrintHEEADSSSAssessmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PrintHEEADSSSAssessmentModel> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IDataEncryptionService _encryptionService;

        public PrintHEEADSSSAssessmentModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<PrintHEEADSSSAssessmentModel> logger,
            IPermissionService permissionService,
            IDataEncryptionService encryptionService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _permissionService = permissionService;
            _encryptionService = encryptionService;
        }

        public HEEADSSSAssessment HEEADSSSAssessment { get; set; }

        public async Task<IActionResult> OnGetAsync(int appointmentId)
        {
            try
            {
                // Nurses have permission to print assessments by default
                _logger.LogInformation("Nurse printing HEEADSSS assessment for appointment {AppointmentId}", appointmentId);

                // Get the appointment first to find the patient
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .FirstOrDefaultAsync(a => a.Id == appointmentId);

                if (appointment == null)
                {
                    TempData["StatusMessage"] = "Error: Appointment not found.";
                    return RedirectToPage("/Nurse/AppointmentDetails", new { id = appointmentId });
                }

                // Get HEEADSSS assessment by UserId (same logic as EditHEEADSSSAssessment)
                HEEADSSSAssessment existingAssessment = null;

                if (appointment.Patient != null)
                {
                    // Decrypt patient data first
                    appointment.Patient.DecryptSensitiveData(_encryptionService, User);
                    
                    // Look for HEEADSSS assessment by UserId
                    existingAssessment = await _context.HEEADSSSAssessments
                        .Where(a => a.UserId == appointment.Patient.UserId)
                        .OrderByDescending(a => a.CreatedAt)
                        .FirstOrDefaultAsync();
                }

                if (existingAssessment == null)
                {
                    TempData["StatusMessage"] = "Error: HEEADSSS assessment not found.";
                    return RedirectToPage("/Nurse/AppointmentDetails", new { id = appointmentId });
                }

                // Decrypt sensitive data for display
                existingAssessment.DecryptSensitiveData(_encryptionService, User);
                
                // Manual decryption fallback for all HEEADSSS fields
                // Personal Information
                if (!string.IsNullOrEmpty(existingAssessment.FullName) && _encryptionService.IsEncrypted(existingAssessment.FullName))
                {
                    existingAssessment.FullName = _encryptionService.DecryptForUser(existingAssessment.FullName, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.Age) && _encryptionService.IsEncrypted(existingAssessment.Age))
                {
                    existingAssessment.Age = _encryptionService.DecryptForUser(existingAssessment.Age, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.Gender) && _encryptionService.IsEncrypted(existingAssessment.Gender))
                {
                    existingAssessment.Gender = _encryptionService.DecryptForUser(existingAssessment.Gender, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.Address) && _encryptionService.IsEncrypted(existingAssessment.Address))
                {
                    existingAssessment.Address = _encryptionService.DecryptForUser(existingAssessment.Address, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.ContactNumber) && _encryptionService.IsEncrypted(existingAssessment.ContactNumber))
                {
                    existingAssessment.ContactNumber = _encryptionService.DecryptForUser(existingAssessment.ContactNumber, User);
                }
                
                // Adolescent Health Information
                if (!string.IsNullOrEmpty(existingAssessment.Height) && _encryptionService.IsEncrypted(existingAssessment.Height))
                {
                    existingAssessment.Height = _encryptionService.DecryptForUser(existingAssessment.Height, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.Weight) && _encryptionService.IsEncrypted(existingAssessment.Weight))
                {
                    existingAssessment.Weight = _encryptionService.DecryptForUser(existingAssessment.Weight, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.BMI) && _encryptionService.IsEncrypted(existingAssessment.BMI))
                {
                    existingAssessment.BMI = _encryptionService.DecryptForUser(existingAssessment.BMI, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.ImmunizationMR) && _encryptionService.IsEncrypted(existingAssessment.ImmunizationMR))
                {
                    existingAssessment.ImmunizationMR = _encryptionService.DecryptForUser(existingAssessment.ImmunizationMR, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.ImmunizationTd) && _encryptionService.IsEncrypted(existingAssessment.ImmunizationTd))
                {
                    existingAssessment.ImmunizationTd = _encryptionService.DecryptForUser(existingAssessment.ImmunizationTd, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.ImmunizationHPV) && _encryptionService.IsEncrypted(existingAssessment.ImmunizationHPV))
                {
                    existingAssessment.ImmunizationHPV = _encryptionService.DecryptForUser(existingAssessment.ImmunizationHPV, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.DateOfMenarche) && _encryptionService.IsEncrypted(existingAssessment.DateOfMenarche))
                {
                    existingAssessment.DateOfMenarche = _encryptionService.DecryptForUser(existingAssessment.DateOfMenarche, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.AgeOfFirstPregnancy) && _encryptionService.IsEncrypted(existingAssessment.AgeOfFirstPregnancy))
                {
                    existingAssessment.AgeOfFirstPregnancy = _encryptionService.DecryptForUser(existingAssessment.AgeOfFirstPregnancy, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.OBScore) && _encryptionService.IsEncrypted(existingAssessment.OBScore))
                {
                    existingAssessment.OBScore = _encryptionService.DecryptForUser(existingAssessment.OBScore, User);
                }
                
                // Vital Signs
                if (!string.IsNullOrEmpty(existingAssessment.VitalTemp) && _encryptionService.IsEncrypted(existingAssessment.VitalTemp))
                {
                    existingAssessment.VitalTemp = _encryptionService.DecryptForUser(existingAssessment.VitalTemp, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.VitalRR) && _encryptionService.IsEncrypted(existingAssessment.VitalRR))
                {
                    existingAssessment.VitalRR = _encryptionService.DecryptForUser(existingAssessment.VitalRR, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.VitalPR) && _encryptionService.IsEncrypted(existingAssessment.VitalPR))
                {
                    existingAssessment.VitalPR = _encryptionService.DecryptForUser(existingAssessment.VitalPR, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.VitalBP) && _encryptionService.IsEncrypted(existingAssessment.VitalBP))
                {
                    existingAssessment.VitalBP = _encryptionService.DecryptForUser(existingAssessment.VitalBP, User);
                }
                
                // Medical Information
                if (!string.IsNullOrEmpty(existingAssessment.ChiefComplaint) && _encryptionService.IsEncrypted(existingAssessment.ChiefComplaint))
                {
                    existingAssessment.ChiefComplaint = _encryptionService.DecryptForUser(existingAssessment.ChiefComplaint, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.HistoryOfPresentIllness) && _encryptionService.IsEncrypted(existingAssessment.HistoryOfPresentIllness))
                {
                    existingAssessment.HistoryOfPresentIllness = _encryptionService.DecryptForUser(existingAssessment.HistoryOfPresentIllness, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.PhysicalExaminationFindings) && _encryptionService.IsEncrypted(existingAssessment.PhysicalExaminationFindings))
                {
                    existingAssessment.PhysicalExaminationFindings = _encryptionService.DecryptForUser(existingAssessment.PhysicalExaminationFindings, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.PastMedicalHistory) && _encryptionService.IsEncrypted(existingAssessment.PastMedicalHistory))
                {
                    existingAssessment.PastMedicalHistory = _encryptionService.DecryptForUser(existingAssessment.PastMedicalHistory, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.WorkingDiagnosis) && _encryptionService.IsEncrypted(existingAssessment.WorkingDiagnosis))
                {
                    existingAssessment.WorkingDiagnosis = _encryptionService.DecryptForUser(existingAssessment.WorkingDiagnosis, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.Management) && _encryptionService.IsEncrypted(existingAssessment.Management))
                {
                    existingAssessment.Management = _encryptionService.DecryptForUser(existingAssessment.Management, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.FamilyHistory) && _encryptionService.IsEncrypted(existingAssessment.FamilyHistory))
                {
                    existingAssessment.FamilyHistory = _encryptionService.DecryptForUser(existingAssessment.FamilyHistory, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.ReferredTo) && _encryptionService.IsEncrypted(existingAssessment.ReferredTo))
                {
                    existingAssessment.ReferredTo = _encryptionService.DecryptForUser(existingAssessment.ReferredTo, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.ReasonForReferral) && _encryptionService.IsEncrypted(existingAssessment.ReasonForReferral))
                {
                    existingAssessment.ReasonForReferral = _encryptionService.DecryptForUser(existingAssessment.ReasonForReferral, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.FollowUpDate) && _encryptionService.IsEncrypted(existingAssessment.FollowUpDate))
                {
                    existingAssessment.FollowUpDate = _encryptionService.DecryptForUser(existingAssessment.FollowUpDate, User);
                }
                
                // Psychosocial History sections already below...
                if (!string.IsNullOrEmpty(existingAssessment.HomeEnvironment) && _encryptionService.IsEncrypted(existingAssessment.HomeEnvironment))
                {
                    existingAssessment.HomeEnvironment = _encryptionService.DecryptForUser(existingAssessment.HomeEnvironment, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.FamilyRelationship) && _encryptionService.IsEncrypted(existingAssessment.FamilyRelationship))
                {
                    existingAssessment.FamilyRelationship = _encryptionService.DecryptForUser(existingAssessment.FamilyRelationship, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.HomeFamilyProblems) && _encryptionService.IsEncrypted(existingAssessment.HomeFamilyProblems))
                {
                    existingAssessment.HomeFamilyProblems = _encryptionService.DecryptForUser(existingAssessment.HomeFamilyProblems, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.HomeParentalListening) && _encryptionService.IsEncrypted(existingAssessment.HomeParentalListening))
                {
                    existingAssessment.HomeParentalListening = _encryptionService.DecryptForUser(existingAssessment.HomeParentalListening, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.SchoolPerformance) && _encryptionService.IsEncrypted(existingAssessment.SchoolPerformance))
                {
                    existingAssessment.SchoolPerformance = _encryptionService.DecryptForUser(existingAssessment.SchoolPerformance, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.AttendanceIssues) && _encryptionService.IsEncrypted(existingAssessment.AttendanceIssues))
                {
                    existingAssessment.AttendanceIssues = _encryptionService.DecryptForUser(existingAssessment.AttendanceIssues, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.CareerPlans) && _encryptionService.IsEncrypted(existingAssessment.CareerPlans))
                {
                    existingAssessment.CareerPlans = _encryptionService.DecryptForUser(existingAssessment.CareerPlans, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.EducationCurrentlyStudying) && _encryptionService.IsEncrypted(existingAssessment.EducationCurrentlyStudying))
                {
                    existingAssessment.EducationCurrentlyStudying = _encryptionService.DecryptForUser(existingAssessment.EducationCurrentlyStudying, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.Hobbies) && _encryptionService.IsEncrypted(existingAssessment.Hobbies))
                {
                    existingAssessment.Hobbies = _encryptionService.DecryptForUser(existingAssessment.Hobbies, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.PhysicalActivity) && _encryptionService.IsEncrypted(existingAssessment.PhysicalActivity))
                {
                    existingAssessment.PhysicalActivity = _encryptionService.DecryptForUser(existingAssessment.PhysicalActivity, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.ScreenTime) && _encryptionService.IsEncrypted(existingAssessment.ScreenTime))
                {
                    existingAssessment.ScreenTime = _encryptionService.DecryptForUser(existingAssessment.ScreenTime, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.ActivitiesRegularExercise) && _encryptionService.IsEncrypted(existingAssessment.ActivitiesRegularExercise))
                {
                    existingAssessment.ActivitiesRegularExercise = _encryptionService.DecryptForUser(existingAssessment.ActivitiesRegularExercise, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.FamilyNo) && _encryptionService.IsEncrypted(existingAssessment.FamilyNo))
                {
                    existingAssessment.FamilyNo = _encryptionService.DecryptForUser(existingAssessment.FamilyNo, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.HomeFamilyChanges) && _encryptionService.IsEncrypted(existingAssessment.HomeFamilyChanges))
                {
                    existingAssessment.HomeFamilyChanges = _encryptionService.DecryptForUser(existingAssessment.HomeFamilyChanges, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.EducationWorking) && _encryptionService.IsEncrypted(existingAssessment.EducationWorking))
                {
                    existingAssessment.EducationWorking = _encryptionService.DecryptForUser(existingAssessment.EducationWorking, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.EducationSchoolWorkProblems) && _encryptionService.IsEncrypted(existingAssessment.EducationSchoolWorkProblems))
                {
                    existingAssessment.EducationSchoolWorkProblems = _encryptionService.DecryptForUser(existingAssessment.EducationSchoolWorkProblems, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.EducationBullying) && _encryptionService.IsEncrypted(existingAssessment.EducationBullying))
                {
                    existingAssessment.EducationBullying = _encryptionService.DecryptForUser(existingAssessment.EducationBullying, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.EatingBodyImageSatisfaction) && _encryptionService.IsEncrypted(existingAssessment.EatingBodyImageSatisfaction))
                {
                    existingAssessment.EatingBodyImageSatisfaction = _encryptionService.DecryptForUser(existingAssessment.EatingBodyImageSatisfaction, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.EatingWeightComments) && _encryptionService.IsEncrypted(existingAssessment.EatingWeightComments))
                {
                    existingAssessment.EatingWeightComments = _encryptionService.DecryptForUser(existingAssessment.EatingWeightComments, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.ActivitiesParticipation) && _encryptionService.IsEncrypted(existingAssessment.ActivitiesParticipation))
                {
                    existingAssessment.ActivitiesParticipation = _encryptionService.DecryptForUser(existingAssessment.ActivitiesParticipation, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.ActivitiesScreenTime) && _encryptionService.IsEncrypted(existingAssessment.ActivitiesScreenTime))
                {
                    existingAssessment.ActivitiesScreenTime = _encryptionService.DecryptForUser(existingAssessment.ActivitiesScreenTime, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.SexualityBodyConcerns) && _encryptionService.IsEncrypted(existingAssessment.SexualityBodyConcerns))
                {
                    existingAssessment.SexualityBodyConcerns = _encryptionService.DecryptForUser(existingAssessment.SexualityBodyConcerns, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.SexualityIntimateRelationships) && _encryptionService.IsEncrypted(existingAssessment.SexualityIntimateRelationships))
                {
                    existingAssessment.SexualityIntimateRelationships = _encryptionService.DecryptForUser(existingAssessment.SexualityIntimateRelationships, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.SexualityPartners) && _encryptionService.IsEncrypted(existingAssessment.SexualityPartners))
                {
                    existingAssessment.SexualityPartners = _encryptionService.DecryptForUser(existingAssessment.SexualityPartners, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.SexualityPregnancyExperience) && _encryptionService.IsEncrypted(existingAssessment.SexualityPregnancyExperience))
                {
                    existingAssessment.SexualityPregnancyExperience = _encryptionService.DecryptForUser(existingAssessment.SexualityPregnancyExperience, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.SexualitySTIExperience) && _encryptionService.IsEncrypted(existingAssessment.SexualitySTIExperience))
                {
                    existingAssessment.SexualitySTIExperience = _encryptionService.DecryptForUser(existingAssessment.SexualitySTIExperience, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.SexualityProtectionUse) && _encryptionService.IsEncrypted(existingAssessment.SexualityProtectionUse))
                {
                    existingAssessment.SexualityProtectionUse = _encryptionService.DecryptForUser(existingAssessment.SexualityProtectionUse, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.SafetyPhysicalAbuse) && _encryptionService.IsEncrypted(existingAssessment.SafetyPhysicalAbuse))
                {
                    existingAssessment.SafetyPhysicalAbuse = _encryptionService.DecryptForUser(existingAssessment.SafetyPhysicalAbuse, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.SafetyRelationshipViolence) && _encryptionService.IsEncrypted(existingAssessment.SafetyRelationshipViolence))
                {
                    existingAssessment.SafetyRelationshipViolence = _encryptionService.DecryptForUser(existingAssessment.SafetyRelationshipViolence, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.SafetyProtectiveGear) && _encryptionService.IsEncrypted(existingAssessment.SafetyProtectiveGear))
                {
                    existingAssessment.SafetyProtectiveGear = _encryptionService.DecryptForUser(existingAssessment.SafetyProtectiveGear, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.SafetyGunsAtHome) && _encryptionService.IsEncrypted(existingAssessment.SafetyGunsAtHome))
                {
                    existingAssessment.SafetyGunsAtHome = _encryptionService.DecryptForUser(existingAssessment.SafetyGunsAtHome, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.SuicideDepressionFeelings) && _encryptionService.IsEncrypted(existingAssessment.SuicideDepressionFeelings))
                {
                    existingAssessment.SuicideDepressionFeelings = _encryptionService.DecryptForUser(existingAssessment.SuicideDepressionFeelings, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.SuicideSelfHarmThoughts) && _encryptionService.IsEncrypted(existingAssessment.SuicideSelfHarmThoughts))
                {
                    existingAssessment.SuicideSelfHarmThoughts = _encryptionService.DecryptForUser(existingAssessment.SuicideSelfHarmThoughts, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.SuicideFamilyHistory) && _encryptionService.IsEncrypted(existingAssessment.SuicideFamilyHistory))
                {
                    existingAssessment.SuicideFamilyHistory = _encryptionService.DecryptForUser(existingAssessment.SuicideFamilyHistory, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.Notes) && _encryptionService.IsEncrypted(existingAssessment.Notes))
                {
                    existingAssessment.Notes = _encryptionService.DecryptForUser(existingAssessment.Notes, User);
                }
                if (!string.IsNullOrEmpty(existingAssessment.AssessedBy) && _encryptionService.IsEncrypted(existingAssessment.AssessedBy))
                {
                    existingAssessment.AssessedBy = _encryptionService.DecryptForUser(existingAssessment.AssessedBy, User);
                }

                HEEADSSSAssessment = existingAssessment;

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading HEEADSSS assessment for printing, appointment {AppointmentId}", appointmentId);
                TempData["StatusMessage"] = "Error: Unable to load assessment for printing.";
                return RedirectToPage("/Nurse/AppointmentDetails", new { id = appointmentId });
            }
        }
    }
}
