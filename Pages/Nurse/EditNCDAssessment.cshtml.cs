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
    public class EditNCDAssessmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EditNCDAssessmentModel> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IDataEncryptionService _encryptionService;

        public EditNCDAssessmentModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<EditNCDAssessmentModel> logger,
            IPermissionService permissionService,
            IDataEncryptionService encryptionService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _permissionService = permissionService;
            _encryptionService = encryptionService;
        }

        [BindProperty]
        public NCDRiskAssessmentViewModel NCDRiskAssessment { get; set; }

        public int AppointmentId { get; set; }
        public string UserId { get; set; }
        public string PatientName { get; set; }

        public async Task<IActionResult> OnGetAsync(int? appointmentId)
        {
            try
            {
                if (appointmentId == null)
                {
                    _logger.LogWarning("Appointment ID not provided to EditN assessment");
                    TempData["StatusMessage"] = "Error: Appointment ID must be provided.";
                    return RedirectToPage("/Nurse/Appointments");
                }
                
                // Nurses have permission to edit assessments by default
                _logger.LogInformation("Nurse editing NCD assessment for appointment {AppointmentId}", appointmentId);

                var assessment = await _context.NCDRiskAssessments
                    .FirstOrDefaultAsync(n => n.AppointmentId == appointmentId);

                if (assessment == null)
                {
                    TempData["StatusMessage"] = "Error: Assessment not found.";
                    return RedirectToPage("/Nurse/AppointmentDetails", new { id = appointmentId });
                }

                // Decrypt sensitive data for display
                try
                {
                    _logger.LogInformation("Attempting to decrypt assessment data for user {User}", User.Identity?.Name);
                    _logger.LogInformation("User roles: {Roles}", string.Join(", ", User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(c => c.Value)));
                    _logger.LogInformation("Can user decrypt: {CanDecrypt}", _encryptionService.CanUserDecrypt(User));
                    
                    // Log some sample encrypted fields before decryption
                    _logger.LogInformation("Before decryption - FirstName: {FirstName}", assessment.FirstName?.Substring(0, Math.Min(20, assessment.FirstName.Length)) + "...");
                    _logger.LogInformation("Before decryption - HealthFacility: {HealthFacility}", assessment.HealthFacility?.Substring(0, Math.Min(20, assessment.HealthFacility?.Length ?? 0)) + "...");
                    
                    assessment.DecryptSensitiveData(_encryptionService, User);
                    
                    // Log some sample decrypted fields after decryption
                    _logger.LogInformation("After decryption - FirstName: {FirstName}", assessment.FirstName?.Substring(0, Math.Min(20, assessment.FirstName?.Length ?? 0)) + "...");
                    _logger.LogInformation("After decryption - HealthFacility: {HealthFacility}", assessment.HealthFacility?.Substring(0, Math.Min(20, assessment.HealthFacility?.Length ?? 0)) + "...");
                    
                    _logger.LogInformation("Assessment data decryption completed successfully");
                    
                    // Manual decryption fallback for all NCD fields
                    // Personal Information
                    if (!string.IsNullOrEmpty(assessment.FirstName) && _encryptionService.IsEncrypted(assessment.FirstName))
                    {
                        assessment.FirstName = _encryptionService.DecryptForUser(assessment.FirstName, User);
                    }
                    
                    // Decrypt boolean fields for checkbox binding
                    if (!string.IsNullOrEmpty(assessment.HasChestPain) && _encryptionService.IsEncrypted(assessment.HasChestPain))
                    {
                        assessment.HasChestPain = _encryptionService.DecryptForUser(assessment.HasChestPain, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.ChestPainSpreadsToArm) && _encryptionService.IsEncrypted(assessment.ChestPainSpreadsToArm))
                    {
                        assessment.ChestPainSpreadsToArm = _encryptionService.DecryptForUser(assessment.ChestPainSpreadsToArm, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.NumbnessWhenWalkingFast) && _encryptionService.IsEncrypted(assessment.NumbnessWhenWalkingFast))
                    {
                        assessment.NumbnessWhenWalkingFast = _encryptionService.DecryptForUser(assessment.NumbnessWhenWalkingFast, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.PainRelievedWithRest) && _encryptionService.IsEncrypted(assessment.PainRelievedWithRest))
                    {
                        assessment.PainRelievedWithRest = _encryptionService.DecryptForUser(assessment.PainRelievedWithRest, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.LossOfConsciousnessLessThan10Min) && _encryptionService.IsEncrypted(assessment.LossOfConsciousnessLessThan10Min))
                    {
                        assessment.LossOfConsciousnessLessThan10Min = _encryptionService.DecryptForUser(assessment.LossOfConsciousnessLessThan10Min, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.PainLastsMoreThan30Min) && _encryptionService.IsEncrypted(assessment.PainLastsMoreThan30Min))
                    {
                        assessment.PainLastsMoreThan30Min = _encryptionService.DecryptForUser(assessment.PainLastsMoreThan30Min, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.SeeDoctorIfYes) && _encryptionService.IsEncrypted(assessment.SeeDoctorIfYes))
                    {
                        assessment.SeeDoctorIfYes = _encryptionService.DecryptForUser(assessment.SeeDoctorIfYes, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HasPolyuria) && _encryptionService.IsEncrypted(assessment.HasPolyuria))
                    {
                        assessment.HasPolyuria = _encryptionService.DecryptForUser(assessment.HasPolyuria, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HasPolydipsia) && _encryptionService.IsEncrypted(assessment.HasPolydipsia))
                    {
                        assessment.HasPolydipsia = _encryptionService.DecryptForUser(assessment.HasPolydipsia, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HasPolyphagia) && _encryptionService.IsEncrypted(assessment.HasPolyphagia))
                    {
                        assessment.HasPolyphagia = _encryptionService.DecryptForUser(assessment.HasPolyphagia, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HasWeightLoss) && _encryptionService.IsEncrypted(assessment.HasWeightLoss))
                    {
                        assessment.HasWeightLoss = _encryptionService.DecryptForUser(assessment.HasWeightLoss, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HasUrineProtein) && _encryptionService.IsEncrypted(assessment.HasUrineProtein))
                    {
                        assessment.HasUrineProtein = _encryptionService.DecryptForUser(assessment.HasUrineProtein, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HasUrineKetones) && _encryptionService.IsEncrypted(assessment.HasUrineKetones))
                    {
                        assessment.HasUrineKetones = _encryptionService.DecryptForUser(assessment.HasUrineKetones, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.BreastCancerScreened) && _encryptionService.IsEncrypted(assessment.BreastCancerScreened))
                    {
                        assessment.BreastCancerScreened = _encryptionService.DecryptForUser(assessment.BreastCancerScreened, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.CervicalCancerScreened) && _encryptionService.IsEncrypted(assessment.CervicalCancerScreened))
                    {
                        assessment.CervicalCancerScreened = _encryptionService.DecryptForUser(assessment.CervicalCancerScreened, User);
                    }
                    
                    // Decrypt additional boolean fields that are still encrypted
                    if (!string.IsNullOrEmpty(assessment.EatsVegetablesDaily) && _encryptionService.IsEncrypted(assessment.EatsVegetablesDaily))
                    {
                        assessment.EatsVegetablesDaily = _encryptionService.DecryptForUser(assessment.EatsVegetablesDaily, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.EatsFruitsDaily) && _encryptionService.IsEncrypted(assessment.EatsFruitsDaily))
                    {
                        assessment.EatsFruitsDaily = _encryptionService.DecryptForUser(assessment.EatsFruitsDaily, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.EatsFishDaily) && _encryptionService.IsEncrypted(assessment.EatsFishDaily))
                    {
                        assessment.EatsFishDaily = _encryptionService.DecryptForUser(assessment.EatsFishDaily, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.EatsMeatDaily) && _encryptionService.IsEncrypted(assessment.EatsMeatDaily))
                    {
                        assessment.EatsMeatDaily = _encryptionService.DecryptForUser(assessment.EatsMeatDaily, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HasUnhealthyDiet) && _encryptionService.IsEncrypted(assessment.HasUnhealthyDiet))
                    {
                        assessment.HasUnhealthyDiet = _encryptionService.DecryptForUser(assessment.HasUnhealthyDiet, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.EatsFattyFoodMoreThan2TimesPerWeek) && _encryptionService.IsEncrypted(assessment.EatsFattyFoodMoreThan2TimesPerWeek))
                    {
                        assessment.EatsFattyFoodMoreThan2TimesPerWeek = _encryptionService.DecryptForUser(assessment.EatsFattyFoodMoreThan2TimesPerWeek, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.EatsSweetFoodMoreThan2TimesPerWeek) && _encryptionService.IsEncrypted(assessment.EatsSweetFoodMoreThan2TimesPerWeek))
                    {
                        assessment.EatsSweetFoodMoreThan2TimesPerWeek = _encryptionService.DecryptForUser(assessment.EatsSweetFoodMoreThan2TimesPerWeek, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.EatsOilyFoodMoreThan2TimesPerWeek) && _encryptionService.IsEncrypted(assessment.EatsOilyFoodMoreThan2TimesPerWeek))
                    {
                        assessment.EatsOilyFoodMoreThan2TimesPerWeek = _encryptionService.DecryptForUser(assessment.EatsOilyFoodMoreThan2TimesPerWeek, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HasHighSaltIntake) && _encryptionService.IsEncrypted(assessment.HasHighSaltIntake))
                    {
                        assessment.HasHighSaltIntake = _encryptionService.DecryptForUser(assessment.HasHighSaltIntake, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.DrinksAlcohol) && _encryptionService.IsEncrypted(assessment.DrinksAlcohol))
                    {
                        assessment.DrinksAlcohol = _encryptionService.DecryptForUser(assessment.DrinksAlcohol, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.DrinksBeer) && _encryptionService.IsEncrypted(assessment.DrinksBeer))
                    {
                        assessment.DrinksBeer = _encryptionService.DecryptForUser(assessment.DrinksBeer, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.DrinksWine) && _encryptionService.IsEncrypted(assessment.DrinksWine))
                    {
                        assessment.DrinksWine = _encryptionService.DecryptForUser(assessment.DrinksWine, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.DrinksWhiskyGinBrandy) && _encryptionService.IsEncrypted(assessment.DrinksWhiskyGinBrandy))
                    {
                        assessment.DrinksWhiskyGinBrandy = _encryptionService.DecryptForUser(assessment.DrinksWhiskyGinBrandy, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.AlcoholAmount1Bottle320ml) && _encryptionService.IsEncrypted(assessment.AlcoholAmount1Bottle320ml))
                    {
                        assessment.AlcoholAmount1Bottle320ml = _encryptionService.DecryptForUser(assessment.AlcoholAmount1Bottle320ml, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.AlcoholAmount2Bottle640ml) && _encryptionService.IsEncrypted(assessment.AlcoholAmount2Bottle640ml))
                    {
                        assessment.AlcoholAmount2Bottle640ml = _encryptionService.DecryptForUser(assessment.AlcoholAmount2Bottle640ml, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.AlcoholAmountLessThan3Shot45ml) && _encryptionService.IsEncrypted(assessment.AlcoholAmountLessThan3Shot45ml))
                    {
                        assessment.AlcoholAmountLessThan3Shot45ml = _encryptionService.DecryptForUser(assessment.AlcoholAmountLessThan3Shot45ml, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.AlcoholAmount3to4WineGlasses300ml) && _encryptionService.IsEncrypted(assessment.AlcoholAmount3to4WineGlasses300ml))
                    {
                        assessment.AlcoholAmount3to4WineGlasses300ml = _encryptionService.DecryptForUser(assessment.AlcoholAmount3to4WineGlasses300ml, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.AlcoholAmountMoreThan4Shots75ml) && _encryptionService.IsEncrypted(assessment.AlcoholAmountMoreThan4Shots75ml))
                    {
                        assessment.AlcoholAmountMoreThan4Shots75ml = _encryptionService.DecryptForUser(assessment.AlcoholAmountMoreThan4Shots75ml, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.AlcoholFrequency1to3TimesPerWeek) && _encryptionService.IsEncrypted(assessment.AlcoholFrequency1to3TimesPerWeek))
                    {
                        assessment.AlcoholFrequency1to3TimesPerWeek = _encryptionService.DecryptForUser(assessment.AlcoholFrequency1to3TimesPerWeek, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.AlcoholFrequencyMoreThan4TimesPerWeek) && _encryptionService.IsEncrypted(assessment.AlcoholFrequencyMoreThan4TimesPerWeek))
                    {
                        assessment.AlcoholFrequencyMoreThan4TimesPerWeek = _encryptionService.DecryptForUser(assessment.AlcoholFrequencyMoreThan4TimesPerWeek, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.IsBingeDrinker) && _encryptionService.IsEncrypted(assessment.IsBingeDrinker))
                    {
                        assessment.IsBingeDrinker = _encryptionService.DecryptForUser(assessment.IsBingeDrinker, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.ModerateIntensityExercise) && _encryptionService.IsEncrypted(assessment.ModerateIntensityExercise))
                    {
                        assessment.ModerateIntensityExercise = _encryptionService.DecryptForUser(assessment.ModerateIntensityExercise, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.VigorousIntensityExercise) && _encryptionService.IsEncrypted(assessment.VigorousIntensityExercise))
                    {
                        assessment.VigorousIntensityExercise = _encryptionService.DecryptForUser(assessment.VigorousIntensityExercise, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.CombinationExercise) && _encryptionService.IsEncrypted(assessment.CombinationExercise))
                    {
                        assessment.CombinationExercise = _encryptionService.DecryptForUser(assessment.CombinationExercise, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.InsufficientPhysicalActivity) && _encryptionService.IsEncrypted(assessment.InsufficientPhysicalActivity))
                    {
                        assessment.InsufficientPhysicalActivity = _encryptionService.DecryptForUser(assessment.InsufficientPhysicalActivity, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FormerSmoker) && _encryptionService.IsEncrypted(assessment.FormerSmoker))
                    {
                        assessment.FormerSmoker = _encryptionService.DecryptForUser(assessment.FormerSmoker, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.NeverSmokedButExposedToSmoke) && _encryptionService.IsEncrypted(assessment.NeverSmokedButExposedToSmoke))
                    {
                        assessment.NeverSmokedButExposedToSmoke = _encryptionService.DecryptForUser(assessment.NeverSmokedButExposedToSmoke, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HasHistoryOfSmoking) && _encryptionService.IsEncrypted(assessment.HasHistoryOfSmoking))
                    {
                        assessment.HasHistoryOfSmoking = _encryptionService.DecryptForUser(assessment.HasHistoryOfSmoking, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HasStress) && _encryptionService.IsEncrypted(assessment.HasStress))
                    {
                        assessment.HasStress = _encryptionService.DecryptForUser(assessment.HasStress, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.LastName) && _encryptionService.IsEncrypted(assessment.LastName))
                    {
                        assessment.LastName = _encryptionService.DecryptForUser(assessment.LastName, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.MiddleName) && _encryptionService.IsEncrypted(assessment.MiddleName))
                    {
                        assessment.MiddleName = _encryptionService.DecryptForUser(assessment.MiddleName, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HealthFacility) && _encryptionService.IsEncrypted(assessment.HealthFacility))
                    {
                        assessment.HealthFacility = _encryptionService.DecryptForUser(assessment.HealthFacility, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyNo) && _encryptionService.IsEncrypted(assessment.FamilyNo))
                    {
                        assessment.FamilyNo = _encryptionService.DecryptForUser(assessment.FamilyNo, User);
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
                    if (!string.IsNullOrEmpty(assessment.Edad) && _encryptionService.IsEncrypted(assessment.Edad))
                    {
                        assessment.Edad = _encryptionService.DecryptForUser(assessment.Edad, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.Relihiyon) && _encryptionService.IsEncrypted(assessment.Relihiyon))
                    {
                        assessment.Relihiyon = _encryptionService.DecryptForUser(assessment.Relihiyon, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.Occupation) && _encryptionService.IsEncrypted(assessment.Occupation))
                    {
                        assessment.Occupation = _encryptionService.DecryptForUser(assessment.Occupation, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.CivilStatus) && _encryptionService.IsEncrypted(assessment.CivilStatus))
                    {
                        assessment.CivilStatus = _encryptionService.DecryptForUser(assessment.CivilStatus, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.Birthday) && _encryptionService.IsEncrypted(assessment.Birthday))
                    {
                        assessment.Birthday = _encryptionService.DecryptForUser(assessment.Birthday, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.Kasarian) && _encryptionService.IsEncrypted(assessment.Kasarian))
                    {
                        assessment.Kasarian = _encryptionService.DecryptForUser(assessment.Kasarian, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.IDNumber) && _encryptionService.IsEncrypted(assessment.IDNumber))
                    {
                        assessment.IDNumber = _encryptionService.DecryptForUser(assessment.IDNumber, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.IDNo) && _encryptionService.IsEncrypted(assessment.IDNo))
                    {
                        assessment.IDNo = _encryptionService.DecryptForUser(assessment.IDNo, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.DateOfAssessment) && _encryptionService.IsEncrypted(assessment.DateOfAssessment))
                    {
                        assessment.DateOfAssessment = _encryptionService.DecryptForUser(assessment.DateOfAssessment, User);
                    }
                    
                    // Medical History
                    if (!string.IsNullOrEmpty(assessment.HasDiabetes) && _encryptionService.IsEncrypted(assessment.HasDiabetes))
                    {
                        assessment.HasDiabetes = _encryptionService.DecryptForUser(assessment.HasDiabetes, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HasHypertension) && _encryptionService.IsEncrypted(assessment.HasHypertension))
                    {
                        assessment.HasHypertension = _encryptionService.DecryptForUser(assessment.HasHypertension, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HasCancer) && _encryptionService.IsEncrypted(assessment.HasCancer))
                    {
                        assessment.HasCancer = _encryptionService.DecryptForUser(assessment.HasCancer, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HasCOPD) && _encryptionService.IsEncrypted(assessment.HasCOPD))
                    {
                        assessment.HasCOPD = _encryptionService.DecryptForUser(assessment.HasCOPD, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HasLungDisease) && _encryptionService.IsEncrypted(assessment.HasLungDisease))
                    {
                        assessment.HasLungDisease = _encryptionService.DecryptForUser(assessment.HasLungDisease, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HasEyeDisease) && _encryptionService.IsEncrypted(assessment.HasEyeDisease))
                    {
                        assessment.HasEyeDisease = _encryptionService.DecryptForUser(assessment.HasEyeDisease, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.CancerType) && _encryptionService.IsEncrypted(assessment.CancerType))
                    {
                        assessment.CancerType = _encryptionService.DecryptForUser(assessment.CancerType, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.CancerYear) && _encryptionService.IsEncrypted(assessment.CancerYear))
                    {
                        assessment.CancerYear = _encryptionService.DecryptForUser(assessment.CancerYear, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.CancerMedication) && _encryptionService.IsEncrypted(assessment.CancerMedication))
                    {
                        assessment.CancerMedication = _encryptionService.DecryptForUser(assessment.CancerMedication, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.DiabetesYear) && _encryptionService.IsEncrypted(assessment.DiabetesYear))
                    {
                        assessment.DiabetesYear = _encryptionService.DecryptForUser(assessment.DiabetesYear, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.DiabetesMedication) && _encryptionService.IsEncrypted(assessment.DiabetesMedication))
                    {
                        assessment.DiabetesMedication = _encryptionService.DecryptForUser(assessment.DiabetesMedication, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HypertensionYear) && _encryptionService.IsEncrypted(assessment.HypertensionYear))
                    {
                        assessment.HypertensionYear = _encryptionService.DecryptForUser(assessment.HypertensionYear, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HypertensionMedication) && _encryptionService.IsEncrypted(assessment.HypertensionMedication))
                    {
                        assessment.HypertensionMedication = _encryptionService.DecryptForUser(assessment.HypertensionMedication, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.LungDiseaseYear) && _encryptionService.IsEncrypted(assessment.LungDiseaseYear))
                    {
                        assessment.LungDiseaseYear = _encryptionService.DecryptForUser(assessment.LungDiseaseYear, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.LungDiseaseMedication) && _encryptionService.IsEncrypted(assessment.LungDiseaseMedication))
                    {
                        assessment.LungDiseaseMedication = _encryptionService.DecryptForUser(assessment.LungDiseaseMedication, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.EyeDiseaseYear) && _encryptionService.IsEncrypted(assessment.EyeDiseaseYear))
                    {
                        assessment.EyeDiseaseYear = _encryptionService.DecryptForUser(assessment.EyeDiseaseYear, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.EyeDiseaseMedication) && _encryptionService.IsEncrypted(assessment.EyeDiseaseMedication))
                    {
                        assessment.EyeDiseaseMedication = _encryptionService.DecryptForUser(assessment.EyeDiseaseMedication, User);
                    }
                    
                    // Family History
                    if (!string.IsNullOrEmpty(assessment.FamilyHasHypertension) && _encryptionService.IsEncrypted(assessment.FamilyHasHypertension))
                    {
                        assessment.FamilyHasHypertension = _encryptionService.DecryptForUser(assessment.FamilyHasHypertension, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHasHeartDisease) && _encryptionService.IsEncrypted(assessment.FamilyHasHeartDisease))
                    {
                        assessment.FamilyHasHeartDisease = _encryptionService.DecryptForUser(assessment.FamilyHasHeartDisease, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHasStroke) && _encryptionService.IsEncrypted(assessment.FamilyHasStroke))
                    {
                        assessment.FamilyHasStroke = _encryptionService.DecryptForUser(assessment.FamilyHasStroke, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHasDiabetes) && _encryptionService.IsEncrypted(assessment.FamilyHasDiabetes))
                    {
                        assessment.FamilyHasDiabetes = _encryptionService.DecryptForUser(assessment.FamilyHasDiabetes, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHasCancer) && _encryptionService.IsEncrypted(assessment.FamilyHasCancer))
                    {
                        assessment.FamilyHasCancer = _encryptionService.DecryptForUser(assessment.FamilyHasCancer, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHasKidneyDisease) && _encryptionService.IsEncrypted(assessment.FamilyHasKidneyDisease))
                    {
                        assessment.FamilyHasKidneyDisease = _encryptionService.DecryptForUser(assessment.FamilyHasKidneyDisease, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHasOtherDisease) && _encryptionService.IsEncrypted(assessment.FamilyHasOtherDisease))
                    {
                        assessment.FamilyHasOtherDisease = _encryptionService.DecryptForUser(assessment.FamilyHasOtherDisease, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyOtherDiseaseDetails) && _encryptionService.IsEncrypted(assessment.FamilyOtherDiseaseDetails))
                    {
                        assessment.FamilyOtherDiseaseDetails = _encryptionService.DecryptForUser(assessment.FamilyOtherDiseaseDetails, User);
                    }
                    
                    // Detailed Family History
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryCancerFather) && _encryptionService.IsEncrypted(assessment.FamilyHistoryCancerFather))
                    {
                        assessment.FamilyHistoryCancerFather = _encryptionService.DecryptForUser(assessment.FamilyHistoryCancerFather, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryCancerMother) && _encryptionService.IsEncrypted(assessment.FamilyHistoryCancerMother))
                    {
                        assessment.FamilyHistoryCancerMother = _encryptionService.DecryptForUser(assessment.FamilyHistoryCancerMother, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryCancerSibling) && _encryptionService.IsEncrypted(assessment.FamilyHistoryCancerSibling))
                    {
                        assessment.FamilyHistoryCancerSibling = _encryptionService.DecryptForUser(assessment.FamilyHistoryCancerSibling, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryDiabetesFather) && _encryptionService.IsEncrypted(assessment.FamilyHistoryDiabetesFather))
                    {
                        assessment.FamilyHistoryDiabetesFather = _encryptionService.DecryptForUser(assessment.FamilyHistoryDiabetesFather, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryDiabetesMother) && _encryptionService.IsEncrypted(assessment.FamilyHistoryDiabetesMother))
                    {
                        assessment.FamilyHistoryDiabetesMother = _encryptionService.DecryptForUser(assessment.FamilyHistoryDiabetesMother, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryDiabetesSibling) && _encryptionService.IsEncrypted(assessment.FamilyHistoryDiabetesSibling))
                    {
                        assessment.FamilyHistoryDiabetesSibling = _encryptionService.DecryptForUser(assessment.FamilyHistoryDiabetesSibling, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryHeartDiseaseFather) && _encryptionService.IsEncrypted(assessment.FamilyHistoryHeartDiseaseFather))
                    {
                        assessment.FamilyHistoryHeartDiseaseFather = _encryptionService.DecryptForUser(assessment.FamilyHistoryHeartDiseaseFather, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryHeartDiseaseMother) && _encryptionService.IsEncrypted(assessment.FamilyHistoryHeartDiseaseMother))
                    {
                        assessment.FamilyHistoryHeartDiseaseMother = _encryptionService.DecryptForUser(assessment.FamilyHistoryHeartDiseaseMother, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryHeartDiseaseSibling) && _encryptionService.IsEncrypted(assessment.FamilyHistoryHeartDiseaseSibling))
                    {
                        assessment.FamilyHistoryHeartDiseaseSibling = _encryptionService.DecryptForUser(assessment.FamilyHistoryHeartDiseaseSibling, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryLungDiseaseFather) && _encryptionService.IsEncrypted(assessment.FamilyHistoryLungDiseaseFather))
                    {
                        assessment.FamilyHistoryLungDiseaseFather = _encryptionService.DecryptForUser(assessment.FamilyHistoryLungDiseaseFather, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryLungDiseaseMother) && _encryptionService.IsEncrypted(assessment.FamilyHistoryLungDiseaseMother))
                    {
                        assessment.FamilyHistoryLungDiseaseMother = _encryptionService.DecryptForUser(assessment.FamilyHistoryLungDiseaseMother, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryLungDiseaseSibling) && _encryptionService.IsEncrypted(assessment.FamilyHistoryLungDiseaseSibling))
                    {
                        assessment.FamilyHistoryLungDiseaseSibling = _encryptionService.DecryptForUser(assessment.FamilyHistoryLungDiseaseSibling, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryStrokeFather) && _encryptionService.IsEncrypted(assessment.FamilyHistoryStrokeFather))
                    {
                        assessment.FamilyHistoryStrokeFather = _encryptionService.DecryptForUser(assessment.FamilyHistoryStrokeFather, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryStrokeMother) && _encryptionService.IsEncrypted(assessment.FamilyHistoryStrokeMother))
                    {
                        assessment.FamilyHistoryStrokeMother = _encryptionService.DecryptForUser(assessment.FamilyHistoryStrokeMother, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryStrokeSibling) && _encryptionService.IsEncrypted(assessment.FamilyHistoryStrokeSibling))
                    {
                        assessment.FamilyHistoryStrokeSibling = _encryptionService.DecryptForUser(assessment.FamilyHistoryStrokeSibling, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryKidneyDiseaseFather) && _encryptionService.IsEncrypted(assessment.FamilyHistoryKidneyDiseaseFather))
                    {
                        assessment.FamilyHistoryKidneyDiseaseFather = _encryptionService.DecryptForUser(assessment.FamilyHistoryKidneyDiseaseFather, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryKidneyDiseaseMother) && _encryptionService.IsEncrypted(assessment.FamilyHistoryKidneyDiseaseMother))
                    {
                        assessment.FamilyHistoryKidneyDiseaseMother = _encryptionService.DecryptForUser(assessment.FamilyHistoryKidneyDiseaseMother, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryKidneyDiseaseSibling) && _encryptionService.IsEncrypted(assessment.FamilyHistoryKidneyDiseaseSibling))
                    {
                        assessment.FamilyHistoryKidneyDiseaseSibling = _encryptionService.DecryptForUser(assessment.FamilyHistoryKidneyDiseaseSibling, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryEyeDiseaseFather) && _encryptionService.IsEncrypted(assessment.FamilyHistoryEyeDiseaseFather))
                    {
                        assessment.FamilyHistoryEyeDiseaseFather = _encryptionService.DecryptForUser(assessment.FamilyHistoryEyeDiseaseFather, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryEyeDiseaseMother) && _encryptionService.IsEncrypted(assessment.FamilyHistoryEyeDiseaseMother))
                    {
                        assessment.FamilyHistoryEyeDiseaseMother = _encryptionService.DecryptForUser(assessment.FamilyHistoryEyeDiseaseMother, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryEyeDiseaseSibling) && _encryptionService.IsEncrypted(assessment.FamilyHistoryEyeDiseaseSibling))
                    {
                        assessment.FamilyHistoryEyeDiseaseSibling = _encryptionService.DecryptForUser(assessment.FamilyHistoryEyeDiseaseSibling, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryOther) && _encryptionService.IsEncrypted(assessment.FamilyHistoryOther))
                    {
                        assessment.FamilyHistoryOther = _encryptionService.DecryptForUser(assessment.FamilyHistoryOther, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryOtherFather) && _encryptionService.IsEncrypted(assessment.FamilyHistoryOtherFather))
                    {
                        assessment.FamilyHistoryOtherFather = _encryptionService.DecryptForUser(assessment.FamilyHistoryOtherFather, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryOtherMother) && _encryptionService.IsEncrypted(assessment.FamilyHistoryOtherMother))
                    {
                        assessment.FamilyHistoryOtherMother = _encryptionService.DecryptForUser(assessment.FamilyHistoryOtherMother, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.FamilyHistoryOtherSibling) && _encryptionService.IsEncrypted(assessment.FamilyHistoryOtherSibling))
                    {
                        assessment.FamilyHistoryOtherSibling = _encryptionService.DecryptForUser(assessment.FamilyHistoryOtherSibling, User);
                    }
                    
                    // Lifestyle Factors
                    if (!string.IsNullOrEmpty(assessment.SmokingStatus) && _encryptionService.IsEncrypted(assessment.SmokingStatus))
                    {
                        assessment.SmokingStatus = _encryptionService.DecryptForUser(assessment.SmokingStatus, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HighSaltIntake) && _encryptionService.IsEncrypted(assessment.HighSaltIntake))
                    {
                        assessment.HighSaltIntake = _encryptionService.DecryptForUser(assessment.HighSaltIntake, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.AlcoholFrequency) && _encryptionService.IsEncrypted(assessment.AlcoholFrequency))
                    {
                        assessment.AlcoholFrequency = _encryptionService.DecryptForUser(assessment.AlcoholFrequency, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.AlcoholConsumption) && _encryptionService.IsEncrypted(assessment.AlcoholConsumption))
                    {
                        assessment.AlcoholConsumption = _encryptionService.DecryptForUser(assessment.AlcoholConsumption, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.ExerciseDuration) && _encryptionService.IsEncrypted(assessment.ExerciseDuration))
                    {
                        assessment.ExerciseDuration = _encryptionService.DecryptForUser(assessment.ExerciseDuration, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.RiskStatus) && _encryptionService.IsEncrypted(assessment.RiskStatus))
                    {
                        assessment.RiskStatus = _encryptionService.DecryptForUser(assessment.RiskStatus, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.AlcoholStoppedDuration) && _encryptionService.IsEncrypted(assessment.AlcoholStoppedDuration))
                    {
                        assessment.AlcoholStoppedDuration = _encryptionService.DecryptForUser(assessment.AlcoholStoppedDuration, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.Smoked100Sticks) && _encryptionService.IsEncrypted(assessment.Smoked100Sticks))
                    {
                        assessment.Smoked100Sticks = _encryptionService.DecryptForUser(assessment.Smoked100Sticks, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HasEnoughExercise) && _encryptionService.IsEncrypted(assessment.HasEnoughExercise))
                    {
                        assessment.HasEnoughExercise = _encryptionService.DecryptForUser(assessment.HasEnoughExercise, User);
                    }
                    
                    // Chest Pain and Symptoms
                    if (!string.IsNullOrEmpty(assessment.ChestPain) && _encryptionService.IsEncrypted(assessment.ChestPain))
                    {
                        assessment.ChestPain = _encryptionService.DecryptForUser(assessment.ChestPain, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.ChestPainLocation) && _encryptionService.IsEncrypted(assessment.ChestPainLocation))
                    {
                        assessment.ChestPainLocation = _encryptionService.DecryptForUser(assessment.ChestPainLocation, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.ChestPainValue) && _encryptionService.IsEncrypted(assessment.ChestPainValue))
                    {
                        assessment.ChestPainValue = _encryptionService.DecryptForUser(assessment.ChestPainValue, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HasDifficultyBreathing) && _encryptionService.IsEncrypted(assessment.HasDifficultyBreathing))
                    {
                        assessment.HasDifficultyBreathing = _encryptionService.DecryptForUser(assessment.HasDifficultyBreathing, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HasAsthma) && _encryptionService.IsEncrypted(assessment.HasAsthma))
                    {
                        assessment.HasAsthma = _encryptionService.DecryptForUser(assessment.HasAsthma, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.HasNoRegularExercise) && _encryptionService.IsEncrypted(assessment.HasNoRegularExercise))
                    {
                        assessment.HasNoRegularExercise = _encryptionService.DecryptForUser(assessment.HasNoRegularExercise, User);
                    }
                    
                    // System Fields
                    if (!string.IsNullOrEmpty(assessment.CreatedAt) && _encryptionService.IsEncrypted(assessment.CreatedAt))
                    {
                        assessment.CreatedAt = _encryptionService.DecryptForUser(assessment.CreatedAt, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.UpdatedAt) && _encryptionService.IsEncrypted(assessment.UpdatedAt))
                    {
                        assessment.UpdatedAt = _encryptionService.DecryptForUser(assessment.UpdatedAt, User);
                    }
                    if (!string.IsNullOrEmpty(assessment.AppointmentType) && _encryptionService.IsEncrypted(assessment.AppointmentType))
                    {
                        assessment.AppointmentType = _encryptionService.DecryptForUser(assessment.AppointmentType, User);
                    }
                }
                catch (Exception decryptEx)
                {
                    _logger.LogError(decryptEx, "Failed to decrypt assessment data for appointment {AppointmentId}", appointmentId);
                    // Continue with encrypted data rather than failing completely
                    _logger.LogWarning("Continuing with encrypted data due to decryption failure");
                }

                // Convert to view model
                _logger.LogInformation("Creating view model for assessment data");
                
                // Test basic properties first
                _logger.LogInformation("Testing first name: {FirstName}", assessment.FirstName);
                
                try
                {
                    // Create complete view model with all fields
                    NCDRiskAssessment = new NCDRiskAssessmentViewModel
                    {
                        AppointmentId = appointmentId.Value,
                        UserId = assessment.UserId?.ToString() ?? "",
                        HealthFacility = assessment.HealthFacility,
                        FamilyNo = assessment.FamilyNo,
                        Address = assessment.Address,
                        FirstName = assessment.FirstName,
                        LastName = assessment.LastName,
                        MiddleName = assessment.MiddleName,
                        DateOfAssessment = !string.IsNullOrEmpty(assessment.DateOfAssessment) ? DateTime.TryParse(assessment.DateOfAssessment, out var date) ? date : DateTime.Now : DateTime.Now,
                        IDNumber = assessment.IDNumber,
                        Barangay = assessment.Barangay,
                        Telepono = assessment.Telepono,
                        Birthday = assessment.Birthday,
                        Edad = assessment.Edad,
                        Kasarian = assessment.Kasarian,
                        Relihiyon = assessment.Relihiyon,
                        Occupation = assessment.Occupation,
                        CivilStatus = assessment.CivilStatus,
                        
                        // Medical History
                        HasDiabetes = assessment.HasDiabetes,
                        HasHypertension = assessment.HasHypertension,
                        HasCancer = assessment.HasCancer,
                        HasCOPD = assessment.HasCOPD,
                        HasLungDisease = assessment.HasLungDisease,
                        HasEyeDisease = assessment.HasEyeDisease,
                        CancerType = assessment.CancerType,
                        CancerYear = assessment.CancerYear,
                        CancerMedication = assessment.CancerMedication,
                        DiabetesYear = assessment.DiabetesYear,
                        DiabetesMedication = assessment.DiabetesMedication,
                        HypertensionYear = assessment.HypertensionYear,
                        HypertensionMedication = assessment.HypertensionMedication,
                        LungDiseaseYear = assessment.LungDiseaseYear,
                        LungDiseaseMedication = assessment.LungDiseaseMedication,
                        EyeDiseaseYear = assessment.EyeDiseaseYear,
                        EyeDiseaseMedication = assessment.EyeDiseaseMedication,
                        HasAsthma = assessment.HasAsthma,
                        HasDifficultyBreathing = assessment.HasDifficultyBreathing,
                        
                        // Chest Pain and Symptoms
                        HasChestPain = assessment.HasChestPain,
                        ChestPainSpreadsToArm = assessment.ChestPainSpreadsToArm,
                        NumbnessWhenWalkingFast = assessment.NumbnessWhenWalkingFast,
                        PainRelievedWithRest = assessment.PainRelievedWithRest,
                        LossOfConsciousnessLessThan10Min = assessment.LossOfConsciousnessLessThan10Min,
                        PainLastsMoreThan30Min = assessment.PainLastsMoreThan30Min,
                        SeeDoctorIfYes = assessment.SeeDoctorIfYes,
                        DoctorName = assessment.DoctorName,
                        
                        // Family History
                        FamilyHasHypertension = assessment.FamilyHasHypertension,
                        FamilyHasHeartDisease = assessment.FamilyHasHeartDisease,
                        FamilyHasStroke = assessment.FamilyHasStroke,
                        FamilyHasDiabetes = assessment.FamilyHasDiabetes,
                        FamilyHasCancer = assessment.FamilyHasCancer,
                        FamilyHasKidneyDisease = assessment.FamilyHasKidneyDisease,
                        FamilyHasOtherDisease = assessment.FamilyHasOtherDisease,
                        FamilyOtherDiseaseDetails = assessment.FamilyOtherDiseaseDetails,
                        
                        // Detailed Family History
                        FamilyHistoryCancerFather = assessment.FamilyHistoryCancerFather,
                        FamilyHistoryCancerMother = assessment.FamilyHistoryCancerMother,
                        FamilyHistoryCancerSibling = assessment.FamilyHistoryCancerSibling,
                        FamilyHistoryDiabetesFather = assessment.FamilyHistoryDiabetesFather,
                        FamilyHistoryDiabetesMother = assessment.FamilyHistoryDiabetesMother,
                        FamilyHistoryDiabetesSibling = assessment.FamilyHistoryDiabetesSibling,
                        FamilyHistoryHeartDiseaseFather = assessment.FamilyHistoryHeartDiseaseFather,
                        FamilyHistoryHeartDiseaseMother = assessment.FamilyHistoryHeartDiseaseMother,
                        FamilyHistoryHeartDiseaseSibling = assessment.FamilyHistoryHeartDiseaseSibling,
                        FamilyHistoryLungDiseaseFather = assessment.FamilyHistoryLungDiseaseFather,
                        FamilyHistoryLungDiseaseMother = assessment.FamilyHistoryLungDiseaseMother,
                        FamilyHistoryLungDiseaseSibling = assessment.FamilyHistoryLungDiseaseSibling,
                        FamilyHistoryStrokeFather = assessment.FamilyHistoryStrokeFather,
                        FamilyHistoryStrokeMother = assessment.FamilyHistoryStrokeMother,
                        FamilyHistoryStrokeSibling = assessment.FamilyHistoryStrokeSibling,
                        FamilyHistoryKidneyDiseaseFather = assessment.FamilyHistoryKidneyDiseaseFather,
                        FamilyHistoryKidneyDiseaseMother = assessment.FamilyHistoryKidneyDiseaseMother,
                        FamilyHistoryKidneyDiseaseSibling = assessment.FamilyHistoryKidneyDiseaseSibling,
                        FamilyHistoryEyeDiseaseFather = assessment.FamilyHistoryEyeDiseaseFather,
                        FamilyHistoryEyeDiseaseMother = assessment.FamilyHistoryEyeDiseaseMother,
                        FamilyHistoryEyeDiseaseSibling = assessment.FamilyHistoryEyeDiseaseSibling,
                        FamilyHistoryOther = assessment.FamilyHistoryOther,
                        FamilyHistoryOtherFather = assessment.FamilyHistoryOtherFather,
                        FamilyHistoryOtherMother = assessment.FamilyHistoryOtherMother,
                        FamilyHistoryOtherSibling = assessment.FamilyHistoryOtherSibling,
                        
                        // Lifestyle Factors
                        EatsVegetablesDaily = assessment.EatsVegetablesDaily,
                        EatsFruitsDaily = assessment.EatsFruitsDaily,
                        EatsFishDaily = assessment.EatsFishDaily,
                        EatsMeatDaily = assessment.EatsMeatDaily,
                        HasUnhealthyDiet = assessment.HasUnhealthyDiet,
                        EatsFattyFoodMoreThan2TimesPerWeek = assessment.EatsFattyFoodMoreThan2TimesPerWeek,
                        EatsSweetFoodMoreThan2TimesPerWeek = assessment.EatsSweetFoodMoreThan2TimesPerWeek,
                        EatsOilyFoodMoreThan2TimesPerWeek = assessment.EatsOilyFoodMoreThan2TimesPerWeek,
                        HasHighSaltIntake = assessment.HasHighSaltIntake,
                        
                        // Alcohol
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
                        AlcoholStoppedDuration = assessment.AlcoholStoppedDuration,
                        
                        // Exercise
                        ModerateIntensityExercise = assessment.ModerateIntensityExercise,
                        VigorousIntensityExercise = assessment.VigorousIntensityExercise,
                        CombinationExercise = assessment.CombinationExercise,
                        InsufficientPhysicalActivity = assessment.InsufficientPhysicalActivity,
                        HasEnoughExercise = assessment.HasEnoughExercise,
                        HasNoRegularExercise = assessment.HasNoRegularExercise,
                        
                        // Smoking
                        HasHistoryOfSmoking = assessment.HasHistoryOfSmoking,
                        FormerSmoker = assessment.FormerSmoker,
                        NeverSmokedButExposedToSmoke = assessment.NeverSmokedButExposedToSmoke,
                        Smoked100Sticks = assessment.Smoked100Sticks,
                        
                        // Stress
                        HasStress = assessment.HasStress,
                        
                        // Anthropometric Measurements
                        Weight = assessment.Weight,
                        Height = assessment.Height,
                        BMI = assessment.BMI,
                        Waist = assessment.Waist,
                        Hip = assessment.Hip,
                        WHRatio = assessment.WHRatio,
                        BMIStatus = assessment.BMIStatus,
                        WHStatus = assessment.WHStatus,
                        
                        // Blood Sugar
                        FastingBloodSugar = assessment.FastingBloodSugar,
                        RandomBloodSugar = assessment.RandomBloodSugar,
                        BloodSugarStatus = assessment.BloodSugarStatus,
                        HasPolyuria = assessment.HasPolyuria,
                        HasPolydipsia = assessment.HasPolydipsia,
                        HasPolyphagia = assessment.HasPolyphagia,
                        HasWeightLoss = assessment.HasWeightLoss,
                        
                        // Blood Pressure
                        LeftArmMeanBP = assessment.LeftArmMeanBP,
                        RightArmMeanBP = assessment.RightArmMeanBP,
                        BaselineBP = assessment.BaselineBP,
                        BPStatus = assessment.BPStatus,
                        
                        // Cholesterol
                        CholesterolResult = assessment.CholesterolResult,
                        CholesterolStatus = assessment.CholesterolStatus,
                        
                        // Urine
                        UrineProtein = assessment.UrineProtein,
                        UrineKetones = assessment.UrineKetones,
                        HasUrineProtein = assessment.HasUrineProtein,
                        HasUrineKetones = assessment.HasUrineKetones,
                        
                        // Risk Profile
                        RiskPercentage = assessment.RiskPercentage,
                        
                        // Cancer Screening
                        BreastCancerScreened = assessment.BreastCancerScreened,
                        CervicalCancerScreened = assessment.CervicalCancerScreened,
                        CancerScreeningStatus = assessment.CancerScreeningStatus,
                        
                        // Assessment Information
                        InterviewedBy = assessment.InterviewedBy,
                        Designation = assessment.Designation,
                        AssessmentDate = assessment.AssessmentDate,
                        PatientSignature = assessment.PatientSignature
                    };

                    _logger.LogInformation("Basic view model created successfully");

                    AppointmentId = appointmentId.Value;
                    UserId = assessment.UserId;
                    
                    // Get patient name from appointment
                    var appointment = await _context.Appointments
                        .Include(a => a.Patient)
                        .ThenInclude(p => p.User)
                        .FirstOrDefaultAsync(a => a.Id == appointmentId.Value);
                    
                    if (appointment?.Patient?.User != null)
                    {
                        PatientName = $"{appointment.Patient.User.FirstName} {appointment.Patient.User.LastName}";
                    }
                    else
                    {
                        PatientName = "Unknown Patient";
                    }
                }
                catch (Exception viewModelEx)
                {
                    _logger.LogError(viewModelEx, "Exception creating view model for appointment {AppointmentId}", appointmentId);
                    throw; // Re-throw to be caught by outer catch
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading NCD assessment for editing, appointment {AppointmentId}", appointmentId);
                TempData["StatusMessage"] = "Error: Unable to load assessment for editing.";
                return RedirectToPage("/Nurse/AppointmentDuration", new { id = appointmentId });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model state is invalid for NCD assessment update");
                    return Page();
                }

                if (NCDRiskAssessment?.AppointmentId == null)
                {
                    _logger.LogWarning("Appointment ID is missing from form data");
                    TempData["StatusMessage"] = "Error: Appointment ID is required.";
                    return RedirectToPage("/Nurse/Appointments");
                }

                _logger.LogInformation("Processing NCD assessment update for appointment {AppointmentId}", NCDRiskAssessment.AppointmentId);

                // Find the existing assessment
                var existingAssessment = await _context.NCDRiskAssessments
                    .FirstOrDefaultAsync(a => a.AppointmentId == NCDRiskAssessment.AppointmentId);

                if (existingAssessment == null)
                {
                    _logger.LogWarning("No existing NCD assessment found for appointment {AppointmentId}", NCDRiskAssessment.AppointmentId);
                    TempData["StatusMessage"] = "Error: Assessment not found.";
                    return RedirectToPage("/Nurse/AppointmentDetails", new { id = NCDRiskAssessment.AppointmentId });
                }

                // Update the assessment with form data
                // Demographics
                existingAssessment.HealthFacility = NCDRiskAssessment.HealthFacility;
                existingAssessment.FamilyNo = NCDRiskAssessment.FamilyNo;
                existingAssessment.Address = NCDRiskAssessment.Address;
                existingAssessment.FirstName = NCDRiskAssessment.FirstName;
                existingAssessment.LastName = NCDRiskAssessment.LastName;
                existingAssessment.MiddleName = NCDRiskAssessment.MiddleName;
                existingAssessment.DateOfAssessment = NCDRiskAssessment.DateOfAssessment?.ToString("yyyy-MM-dd HH:mm:ss");
                existingAssessment.IDNumber = NCDRiskAssessment.IDNumber;
                existingAssessment.Barangay = NCDRiskAssessment.Barangay;
                existingAssessment.Telepono = NCDRiskAssessment.Telepono;
                existingAssessment.Birthday = NCDRiskAssessment.Birthday;
                existingAssessment.Edad = NCDRiskAssessment.Edad;
                existingAssessment.Kasarian = NCDRiskAssessment.Kasarian;
                existingAssessment.Relihiyon = NCDRiskAssessment.Relihiyon;
                existingAssessment.Occupation = NCDRiskAssessment.Occupation;
                existingAssessment.CivilStatus = NCDRiskAssessment.CivilStatus;
                
                // Medical History
                existingAssessment.HasDiabetes = NCDRiskAssessment.HasDiabetes;
                existingAssessment.HasHypertension = NCDRiskAssessment.HasHypertension;
                existingAssessment.HasCancer = NCDRiskAssessment.HasCancer;
                existingAssessment.HasCOPD = NCDRiskAssessment.HasCOPD;
                existingAssessment.HasLungDisease = NCDRiskAssessment.HasLungDisease;
                existingAssessment.HasEyeDisease = NCDRiskAssessment.HasEyeDisease;
                existingAssessment.CancerType = NCDRiskAssessment.CancerType;
                existingAssessment.CancerYear = NCDRiskAssessment.CancerYear;
                existingAssessment.CancerMedication = NCDRiskAssessment.CancerMedication;
                existingAssessment.DiabetesYear = NCDRiskAssessment.DiabetesYear;
                existingAssessment.DiabetesMedication = NCDRiskAssessment.DiabetesMedication;
                existingAssessment.HypertensionYear = NCDRiskAssessment.HypertensionYear;
                existingAssessment.HypertensionMedication = NCDRiskAssessment.HypertensionMedication;
                existingAssessment.LungDiseaseYear = NCDRiskAssessment.LungDiseaseYear;
                existingAssessment.LungDiseaseMedication = NCDRiskAssessment.LungDiseaseMedication;
                existingAssessment.EyeDiseaseYear = NCDRiskAssessment.EyeDiseaseYear;
                existingAssessment.EyeDiseaseMedication = NCDRiskAssessment.EyeDiseaseMedication;
                existingAssessment.HasAsthma = NCDRiskAssessment.HasAsthma;
                existingAssessment.HasDifficultyBreathing = NCDRiskAssessment.HasDifficultyBreathing;
                
                // Chest Pain and Symptoms
                existingAssessment.HasChestPain = NCDRiskAssessment.HasChestPain;
                existingAssessment.ChestPainSpreadsToArm = NCDRiskAssessment.ChestPainSpreadsToArm;
                existingAssessment.NumbnessWhenWalkingFast = NCDRiskAssessment.NumbnessWhenWalkingFast;
                existingAssessment.PainRelievedWithRest = NCDRiskAssessment.PainRelievedWithRest;
                existingAssessment.LossOfConsciousnessLessThan10Min = NCDRiskAssessment.LossOfConsciousnessLessThan10Min;
                existingAssessment.PainLastsMoreThan30Min = NCDRiskAssessment.PainLastsMoreThan30Min;
                existingAssessment.SeeDoctorIfYes = NCDRiskAssessment.SeeDoctorIfYes;
                existingAssessment.DoctorName = NCDRiskAssessment.DoctorName;
                
                // Family History
                existingAssessment.FamilyHasHypertension = NCDRiskAssessment.FamilyHasHypertension;
                existingAssessment.FamilyHasHeartDisease = NCDRiskAssessment.FamilyHasHeartDisease;
                existingAssessment.FamilyHasStroke = NCDRiskAssessment.FamilyHasStroke;
                existingAssessment.FamilyHasDiabetes = NCDRiskAssessment.FamilyHasDiabetes;
                existingAssessment.FamilyHasCancer = NCDRiskAssessment.FamilyHasCancer;
                existingAssessment.FamilyHasKidneyDisease = NCDRiskAssessment.FamilyHasKidneyDisease;
                existingAssessment.FamilyHasOtherDisease = NCDRiskAssessment.FamilyHasOtherDisease;
                existingAssessment.FamilyOtherDiseaseDetails = NCDRiskAssessment.FamilyOtherDiseaseDetails;
                
                // Detailed Family History
                existingAssessment.FamilyHistoryCancerFather = NCDRiskAssessment.FamilyHistoryCancerFather;
                existingAssessment.FamilyHistoryCancerMother = NCDRiskAssessment.FamilyHistoryCancerMother;
                existingAssessment.FamilyHistoryCancerSibling = NCDRiskAssessment.FamilyHistoryCancerSibling;
                existingAssessment.FamilyHistoryDiabetesFather = NCDRiskAssessment.FamilyHistoryDiabetesFather;
                existingAssessment.FamilyHistoryDiabetesMother = NCDRiskAssessment.FamilyHistoryDiabetesMother;
                existingAssessment.FamilyHistoryDiabetesSibling = NCDRiskAssessment.FamilyHistoryDiabetesSibling;
                existingAssessment.FamilyHistoryHeartDiseaseFather = NCDRiskAssessment.FamilyHistoryHeartDiseaseFather;
                existingAssessment.FamilyHistoryHeartDiseaseMother = NCDRiskAssessment.FamilyHistoryHeartDiseaseMother;
                existingAssessment.FamilyHistoryHeartDiseaseSibling = NCDRiskAssessment.FamilyHistoryHeartDiseaseSibling;
                existingAssessment.FamilyHistoryLungDiseaseFather = NCDRiskAssessment.FamilyHistoryLungDiseaseFather;
                existingAssessment.FamilyHistoryLungDiseaseMother = NCDRiskAssessment.FamilyHistoryLungDiseaseMother;
                existingAssessment.FamilyHistoryLungDiseaseSibling = NCDRiskAssessment.FamilyHistoryLungDiseaseSibling;
                existingAssessment.FamilyHistoryStrokeFather = NCDRiskAssessment.FamilyHistoryStrokeFather;
                existingAssessment.FamilyHistoryStrokeMother = NCDRiskAssessment.FamilyHistoryStrokeMother;
                existingAssessment.FamilyHistoryStrokeSibling = NCDRiskAssessment.FamilyHistoryStrokeSibling;
                existingAssessment.FamilyHistoryKidneyDiseaseFather = NCDRiskAssessment.FamilyHistoryKidneyDiseaseFather;
                existingAssessment.FamilyHistoryKidneyDiseaseMother = NCDRiskAssessment.FamilyHistoryKidneyDiseaseMother;
                existingAssessment.FamilyHistoryKidneyDiseaseSibling = NCDRiskAssessment.FamilyHistoryKidneyDiseaseSibling;
                existingAssessment.FamilyHistoryEyeDiseaseFather = NCDRiskAssessment.FamilyHistoryEyeDiseaseFather;
                existingAssessment.FamilyHistoryEyeDiseaseMother = NCDRiskAssessment.FamilyHistoryEyeDiseaseMother;
                existingAssessment.FamilyHistoryEyeDiseaseSibling = NCDRiskAssessment.FamilyHistoryEyeDiseaseSibling;
                existingAssessment.FamilyHistoryOther = NCDRiskAssessment.FamilyHistoryOther;
                existingAssessment.FamilyHistoryOtherFather = NCDRiskAssessment.FamilyHistoryOtherFather;
                existingAssessment.FamilyHistoryOtherMother = NCDRiskAssessment.FamilyHistoryOtherMother;
                existingAssessment.FamilyHistoryOtherSibling = NCDRiskAssessment.FamilyHistoryOtherSibling;
                
                // Lifestyle Factors
                existingAssessment.EatsVegetablesDaily = NCDRiskAssessment.EatsVegetablesDaily;
                existingAssessment.EatsFruitsDaily = NCDRiskAssessment.EatsFruitsDaily;
                existingAssessment.EatsFishDaily = NCDRiskAssessment.EatsFishDaily;
                existingAssessment.EatsMeatDaily = NCDRiskAssessment.EatsMeatDaily;
                existingAssessment.HasUnhealthyDiet = NCDRiskAssessment.HasUnhealthyDiet;
                existingAssessment.EatsFattyFoodMoreThan2TimesPerWeek = NCDRiskAssessment.EatsFattyFoodMoreThan2TimesPerWeek;
                existingAssessment.EatsSweetFoodMoreThan2TimesPerWeek = NCDRiskAssessment.EatsSweetFoodMoreThan2TimesPerWeek;
                existingAssessment.EatsOilyFoodMoreThan2TimesPerWeek = NCDRiskAssessment.EatsOilyFoodMoreThan2TimesPerWeek;
                existingAssessment.HasHighSaltIntake = NCDRiskAssessment.HasHighSaltIntake;
                
                // Alcohol
                existingAssessment.DrinksAlcohol = NCDRiskAssessment.DrinksAlcohol;
                existingAssessment.DrinksBeer = NCDRiskAssessment.DrinksBeer;
                existingAssessment.DrinksWine = NCDRiskAssessment.DrinksWine;
                existingAssessment.DrinksWhiskyGinBrandy = NCDRiskAssessment.DrinksWhiskyGinBrandy;
                existingAssessment.AlcoholAmount1Bottle320ml = NCDRiskAssessment.AlcoholAmount1Bottle320ml;
                existingAssessment.AlcoholAmount2Bottle640ml = NCDRiskAssessment.AlcoholAmount2Bottle640ml;
                existingAssessment.AlcoholAmountLessThan3Shot45ml = NCDRiskAssessment.AlcoholAmountLessThan3Shot45ml;
                existingAssessment.AlcoholAmount3to4WineGlasses300ml = NCDRiskAssessment.AlcoholAmount3to4WineGlasses300ml;
                existingAssessment.AlcoholAmountMoreThan4Shots75ml = NCDRiskAssessment.AlcoholAmountMoreThan4Shots75ml;
                existingAssessment.AlcoholFrequency1to3TimesPerWeek = NCDRiskAssessment.AlcoholFrequency1to3TimesPerWeek;
                existingAssessment.AlcoholFrequencyMoreThan4TimesPerWeek = NCDRiskAssessment.AlcoholFrequencyMoreThan4TimesPerWeek;
                existingAssessment.IsBingeDrinker = NCDRiskAssessment.IsBingeDrinker;
                existingAssessment.AlcoholStoppedDuration = NCDRiskAssessment.AlcoholStoppedDuration;
                
                // Exercise
                existingAssessment.ModerateIntensityExercise = NCDRiskAssessment.ModerateIntensityExercise;
                existingAssessment.VigorousIntensityExercise = NCDRiskAssessment.VigorousIntensityExercise;
                existingAssessment.CombinationExercise = NCDRiskAssessment.CombinationExercise;
                existingAssessment.InsufficientPhysicalActivity = NCDRiskAssessment.InsufficientPhysicalActivity;
                existingAssessment.HasEnoughExercise = NCDRiskAssessment.HasEnoughExercise;
                existingAssessment.HasNoRegularExercise = NCDRiskAssessment.HasNoRegularExercise;
                
                // Smoking
                existingAssessment.HasHistoryOfSmoking = NCDRiskAssessment.HasHistoryOfSmoking;
                existingAssessment.FormerSmoker = NCDRiskAssessment.FormerSmoker;
                existingAssessment.NeverSmokedButExposedToSmoke = NCDRiskAssessment.NeverSmokedButExposedToSmoke;
                existingAssessment.Smoked100Sticks = NCDRiskAssessment.Smoked100Sticks;
                
                // Stress
                existingAssessment.HasStress = NCDRiskAssessment.HasStress;
                
                // Anthropometric Measurements
                existingAssessment.Weight = NCDRiskAssessment.Weight;
                existingAssessment.Height = NCDRiskAssessment.Height;
                existingAssessment.BMI = NCDRiskAssessment.BMI;
                existingAssessment.Waist = NCDRiskAssessment.Waist;
                existingAssessment.Hip = NCDRiskAssessment.Hip;
                existingAssessment.WHRatio = NCDRiskAssessment.WHRatio;
                existingAssessment.BMIStatus = NCDRiskAssessment.BMIStatus;
                existingAssessment.WHStatus = NCDRiskAssessment.WHStatus;
                
                // Blood Sugar
                existingAssessment.FastingBloodSugar = NCDRiskAssessment.FastingBloodSugar;
                existingAssessment.RandomBloodSugar = NCDRiskAssessment.RandomBloodSugar;
                existingAssessment.BloodSugarStatus = NCDRiskAssessment.BloodSugarStatus;
                existingAssessment.HasPolyuria = NCDRiskAssessment.HasPolyuria;
                existingAssessment.HasPolydipsia = NCDRiskAssessment.HasPolydipsia;
                existingAssessment.HasPolyphagia = NCDRiskAssessment.HasPolyphagia;
                existingAssessment.HasWeightLoss = NCDRiskAssessment.HasWeightLoss;
                
                // Blood Pressure
                existingAssessment.LeftArmMeanBP = NCDRiskAssessment.LeftArmMeanBP;
                existingAssessment.RightArmMeanBP = NCDRiskAssessment.RightArmMeanBP;
                existingAssessment.BaselineBP = NCDRiskAssessment.BaselineBP;
                existingAssessment.BPStatus = NCDRiskAssessment.BPStatus;
                
                // Cholesterol
                existingAssessment.CholesterolResult = NCDRiskAssessment.CholesterolResult;
                existingAssessment.CholesterolStatus = NCDRiskAssessment.CholesterolStatus;
                
                // Urine
                existingAssessment.UrineProtein = NCDRiskAssessment.UrineProtein;
                existingAssessment.UrineKetones = NCDRiskAssessment.UrineKetones;
                existingAssessment.HasUrineProtein = NCDRiskAssessment.HasUrineProtein;
                existingAssessment.HasUrineKetones = NCDRiskAssessment.HasUrineKetones;
                
                // Risk Profile
                existingAssessment.RiskPercentage = NCDRiskAssessment.RiskPercentage;
                
                // Cancer Screening
                existingAssessment.BreastCancerScreened = NCDRiskAssessment.BreastCancerScreened;
                existingAssessment.CervicalCancerScreened = NCDRiskAssessment.CervicalCancerScreened;
                existingAssessment.CancerScreeningStatus = NCDRiskAssessment.CancerScreeningStatus;
                
                // Assessment Information
                existingAssessment.InterviewedBy = NCDRiskAssessment.InterviewedBy;
                existingAssessment.Designation = NCDRiskAssessment.Designation;
                existingAssessment.AssessmentDate = NCDRiskAssessment.AssessmentDate;
                existingAssessment.PatientSignature = NCDRiskAssessment.PatientSignature;

                // Encrypt sensitive data before saving
                try
                {
                    existingAssessment.EncryptSensitiveData(_encryptionService);
                    _logger.LogInformation("Assessment data encrypted successfully");
                }
                catch (Exception encryptEx)
                {
                    _logger.LogError(encryptEx, "Failed to encrypt assessment data");
                    TempData["StatusMessage"] = "Error: Failed to save assessment data securely.";
                    return Page();
                }

                // Save changes
                await _context.SaveChangesAsync();
                _logger.LogInformation("NCD assessment updated successfully for appointment {AppointmentId}", NCDRiskAssessment.AppointmentId);

                TempData["StatusMessage"] = "NCD assessment updated successfully.";
                return RedirectToPage("/Nurse/AppointmentDetails", new { id = NCDRiskAssessment.AppointmentId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating NCD assessment for appointment {AppointmentId}", NCDRiskAssessment?.AppointmentId);
                TempData["StatusMessage"] = "Error: Failed to update assessment.";
            return Page();
            }
        }
    }
}