using System;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class Assessment
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Family Number")]
        public string FamilyNumber { get; set; }
        
        [Required]
        [Display(Name = "Reason for Visit")]
        public string ReasonForVisit { get; set; }
        
        [Required]
        [Display(Name = "Symptoms")]
        public string Symptoms { get; set; }
        
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
} 