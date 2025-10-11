using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class ConsultationTimeSlot
{
    public int Id { get; set; }

    public string ConsultationType { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public bool IsBooked { get; set; }

    public string BookedById { get; set; } = null!;

    public DateTime? BookedAt { get; set; }

    public DateTime CreatedAt { get; set; }
}
