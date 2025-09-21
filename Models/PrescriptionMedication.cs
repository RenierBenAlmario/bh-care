using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    public class PrescriptionMedication
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int PrescriptionId { get; set; }
        
        [Required]
        public int MedicationId { get; set; }

        [Required]
        public string MedicationName { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Dosage { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Unit { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Frequency { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Instructions { get; set; } = string.Empty;
        
        // Existing properties
        public int MedicalRecordId { get; set; }
        
        public string Duration { get; set; } = string.Empty;
        
        [ForeignKey("PrescriptionId")]
        public virtual Prescription Prescription { get; set; } = null!;
        
        [ForeignKey("MedicationId")]
        public virtual Medication Medication { get; set; } = null!;
        
        // Adding MedicalRecord navigation property
        [ForeignKey("MedicalRecordId")]
        public MedicalRecord MedicalRecord { get; set; } = null!;
    }
}