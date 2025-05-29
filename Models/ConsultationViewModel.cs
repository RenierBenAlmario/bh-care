using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class ConsultationViewModel
    {
        [Required]
        public string Notes { get; set; } = string.Empty;
        [Required]
        public string Diagnosis { get; set; } = string.Empty;
        [Required]
        public string Prescription { get; set; } = string.Empty;
        public string? ChiefComplaint { get; set; }
        public string? Treatment { get; set; }
        public List<PrescriptionMedicationViewModel> Medications { get; set; } = new();
    }

    public class PrescriptionMedicationViewModel
    {
        public string MedicationName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public string? Frequency { get; set; }
        public string? Duration { get; set; }
    }
} 