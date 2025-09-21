using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    public class LabResult
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PatientId { get; set; }

        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        public DateTime Date { get; set; }
        public string TestName { get; set; }
        public string Result { get; set; }
        public string ReferenceRange { get; set; }
        public string Status { get; set; }
        public string? Notes { get; set; }
    }
}
