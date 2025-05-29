using System;

namespace Barangay.Models
{
    public class MedicalRecordViewModel
    {
        public int Id { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}