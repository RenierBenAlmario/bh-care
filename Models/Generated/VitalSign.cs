using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class VitalSign
{
    public int Id { get; set; }

    public string? PatientId { get; set; }

    public decimal? Temperature { get; set; }

    public string? BloodPressure { get; set; }

    public int? HeartRate { get; set; }

    public int? RespiratoryRate { get; set; }

    public decimal? SpO2 { get; set; }

    public decimal? Weight { get; set; }

    public decimal? Height { get; set; }

    public DateTime RecordedAt { get; set; }

    public string? Notes { get; set; }

    public string? EncryptedBloodPressure { get; set; }

    public string? EncryptedHeartRate { get; set; }

    public string? EncryptedHeight { get; set; }

    public string? EncryptedRespiratoryRate { get; set; }

    public string? EncryptedSpO2 { get; set; }

    public string? EncryptedTemperature { get; set; }

    public string? EncryptedWeight { get; set; }

    public virtual Patient? Patient { get; set; }
}
