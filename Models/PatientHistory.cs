using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    public class PatientHistory
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string PatientId { get; set; }
        
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        
        public int? AppointmentId { get; set; }
        
        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointment { get; set; }
        
        public string DoctorId { get; set; }
        
        [StringLength(500)]
        public string Diagnosis { get; set; }
        
        [Column(TypeName = "ntext")]
        public string Symptoms { get; set; }
        
        [Column(TypeName = "ntext")]
        public string Treatment { get; set; }
        
        [Column(TypeName = "ntext")]
        public string Notes { get; set; }
        
        [StringLength(500)]
        public string Medications { get; set; }
        
        [Required]
        public DateTime RecordDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
    }
} 