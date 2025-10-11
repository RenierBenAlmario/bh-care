using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class Message
{
    public int Id { get; set; }

    public string SenderId { get; set; } = null!;

    public string ReceiverId { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string SenderName { get; set; } = null!;

    public string RecipientGroup { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public DateTime SentAt { get; set; }

    public bool IsRead { get; set; }

    public virtual AspNetUser Receiver { get; set; } = null!;

    public virtual AspNetUser Sender { get; set; } = null!;
}
