using System;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class EmailVerification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(10)]
        public string VerificationCode { get; set; }

        [Required]
        public DateTime ExpiryTime { get; set; }

        public bool IsVerified { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? VerifiedAt { get; set; }
    }
} 