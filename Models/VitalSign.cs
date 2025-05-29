using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    public class VitalSign
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(450)]
        public string PatientId { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? Temperature { get; set; }
        
        [StringLength(20)]
        public string? BloodPressure { get; set; }
        
        public int? HeartRate { get; set; }
        
        public int? RespiratoryRate { get; set; }
        
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? SpO2 { get; set; }
        
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? Weight { get; set; }
        
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? Height { get; set; }
        
        public DateTime RecordedAt { get; set; } = DateTime.Now;
        
        [StringLength(1000)]
        public string? Notes { get; set; }
        
        // Navigation property
        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }
    }
}