using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    public class GuardianInformation
    {
        [Key]
        public int GuardianId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Guardian's First Name")]
        public string GuardianFirstName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Guardian's Last Name")]
        public string GuardianLastName { get; set; }

        [Display(Name = "Residency Proof")]
        public byte[] ResidencyProof { get; set; }

        [Display(Name = "Residency Proof Path")]
        public string ResidencyProofPath { get; set; }

        [Display(Name = "Proof Type")]
        public string ProofType { get; set; } = "GuardianResidencyProof";

        [Display(Name = "Consent Status")]
        public string ConsentStatus { get; set; } = "Pending";

        // Alias properties to fix compatibility issues
        public string FirstName 
        { 
            get => GuardianFirstName; 
            set => GuardianFirstName = value; 
        }

        public string LastName 
        { 
            get => GuardianLastName; 
            set => GuardianLastName = value; 
        }

        public DateTime CreatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
} 


