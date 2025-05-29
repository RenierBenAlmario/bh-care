using System;
using System.Collections.Generic;

namespace Barangay.Models
{
    public class UserData
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastActive { get; set; }
        public bool IsActive { get; set; }
        public string PhilHealthId { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Address { get; set; } = string.Empty;
        public List<UserDocument> Documents { get; set; } = new();
    }
} 