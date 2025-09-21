using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Barangay.Attributes;
using Barangay.Services;

namespace Barangay.Models
{
    public class ImmunizationRecord
    {
        public int Id { get; set; }

        [Required]
        [StringLength(4000)]
        [Encrypted]
        public string ChildName { get; set; } = string.Empty;

        [Required]
        [StringLength(4000)]
        [Encrypted]
        public string DateOfBirth { get; set; } = string.Empty;

        [StringLength(4000)]
        [Encrypted]
        public string PlaceOfBirth { get; set; } = string.Empty;

        [StringLength(4000)]
        [Encrypted]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(4000)]
        [Encrypted]
        public string MotherName { get; set; } = string.Empty;

        [StringLength(4000)]
        [Encrypted]
        public string FatherName { get; set; } = string.Empty;

        [StringLength(4000)]
        [Encrypted]
        public string Sex { get; set; } = string.Empty; // Male/Female

        [StringLength(4000)]
        [Encrypted]
        public string BirthHeight { get; set; } = string.Empty;

        [StringLength(4000)]
        [Encrypted]
        public string BirthWeight { get; set; } = string.Empty;

        [StringLength(4000)]
        [Encrypted]
        public string HealthCenter { get; set; } = string.Empty;

        [StringLength(4000)]
        [Encrypted]
        public string Barangay { get; set; } = string.Empty;

        [StringLength(4000)]
        [Encrypted]
        public string FamilyNumber { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(4000)]
        [Encrypted]
        public string Email { get; set; } = string.Empty;

        [StringLength(4000)]
        [Encrypted]
        public string ContactNumber { get; set; } = string.Empty;

        // Vaccination Records
        [StringLength(4000)]
        [Encrypted]
        public string? BCGVaccineDate { get; set; }
        [StringLength(4000)]
        [Encrypted]
        public string? BCGVaccineRemarks { get; set; }

        [StringLength(4000)]
        [Encrypted]
        public string? HepatitisBVaccineDate { get; set; }
        [StringLength(4000)]
        [Encrypted]
        public string? HepatitisBVaccineRemarks { get; set; }

        [StringLength(4000)]
        [Encrypted]
        public string? Pentavalent1Date { get; set; }
        [StringLength(4000)]
        [Encrypted]
        public string? Pentavalent1Remarks { get; set; }

        [StringLength(4000)]
        [Encrypted]
        public string? Pentavalent2Date { get; set; }
        [StringLength(4000)]
        [Encrypted]
        public string? Pentavalent2Remarks { get; set; }

        [StringLength(4000)]
        [Encrypted]
        public string? Pentavalent3Date { get; set; }
        [StringLength(4000)]
        [Encrypted]
        public string? Pentavalent3Remarks { get; set; }

        [StringLength(4000)]
        [Encrypted]
        public string? OPV1Date { get; set; }
        [StringLength(4000)]
        [Encrypted]
        public string? OPV1Remarks { get; set; }

        [StringLength(4000)]
        [Encrypted]
        public string? OPV2Date { get; set; }
        [StringLength(4000)]
        [Encrypted]
        public string? OPV2Remarks { get; set; }

        [StringLength(4000)]
        [Encrypted]
        public string? OPV3Date { get; set; }
        [StringLength(4000)]
        [Encrypted]
        public string? OPV3Remarks { get; set; }

        [StringLength(4000)]
        [Encrypted]
        public string? IPV1Date { get; set; }
        [StringLength(4000)]
        [Encrypted]
        public string? IPV1Remarks { get; set; }

        [StringLength(4000)]
        [Encrypted]
        public string? IPV2Date { get; set; }
        [StringLength(4000)]
        [Encrypted]
        public string? IPV2Remarks { get; set; }

        [StringLength(4000)]
        [Encrypted]
        public string? PCV1Date { get; set; }
        [StringLength(4000)]
        [Encrypted]
        public string? PCV1Remarks { get; set; }

        [StringLength(4000)]
        [Encrypted]
        public string? PCV2Date { get; set; }
        [StringLength(4000)]
        [Encrypted]
        public string? PCV2Remarks { get; set; }

        [StringLength(4000)]
        [Encrypted]
        public string? PCV3Date { get; set; }
        [StringLength(4000)]
        [Encrypted]
        public string? PCV3Remarks { get; set; }

        [StringLength(4000)]
        [Encrypted]
        public string? MMR1Date { get; set; }
        [StringLength(4000)]
        [Encrypted]
        public string? MMR1Remarks { get; set; }

        [StringLength(4000)]
        [Encrypted]
        public string? MMR2Date { get; set; }
        [StringLength(4000)]
        [Encrypted]
        public string? MMR2Remarks { get; set; }

        // Record Management
        [StringLength(4000)]
        [Encrypted]
        public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        
        [StringLength(4000)]
        [Encrypted]
        public string UpdatedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        
        [StringLength(4000)]
        [Encrypted]
        public string CreatedBy { get; set; } = string.Empty; // Nurse ID
        
        [StringLength(4000)]
        [Encrypted]
        public string UpdatedBy { get; set; } = string.Empty;
        
        // Additional field to trigger migration
        [StringLength(4000)]
        [Encrypted]
        public string Status { get; set; } = "Active";
    }
}
