using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class PrescriptionMedication
{
    public int Id { get; set; }

    public int PrescriptionId { get; set; }

    public int MedicationId { get; set; }

    public string Dosage { get; set; } = null!;

    public string Frequency { get; set; } = null!;

    public string Instructions { get; set; } = null!;

    public int MedicalRecordId { get; set; }

    public string Duration { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public string MedicationName { get; set; } = null!;

    public virtual MedicalRecord MedicalRecord { get; set; } = null!;

    public virtual Medication Medication { get; set; } = null!;

    public virtual Prescription Prescription { get; set; } = null!;
}
