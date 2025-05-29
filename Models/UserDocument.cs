using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    public class UserDocument
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [Required]
        public string FileName { get; set; }
        
        [Required]
        public string FilePath { get; set; }
        
        public string ContentType { get; set; }
        
        public string FileType { get; set; }
        
        public long FileSize { get; set; }
        
        public string Status { get; set; } = "Pending"; // Pending, Verified, Rejected
        
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        
        [StringLength(450)]
        public string? ApprovedBy { get; set; }
        
        public DateTime? ApprovedAt { get; set; }
        
        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        
        [ForeignKey("ApprovedBy")]
        public virtual ApplicationUser Approver { get; set; }
    }
} 