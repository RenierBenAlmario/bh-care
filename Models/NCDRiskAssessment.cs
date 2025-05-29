using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    public class NCDRiskAssessment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [ForeignKey("AppointmentId")]
        public Appointment Appointment { get; set; }

        [Required]
        [Display(Name = "Health Facility")]
        public string HealthFacility { get; set; }

        [Required]
        [Display(Name = "Family No")]
        public string FamilyNo { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Barangay")]
        public string Barangay { get; set; }

        [Required]
        [Display(Name = "Birthday")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [Required]
        [Display(Name = "Telepono")]
        public string Telepono { get; set; }

        [Required]
        [Display(Name = "Edad")]
        [Range(20, 120, ErrorMessage = "Age must be between 20 and 120")]
        public int Edad { get; set; }

        [Required]
        [Display(Name = "Kasarian")]
        [StringLength(10)]
        public string Kasarian { get; set; }

        [Display(Name = "Relihiyon")]
        [StringLength(50)]
        public string Relihiyon { get; set; }

        // Medical History
        public bool HasDiabetes { get; set; }
        public bool HasHypertension { get; set; }
        public bool HasCancer { get; set; }
        public bool HasCOPD { get; set; }
        public bool HasLungDisease { get; set; }
        public bool HasEyeDisease { get; set; }

        [StringLength(100)]
        public string CancerType { get; set; }

        // Family History
        public bool FamilyHasHypertension { get; set; }
        public bool FamilyHasHeartDisease { get; set; }
        public bool FamilyHasStroke { get; set; }
        public bool FamilyHasDiabetes { get; set; }
        public bool FamilyHasCancer { get; set; }
        public bool FamilyHasKidneyDisease { get; set; }
        public bool FamilyHasOtherDisease { get; set; }

        public string FamilyOtherDiseaseDetails { get; set; }

        // Risk Factors
        [Required]
        [StringLength(20)]
        public string SmokingStatus { get; set; } = "Non-smoker"; // Default value

        public bool HighSaltIntake { get; set; }

        [StringLength(50)]
        public string AlcoholFrequency { get; set; }

        [StringLength(50)]
        public string AlcoholConsumption { get; set; }

        [StringLength(50)]
        public string ExerciseDuration { get; set; }

        [Required]
        [StringLength(20)]
        public string RiskStatus { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
} 