using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    public class VitalSigns
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string PatientId { get; set; } = string.Empty;
        
        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal? Temperature { get; set; }
        
        [StringLength(20)]
        public string? BloodPressure { get; set; }
        
        public int? HeartRate { get; set; }
        
        public int? RespiratoryRate { get; set; }
        
        [Range(0, 100)]
        public int? SpO2 { get; set; }
        
        // Add Weight and Height properties
        [Column(TypeName = "decimal(5,2)")]
        public decimal? Weight { get; set; }
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal? Height { get; set; }
        
        [Required]
        public DateTime RecordedAt { get; set; } = DateTime.Now;
        
        public string? Notes { get; set; }
    }
}