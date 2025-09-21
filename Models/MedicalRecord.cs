using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Barangay.Attributes;

namespace Barangay.Models
{
    public class MedicalRecord
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(450)]
        public string PatientId { get; set; } = string.Empty;
        
        [Required]
        public DateTime RecordDate { get; set; } = DateTime.Now;
        
        [StringLength(2000)] // Increased for encrypted data
        [Encrypted]
        public string Diagnosis { get; set; } = string.Empty;
        
        [StringLength(2000)] // Increased for encrypted data
        [Encrypted]
        public string Treatment { get; set; } = string.Empty;
        
        [StringLength(2000)] // Increased for encrypted data
        [Encrypted]
        public string Notes { get; set; } = string.Empty;
        
        [StringLength(450)]
        public string DoctorId { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? UpdatedAt { get; set; }
        
        // New properties as requested
        public DateTime Date { get; set; }
        
        public string Type { get; set; } = string.Empty;
        
        [Encrypted]
        public string ChiefComplaint { get; set; } = string.Empty;
        
        public string Status { get; set; } = string.Empty;
        
        [Encrypted]
        public string Duration { get; set; } = string.Empty;
        
        [Encrypted]
        public string Medications { get; set; } = string.Empty;
        
        [Encrypted]
        public string Prescription { get; set; } = string.Empty;
        
        [Encrypted]
        public string Instructions { get; set; } = string.Empty;
        
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } = null!;
        
        [ForeignKey("DoctorId")]
        public virtual ApplicationUser Doctor { get; set; } = null!;

        // Foreign key for Appointment
        public int? AppointmentId { get; set; }

        [ForeignKey("AppointmentId")]
        public virtual Appointment? Appointment { get; set; }
    }
}