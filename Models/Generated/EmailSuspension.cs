using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class EmailSuspension
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public int FailureCount { get; set; }

    public DateTime LastFailureDate { get; set; }

    public DateTime? SuspensionStartDate { get; set; }

    public DateTime? SuspensionEndDate { get; set; }

    public string SuspensionReason { get; set; } = null!;

    public string SuspensionLevel { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
