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

        public async Task<IActionResult> OnGetAsync(int appointmentId)
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
                    return RedirectToPage("/Nurse/AppointmentDetails", new { id = appointmentId });
                }

                // Decrypt sensitive data for display
                assessment.DecryptSensitiveData(_encryptionService, User);

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
                    DateOfAssessment = assessment.DateOfAssessment != null ? DateTime.Parse(assessment.DateOfAssessment) : DateTime.Now,
                    
                    // Medical History
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
                    
                    // Chest Pain Details
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
                    FamilyOtherDiseaseDetails = assessment.FamilyOtherDiseaseDetails,
                    
                    // Nutrition Details
                    EatsVegetablesDaily = assessment.EatsVegetablesDaily,
                    EatsFruitsDaily = assessment.EatsFruitsDaily,
                    EatsFishDaily = assessment.EatsFishDaily,
                    EatsMeatDaily = assessment.EatsMeatDaily,
                    HasUnhealthyDiet = assessment.HasUnhealthyDiet,
                    EatsFattyFoodMoreThan2TimesPerWeek = assessment.EatsFattyFoodMoreThan2TimesPerWeek,
                    EatsSweetFoodMoreThan2TimesPerWeek = assessment.EatsSweetFoodMoreThan2TimesPerWeek,
                    EatsOilyFoodMoreThan2TimesPerWeek = assessment.EatsOilyFoodMoreThan2TimesPerWeek,
                    HasHighSaltIntake = assessment.HasHighSaltIntake,
                    
                    // Alcohol Details
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
                    
                    // Physical Activity Details
                    ModerateIntensityExercise = assessment.ModerateIntensityExercise,
                    VigorousIntensityExercise = assessment.VigorousIntensityExercise,
                    CombinationExercise = assessment.CombinationExercise,
                    InsufficientPhysicalActivity = assessment.InsufficientPhysicalActivity,
                    
                    // Smoking Details
                    FormerSmoker = assessment.FormerSmoker,
                    NeverSmokedButExposedToSmoke = assessment.NeverSmokedButExposedToSmoke,
                    HasHistoryOfSmoking = assessment.HasHistoryOfSmoking,
                    
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
                    
                    // Urine Dipstick Test
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
                    PatientSignature = assessment.PatientSignature,
                    
                    // Legacy properties
                    HighSaltIntake = assessment.HighSaltIntake,
                    AlcoholFrequency = assessment.AlcoholFrequency,
                    ExerciseDuration = assessment.ExerciseDuration,
                    AppointmentType = assessment.AppointmentType,
                    SmokingStatus = assessment.SmokingStatus,
                    AlcoholConsumption = assessment.AlcoholConsumption,
                    RiskStatus = assessment.RiskStatus
                };

                AppointmentId = appointmentId;
                UserId = assessment.UserId;
                
                // Get patient name from appointment
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                    .FirstOrDefaultAsync(a => a.Id == appointmentId);
                
                if (appointment?.Patient?.User != null)
                {
                    PatientName = $"{appointment.Patient.User.FirstName} {appointment.Patient.User.LastName}";
                }
                else
                {
                    PatientName = "Unknown Patient";
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading NCD assessment for editing, appointment {AppointmentId}", appointmentId);
                TempData["StatusMessage"] = "Error: Unable to load assessment for editing.";
                return RedirectToPage("/Nurse/AppointmentDetails", new { id = appointmentId });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Nurses have permission to edit assessments by default
                _logger.LogInformation("Nurse updating NCD assessment for appointment {AppointmentId}", NCDRiskAssessment.AppointmentId);

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var assessment = await _context.NCDRiskAssessments
                    .FirstOrDefaultAsync(n => n.AppointmentId == NCDRiskAssessment.AppointmentId);

                if (assessment == null)
                {
                    TempData["StatusMessage"] = "Error: Assessment not found.";
                    return RedirectToPage("/Nurse/AppointmentDetails", new { id = NCDRiskAssessment.AppointmentId });
                }

                // Update assessment
                assessment.HealthFacility = NCDRiskAssessment.HealthFacility;
                assessment.FamilyNo = NCDRiskAssessment.FamilyNo;
                assessment.IDNumber = NCDRiskAssessment.IDNumber;
                assessment.Address = NCDRiskAssessment.Address;
                assessment.Barangay = NCDRiskAssessment.Barangay;
                assessment.Birthday = NCDRiskAssessment.Birthday;
                assessment.Telepono = NCDRiskAssessment.Telepono;
                assessment.Edad = NCDRiskAssessment.Edad;
                assessment.Kasarian = NCDRiskAssessment.Kasarian;
                assessment.Relihiyon = NCDRiskAssessment.Relihiyon;
                assessment.FirstName = NCDRiskAssessment.FirstName;
                assessment.MiddleName = NCDRiskAssessment.MiddleName;
                assessment.LastName = NCDRiskAssessment.LastName;
                assessment.Occupation = NCDRiskAssessment.Occupation;
                assessment.CivilStatus = NCDRiskAssessment.CivilStatus;
                assessment.DateOfAssessment = NCDRiskAssessment.DateOfAssessment?.ToString("yyyy-MM-dd");
                
                // Medical History
                assessment.HasDiabetes = NCDRiskAssessment.HasDiabetes;
                assessment.DiabetesYear = NCDRiskAssessment.DiabetesYear;
                assessment.DiabetesMedication = NCDRiskAssessment.DiabetesMedication;
                assessment.HasHypertension = NCDRiskAssessment.HasHypertension;
                assessment.HypertensionYear = NCDRiskAssessment.HypertensionYear;
                assessment.HypertensionMedication = NCDRiskAssessment.HypertensionMedication;
                assessment.HasCancer = NCDRiskAssessment.HasCancer;
                assessment.CancerType = NCDRiskAssessment.CancerType;
                assessment.CancerYear = NCDRiskAssessment.CancerYear;
                assessment.CancerMedication = NCDRiskAssessment.CancerMedication;
                assessment.HasCOPD = NCDRiskAssessment.HasCOPD;
                assessment.HasLungDisease = NCDRiskAssessment.HasLungDisease;
                assessment.LungDiseaseYear = NCDRiskAssessment.LungDiseaseYear;
                assessment.LungDiseaseMedication = NCDRiskAssessment.LungDiseaseMedication;
                assessment.HasEyeDisease = NCDRiskAssessment.HasEyeDisease;
                
                // Chest Pain Details
                assessment.HasChestPain = NCDRiskAssessment.HasChestPain;
                assessment.ChestPainSpreadsToArm = NCDRiskAssessment.ChestPainSpreadsToArm;
                assessment.NumbnessWhenWalkingFast = NCDRiskAssessment.NumbnessWhenWalkingFast;
                assessment.PainRelievedWithRest = NCDRiskAssessment.PainRelievedWithRest;
                assessment.LossOfConsciousnessLessThan10Min = NCDRiskAssessment.LossOfConsciousnessLessThan10Min;
                assessment.PainLastsMoreThan30Min = NCDRiskAssessment.PainLastsMoreThan30Min;
                assessment.SeeDoctorIfYes = NCDRiskAssessment.SeeDoctorIfYes;
                assessment.DoctorName = NCDRiskAssessment.DoctorName;
                
                // Family History
                assessment.FamilyHasHypertension = NCDRiskAssessment.FamilyHasHypertension;
                assessment.FamilyHasHeartDisease = NCDRiskAssessment.FamilyHasHeartDisease;
                assessment.FamilyHasStroke = NCDRiskAssessment.FamilyHasStroke;
                assessment.FamilyHasDiabetes = NCDRiskAssessment.FamilyHasDiabetes;
                assessment.FamilyHasCancer = NCDRiskAssessment.FamilyHasCancer;
                assessment.FamilyHasKidneyDisease = NCDRiskAssessment.FamilyHasKidneyDisease;
                assessment.FamilyOtherDiseaseDetails = NCDRiskAssessment.FamilyOtherDiseaseDetails;
                
                // Nutrition Details
                assessment.EatsVegetablesDaily = NCDRiskAssessment.EatsVegetablesDaily;
                assessment.EatsFruitsDaily = NCDRiskAssessment.EatsFruitsDaily;
                assessment.EatsFishDaily = NCDRiskAssessment.EatsFishDaily;
                assessment.EatsMeatDaily = NCDRiskAssessment.EatsMeatDaily;
                assessment.HasUnhealthyDiet = NCDRiskAssessment.HasUnhealthyDiet;
                assessment.EatsFattyFoodMoreThan2TimesPerWeek = NCDRiskAssessment.EatsFattyFoodMoreThan2TimesPerWeek;
                assessment.EatsSweetFoodMoreThan2TimesPerWeek = NCDRiskAssessment.EatsSweetFoodMoreThan2TimesPerWeek;
                assessment.EatsOilyFoodMoreThan2TimesPerWeek = NCDRiskAssessment.EatsOilyFoodMoreThan2TimesPerWeek;
                assessment.HasHighSaltIntake = NCDRiskAssessment.HasHighSaltIntake;
                
                // Alcohol Details
                assessment.DrinksAlcohol = NCDRiskAssessment.DrinksAlcohol;
                assessment.DrinksBeer = NCDRiskAssessment.DrinksBeer;
                assessment.DrinksWine = NCDRiskAssessment.DrinksWine;
                assessment.DrinksWhiskyGinBrandy = NCDRiskAssessment.DrinksWhiskyGinBrandy;
                assessment.AlcoholAmount1Bottle320ml = NCDRiskAssessment.AlcoholAmount1Bottle320ml;
                assessment.AlcoholAmount2Bottle640ml = NCDRiskAssessment.AlcoholAmount2Bottle640ml;
                assessment.AlcoholAmountLessThan3Shot45ml = NCDRiskAssessment.AlcoholAmountLessThan3Shot45ml;
                assessment.AlcoholAmount3to4WineGlasses300ml = NCDRiskAssessment.AlcoholAmount3to4WineGlasses300ml;
                assessment.AlcoholAmountMoreThan4Shots75ml = NCDRiskAssessment.AlcoholAmountMoreThan4Shots75ml;
                assessment.AlcoholFrequency1to3TimesPerWeek = NCDRiskAssessment.AlcoholFrequency1to3TimesPerWeek;
                assessment.AlcoholFrequencyMoreThan4TimesPerWeek = NCDRiskAssessment.AlcoholFrequencyMoreThan4TimesPerWeek;
                assessment.IsBingeDrinker = NCDRiskAssessment.IsBingeDrinker;
                
                // Physical Activity Details
                assessment.ModerateIntensityExercise = NCDRiskAssessment.ModerateIntensityExercise;
                assessment.VigorousIntensityExercise = NCDRiskAssessment.VigorousIntensityExercise;
                assessment.CombinationExercise = NCDRiskAssessment.CombinationExercise;
                assessment.InsufficientPhysicalActivity = NCDRiskAssessment.InsufficientPhysicalActivity;
                
                // Smoking Details
                assessment.FormerSmoker = NCDRiskAssessment.FormerSmoker;
                assessment.NeverSmokedButExposedToSmoke = NCDRiskAssessment.NeverSmokedButExposedToSmoke;
                assessment.HasHistoryOfSmoking = NCDRiskAssessment.HasHistoryOfSmoking;
                
                // Stress
                assessment.HasStress = NCDRiskAssessment.HasStress;
                
                // Anthropometric Measurements
                assessment.Weight = NCDRiskAssessment.Weight;
                assessment.Height = NCDRiskAssessment.Height;
                assessment.BMI = NCDRiskAssessment.BMI;
                assessment.Waist = NCDRiskAssessment.Waist;
                assessment.Hip = NCDRiskAssessment.Hip;
                assessment.WHRatio = NCDRiskAssessment.WHRatio;
                assessment.BMIStatus = NCDRiskAssessment.BMIStatus;
                assessment.WHStatus = NCDRiskAssessment.WHStatus;
                
                // Blood Sugar
                assessment.FastingBloodSugar = NCDRiskAssessment.FastingBloodSugar;
                assessment.RandomBloodSugar = NCDRiskAssessment.RandomBloodSugar;
                assessment.BloodSugarStatus = NCDRiskAssessment.BloodSugarStatus;
                assessment.HasPolyuria = NCDRiskAssessment.HasPolyuria;
                assessment.HasPolydipsia = NCDRiskAssessment.HasPolydipsia;
                assessment.HasPolyphagia = NCDRiskAssessment.HasPolyphagia;
                assessment.HasWeightLoss = NCDRiskAssessment.HasWeightLoss;
                
                // Blood Pressure
                assessment.LeftArmMeanBP = NCDRiskAssessment.LeftArmMeanBP;
                assessment.RightArmMeanBP = NCDRiskAssessment.RightArmMeanBP;
                assessment.BaselineBP = NCDRiskAssessment.BaselineBP;
                assessment.BPStatus = NCDRiskAssessment.BPStatus;
                
                // Cholesterol
                assessment.CholesterolResult = NCDRiskAssessment.CholesterolResult;
                assessment.CholesterolStatus = NCDRiskAssessment.CholesterolStatus;
                
                // Urine Dipstick Test
                assessment.UrineProtein = NCDRiskAssessment.UrineProtein;
                assessment.UrineKetones = NCDRiskAssessment.UrineKetones;
                assessment.HasUrineProtein = NCDRiskAssessment.HasUrineProtein;
                assessment.HasUrineKetones = NCDRiskAssessment.HasUrineKetones;
                
                // Risk Profile
                assessment.RiskPercentage = NCDRiskAssessment.RiskPercentage;
                
                // Cancer Screening
                assessment.BreastCancerScreened = NCDRiskAssessment.BreastCancerScreened;
                assessment.CervicalCancerScreened = NCDRiskAssessment.CervicalCancerScreened;
                assessment.CancerScreeningStatus = NCDRiskAssessment.CancerScreeningStatus;
                
                // Assessment Information
                assessment.InterviewedBy = NCDRiskAssessment.InterviewedBy;
                assessment.Designation = NCDRiskAssessment.Designation;
                assessment.AssessmentDate = NCDRiskAssessment.AssessmentDate;
                assessment.PatientSignature = NCDRiskAssessment.PatientSignature;
                
                // Legacy properties
                assessment.HighSaltIntake = NCDRiskAssessment.HighSaltIntake;
                assessment.AlcoholFrequency = NCDRiskAssessment.AlcoholFrequency;
                assessment.ExerciseDuration = NCDRiskAssessment.ExerciseDuration;
                assessment.AppointmentType = NCDRiskAssessment.AppointmentType;
                assessment.SmokingStatus = NCDRiskAssessment.SmokingStatus;
                assessment.AlcoholConsumption = NCDRiskAssessment.AlcoholConsumption;
                assessment.RiskStatus = NCDRiskAssessment.RiskStatus;
                assessment.UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                await _context.SaveChangesAsync();

                _logger.LogInformation("NCD Risk Assessment updated successfully for appointment {AppointmentId}", NCDRiskAssessment.AppointmentId);
                TempData["StatusMessage"] = "NCD Risk Assessment updated successfully.";
                return RedirectToPage("/Nurse/AppointmentDetails", new { id = NCDRiskAssessment.AppointmentId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating NCD assessment for appointment {AppointmentId}", NCDRiskAssessment.AppointmentId);
                TempData["StatusMessage"] = "Error: Unable to update assessment.";
                return Page();
            }
        }
    }
}
