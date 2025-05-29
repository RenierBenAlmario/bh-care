using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Barangay.Models
{
    public class RolePermission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RoleId { get; set; } = string.Empty;

        [Required]
        public int PermissionId { get; set; }

        [ForeignKey("RoleId")]
        public virtual IdentityRole Role { get; set; } = null!;

        [ForeignKey("PermissionId")]
        public virtual Permission Permission { get; set; } = null!;
    }
} 