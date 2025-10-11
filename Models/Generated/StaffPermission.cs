using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class StaffPermission
{
    public int Id { get; set; }

    public int StaffMemberId { get; set; }

    public int PermissionId { get; set; }

    public DateTime GrantedAt { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual StaffMember StaffMember { get; set; } = null!;
}
