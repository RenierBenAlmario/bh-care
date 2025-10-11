using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class FamilyRecord
{
    public int Id { get; set; }

    public string FamilyNumber { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string Address { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
