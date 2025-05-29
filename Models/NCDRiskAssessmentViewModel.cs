using System;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class NCDRiskAssessmentViewModel
    {
        public string AppointmentId { get; set; }
        public string UserId { get; set; }
        public string HealthFacility { get; set; }
        public string FamilyNo { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        
        [Required(ErrorMessage = "Barangay is required")]
        public string Barangay { get; set; }
        
        [Required(ErrorMessage = "Birthday is required")]
        public DateTime Birthday { get; set; }
        
        [Required(ErrorMessage = "Phone number is required")]
        public string Telepono { get; set; }
        
        public int Edad { get; set; }
        
        [Required(ErrorMessage = "Gender is required")]
        public string Kasarian { get; set; }
        
        // Medical History
        public bool HasDiabetes { get; set; }
        public bool HasHypertension { get; set; }
        public bool HasCancer { get; set; }
        public bool HasCOPD { get; set; }
        public bool HasLungDisease { get; set; }
        public bool HasEyeDisease { get; set; }
        
        // Risk Factors
        public bool HighSaltIntake { get; set; }
        public string AlcoholFrequency { get; set; }
        public string ExerciseDuration { get; set; }

        [Display(Name = "Appointment Type")]
        public string AppointmentType { get; set; } = "General Checkup";
        
        [Display(Name = "Religion")]
        public string Relihiyon { get; set; }
        
        // Lifestyle
        [Display(Name = "Smoking Status")]
        public string SmokingStatus { get; set; } = "Non-smoker";
        
        [Display(Name = "Alcohol Consumption")]
        public string AlcoholConsumption { get; set; } = string.Empty;
        
        // Symptoms
        [Display(Name = "Has Chest Pain")]
        public bool HasChestPain { get; set; }
        
        [Display(Name = "Chest Pain Location")]
        public bool ChestPainLocation { get; set; }
        
        [Display(Name = "Chest Pain When Walking")]
        public bool ChestPainWhenWalking { get; set; }
        
        [Display(Name = "Stops When Pain")]
        public bool StopsWhenPain { get; set; }
        
        [Display(Name = "Pain Relieved With Rest")]
        public bool PainRelievedWithRest { get; set; }
        
        [Display(Name = "Pain Gone In 10 Minutes")]
        public bool PainGoneIn10Min { get; set; }
        
        [Display(Name = "Pain More Than 30 Minutes")]
        public bool PainMoreThan30Min { get; set; }
        
        [Display(Name = "Has Numbness")]
        public bool HasNumbness { get; set; }
        
        // Family History
        [Display(Name = "Family Has Hypertension")]
        public bool FamilyHasHypertension { get; set; }
        
        [Display(Name = "Family Has Heart Disease")]
        public bool FamilyHasHeartDisease { get; set; }
        
        [Display(Name = "Family Has Stroke")]
        public bool FamilyHasStroke { get; set; }
        
        [Display(Name = "Family Has Diabetes")]
        public bool FamilyHasDiabetes { get; set; }
        
        [Display(Name = "Family Has Cancer")]
        public bool FamilyHasCancer { get; set; }
        
        [Display(Name = "Family Has Kidney Disease")]
        public bool FamilyHasKidneyDisease { get; set; }
        
        [Display(Name = "Family Has Other Disease")]
        public bool FamilyHasOtherDisease { get; set; }
        
        [Display(Name = "Family Other Disease Details")]
        public string FamilyOtherDiseaseDetails { get; set; } = string.Empty;
        
        // Diet
        [Display(Name = "Eats Vegetables")]
        public bool EatsVegetables { get; set; }
        
        [Display(Name = "Eats Fruits")]
        public bool EatsFruits { get; set; }
        
        [Display(Name = "Eats Fish")]
        public bool EatsFish { get; set; }
        
        [Display(Name = "Eats Meat")]
        public bool EatsMeat { get; set; }
        
        [Display(Name = "Eats Processed Food")]
        public bool EatsProcessedFood { get; set; }
        
        [Display(Name = "Eats Salty Food")]
        public bool EatsSaltyFood { get; set; }
        
        [Display(Name = "Eats Sweet Food")]
        public bool EatsSweetFood { get; set; }
        
        [Display(Name = "Eats Fatty Food")]
        public bool EatsFattyFood { get; set; }
        
        // Alcohol
        [Display(Name = "Drinks Alcohol")]
        public bool DrinksAlcohol { get; set; }
        
        [Display(Name = "Drinks Beer")]
        public bool DrinksBeer { get; set; }
        
        [Display(Name = "Drinks Wine")]
        public bool DrinksWine { get; set; }
        
        [Display(Name = "Drinks Hard Liquor")]
        public bool DrinksHardLiquor { get; set; }
        
        [Display(Name = "Is Binge Drinker")]
        public bool IsBingeDrinker { get; set; }
        
        // Exercise
        [Display(Name = "Has Regular Exercise")]
        public bool HasRegularExercise { get; set; }
        
        [Display(Name = "Has No Regular Exercise")]
        public bool HasNoRegularExercise { get; set; }
        
        // Assessment
        [Display(Name = "Risk Status")]
        public string RiskStatus { get; set; } = "Low Risk";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
} 