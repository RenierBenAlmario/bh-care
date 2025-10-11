using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class StaffPosition
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
