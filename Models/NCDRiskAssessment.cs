using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Barangay.Attributes;

namespace Barangay.Models
{
    public class NCDRiskAssessment
    {
        [Key]
        public int Id { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        public int? AppointmentId { get; set; }

        [ForeignKey("AppointmentId")]
        public virtual Appointment? Appointment { get; set; }

        // Health Facility Information
        [Display(Name = "Health Facility")]
        [StringLength(100)]
        [Encrypted]
        public string? HealthFacility { get; set; }

        [Display(Name = "Family No")]
        [StringLength(100)]
        [Encrypted]
        public string? FamilyNo { get; set; }

        [Display(Name = "Address")]
        [StringLength(500)]
        [Encrypted]
        public string? Address { get; set; }

        [Display(Name = "Barangay")]
        [StringLength(100)]
        [Encrypted]
        public string? Barangay { get; set; }

        [Display(Name = "Birthday")]
        [StringLength(50)]
        [Encrypted]
        public string? Birthday { get; set; }

        [Display(Name = "Telepono")]
        [StringLength(100)]
        [Encrypted]
        public string? Telepono { get; set; }

        [Display(Name = "Edad")]
        [StringLength(10)]
        [Encrypted]
        public string? Edad { get; set; }

        [Display(Name = "Kasarian")]
        [StringLength(50)]
        [Encrypted]
        public string? Kasarian { get; set; }

        [Display(Name = "Relihiyon")]
        [StringLength(100)]
        [Encrypted]
        public string? Relihiyon { get; set; }

        // Medical History
        [Display(Name = "Has Diabetes")]
        [StringLength(10)]
        [Encrypted]
        public string? HasDiabetes { get; set; } = "false";

        [Display(Name = "Has Hypertension")]
        [StringLength(10)]
        [Encrypted]
        public string? HasHypertension { get; set; } = "false";

        [Display(Name = "Has Cancer")]
        [StringLength(10)]
        [Encrypted]
        public string? HasCancer { get; set; } = "false";

        [Display(Name = "Has COPD")]
        [StringLength(10)]
        [Encrypted]
        public string? HasCOPD { get; set; } = "false";

        [Display(Name = "Has Lung Disease")]
        [StringLength(10)]
        [Encrypted]
        public string? HasLungDisease { get; set; } = "false";

        [Display(Name = "Has Eye Disease")]
        [StringLength(10)]
        [Encrypted]
        public string? HasEyeDisease { get; set; } = "false";

        [Display(Name = "Cancer Type")]
        [StringLength(200)]
        [Encrypted]
        public string? CancerType { get; set; }

        // Family History
        [Display(Name = "Family Has Hypertension")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHasHypertension { get; set; } = "false";

        [Display(Name = "Family Has Heart Disease")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHasHeartDisease { get; set; } = "false";

        [Display(Name = "Family Has Stroke")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHasStroke { get; set; } = "false";

        [Display(Name = "Family Has Diabetes")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHasDiabetes { get; set; } = "false";

        [Display(Name = "Family Has Cancer")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHasCancer { get; set; } = "false";

        [Display(Name = "Family Has Kidney Disease")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHasKidneyDisease { get; set; } = "false";

        [Display(Name = "Family Has Other Disease")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHasOtherDisease { get; set; } = "false";

        [Display(Name = "Family Other Disease Details")]
        [StringLength(4000)]
        [Encrypted]
        public string? FamilyOtherDiseaseDetails { get; set; }

        // Lifestyle Factors
        [Display(Name = "Smoking Status")]
        [StringLength(100)]
        [Encrypted]
        public string? SmokingStatus { get; set; }

        [Display(Name = "High Salt Intake")]
        [StringLength(10)]
        [Encrypted]
        public string? HighSaltIntake { get; set; } = "false";

        [Display(Name = "Alcohol Frequency")]
        [StringLength(100)]
        [Encrypted]
        public string? AlcoholFrequency { get; set; }

        [Display(Name = "Alcohol Consumption")]
        [StringLength(100)]
        [Encrypted]
        public string? AlcoholConsumption { get; set; }

        [Display(Name = "Exercise Duration")]
        [StringLength(100)]
        [Encrypted]
        public string? ExerciseDuration { get; set; }

        [Display(Name = "Risk Status")]
        [StringLength(100)]
        [Encrypted]
        public string? RiskStatus { get; set; }

        // Chest Pain
        [Display(Name = "Chest Pain")]
        [StringLength(200)]
        [Encrypted]
        public string? ChestPain { get; set; }

        [Display(Name = "Chest Pain Location")]
        [StringLength(200)]
        [Encrypted]
        public string? ChestPainLocation { get; set; }

        [Display(Name = "Chest Pain Value")]
        [StringLength(10)]
        [Encrypted]
        public string? ChestPainValue { get; set; }

        [Display(Name = "Has Difficulty Breathing")]
        [StringLength(10)]
        [Encrypted]
        public string? HasDifficultyBreathing { get; set; } = "false";

        [Display(Name = "Has Asthma")]
        [StringLength(10)]
        [Encrypted]
        public string? HasAsthma { get; set; } = "false";

        [Display(Name = "Has No Regular Exercise")]
        [StringLength(10)]
        [Encrypted]
        public string? HasNoRegularExercise { get; set; } = "false";

        // System Fields
        [Display(Name = "Created At")]
        [StringLength(50)]
        [Encrypted]
        public string? CreatedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        [Display(Name = "Updated At")]
        [StringLength(50)]
        [Encrypted]
        public string? UpdatedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        [Display(Name = "Appointment Type")]
        [StringLength(200)]
        [Encrypted]
        public string? AppointmentType { get; set; }

        // Medication and Year Fields
        [Display(Name = "Cancer Medication")]
        [StringLength(200)]
        [Encrypted]
        public string? CancerMedication { get; set; }

        [Display(Name = "Cancer Year")]
        [StringLength(50)]
        [Encrypted]
        public string? CancerYear { get; set; }

        [Display(Name = "Civil Status")]
        [StringLength(100)]
        [Encrypted]
        public string? CivilStatus { get; set; }

        [Display(Name = "Diabetes Medication")]
        [StringLength(200)]
        [Encrypted]
        public string? DiabetesMedication { get; set; }

        [Display(Name = "Diabetes Year")]
        [StringLength(50)]
        [Encrypted]
        public string? DiabetesYear { get; set; }

        // Family History Details
        [Display(Name = "Family History Cancer Father")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHistoryCancerFather { get; set; } = "false";

        [Display(Name = "Family History Cancer Mother")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHistoryCancerMother { get; set; } = "false";

        [Display(Name = "Family History Cancer Sibling")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHistoryCancerSibling { get; set; } = "false";

        [Display(Name = "Family History Diabetes Father")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHistoryDiabetesFather { get; set; } = "false";

        [Display(Name = "Family History Diabetes Mother")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHistoryDiabetesMother { get; set; } = "false";

        [Display(Name = "Family History Diabetes Sibling")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHistoryDiabetesSibling { get; set; } = "false";

        [Display(Name = "Family History Heart Disease Father")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHistoryHeartDiseaseFather { get; set; } = "false";

        [Display(Name = "Family History Heart Disease Mother")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHistoryHeartDiseaseMother { get; set; } = "false";

        [Display(Name = "Family History Heart Disease Sibling")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHistoryHeartDiseaseSibling { get; set; } = "false";

        [Display(Name = "Family History Lung Disease Father")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHistoryLungDiseaseFather { get; set; } = "false";

        [Display(Name = "Family History Lung Disease Mother")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHistoryLungDiseaseMother { get; set; } = "false";

        [Display(Name = "Family History Lung Disease Sibling")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHistoryLungDiseaseSibling { get; set; } = "false";

        [Display(Name = "Family History Other")]
        [StringLength(200)]
        [Encrypted]
        public string? FamilyHistoryOther { get; set; }

        [Display(Name = "Family History Other Father")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHistoryOtherFather { get; set; } = "false";

        [Display(Name = "Family History Other Mother")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHistoryOtherMother { get; set; } = "false";

        [Display(Name = "Family History Other Sibling")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHistoryOtherSibling { get; set; } = "false";

        [Display(Name = "Family History Stroke Father")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHistoryStrokeFather { get; set; } = "false";

        [Display(Name = "Family History Stroke Mother")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHistoryStrokeMother { get; set; } = "false";

        [Display(Name = "Family History Stroke Sibling")]
        [StringLength(10)]
        [Encrypted]
        public string? FamilyHistoryStrokeSibling { get; set; } = "false";

        [Display(Name = "First Name")]
        [StringLength(100)]
        [Encrypted]
        public string? FirstName { get; set; }

        [Display(Name = "Hypertension Medication")]
        [StringLength(200)]
        [Encrypted]
        public string? HypertensionMedication { get; set; }

        [Display(Name = "Hypertension Year")]
        [StringLength(50)]
        [Encrypted]
        public string? HypertensionYear { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(100)]
        [Encrypted]
        public string? LastName { get; set; }

        [Display(Name = "Lung Disease Medication")]
        [StringLength(200)]
        [Encrypted]
        public string? LungDiseaseMedication { get; set; }

        [Display(Name = "Lung Disease Year")]
        [StringLength(50)]
        [Encrypted]
        public string? LungDiseaseYear { get; set; }

        [Display(Name = "Middle Name")]
        [StringLength(100)]
        [Encrypted]
        public string? MiddleName { get; set; }

        [Display(Name = "Occupation")]
        [StringLength(200)]
        [Encrypted]
        public string? Occupation { get; set; }

    }
}