using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
        [StringLength(1000)]
        public string Diagnosis { get; set; }
        
        [Required]
        public int Duration { get; set; }
        
        [StringLength(1000)]
        public string Notes { get; set; } = string.Empty;
        
        [Required]
        public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Created;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime PrescriptionDate { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        
        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } = null!;
        
        [ForeignKey("DoctorId")]
        public virtual ApplicationUser Doctor { get; set; } = null!;

        [NotMapped]
        public IEnumerable<string> Medications => PrescriptionMedicines.Select(pm => pm.MedicationName);

        public virtual ICollection<PrescriptionMedicine> PrescriptionMedicines { get; set; } = new List<PrescriptionMedicine>();
    }

    public class PrescriptionMedicine
    {
        [Key]
        public int Id { get; set; }

        public int PrescriptionId { get; set; }

        [Required]
        [StringLength(200)]
        public string MedicationName { get; set; }

        [Required]
        public decimal Dosage { get; set; }

        [Required]
        [StringLength(20)]
        public string Unit { get; set; }

        [Required]
        [StringLength(200)]
        public string Frequency { get; set; }

        // Navigation property
        [ForeignKey("PrescriptionId")]
        public virtual Prescription Prescription { get; set; }
    }
}