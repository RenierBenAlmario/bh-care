using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class UserPermission
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int PermissionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
