using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class UserDocument
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public long FileSize { get; set; }

    public string Status { get; set; } = null!;

    public DateTime UploadDate { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public virtual AspNetUser? ApprovedByNavigation { get; set; }

    public virtual AspNetUser User { get; set; } = null!;
}
