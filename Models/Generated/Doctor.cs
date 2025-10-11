using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class Doctor
{
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Specialization { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
