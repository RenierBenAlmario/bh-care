using System;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class NCDRiskAssessmentViewModel
    {
        public int? AppointmentId { get; set; }
        public string? UserId { get; set; }
        
        // Health Facility Information
        public string? HealthFacility { get; set; } = "Barangay Health Center";
        public DateTime? DateOfAssessment { get; set; } = DateTime.Now;
        
        // Part I: Demographic-Socio-Economic Profile
        public string? FamilyNo { get; set; }
        public string? IDNo { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? Barangay { get; set; }
        public string? Telepono { get; set; }
        public string? Birthday { get; set; }
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
        public bool HasHighSaltIntake { get; set; }

        // B.2 Alcohol
        public bool DrinksAlcohol { get; set; }
        public string? AlcoholStoppedDuration { get; set; }
        public bool DrinksBeer { get; set; }
        public bool DrinksWine { get; set; }
        public bool DrinksHardLiquor { get; set; }
        public string? BeerAmount { get; set; }
        public string? WineAmount { get; set; }
        public string? HardLiquorAmount { get; set; }
        public string? AlcoholFrequency { get; set; }
        public string? AlcoholPerOccasion { get; set; }
        public bool IsBingeDrinker { get; set; }

        // B.3 Exercise
        public bool HasRegularExercise { get; set; }
        public string? ExerciseType { get; set; }
        public string? ExerciseDuration { get; set; }

        // B.4 Smoking
        public bool IsSmoker { get; set; }
        public string? SmokingSticksPerDay { get; set; }
        public string? SmokingDuration { get; set; }
        public string? SmokingQuitDuration { get; set; }
        public bool ExposedToSmoke { get; set; }
        public bool Smoked100Sticks { get; set; }

        // B.5 Stress
        public bool IsStressed { get; set; }
        public string? StressCauses { get; set; }
        public bool StressAffectsDailyLife { get; set; }

        // Part IV: Risk Screening
        // 4.1 Anthropometric Measurement
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public decimal? BMI { get; set; }
        public string? BMIStatus { get; set; }

        // 4.2 Blood Pressure
        public int? SystolicBP { get; set; }
        public int? DiastolicBP { get; set; }
        public string? BPStatus { get; set; }

        // 4.3 Blood Sugar
        public decimal? FastingBloodSugar { get; set; }
        public decimal? RandomBloodSugar { get; set; }
        public string? BloodSugarStatus { get; set; }

        // 4.4 Cholesterol
        public decimal? TotalCholesterol { get; set; }
        public decimal? HDLCholesterol { get; set; }
        public decimal? LDLCholesterol { get; set; }
        public decimal? Triglycerides { get; set; }
        public string? CholesterolStatus { get; set; }

        // 4.5 Urine Dipstick Test
        public string? UrineProtein { get; set; }
        public string? UrineKetones { get; set; }

        // 4.6 Risk Profile For Doctors Only
        public string? RiskPercentage { get; set; }

        // 4.7 Cancer Screening (Women 30 years old and above)
        public bool BreastCancerScreened { get; set; }
        public bool CervicalCancerScreened { get; set; }
        public string? CancerScreeningStatus { get; set; }

        // Assessment Information
        public string? InterviewedBy { get; set; }
        public string? Designation { get; set; }
        public DateTime? AssessmentDate { get; set; }
        public string? PatientSignature { get; set; }

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
    }
}
