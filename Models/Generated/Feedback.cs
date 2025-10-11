using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class Feedback
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string Comment { get; set; } = null!;

    public int Rating { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AspNetUser User { get; set; } = null!;
}
