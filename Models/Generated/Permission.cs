using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class Permission
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Category { get; set; } = null!;

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    public virtual ICollection<StaffPermission> StaffPermissions { get; set; } = new List<StaffPermission>();

    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

    public virtual ICollection<StaffPosition> StaffPositions { get; set; } = new List<StaffPosition>();
}
