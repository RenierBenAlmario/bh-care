using System;
using System.Collections.Generic;

namespace Barangay.SchemaModels;

public partial class VitalSign
{
    public int Id { get; set; }

    public string PatientId { get; set; } = null!;

    public decimal? Temperature { get; set; }

    public string? BloodPressure { get; set; }

    public int? HeartRate { get; set; }

    public int? RespiratoryRate { get; set; }

    public decimal? SpO2 { get; set; }

    public decimal? Weight { get; set; }

    public decimal? Height { get; set; }

    public DateTime RecordedAt { get; set; }

    public string? Notes { get; set; }
}
