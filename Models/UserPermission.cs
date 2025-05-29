using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    public class UserPermission
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public int PermissionId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
        
        [ForeignKey("PermissionId")]
        public virtual Permission? Permission { get; set; }

        // New columns added in AddTimestampsToUserPermissions migration
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
} 