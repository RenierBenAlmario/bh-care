using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    public class HealthReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime CheckupDate { get; set; }

        [StringLength(20)]
        public string BloodPressure { get; set; }

        public int? HeartRate { get; set; }

        [Column(TypeName = "decimal(5,1)")]
        public decimal? BloodSugar { get; set; }

        [Column(TypeName = "decimal(5,1)")]
        public decimal? Weight { get; set; }

        [Column(TypeName = "decimal(4,1)")]
        public decimal? Temperature { get; set; }

        [StringLength(100)]
        public string PhysicalActivity { get; set; }

        public string Notes { get; set; }

        public string DoctorId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("DoctorId")]
        public virtual ApplicationUser Doctor { get; set; }
    }
} 