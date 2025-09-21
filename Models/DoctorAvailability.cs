using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    public class DoctorAvailability
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string DoctorId { get; set; }

        [Required]
        public bool IsAvailable { get; set; }

        [Required]
        public bool Monday { get; set; } = true;
        
        [Required]
        public bool Tuesday { get; set; } = true;
        
        [Required]
        public bool Wednesday { get; set; } = true;
        
        [Required]
        public bool Thursday { get; set; } = true;
        
        [Required]
        public bool Friday { get; set; } = true;
        
        [Required]
        public bool Saturday { get; set; } = true;  // ENABLE WEEKENDS BY DEFAULT
        
        [Required]
        public bool Sunday { get; set; } = true;    // ENABLE WEEKENDS BY DEFAULT

        [Required]
        public TimeSpan StartTime { get; set; } = new TimeSpan(9, 0, 0); // 9:00 AM

        [Required]
        public TimeSpan EndTime { get; set; } = new TimeSpan(17, 0, 0); // 5:00 PM

        [Required]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        [ForeignKey("DoctorId")]
        public virtual ApplicationUser Doctor { get; set; }
    }
} 