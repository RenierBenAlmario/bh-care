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
        private readonly EncryptedDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PrintNCDAssessmentModel> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IDataEncryptionService _encryptionService;

        public PrintNCDAssessmentModel(
            EncryptedDbContext context,
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
                
                // Manual decryption fallback for all NCD fields
                // Personal Information
                if (!string.IsNullOrEmpty(assessment.FirstName) && _encryptionService.IsEncrypted(assessment.FirstName))
                {
                    assessment.FirstName = _encryptionService.DecryptForUser(assessment.FirstName, User);
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
                // Birthday is now a DateTime, no decryption needed
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
                if (!string.IsNullOrEmpty(assessment.AssessmentDate) && _encryptionService.IsEncrypted(assessment.AssessmentDate))
                {
                    assessment.AssessmentDate = _encryptionService.DecryptForUser(assessment.AssessmentDate, User);
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
                // CreatedAt and UpdatedAt are now DateTime, no decryption needed
                if (!string.IsNullOrEmpty(assessment.AppointmentType) && _encryptionService.IsEncrypted(assessment.AppointmentType))
                {
                    assessment.AppointmentType = _encryptionService.DecryptForUser(assessment.AppointmentType, User);
                }

                // Convert to view model
                NCDRiskAssessment = new NCDRiskAssessmentViewModel
                {
                    AppointmentId = appointmentId,
                    UserId = assessment.UserId.ToString(),
                    HealthFacility = assessment.HealthFacility,
                    FamilyNo = assessment.FamilyNo,
                    Address = assessment.Address,
                    Barangay = assessment.Barangay,
                    Birthday = DateTime.TryParse(assessment.Birthday, out var birthday) ? birthday : null,
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
                    EyeDiseaseYear = assessment.EyeDiseaseYear,
                    EyeDiseaseMedication = assessment.EyeDiseaseMedication,
                    
                    // Individual Family History Fields
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
                    FamilyHistoryOther = assessment.FamilyHistoryOther,
                    FamilyHistoryOtherFather = assessment.FamilyHistoryOtherFather,
                    FamilyHistoryOtherMother = assessment.FamilyHistoryOtherMother,
                    FamilyHistoryOtherSibling = assessment.FamilyHistoryOtherSibling,
                    
                    // Additional Lifestyle Fields
                    AlcoholStoppedDuration = assessment.AlcoholStoppedDuration,
                    Smoked100Sticks = assessment.Smoked100Sticks,
                    
                    // Additional Identity Fields
                    IDNo = assessment.IDNo,
                    
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
