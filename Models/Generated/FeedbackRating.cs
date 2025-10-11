using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class FeedbackRating
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string ServiceType { get; set; } = null!;

    public int? AppointmentId { get; set; }

    public int Rating { get; set; }

    public string Comments { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
