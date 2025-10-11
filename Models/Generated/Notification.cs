using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class Notification
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Link { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string RecipientId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public bool IsRead { get; set; }

    public virtual AspNetUser User { get; set; } = null!;
}
