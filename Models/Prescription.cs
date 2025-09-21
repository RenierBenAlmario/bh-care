using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Barangay.Attributes;

namespace Barangay.Models
{
    public class Prescription
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(450)]
        public string PatientId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(450)]
        public string DoctorId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(2000)] // Increased for encrypted data
        [Encrypted]
        public string Diagnosis { get; set; }
        
        [Required]
        public int Duration { get; set; }
        
        [StringLength(2000)] // Increased for encrypted data
        [Encrypted]
        public string Notes { get; set; } = string.Empty;
        
        [Required]
        public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Created;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime PrescriptionDate { get; set; } = DateTime.UtcNow;
        // New: explicit persisted validity date; computed from PrescriptionDate + Duration on creation
        public DateTime? ValidUntil { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        
        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } = null!;
        
        [ForeignKey("DoctorId")]
        public virtual ApplicationUser Doctor { get; set; } = null!;

        [NotMapped]
        public IEnumerable<string> Medications => PrescriptionMedicines.Select(pm => pm.Medication?.Name ?? "Unknown Medication");

        public virtual ICollection<PrescriptionMedication> PrescriptionMedicines { get; set; }
    }
}
