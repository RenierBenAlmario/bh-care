using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    public class UrlToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Token { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ResourceType { get; set; } = string.Empty; // "User", "Nurse", "Doctor", "Admin"

        [Required]
        [StringLength(450)]
        public string ResourceId { get; set; } = string.Empty; // User ID, Doctor ID, etc.

        [Required]
        [StringLength(500)]
        public string OriginalUrl { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; } = false;

        public DateTime? UsedAt { get; set; }

        [StringLength(45)]
        public string? ClientIp { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        // Navigation properties
        [ForeignKey("ResourceId")]
        public ApplicationUser? User { get; set; }
    }
}
