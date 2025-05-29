using System;
using System.Collections.Generic;

namespace Barangay.Models
{
    public class PrescriptionViewModel
    {
        public int Id { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public DateTime DateIssued { get; set; }
        public string Medications { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}