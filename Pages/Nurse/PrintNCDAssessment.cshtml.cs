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
    public class PrintNCDAssessmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PrintNCDAssessmentModel> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IDataEncryptionService _encryptionService;

        public PrintNCDAssessmentModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<PrintNCDAssessmentModel> logger,
            IPermissionService permissionService,
            IDataEncryptionService encryptionService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _permissionService = permissionService;
            _encryptionService = encryptionService;
        }

        public NCDRiskAssessmentViewModel NCDRiskAssessment { get; set; }

        public async Task<IActionResult> OnGetAsync(int appointmentId)
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
                    return RedirectToPage("/Nurse/AppointmentDetails", new { id = appointmentId });
                }

                // Decrypt sensitive data for display
                assessment.DecryptSensitiveData(_encryptionService, User);
                
                // Manual decryption fallback for critical NCD fields
                if (!string.IsNullOrEmpty(assessment.HealthFacility) && _encryptionService.IsEncrypted(assessment.HealthFacility))
                {
                    assessment.HealthFacility = _encryptionService.DecryptForUser(assessment.HealthFacility, User);
                }
                if (!string.IsNullOrEmpty(assessment.FamilyNo) && _encryptionService.IsEncrypted(assessment.FamilyNo))
                {
                    assessment.FamilyNo = _encryptionService.DecryptForUser(assessment.FamilyNo, User);
                }
                if (!string.IsNullOrEmpty(assessment.IDNumber) && _encryptionService.IsEncrypted(assessment.IDNumber))
                {
                    assessment.IDNumber = _encryptionService.DecryptForUser(assessment.IDNumber, User);
                }
                if (!string.IsNullOrEmpty(assessment.FirstName) && _encryptionService.IsEncrypted(assessment.FirstName))
                {
                    assessment.FirstName = _encryptionService.DecryptForUser(assessment.FirstName, User);
                }
                if (!string.IsNullOrEmpty(assessment.MiddleName) && _encryptionService.IsEncrypted(assessment.MiddleName))
                {
                    assessment.MiddleName = _encryptionService.DecryptForUser(assessment.MiddleName, User);
                }
                if (!string.IsNullOrEmpty(assessment.LastName) && _encryptionService.IsEncrypted(assessment.LastName))
                {
                    assessment.LastName = _encryptionService.DecryptForUser(assessment.LastName, User);
                }
                if (!string.IsNullOrEmpty(assessment.Birthday) && _encryptionService.IsEncrypted(assessment.Birthday))
                {
                    assessment.Birthday = _encryptionService.DecryptForUser(assessment.Birthday, User);
                }
                if (!string.IsNullOrEmpty(assessment.Edad) && _encryptionService.IsEncrypted(assessment.Edad))
                {
                    assessment.Edad = _encryptionService.DecryptForUser(assessment.Edad, User);
                }
                if (!string.IsNullOrEmpty(assessment.Kasarian) && _encryptionService.IsEncrypted(assessment.Kasarian))
                {
                    assessment.Kasarian = _encryptionService.DecryptForUser(assessment.Kasarian, User);
                }
                if (!string.IsNullOrEmpty(assessment.CivilStatus) && _encryptionService.IsEncrypted(assessment.CivilStatus))
                {
                    assessment.CivilStatus = _encryptionService.DecryptForUser(assessment.CivilStatus, User);
                }
                if (!string.IsNullOrEmpty(assessment.Occupation) && _encryptionService.IsEncrypted(assessment.Occupation))
                {
                    assessment.Occupation = _encryptionService.DecryptForUser(assessment.Occupation, User);
                }
                if (!string.IsNullOrEmpty(assessment.Relihiyon) && _encryptionService.IsEncrypted(assessment.Relihiyon))
                {
                    assessment.Relihiyon = _encryptionService.DecryptForUser(assessment.Relihiyon, User);
                }
                if (!string.IsNullOrEmpty(assessment.Address) && _encryptionService.IsEncrypted(assessment.Address))
                {
                    assessment.Address = _encryptionService.DecryptForUser(assessment.Address, User);
                }
                if (!string.IsNullOrEmpty(assessment.Barangay) && _encryptionService.IsEncrypted(assessment.Barangay))
                {
                    assessment.Barangay = _encryptionService.DecryptForUser(assessment.Barangay, User);
                }
                if (!string.IsNullOrEmpty(assessment.Telepono) && _encryptionService.IsEncrypted(assessment.Telepono))
                {
                    assessment.Telepono = _encryptionService.DecryptForUser(assessment.Telepono, User);
                }
                if (!string.IsNullOrEmpty(assessment.SmokingStatus) && _encryptionService.IsEncrypted(assessment.SmokingStatus))
                {
                    assessment.SmokingStatus = _encryptionService.DecryptForUser(assessment.SmokingStatus, User);
                }
                if (!string.IsNullOrEmpty(assessment.AlcoholFrequency) && _encryptionService.IsEncrypted(assessment.AlcoholFrequency))
                {
                    assessment.AlcoholFrequency = _encryptionService.DecryptForUser(assessment.AlcoholFrequency, User);
                }
                if (!string.IsNullOrEmpty(assessment.HighSaltIntake) && _encryptionService.IsEncrypted(assessment.HighSaltIntake))
                {
                    assessment.HighSaltIntake = _encryptionService.DecryptForUser(assessment.HighSaltIntake, User);
                }
                if (!string.IsNullOrEmpty(assessment.ExerciseDuration) && _encryptionService.IsEncrypted(assessment.ExerciseDuration))
                {
                    assessment.ExerciseDuration = _encryptionService.DecryptForUser(assessment.ExerciseDuration, User);
                }
                if (!string.IsNullOrEmpty(assessment.RiskStatus) && _encryptionService.IsEncrypted(assessment.RiskStatus))
                {
                    assessment.RiskStatus = _encryptionService.DecryptForUser(assessment.RiskStatus, User);
                }
                if (!string.IsNullOrEmpty(assessment.CreatedAt) && _encryptionService.IsEncrypted(assessment.CreatedAt))
                {
                    assessment.CreatedAt = _encryptionService.DecryptForUser(assessment.CreatedAt, User);
                }
                if (!string.IsNullOrEmpty(assessment.UpdatedAt) && _encryptionService.IsEncrypted(assessment.UpdatedAt))
                {
                    assessment.UpdatedAt = _encryptionService.DecryptForUser(assessment.UpdatedAt, User);
                }

                // Convert to view model
                NCDRiskAssessment = new NCDRiskAssessmentViewModel
                {
                    AppointmentId = appointmentId,
                    UserId = assessment.UserId.ToString(),
                    HealthFacility = assessment.HealthFacility,
                    FamilyNo = assessment.FamilyNo,
                    IDNumber = assessment.IDNumber,
                    Address = assessment.Address,
                    Barangay = assessment.Barangay,
                    Birthday = assessment.Birthday,
                    Telepono = assessment.Telepono,
                    Edad = assessment.Edad,
                    Kasarian = assessment.Kasarian,
                    Relihiyon = assessment.Relihiyon,
                    FirstName = assessment.FirstName,
                    MiddleName = assessment.MiddleName,
                    LastName = assessment.LastName,
                    Occupation = assessment.Occupation,
                    CivilStatus = assessment.CivilStatus,
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
                    HasChestPain = assessment.HasChestPain,
                    ChestPainSpreadsToArm = assessment.ChestPainSpreadsToArm,
                    NumbnessWhenWalkingFast = assessment.NumbnessWhenWalkingFast,
                    PainRelievedWithRest = assessment.PainRelievedWithRest,
                    LossOfConsciousnessLessThan10Min = assessment.LossOfConsciousnessLessThan10Min,
                    PainLastsMoreThan30Min = assessment.PainLastsMoreThan30Min,
                    SeeDoctorIfYes = assessment.SeeDoctorIfYes,
                    DoctorName = assessment.DoctorName,
                    FamilyHasHypertension = assessment.FamilyHasHypertension,
                    FamilyHasHeartDisease = assessment.FamilyHasHeartDisease,
                    FamilyHasStroke = assessment.FamilyHasStroke,
                    FamilyHasDiabetes = assessment.FamilyHasDiabetes,
                    FamilyHasCancer = assessment.FamilyHasCancer,
                    FamilyHasKidneyDisease = assessment.FamilyHasKidneyDisease,
                    FamilyHasOtherDisease = assessment.FamilyHasOtherDisease,
                    FamilyOtherDiseaseDetails = assessment.FamilyOtherDiseaseDetails,
                    EatsVegetablesDaily = assessment.EatsVegetablesDaily,
                    EatsFruitsDaily = assessment.EatsFruitsDaily,
                    EatsFishDaily = assessment.EatsFishDaily,
                    EatsMeatDaily = assessment.EatsMeatDaily,
                    HasUnhealthyDiet = assessment.HasUnhealthyDiet,
                    EatsFattyFoodMoreThan2TimesPerWeek = assessment.EatsFattyFoodMoreThan2TimesPerWeek,
                    EatsSweetFoodMoreThan2TimesPerWeek = assessment.EatsSweetFoodMoreThan2TimesPerWeek,
                    EatsOilyFoodMoreThan2TimesPerWeek = assessment.EatsOilyFoodMoreThan2TimesPerWeek,
                    HasHighSaltIntake = assessment.HasHighSaltIntake,
                    DrinksAlcohol = assessment.DrinksAlcohol,
                    DrinksBeer = assessment.DrinksBeer,
                    DrinksWine = assessment.DrinksWine,
                    DrinksWhiskyGinBrandy = assessment.DrinksWhiskyGinBrandy,
                    AlcoholAmount1Bottle320ml = assessment.AlcoholAmount1Bottle320ml,
                    AlcoholAmount2Bottle640ml = assessment.AlcoholAmount2Bottle640ml,
                    AlcoholAmountLessThan3Shot45ml = assessment.AlcoholAmountLessThan3Shot45ml,
                    AlcoholAmount3to4WineGlasses300ml = assessment.AlcoholAmount3to4WineGlasses300ml,
                    AlcoholAmountMoreThan4Shots75ml = assessment.AlcoholAmountMoreThan4Shots75ml,
                    AlcoholFrequency1to3TimesPerWeek = assessment.AlcoholFrequency1to3TimesPerWeek,
                    AlcoholFrequencyMoreThan4TimesPerWeek = assessment.AlcoholFrequencyMoreThan4TimesPerWeek,
                    IsBingeDrinker = assessment.IsBingeDrinker,
                    ModerateIntensityExercise = assessment.ModerateIntensityExercise,
                    VigorousIntensityExercise = assessment.VigorousIntensityExercise,
                    CombinationExercise = assessment.CombinationExercise,
                    InsufficientPhysicalActivity = assessment.InsufficientPhysicalActivity,
                    FormerSmoker = assessment.FormerSmoker,
                    NeverSmokedButExposedToSmoke = assessment.NeverSmokedButExposedToSmoke,
                    HasHistoryOfSmoking = assessment.HasHistoryOfSmoking,
                    HasStress = assessment.HasStress,
                    Weight = assessment.Weight,
                    Height = assessment.Height,
                    BMI = assessment.BMI,
                    Waist = assessment.Waist,
                    Hip = assessment.Hip,
                    WHRatio = assessment.WHRatio,
                    FastingBloodSugar = assessment.FastingBloodSugar,
                    RandomBloodSugar = assessment.RandomBloodSugar,
                    HasPolyuria = assessment.HasPolyuria,
                    HasPolydipsia = assessment.HasPolydipsia,
                    HasPolyphagia = assessment.HasPolyphagia,
                    HasWeightLoss = assessment.HasWeightLoss,
                    LeftArmMeanBP = assessment.LeftArmMeanBP,
                    RightArmMeanBP = assessment.RightArmMeanBP,
                    BaselineBP = assessment.BaselineBP,
                    CholesterolResult = assessment.CholesterolResult,
                    UrineProtein = assessment.UrineProtein,
                    UrineKetones = assessment.UrineKetones,
                    HasUrineProtein = assessment.HasUrineProtein,
                    HasUrineKetones = assessment.HasUrineKetones,
                    BreastCancerScreened = assessment.BreastCancerScreened,
                    CervicalCancerScreened = assessment.CervicalCancerScreened,
                    InterviewedBy = assessment.InterviewedBy,
                    Designation = assessment.Designation,
                    AssessmentDate = assessment.AssessmentDate,
                    PatientSignature = assessment.PatientSignature,
                    HighSaltIntake = assessment.HighSaltIntake,
                    AlcoholFrequency = assessment.AlcoholFrequency,
                    ExerciseDuration = assessment.ExerciseDuration,
                    AppointmentType = assessment.AppointmentType,
                    SmokingStatus = assessment.SmokingStatus,
                    AlcoholConsumption = assessment.AlcoholConsumption,
                    RiskStatus = assessment.RiskStatus
                };

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading NCD assessment for printing, appointment {AppointmentId}", appointmentId);
                TempData["StatusMessage"] = "Error: Unable to load assessment for printing.";
                return RedirectToPage("/Nurse/AppointmentDetails", new { id = appointmentId });
            }
        }
    }
}
