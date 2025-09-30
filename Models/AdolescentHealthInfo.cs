using System;
using System.ComponentModel.DataAnnotations;
using Barangay.Attributes;

namespace Barangay.Models
{
    public class AdolescentHealthInfo
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        public string? AppointmentId { get; set; }
        
        // Patient Information
        [Encrypted]
        public string? PatientName { get; set; }
        
        [Encrypted]
        public string? PatientAge { get; set; }
        
        [Encrypted]
        public string? PatientGender { get; set; }
        
        [Encrypted]
        public string? PatientAddress { get; set; }
        
        [Encrypted]
        public string? PatientContact { get; set; }
        
        // Measurements
        [Encrypted]
        public string? HeightCm { get; set; }
        
        [Encrypted]
        public string? WeightKg { get; set; }
        
        [Encrypted]
        public string? BMI { get; set; }
        
        [Encrypted]
        public string? BMICategory { get; set; }
        
        // Immunization
        [Encrypted]
        public string? MRMMRDateGiven { get; set; }
        
        [Encrypted]
        public string? TdDateGiven { get; set; }
        
        [Encrypted]
        public string? HPVDateGiven { get; set; }
        
        // Vital Signs
        [Encrypted]
        public string? Temperature { get; set; }
        
        [Encrypted]
        public string? BloodPressure { get; set; }
        
        [Encrypted]
        public string? PulseRate { get; set; }
        
        [Encrypted]
        public string? RespiratoryRate { get; set; }
        
        // Clinical Information
        [Encrypted]
        public string? ChiefComplaint { get; set; }
        
        [Encrypted]
        public string? WorkingDiagnosis { get; set; }
        
        [Encrypted]
        public string? ReferredTo { get; set; }
        
        // Additional Clinical Fields
        [Encrypted]
        public string? DateOfMenarche { get; set; }
        
        [Encrypted]
        public string? AgeOf1stPregnancy { get; set; }
        
        [Encrypted]
        public string? OBScoreGravida { get; set; }
        
        [Encrypted]
        public string? OBScoreParity { get; set; }
        
        [Encrypted]
        public string? HistoryOfPresentIllness { get; set; }
        
        [Encrypted]
        public string? PhysicalExaminationFindings { get; set; }
        
        [Encrypted]
        public string? PastMedicalHistory { get; set; }
        
        [Encrypted]
        public string? FamilyHistory { get; set; }
        
        [Encrypted]
        public string? Management { get; set; }
        
        [Encrypted]
        public string? ReasonForReferral { get; set; }
        
        [Encrypted]
        public string? FollowUpDate { get; set; }
        
        // Record Information
        [Encrypted]
        public string? RecordedBy { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
    }
}
