using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class UserSuspension
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int DenialCount { get; set; }

    public DateTime LastDenialDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime? SuspensionStartDate { get; set; }

    public DateTime? SuspensionEndDate { get; set; }

    public string? SuspensionLevel { get; set; }

    public string? SuspensionReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual AspNetUser User { get; set; } = null!;
}
