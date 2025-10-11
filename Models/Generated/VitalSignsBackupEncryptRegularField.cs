using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class VitalSignsBackupEncryptRegularField
{
    public int Id { get; set; }

    public string? PatientId { get; set; }

    public string? BloodPressure { get; set; }

    public DateTime RecordedAt { get; set; }

    public string? Notes { get; set; }

    public string? EncryptedBloodPressure { get; set; }

    public string? EncryptedHeartRate { get; set; }

    public string? EncryptedHeight { get; set; }

    public string? EncryptedRespiratoryRate { get; set; }

    public string? EncryptedSpO2 { get; set; }

    public string? EncryptedTemperature { get; set; }

    public string? EncryptedWeight { get; set; }

    public string? Temperature { get; set; }

    public string? HeartRate { get; set; }

    public string? RespiratoryRate { get; set; }

    public string? SpO2 { get; set; }

    public string? Weight { get; set; }

    public string? Height { get; set; }
}
