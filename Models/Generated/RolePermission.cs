using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class RolePermission
{
    public int Id { get; set; }

    public string RoleId { get; set; } = null!;

    public int PermissionId { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual AspNetRole Role { get; set; } = null!;
}
