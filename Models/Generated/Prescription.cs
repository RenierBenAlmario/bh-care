using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class Prescription
{
    public int Id { get; set; }

    public string PatientId { get; set; } = null!;

    public string DoctorId { get; set; } = null!;

    public string Diagnosis { get; set; } = null!;

    public int Duration { get; set; }

    public string Notes { get; set; } = null!;

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime PrescriptionDate { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? ApplicationUserId { get; set; }

    public string? PatientUserId { get; set; }

    public DateTime? ValidUntil { get; set; }

    public virtual AspNetUser? ApplicationUser { get; set; }

    public virtual AspNetUser Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public virtual Patient? PatientUser { get; set; }

    public virtual ICollection<PrescriptionMedication> PrescriptionMedications { get; set; } = new List<PrescriptionMedication>();
}
