using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class MedicalRecord
{
    public int Id { get; set; }

    public string PatientId { get; set; } = null!;

    public DateTime RecordDate { get; set; }

    public string Diagnosis { get; set; } = null!;

    public string Treatment { get; set; } = null!;

    public string Notes { get; set; } = null!;

    public string DoctorId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime Date { get; set; }

    public string Type { get; set; } = null!;

    public string ChiefComplaint { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Duration { get; set; } = null!;

    public string Medications { get; set; } = null!;

    public string Prescription { get; set; } = null!;

    public string Instructions { get; set; } = null!;

    public string? ApplicationUserId { get; set; }

    public int? AppointmentId { get; set; }

    public virtual AspNetUser? ApplicationUser { get; set; }

    public virtual Appointment? Appointment { get; set; }

    public virtual AspNetUser Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public virtual ICollection<PrescriptionMedication> PrescriptionMedications { get; set; } = new List<PrescriptionMedication>();
}
