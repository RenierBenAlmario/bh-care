using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Data
{
    [Table("VitalSigns", Schema = "dbo")]
    public class VitalSigns
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int AppointmentId { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Temperature { get; set; }
        
        [Required]
        public int RespiratoryRate { get; set; }
        
        [Required]
        public int OxygenSaturation { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Weight { get; set; }
        
        [Required]
        public int Height { get; set; }
        
        [StringLength(20)]
        public string BloodPressure { get; set; }
        
        [StringLength(500)]
        public string Notes { get; set; }
        
        public string LastUpdatedBy { get; set; }
        
        public DateTime? LastUpdatedDate { get; set; }
        
        // Navigation properties can be added if using Entity Framework
        // public virtual Appointment Appointment { get; set; }
    }
} 