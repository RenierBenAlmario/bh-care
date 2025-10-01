using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    public class UserSuspension
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(450)]
        public string UserId { get; set; }
        
        [Required]
        public int DenialCount { get; set; }
        
        [Required]
        public DateTime LastDenialDate { get; set; }
        
        public DateTime? SuspensionStartDate { get; set; }
        
        public DateTime? SuspensionEndDate { get; set; }
        
        [StringLength(50)]
        public string SuspensionReason { get; set; }
        
        [StringLength(50)]
        public string SuspensionLevel { get; set; } // "24h", "3d", "1m"
        
        public bool IsActive { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}



