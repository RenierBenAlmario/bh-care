using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    public class EmailSuspension
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }
        
        [Required]
        public int FailureCount { get; set; }
        
        [Required]
        public DateTime LastFailureDate { get; set; }
        
        public DateTime? SuspensionStartDate { get; set; }
        
        public DateTime? SuspensionEndDate { get; set; }
        
        [StringLength(50)]
        public string SuspensionReason { get; set; }
        
        [StringLength(50)]
        public string SuspensionLevel { get; set; } // "3f", "5f", "10f"
        
        public bool IsActive { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
    }
}
