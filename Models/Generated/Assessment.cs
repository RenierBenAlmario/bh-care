using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class Assessment
{
    public int Id { get; set; }

    public string? FamilyNumber { get; set; }

    public string? ReasonForVisit { get; set; }

    public string? Symptoms { get; set; }

    public DateTime CreatedAt { get; set; }
}
