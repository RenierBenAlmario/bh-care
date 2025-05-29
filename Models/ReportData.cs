using System;
using System.Collections.Generic;

namespace Barangay.Models
{
    public class ReportData
    {
        public DateTime ReportDate { get; set; }
        public int TotalRegistrations { get; set; }
        public int TotalConsultations { get; set; }
        public double AverageHealthIndex { get; set; }
        public Dictionary<string, int> ConsultationsByType { get; set; } = new();
        public Dictionary<string, int> MonthlyRegistrations { get; set; } = new();
        public Dictionary<string, double> HealthIndexByCategory { get; set; } = new();
        public List<string> ChartImagePaths { get; set; } = new();
    }

    public class AdminReportsViewModel
    {
        public DateTime CurrentDateTime { get; set; }
        public ReportData ReportData { get; set; } = new();
        public string PdfGenerationStatus { get; set; } = string.Empty;
    }
} 