using System;
using System.Collections.Generic;

namespace Barangay.TempModels;

public partial class UserPermission
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int PermissionId { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
