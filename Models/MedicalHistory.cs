using System;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class MedicalHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PatientId { get; set; } = string.Empty;
        public virtual Patient Patient { get; set; } = null!;

        [Required]
        public string ChiefComplaint { get; set; } = string.Empty;

        [Required]
        public string HistoryOfPresentIllness { get; set; } = string.Empty;

        public string? Allergies { get; set; }

        public string? CurrentMedications { get; set; }

        public string PastMedicalHistory { get; set; } = string.Empty;

        public string FamilyHistory { get; set; } = string.Empty;

        public string PersonalSocialHistory { get; set; } = string.Empty;

        public string ReviewOfSystems { get; set; } = string.Empty;

        public string PhysicalExamination { get; set; } = string.Empty;

        public DateTime DateRecorded { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
} 