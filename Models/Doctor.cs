using System;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class Doctor
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = new ApplicationUser();
    }
}