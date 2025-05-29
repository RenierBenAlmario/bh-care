using System;
using System.Collections.Generic;

namespace Barangay.TempModels;

public partial class Permission
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Category { get; set; } = null!;

    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}
