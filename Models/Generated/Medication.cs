using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class Medication
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Category { get; set; }

    public string? Manufacturer { get; set; }

    public virtual ICollection<PrescriptionMedication> PrescriptionMedications { get; set; } = new List<PrescriptionMedication>();
}
