using System;

namespace Barangay.Attributes
{
    /// <summary>
    /// Attribute to mark properties that should be encrypted/decrypted automatically
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EncryptedAttribute : Attribute
    {
        public EncryptedAttribute()
        {
        }
    }
}
