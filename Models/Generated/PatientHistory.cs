using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class PatientHistory
{
    public int Id { get; set; }

    public string PatientId { get; set; } = null!;

    public int? AppointmentId { get; set; }

    public string DoctorId { get; set; } = null!;

    public string Diagnosis { get; set; } = null!;

    public string Symptoms { get; set; } = null!;

    public string Treatment { get; set; } = null!;

    public string Notes { get; set; } = null!;

    public string Medications { get; set; } = null!;

    public DateTime RecordDate { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
