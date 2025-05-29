using System;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class MedicalHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ChiefComplaint { get; set; } = string.Empty;

        [Required]
        public string HistoryOfPresentIllness { get; set; } = string.Empty;

        public string PastMedicalHistory { get; set; } = string.Empty;

        public string FamilyHistory { get; set; } = string.Empty;

        public string PersonalSocialHistory { get; set; } = string.Empty;

        public string ReviewOfSystems { get; set; } = string.Empty;

        public string PhysicalExamination { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
} 