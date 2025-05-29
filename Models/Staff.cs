using System;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class Staff
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
        
        [Required]
        [StringLength(50)]
        public required string Role { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(256)]
        public required string Email { get; set; }
        
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        public string? Specialization { get; set; }

        public DateTime JoinDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        [StringLength(100)]
        public string? WorkingDays { get; set; } = "Monday,Tuesday,Wednesday,Thursday,Friday";

        [StringLength(50)]
        public string? WorkingHours { get; set; } = "9:00 AM - 5:00 PM";

        public int MaxDailyPatients { get; set; } = 20;
    }
}
