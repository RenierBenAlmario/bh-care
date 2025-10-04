using System;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class NCDRiskAssessmentViewModel
    {
        public int? Id { get; set; }
        public int? AppointmentId { get; set; }
        public string? UserId { get; set; }
        
        // Health Facility Information
        public string? HealthFacility { get; set; } = "Barangay Health Center";
        public DateTime? DateOfAssessment { get; set; } = DateTime.Now;
        
        // Part I: Demographic-Socio-Economic Profile
        public string? FamilyNo { get; set; }
        public string? IDNo { get; set; }
        [Required(ErrorMessage = "First name is required")]
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? Barangay { get; set; }
        public string? Telepono { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Edad { get; set; }
        public string? Kasarian { get; set; }
        public string? Relihiyon { get; set; }
        public string? CivilStatus { get; set; }
        public string? Occupation { get; set; }
        public string? AppointmentType { get; set; } = "General Checkup";

        // Part II: Past Medical History
        public string? HasDiabetes { get; set; } = "false";
        public string? DiabetesYear { get; set; }
        public string? DiabetesMedication { get; set; }

        public string? HasHypertension { get; set; } = "false";
        public string? HypertensionYear { get; set; }
        public string? HypertensionMedication { get; set; }

        public string? HasCancer { get; set; } = "false";
        public string? CancerType { get; set; }
        public string? CancerYear { get; set; }
        public string? CancerMedication { get; set; }

        public string? HasLungDisease { get; set; } = "false";
        public string? LungDiseaseYear { get; set; }
        public string? LungDiseaseMedication { get; set; }

        public string? HasCOPD { get; set; } = "false";
        public string? COPDYear { get; set; }
        public string? COPDMedication { get; set; }

        public string? HasEyeDisease { get; set; } = "false";
        public string? EyeDiseaseYear { get; set; }
        public string? EyeDiseaseMedication { get; set; }

        // Part III: Lifestyle Factors
        // B.1 Diet
        public bool EatsSaltyFood { get; set; }
        public bool EatsSweetFood { get; set; }
        public bool EatsFattyFood { get; set; }

        // B.2 Alcohol
        public string? AlcoholStoppedDuration { get; set; }
        public bool DrinksHardLiquor { get; set; }
        public string? BeerAmount { get; set; }
        public string? WineAmount { get; set; }
        public string? HardLiquorAmount { get; set; }
        public string? AlcoholPerOccasion { get; set; }

        // B.3 Exercise
        public bool HasRegularExercise { get; set; }
        public string? ExerciseType { get; set; }

        // B.4 Smoking
        public bool IsSmoker { get; set; }
        public string? SmokingSticksPerDay { get; set; }
        public string? SmokingDuration { get; set; }
        public string? SmokingQuitDuration { get; set; }
        public bool ExposedToSmoke { get; set; }
        public string? Smoked100Sticks { get; set; } = "false";

        // B.5 Stress
        public bool IsStressed { get; set; }
        public string? StressCauses { get; set; }
        public bool StressAffectsDailyLife { get; set; }

        // Part IV: Risk Screening
        // 4.1 Anthropometric Measurement
        public decimal? SystolicBP { get; set; }
        public decimal? DiastolicBP { get; set; }

        // 4.4 Cholesterol
        public decimal? TotalCholesterol { get; set; }
        public decimal? HDLCholesterol { get; set; }
        public decimal? LDLCholesterol { get; set; }
        public decimal? Triglycerides { get; set; }

        // Risk Status Summary
        public string? RiskStatus { get; set; }
        public string? RiskFactors { get; set; }
        
        // Legacy properties for backward compatibility
        [StringLength(50)]
        public string? SmokingStatus { get; set; } = "Non-smoker";
        
        [StringLength(50)]
        public string? AlcoholConsumption { get; set; } = string.Empty;
        
        public string? HighSaltIntake { get; set; } = "false";
        
        // Family History (legacy properties)
        public string? FamilyHasHypertension { get; set; } = "false";
        public string? FamilyHasHeartDisease { get; set; } = "false";
        public string? FamilyHasStroke { get; set; } = "false";
        public string? FamilyHasDiabetes { get; set; } = "false";
        public string? FamilyHasCancer { get; set; } = "false";
        public string? FamilyHasKidneyDisease { get; set; } = "false";
        public string? FamilyHasOtherDisease { get; set; } = "false";
        public string? FamilyOtherDiseaseDetails { get; set; } = string.Empty;

        // Family History Detailed (by Family Member)
        public string? FamilyHistoryHypertensionFather { get; set; } = "false";
        public string? FamilyHistoryHypertensionMother { get; set; } = "false";
        public string? FamilyHistoryHypertensionSibling { get; set; } = "false";

        public string? FamilyHistoryStrokeFather { get; set; } = "false";
        public string? FamilyHistoryStrokeMother { get; set; } = "false";
        public string? FamilyHistoryStrokeSibling { get; set; } = "false";

        public string? FamilyHistoryDiabetesFather { get; set; } = "false";
        public string? FamilyHistoryDiabetesMother { get; set; } = "false";
        public string? FamilyHistoryDiabetesSibling { get; set; } = "false";

        public string? FamilyHistoryCancerFather { get; set; } = "false";
        public string? FamilyHistoryCancerMother { get; set; } = "false";
        public string? FamilyHistoryCancerSibling { get; set; } = "false";

        public string? FamilyHistoryHeartDiseaseFather { get; set; } = "false";
        public string? FamilyHistoryHeartDiseaseMother { get; set; } = "false";
        public string? FamilyHistoryHeartDiseaseSibling { get; set; } = "false";

        public string? FamilyHistoryLungDiseaseFather { get; set; } = "false";
        public string? FamilyHistoryLungDiseaseMother { get; set; } = "false";
        public string? FamilyHistoryLungDiseaseSibling { get; set; } = "false";

        public string? FamilyHistoryKidneyDiseaseFather { get; set; } = "false";
        public string? FamilyHistoryKidneyDiseaseMother { get; set; } = "false";
        public string? FamilyHistoryKidneyDiseaseSibling { get; set; } = "false";

        public string? FamilyHistoryEyeDiseaseFather { get; set; } = "false";
        public string? FamilyHistoryEyeDiseaseMother { get; set; } = "false";
        public string? FamilyHistoryEyeDiseaseSibling { get; set; } = "false";

        public string? FamilyHistoryOther { get; set; } = "false";
        public string? FamilyHistoryOtherFather { get; set; } = "false";
        public string? FamilyHistoryOtherMother { get; set; } = "false";
        public string? FamilyHistoryOtherSibling { get; set; } = "false";
        
        // Legacy diet properties
        public string? EatsVegetables { get; set; } = "false";
        public string? EatsFruits { get; set; } = "false";
        public string? EatsFish { get; set; } = "false";
        public string? EatsMeat { get; set; } = "false";
        public string? EatsProcessedFood { get; set; } = "false";
        
        // Legacy exercise properties
        public string? HasNoRegularExercise { get; set; } = "false";
        
        // Legacy health conditions
        public string? HasDifficultyBreathing { get; set; } = "false";
        public string? HasAsthma { get; set; } = "false";
        
        // Stroke Symptoms (Q2.8)
        public string? HasStrokeSymptoms { get; set; } = "false";
        
        // Additional health conditions added to existing properties above
        public string? HasEnoughExercise { get; set; } = "false";
        
        // Legacy chest pain properties
        public string? HasChestPain { get; set; } = "false";
        public string? ChestPainLocation { get; set; } = "false";
        public string? ChestPainWhenWalking { get; set; } = "false";
        public string? StopsWhenPain { get; set; } = "false";
        public string? PainRelievedWithRest { get; set; } = "false";
        public string? PainGoneIn10Min { get; set; } = "false";
        public string? PainMoreThan30Min { get; set; } = "false";
        public string? HasNumbness { get; set; } = "false";
        
        // Legacy chest pain string properties
        [StringLength(100)]
        public string? ChestPain { get; set; }
        
        [StringLength(100)]
        public string? ChestPainLocationString { get; set; }
        
        public string? ChestPainValue { get; set; }
        
        // Additional properties for comprehensive form
        public string? IDNumber { get; set; }
        public string? DateOfAssessmentString { get; set; }
        
        // Chest Pain Details (Q2.1-2.7)
        public string? ChestPainSpreadsToArm { get; set; } = "false";
        public string? NumbnessWhenWalkingFast { get; set; } = "false";
        public string? LossOfConsciousnessLessThan10Min { get; set; } = "false";
        public string? PainLastsMoreThan30Min { get; set; } = "false";
        public string? SeeDoctorIfYes { get; set; } = "false";
        public string? DoctorName { get; set; }
        
        // Nutrition Details
        public string? EatsVegetablesDaily { get; set; } = "false";
        public string? EatsFruitsDaily { get; set; } = "false";
        public string? EatsFishDaily { get; set; } = "false";
        public string? EatsMeatDaily { get; set; } = "false";
        public string? HasUnhealthyDiet { get; set; } = "false";
        public string? EatsFattyFoodMoreThan2TimesPerWeek { get; set; } = "false";
        public string? EatsSweetFoodMoreThan2TimesPerWeek { get; set; } = "false";
        public string? EatsOilyFoodMoreThan2TimesPerWeek { get; set; } = "false";
        
        // Alcohol Details
        public string? DrinksAlcohol { get; set; } = "false";
        public string? DrinksBeer { get; set; } = "false";
        public string? DrinksWine { get; set; } = "false";
        public string? DrinksWhiskyGinBrandy { get; set; } = "false";
        public string? AlcoholAmount1Bottle320ml { get; set; } = "false";
        public string? AlcoholAmount2Bottle640ml { get; set; } = "false";
        public string? AlcoholAmountLessThan3Shot45ml { get; set; } = "false";
        public string? AlcoholAmount3to4WineGlasses300ml { get; set; } = "false";
        public string? AlcoholAmountMoreThan4Shots75ml { get; set; } = "false";
        public string? AlcoholFrequency1to3TimesPerWeek { get; set; } = "false";
        public string? AlcoholFrequencyMoreThan4TimesPerWeek { get; set; } = "false";
        public string? IsBingeDrinker { get; set; } = "false";
        
        // Physical Activity Details
        public string? ModerateIntensityExercise { get; set; } = "false";
        public string? VigorousIntensityExercise { get; set; } = "false";
        public string? CombinationExercise { get; set; } = "false";
        public string? InsufficientPhysicalActivity { get; set; } = "false";
        
        // Smoking Details
        public string? FormerSmoker { get; set; } = "false";
        public string? NeverSmokedButExposedToSmoke { get; set; } = "false";
        public string? HasHistoryOfSmoking { get; set; } = "false";
        
        // Stress
        public string? HasStress { get; set; } = "false";
        
        // Anthropometric Measurements
        public string? Weight { get; set; }
        public string? Height { get; set; }
        public string? BMI { get; set; }
        public string? Waist { get; set; }
        public string? Hip { get; set; }
        public string? WHRatio { get; set; }
        public string? BMIStatus { get; set; }
        public string? WHStatus { get; set; }
        
        // Blood Sugar
        public string? FastingBloodSugar { get; set; }
        public string? RandomBloodSugar { get; set; }
        public string? BloodSugarStatus { get; set; }
        public string? HasPolyuria { get; set; } = "false";
        public string? HasPolydipsia { get; set; } = "false";
        public string? HasPolyphagia { get; set; } = "false";
        public string? HasWeightLoss { get; set; } = "false";
        
        // Blood Pressure
        public string? LeftArmMeanBP { get; set; }
        public string? RightArmMeanBP { get; set; }
        public string? BaselineBP { get; set; }
        public string? BPStatus { get; set; }
        
        // Cholesterol
        public string? CholesterolResult { get; set; }
        public string? CholesterolStatus { get; set; }
        
        // Urine Dipstick Test
        public string? UrineProtein { get; set; }
        public string? UrineKetones { get; set; }
        public string? HasUrineProtein { get; set; } = "false";
        public string? HasUrineKetones { get; set; } = "false";
        
        // Risk Profile (For Doctors Only)
        public string? RiskPercentage { get; set; }
        
        // Cancer Screening (Women 30 years old and above)
        public string? BreastCancerScreened { get; set; } = "false";
        public string? CervicalCancerScreened { get; set; } = "false";
        public string? CancerScreeningStatus { get; set; }
        
        // Assessment Information
        public string? InterviewedBy { get; set; }
        public string? Designation { get; set; }
        public string? AssessmentDate { get; set; }
        public string? PatientSignature { get; set; }
        
        // Additional properties for compatibility
        public string? ExerciseDuration { get; set; }
        public string? AlcoholFrequency { get; set; }
        public string? HasHighSaltIntake { get; set; } = "false";
        
        // Missing form fields that need to be added for complete mapping
        
        // Chest Pain Questions (Q2.1-2.8) - Missing mappings
        public string? Pananakit21 { get; set; } = "false"; // Q2.1 Nakakaramdam ka ba ng Pananakit o kabigatan sa dibdib?
        public string? Pananakit22 { get; set; } = "false"; // Q2.2 Ang sakit ba ay nasa gitna ng dibdib, kaliwang bahagi ng dibdib hanggang sa kaliwang braso?
        public string? Pananakit23 { get; set; } = "false"; // Q2.3 Nararamdaman mo ba ito kung nagmamadali o naglalakad ng mabilis o pakiramdam na paakyat?
        public string? Pananakit24 { get; set; } = "false"; // Q2.4 Napapatigil ka ba sa paglalakad kapag sumasakit ang iyong dibdib?
        public string? Pananakit25 { get; set; } = "false"; // Q2.5 Nawawala ba ang sakit kapag hindi ka kumikilos o naglalagay ng gamot sa ilalim ng dila?
        public string? Pananakit26 { get; set; } = "false"; // Q2.6 Nawawala ba ang pananakit sa loob ng 10 minuto?
        public string? Pananakit27 { get; set; } = "false"; // Q2.7 Nakakaramdam ka ba ng sakit sa dibdib na tumatagal higit sa 30 minuto?
        public string? Pananakit28 { get; set; } = "false"; // Q2.8 Nakakaramdam ka ba hirap sa pagsasalita, panghihina ng braso at/o binti o pamamanhid sa kalahating bahagi ng katawan?
        
        // Nutrition - Missing detailed mappings
        public string? NutrisyonMadalasGulay { get; set; } = "false"; // Madalas mo bang kainin ang mga sumusunod? - Gulay
        public string? NutrisyonMadalasPratas { get; set; } = "false"; // Madalas mo bang kainin ang mga sumusunod? - Prutas
        public string? NutrisyonMadalasIsda { get; set; } = "false"; // Madalas mo bang kainin ang mga sumusunod? - Isda
        public string? NutrisyonMadalasKarne { get; set; } = "false"; // Madalas mo bang kainin ang mga sumusunod? - Karne
        public string? NutrisyonKumakainMatatamis { get; set; } = "false"; // Kumakain ka ba ng higit sa 2 beses kada linggo ng - Matatamis na pagkain
        public string? NutrisyonKumakainMamantika { get; set; } = "false"; // Kumakain ka ba ng higit sa 2 beses kada linggo ng - Mamantikang pagkain
        
        // Alcohol - Missing detailed mappings
        public string? AlcoholInom { get; set; } = "false"; // Umiinom ka ba ng alak?
        public string? AlchoholTypeBeer { get; set; } = "false"; // Alcohol type - Beer
        public string? AlchoholTypeWine { get; set; } = "false"; // Alcohol type - Wine
        public string? AlchoholTypeWhisky { get; set; } = "false"; // Alcohol type - Whisky/Gin/Brandy
        public string? BeerConsumption1 { get; set; } = "false"; // Beer consumption - 1 bote (320 ml)
        public string? BeerConsumption2 { get; set; } = "false"; // Beer consumption - 2 bote (640 ml)
        public string? BeerConsumption3 { get; set; } = "false"; // Beer consumption - ≥ 3 bote
        public string? WineConsumption1 { get; set; } = "false"; // Wine consumption - ≤ 3 wine glasses (300 ml)
        public string? WineConsumption2 { get; set; } = "false"; // Wine consumption - > 4 wine glasses
        public string? WhiskyConsumption1 { get; set; } = "false"; // Whisky consumption - ≤ 3 shots/jigger (75 ml)
        public string? WhiskyConsumption2 { get; set; } = "false"; // Whisky consumption - ≥ 4 shots / jigger
        public string? AlcoholOkasyon { get; set; } = "false"; // Sa isang okasyon, ilang bote ng alak ang naiinom mo?
        
        // Exercise - Missing detailed mappings
        public string? EhersisyoRegular { get; set; } = "false"; // May sapat ka bang ehersisyo?
        public string? EhersisyoDuration { get; set; } = "false"; // Gaano katagal sa isang araw?
        public string? EhersisyoType { get; set; } = "false"; // Anong klaseng ehersisyo ang ginagawa mo?
        
        // Smoking - Missing detailed mappings
        public string? SigarilyoKadami { get; set; } = "false"; // Gaano kadami ang nauubos sa isang araw?
        public string? SigarilyoTumigil { get; set; } = "false"; // Tumigil na sa paninigarilyo?
        public string? SigarilyoUsok { get; set; } = "false"; // Hindi, pero lantad ka ba sa usok ng sigarilyo?
        public string? SigarilyoSticks { get; set; } = "false"; // Ikaw ba ay nanatili sa usok ng sigarilyo?
        
        // Stress - Missing detailed mappings
        public string? StressMadalas { get; set; } = "false"; // Madalas ka bang ma-stress?
        public string? StressSino { get; set; } = "false"; // Ano o Sino ang dahilan ng iyong stress?
        public string? StressEpekto { get; set; } = "false"; // Naepektuhan ba nito ang iyong pang-araw-araw na gawain?
        
        // Additional missing fields for complete form mapping
        public string? HealthFacilityName { get; set; } = "Baesa Health Center"; // Health Facility name
        public string? DateAssessment { get; set; } = ""; // Date of Assessment
        
        // Lung Disease - Missing proper mapping
        public string? HasLungDiseaseNonInfectious { get; set; } = "false"; // Sakit sa baga hindu nakakahawa
        
        // Eye Disease - Missing proper mapping  
        public string? HasEyeDiseaseCondition { get; set; } = "false"; // Sakit sa mata
    }
}