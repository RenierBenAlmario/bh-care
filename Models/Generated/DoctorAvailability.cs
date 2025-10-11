using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class DoctorAvailability
{
    public int Id { get; set; }

    public string DoctorId { get; set; } = null!;

    public bool IsAvailable { get; set; }

    public bool Monday { get; set; }

    public bool Tuesday { get; set; }

    public bool Wednesday { get; set; }

    public bool Thursday { get; set; }

    public bool Friday { get; set; }

    public bool Saturday { get; set; }

    public bool Sunday { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public DateTime LastUpdated { get; set; }

    public virtual AspNetUser Doctor { get; set; } = null!;
}
