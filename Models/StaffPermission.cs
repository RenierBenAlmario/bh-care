using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    public class StaffPermission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StaffMemberId { get; set; }

        [Required]
        public int PermissionId { get; set; }

        public DateTime GrantedAt { get; set; }

        [ForeignKey("StaffMemberId")]
        public virtual StaffMember StaffMember { get; set; }

        [ForeignKey("PermissionId")]
        public virtual Permission Permission { get; set; }
    }
} 