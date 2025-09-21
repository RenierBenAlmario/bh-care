using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    public class StaffMember
    {
        [Key]
        public int Id { get; set; }
        
        [Required(AllowEmptyStrings = true)]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public string? Department { get; set; }
        
        [Required]
        public string? Position { get; set; }
        
        public string? Specialization { get; set; }
        
        public string? LicenseNumber { get; set; }
        
        [Required]
        [Phone]
        [Display(Name = "Contact Number")]
        public string? ContactNumber { get; set; }
        
        [Required]
        [Display(Name = "Working Days")]
        public string? WorkingDays { get; set; }
        
        [Required]
        [Display(Name = "Working Hours")]
        public string? WorkingHours { get; set; }
        
        public DateTime JoinDate { get; set; } = DateTime.Now;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public int MaxDailyPatients { get; set; } = 20;
        
        public bool IsActive { get; set; } = true;
        
        [Required]
        public string Role { get; set; } = string.Empty;
        
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<StaffPermission> StaffPermissions { get; set; } = new List<StaffPermission>();
    }
}
