using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class StaffMember
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Department { get; set; }

    public string Position { get; set; } = null!;

    public string? Specialization { get; set; }

    public string? LicenseNumber { get; set; }

    public string ContactNumber { get; set; } = null!;

    public string WorkingDays { get; set; } = null!;

    public string WorkingHours { get; set; } = null!;

    public DateTime JoinDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public int MaxDailyPatients { get; set; }

    public bool IsActive { get; set; }

    public string Role { get; set; } = null!;

    public virtual ICollection<StaffPermission> StaffPermissions { get; set; } = new List<StaffPermission>();

    public virtual AspNetUser User { get; set; } = null!;
}
