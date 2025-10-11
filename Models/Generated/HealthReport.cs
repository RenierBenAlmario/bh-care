using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class HealthReport
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime CheckupDate { get; set; }

    public string BloodPressure { get; set; } = null!;

    public int? HeartRate { get; set; }

    public decimal? BloodSugar { get; set; }

    public decimal? Weight { get; set; }

    public decimal? Temperature { get; set; }

    public string PhysicalActivity { get; set; } = null!;

    public string Notes { get; set; } = null!;

    public string DoctorId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AspNetUser Doctor { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
