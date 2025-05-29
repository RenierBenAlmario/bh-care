using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public string Type { get; set; } // Info, Success, Warning, Danger

        // Making this nullable and not required to maintain backward compatibility
        public string Link { get; set; }

        public string UserId { get; set; } // User ID of the notification owner

        public string RecipientId { get; set; } // If null, notification is for all admins

        [Required]
        public DateTime CreatedAt { get; set; }

        // Making this nullable and not required to maintain backward compatibility
        public DateTime? ReadAt { get; set; }

        // This property should be used instead of ReadAt for backward compatibility
        [Required]
        public bool IsRead { get; set; } = false;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
