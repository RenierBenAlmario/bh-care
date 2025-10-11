using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class LabResult
{
    public int Id { get; set; }

    public string PatientId { get; set; } = null!;

    public DateTime Date { get; set; }

    public string TestName { get; set; } = null!;

    public string Result { get; set; } = null!;

    public string ReferenceRange { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public virtual Patient Patient { get; set; } = null!;
}
