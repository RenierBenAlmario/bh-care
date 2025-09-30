using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Barangay.Controllers
{
    [Authorize(Roles = "Nurse")]
    public class NurseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<NurseController> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IDataEncryptionService _encryptionService;

        public NurseController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<NurseController> logger,
            IPermissionService permissionService,
            IDataEncryptionService encryptionService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _permissionService = permissionService;
            _encryptionService = encryptionService;
        }

        [HttpGet]
        public async Task<IActionResult> CreateNCDAssessment(int appointmentId)
        {
            try
            {
                // Nurses have permission to create assessments by default
                _logger.LogInformation("Nurse creating NCD assessment for appointment {AppointmentId}", appointmentId);

                // Get appointment details
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .FirstOrDefaultAsync(a => a.Id == appointmentId);

                if (appointment == null)
                {
                    TempData["StatusMessage"] = "Error: Appointment not found.";
                    return RedirectToAction("Appointments", "Nurse");
                }

                // Check if assessment already exists
                var existingAssessment = await _context.NCDRiskAssessments
                    .FirstOrDefaultAsync(n => n.AppointmentId == appointmentId);

                if (existingAssessment != null)
                {
                    TempData["StatusMessage"] = "Error: NCD Risk Assessment already exists for this appointment.";
                    return RedirectToAction("AppointmentDetails", "Nurse", new { id = appointmentId });
                }

                // Create view model with appointment data
                var model = new NCDRiskAssessmentViewModel
                {
                    AppointmentId = appointmentId,
                    UserId = appointment.PatientId,
                    HealthFacility = "Barangay Health Center",
                    FamilyNo = "", // Not available in Patient model
                    Address = appointment.Patient?.Address ?? "",
                    Barangay = "", // Not available in Patient model
                    Birthday = appointment.Patient?.BirthDate != DateTime.MinValue ? appointment.Patient.BirthDate.ToString("yyyy-MM-dd") : null,
                    Telepono = appointment.Patient?.ContactNumber ?? "",
                    Edad = appointment.Patient?.Age.ToString(),
                    Kasarian = appointment.Patient?.Gender ?? "",
                    Relihiyon = "", // Not available in Patient model
                    AppointmentType = appointment.Type ?? "General Checkup",
                    SmokingStatus = "Non-smoker",
                    AlcoholConsumption = "",
                    FirstName = "", // Will be filled from FullName if needed
                    MiddleName = "",
                    LastName = "",
                    Occupation = "", // Not available in Patient model
                    CivilStatus = "" // Not available in Patient model
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading NCD assessment form for appointment {AppointmentId}", appointmentId);
                TempData["StatusMessage"] = "Error: Unable to load assessment form.";
                return RedirectToAction("Appointments", "Nurse");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNCDAssessment(NCDRiskAssessmentViewModel model)
        {
            try
            {
                // Check permissions
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var hasPermission = await _permissionService.UserHasPermissionAsync(userId, "Assessment") ||
                                  await _permissionService.UserHasPermissionAsync(userId, "Consultation");
                
                if (!hasPermission)
                {
                    TempData["StatusMessage"] = "Error: You do not have permission to create assessments.";
                    return RedirectToAction("AppointmentDetails", "Nurse", new { id = model.AppointmentId });
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Create NCD Risk Assessment entity
                var assessment = new NCDRiskAssessment
                {
                    UserId = model.UserId,
                    AppointmentId = model.AppointmentId,
                    HealthFacility = model.HealthFacility,
                    FamilyNo = model.FamilyNo,
                    Address = model.Address,
                    Barangay = model.Barangay,
                    Birthday = model.Birthday,
                    Telepono = model.Telepono,
                    Edad = model.Edad,
                    Kasarian = model.Kasarian,
                    Relihiyon = model.Relihiyon,
                    HasDiabetes = model.HasDiabetes,
                    HasHypertension = model.HasHypertension,
                    HasCancer = model.HasCancer,
                    CancerType = model.CancerType,
                    HasCOPD = model.HasCOPD,
                    HasLungDisease = model.HasLungDisease,
                    HasEyeDisease = model.HasEyeDisease,
                    HighSaltIntake = model.HighSaltIntake,
                    AlcoholFrequency = model.AlcoholFrequency,
                    ExerciseDuration = model.ExerciseDuration,
                    AppointmentType = model.AppointmentType,
                    SmokingStatus = model.SmokingStatus,
                    AlcoholConsumption = model.AlcoholConsumption,
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName,
                    Occupation = model.Occupation,
                    CivilStatus = model.CivilStatus,
                    CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                };

                // Encrypt sensitive data before saving
                assessment.EncryptSensitiveData(_encryptionService);

                _context.NCDRiskAssessments.Add(assessment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("NCD Risk Assessment created successfully for appointment {AppointmentId}", model.AppointmentId);
                TempData["StatusMessage"] = "NCD Risk Assessment created successfully.";
                return RedirectToAction("AppointmentDetails", "Nurse", new { id = model.AppointmentId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating NCD assessment for appointment {AppointmentId}", model.AppointmentId);
                TempData["StatusMessage"] = "Error: Unable to create assessment.";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateHEEADSSSAssessment(int appointmentId)
        {
            try
            {
                // Check permissions
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var hasPermission = await _permissionService.UserHasPermissionAsync(userId, "Assessment") ||
                                  await _permissionService.UserHasPermissionAsync(userId, "Consultation");
                
                if (!hasPermission)
                {
                    TempData["StatusMessage"] = "Error: You do not have permission to create assessments.";
                    return RedirectToAction("AppointmentDetails", "Nurse", new { id = appointmentId });
                }

                // Get appointment details
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .FirstOrDefaultAsync(a => a.Id == appointmentId);

                if (appointment == null)
                {
                    TempData["StatusMessage"] = "Error: Appointment not found.";
                    return RedirectToAction("Appointments", "Nurse");
                }

                // Check if assessment already exists
                var existingAssessment = await _context.HEEADSSSAssessments
                    .FirstOrDefaultAsync(h => h.AppointmentId == appointmentId.ToString());

                if (existingAssessment != null)
                {
                    TempData["StatusMessage"] = "Error: HEEADSSS Assessment already exists for this appointment.";
                    return RedirectToAction("AppointmentDetails", "Nurse", new { id = appointmentId });
                }

                // Create view model with appointment data
                var model = new HEEADSSSAssessmentViewModel
                {
                    AppointmentId = appointmentId,
                    UserId = appointment.PatientId,
                    HealthFacility = "Barangay Health Center",
                    FamilyNo = "", // Not available in Patient model
                    FullName = appointment.Patient?.FullName ?? "",
                    Age = appointment.Patient?.Age.ToString(),
                    Gender = appointment.Patient?.Gender ?? "",
                    Address = appointment.Patient?.Address ?? "",
                    ContactNumber = appointment.Patient?.ContactNumber ?? ""
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading HEEADSSS assessment form for appointment {AppointmentId}", appointmentId);
                TempData["StatusMessage"] = "Error: Unable to load assessment form.";
                return RedirectToAction("Appointments", "Nurse");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHEEADSSSAssessment(HEEADSSSAssessmentViewModel model)
        {
            try
            {
                // Check permissions
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var hasPermission = await _permissionService.UserHasPermissionAsync(userId, "Assessment") ||
                                  await _permissionService.UserHasPermissionAsync(userId, "Consultation");
                
                if (!hasPermission)
                {
                    TempData["StatusMessage"] = "Error: You do not have permission to create assessments.";
                    return RedirectToAction("AppointmentDetails", "Nurse", new { id = model.AppointmentId });
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Create HEEADSSS Assessment entity
                var assessment = new HEEADSSSAssessment
                {
                    UserId = model.UserId,
                    AppointmentId = model.AppointmentId.ToString(),
                    HealthFacility = model.HealthFacility,
                    FamilyNo = model.FamilyNo,
                    FullName = model.FullName,
                    Age = model.Age,
                    Gender = model.Gender,
                    Address = model.Address,
                    ContactNumber = model.ContactNumber,
                    HomeEnvironment = model.HomeEnvironment,
                    FamilyRelationship = model.FamilyRelationship,
                    HomeFamilyProblems = model.HomeFamilyProblems,
                    HomeParentalListening = model.HomeParentalListening,
                    HomeParentalBlame = model.HomeParentalBlame,
                    HomeFamilyChanges = model.HomeFamilyChanges,
                    SchoolPerformance = model.SchoolPerformance,
                    AttendanceIssues = model.AttendanceIssues,
                    CareerPlans = model.CareerPlans,
                    EducationCurrentlyStudying = model.EducationCurrentlyStudying,
                    EducationWorking = model.EducationWorking,
                    EducationSchoolWorkProblems = model.EducationSchoolWorkProblems,
                    EducationBullying = model.EducationBullying,
                    EducationEmployment = model.EducationEmployment,
                    DietDescription = model.DietDescription,
                    WeightConcerns = model.WeightConcerns,
                    EatingDisorderSymptoms = model.EatingDisorderSymptoms,
                    EatingBodyImageSatisfaction = model.EatingBodyImageSatisfaction,
                    EatingDisorderedEatingBehaviors = model.EatingDisorderedEatingBehaviors,
                    EatingWeightComments = model.EatingWeightComments,
                    Hobbies = model.Hobbies,
                    PhysicalActivity = model.PhysicalActivity,
                    ScreenTime = model.ScreenTime,
                    ActivitiesParticipation = model.ActivitiesParticipation,
                    ActivitiesRegularExercise = model.ActivitiesRegularExercise,
                    ActivitiesScreenTime = model.ActivitiesScreenTime,
                    SubstanceUse = model.SubstanceUse,
                    SubstanceType = model.SubstanceType,
                    DrugsTobaccoUse = model.DrugsTobaccoUse,
                    DrugsAlcoholUse = model.DrugsAlcoholUse,
                    DrugsIllicitDrugUse = model.DrugsIllicitDrugUse,
                    DatingRelationships = model.DatingRelationships,
                    SexualActivity = model.SexualActivity,
                    SexualOrientation = model.SexualOrientation,
                    SexualityBodyConcerns = model.SexualityBodyConcerns,
                    SexualityIntimateRelationships = model.SexualityIntimateRelationships,
                    SexualityPartners = model.SexualityPartners,
                    SexualitySexualOrientation = model.SexualitySexualOrientation,
                    SexualityPregnancy = model.SexualityPregnancy,
                    SexualitySTI = model.SexualitySTI,
                    SexualityProtection = model.SexualityProtection,
                    MoodChanges = model.MoodChanges,
                    SuicidalThoughts = model.SuicidalThoughts,
                    SelfHarmBehavior = model.SelfHarmBehavior,
                    FeelsSafeAtHome = model.FeelsSafeAtHome,
                    FeelsSafeAtSchool = model.FeelsSafeAtSchool,
                    ExperiencedBullying = model.ExperiencedBullying,
                    PersonalStrengths = model.PersonalStrengths,
                    SupportSystems = model.SupportSystems,
                    CopingMechanisms = model.CopingMechanisms,
                    SafetyPhysicalAbuse = model.SafetyPhysicalAbuse,
                    SafetyRelationshipViolence = model.SafetyRelationshipViolence,
                    SafetyProtectiveGear = model.SafetyProtectiveGear,
                    SafetyGunsAtHome = model.SafetyGunsAtHome,
                    SuicideDepressionFeelings = model.SuicideDepressionFeelings,
                    SuicideSelfHarmThoughts = model.SuicideSelfHarmThoughts,
                    SuicideFamilyHistory = model.SuicideFamilyHistory,
                    AssessmentNotes = model.AssessmentNotes,
                    RecommendedActions = model.RecommendedActions,
                    FollowUpPlan = model.FollowUpPlan,
                    Notes = model.Notes,
                    AssessedBy = User.Identity?.Name ?? "Unknown",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.HEEADSSSAssessments.Add(assessment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("HEEADSSS Assessment created successfully for appointment {AppointmentId}", model.AppointmentId);
                TempData["StatusMessage"] = "HEEADSSS Assessment created successfully.";
                return RedirectToAction("AppointmentDetails", "Nurse", new { id = model.AppointmentId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating HEEADSSS assessment for appointment {AppointmentId}", model.AppointmentId);
                TempData["StatusMessage"] = "Error: Unable to create assessment.";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditNCDAssessment(int appointmentId)
        {
            try
            {
                // Nurses have permission to edit assessments by default
                _logger.LogInformation("Nurse editing NCD assessment for appointment {AppointmentId}", appointmentId);

                var assessment = await _context.NCDRiskAssessments
                    .FirstOrDefaultAsync(n => n.AppointmentId == appointmentId);

                if (assessment == null)
                {
                    TempData["StatusMessage"] = "Error: Assessment not found.";
                    return RedirectToAction("AppointmentDetails", "Nurse", new { id = appointmentId });
                }

                // Decrypt sensitive data for display
                assessment.DecryptSensitiveData(_encryptionService, User);

                // Convert to view model
                var model = new NCDRiskAssessmentViewModel
                {
                    AppointmentId = appointmentId,
                    UserId = assessment.UserId,
                    HealthFacility = assessment.HealthFacility,
                    FamilyNo = assessment.FamilyNo,
                    Address = assessment.Address,
                    Barangay = assessment.Barangay,
                    Birthday = assessment.Birthday,
                    Telepono = assessment.Telepono,
                    Edad = assessment.Edad,
                    Kasarian = assessment.Kasarian,
                    Relihiyon = assessment.Relihiyon,
                    HasDiabetes = assessment.HasDiabetes,
                    DiabetesYear = assessment.DiabetesYear,
                    DiabetesMedication = assessment.DiabetesMedication,
                    HasHypertension = assessment.HasHypertension,
                    HypertensionYear = assessment.HypertensionYear,
                    HypertensionMedication = assessment.HypertensionMedication,
                    HasCancer = assessment.HasCancer,
                    CancerType = assessment.CancerType,
                    CancerYear = assessment.CancerYear,
                    CancerMedication = assessment.CancerMedication,
                    HasCOPD = assessment.HasCOPD,
                    HasLungDisease = assessment.HasLungDisease,
                    LungDiseaseYear = assessment.LungDiseaseYear,
                    LungDiseaseMedication = assessment.LungDiseaseMedication,
                    HasEyeDisease = assessment.HasEyeDisease,
                    HighSaltIntake = assessment.HighSaltIntake,
                    AlcoholFrequency = assessment.AlcoholFrequency,
                    ExerciseDuration = assessment.ExerciseDuration,
                    AppointmentType = assessment.AppointmentType,
                    SmokingStatus = assessment.SmokingStatus,
                    AlcoholConsumption = assessment.AlcoholConsumption,
                    FirstName = assessment.FirstName,
                    MiddleName = assessment.MiddleName,
                    LastName = assessment.LastName,
                    Occupation = assessment.Occupation,
                    CivilStatus = assessment.CivilStatus
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading NCD assessment for editing, appointment {AppointmentId}", appointmentId);
                TempData["StatusMessage"] = "Error: Unable to load assessment for editing.";
                return RedirectToAction("AppointmentDetails", "Nurse", new { id = appointmentId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> PrintNCDAssessment(int appointmentId)
        {
            try
            {
                // Nurses have permission to print assessments by default
                _logger.LogInformation("Nurse printing NCD assessment for appointment {AppointmentId}", appointmentId);

                var assessment = await _context.NCDRiskAssessments
                    .FirstOrDefaultAsync(n => n.AppointmentId == appointmentId);

                if (assessment == null)
                {
                    TempData["StatusMessage"] = "Error: Assessment not found.";
                    return RedirectToAction("AppointmentDetails", "Nurse", new { id = appointmentId });
                }

                // Decrypt sensitive data for display
                assessment.DecryptSensitiveData(_encryptionService, User);

                // Convert to view model
                var model = new NCDRiskAssessmentViewModel
                {
                    AppointmentId = appointmentId,
                    UserId = assessment.UserId,
                    HealthFacility = assessment.HealthFacility,
                    FamilyNo = assessment.FamilyNo,
                    Address = assessment.Address,
                    Barangay = assessment.Barangay,
                    Birthday = assessment.Birthday,
                    Telepono = assessment.Telepono,
                    Edad = assessment.Edad,
                    Kasarian = assessment.Kasarian,
                    Relihiyon = assessment.Relihiyon,
                    HasDiabetes = assessment.HasDiabetes,
                    DiabetesYear = assessment.DiabetesYear,
                    DiabetesMedication = assessment.DiabetesMedication,
                    HasHypertension = assessment.HasHypertension,
                    HypertensionYear = assessment.HypertensionYear,
                    HypertensionMedication = assessment.HypertensionMedication,
                    HasCancer = assessment.HasCancer,
                    CancerType = assessment.CancerType,
                    CancerYear = assessment.CancerYear,
                    CancerMedication = assessment.CancerMedication,
                    HasCOPD = assessment.HasCOPD,
                    HasLungDisease = assessment.HasLungDisease,
                    LungDiseaseYear = assessment.LungDiseaseYear,
                    LungDiseaseMedication = assessment.LungDiseaseMedication,
                    HasEyeDisease = assessment.HasEyeDisease,
                    HighSaltIntake = assessment.HighSaltIntake,
                    AlcoholFrequency = assessment.AlcoholFrequency,
                    ExerciseDuration = assessment.ExerciseDuration,
                    AppointmentType = assessment.AppointmentType,
                    SmokingStatus = assessment.SmokingStatus,
                    AlcoholConsumption = assessment.AlcoholConsumption,
                    FirstName = assessment.FirstName,
                    MiddleName = assessment.MiddleName,
                    LastName = assessment.LastName,
                    Occupation = assessment.Occupation,
                    CivilStatus = assessment.CivilStatus,
                    FamilyHasHypertension = assessment.FamilyHasHypertension,
                    FamilyHasHeartDisease = assessment.FamilyHasHeartDisease,
                    FamilyHasStroke = assessment.FamilyHasStroke,
                    FamilyHasDiabetes = assessment.FamilyHasDiabetes,
                    FamilyHasCancer = assessment.FamilyHasCancer,
                    FamilyHasKidneyDisease = assessment.FamilyHasKidneyDisease,
                    FamilyOtherDiseaseDetails = assessment.FamilyOtherDiseaseDetails,
                    RiskStatus = assessment.RiskStatus
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading NCD assessment for printing, appointment {AppointmentId}", appointmentId);
                TempData["StatusMessage"] = "Error: Unable to load assessment for printing.";
                return RedirectToAction("AppointmentDetails", "Nurse", new { id = appointmentId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNCDAssessment(NCDRiskAssessmentViewModel model)
        {
            try
            {
                // Nurses have permission to edit assessments by default
                _logger.LogInformation("Nurse updating NCD assessment for appointment {AppointmentId}", model.AppointmentId);

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var assessment = await _context.NCDRiskAssessments
                    .FirstOrDefaultAsync(n => n.AppointmentId == model.AppointmentId);

                if (assessment == null)
                {
                    TempData["StatusMessage"] = "Error: Assessment not found.";
                    return RedirectToAction("AppointmentDetails", "Nurse", new { id = model.AppointmentId });
                }

                // Update assessment
                assessment.HealthFacility = model.HealthFacility;
                assessment.FamilyNo = model.FamilyNo;
                assessment.Address = model.Address;
                assessment.Barangay = model.Barangay;
                assessment.Birthday = model.Birthday;
                assessment.Telepono = model.Telepono;
                assessment.Edad = model.Edad;
                assessment.Kasarian = model.Kasarian;
                assessment.Relihiyon = model.Relihiyon;
                assessment.HasDiabetes = model.HasDiabetes;
                assessment.DiabetesYear = model.DiabetesYear;
                assessment.DiabetesMedication = model.DiabetesMedication;
                assessment.HasHypertension = model.HasHypertension;
                assessment.HypertensionYear = model.HypertensionYear;
                assessment.HypertensionMedication = model.HypertensionMedication;
                assessment.HasCancer = model.HasCancer;
                assessment.CancerType = model.CancerType;
                assessment.CancerYear = model.CancerYear;
                assessment.CancerMedication = model.CancerMedication;
                assessment.HasCOPD = model.HasCOPD;
                assessment.HasLungDisease = model.HasLungDisease;
                assessment.LungDiseaseYear = model.LungDiseaseYear;
                assessment.LungDiseaseMedication = model.LungDiseaseMedication;
                assessment.HasEyeDisease = model.HasEyeDisease;
                assessment.HighSaltIntake = model.HighSaltIntake;
                assessment.AlcoholFrequency = model.AlcoholFrequency;
                assessment.ExerciseDuration = model.ExerciseDuration;
                assessment.AppointmentType = model.AppointmentType;
                assessment.SmokingStatus = model.SmokingStatus;
                assessment.AlcoholConsumption = model.AlcoholConsumption;
                assessment.FirstName = model.FirstName;
                assessment.MiddleName = model.MiddleName;
                assessment.LastName = model.LastName;
                assessment.Occupation = model.Occupation;
                assessment.CivilStatus = model.CivilStatus;
                assessment.UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                await _context.SaveChangesAsync();

                _logger.LogInformation("NCD Risk Assessment updated successfully for appointment {AppointmentId}", model.AppointmentId);
                TempData["StatusMessage"] = "NCD Risk Assessment updated successfully.";
                return RedirectToAction("AppointmentDetails", "Nurse", new { id = model.AppointmentId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating NCD assessment for appointment {AppointmentId}", model.AppointmentId);
                TempData["StatusMessage"] = "Error: Unable to update assessment.";
                return View(model);
            }
        }
    }
}
