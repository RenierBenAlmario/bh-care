using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string FullName { get; set; } = null!;
}
