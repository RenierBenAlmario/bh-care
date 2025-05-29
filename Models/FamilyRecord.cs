using System;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class FamilyRecord
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Family Number")]
        public string FamilyNumber { get; set; }
        
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }
        
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
} 