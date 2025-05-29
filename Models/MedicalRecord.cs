using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        
        [StringLength(1000)]
        public string Diagnosis { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Treatment { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Notes { get; set; } = string.Empty;
        
        [StringLength(450)]
        public string DoctorId { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? UpdatedAt { get; set; }
        
        // New properties as requested
        public DateTime Date { get; set; }
        
        public string Type { get; set; } = string.Empty;
        
        public string ChiefComplaint { get; set; } = string.Empty;
        
        public string Status { get; set; } = string.Empty;
        
        public string Duration { get; set; } = string.Empty;
        
        public string Medications { get; set; } = string.Empty;
        
        public string Prescription { get; set; } = string.Empty;
        
        public string Instructions { get; set; } = string.Empty;
        
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } = null!;
        
        [ForeignKey("DoctorId")]
        public virtual ApplicationUser Doctor { get; set; } = null!;
    }
}