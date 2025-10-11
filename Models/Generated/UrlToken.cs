using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class UrlToken
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public string ResourceType { get; set; } = null!;

    public string ResourceId { get; set; } = null!;

    public string OriginalUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; }

    public DateTime? UsedAt { get; set; }

    public string? ClientIp { get; set; }

    public string? UserAgent { get; set; }
}
