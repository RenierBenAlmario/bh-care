using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class AppointmentFile
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    public string FileName { get; set; } = null!;

    public string OriginalFileName { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public DateTime UploadedAt { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;
}
