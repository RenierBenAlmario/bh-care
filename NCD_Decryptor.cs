using System;
using System.Security.Cryptography;
using System.Text;

namespace Barangay.Tools
{
    public class NCDDecryptor
    {
        public static string DecryptNCDValue(string encryptedValue)
        {
            if (string.IsNullOrEmpty(encryptedValue))
                return encryptedValue;

            try
            {
                // Try multiple possible encryption keys that might be configured in the system
                string[] possibleKeys = {
                    "YourStrongEncryptionKeyHere1234567890123456", // Legacy key
                    Environment.GetEnvironmentVariable("BHCARE_ENCRYPTION_KEY") ?? "",
                    Environment.GetEnvironmentVariable("DataEncryption:Key") ?? "",
                    Environment.GetEnvironmentVariable("LEGACY_ENCRYPTION_KEY") ?? ""
                };

                foreach (var key in possibleKeys)
                {
                    if (!string.IsNullOrEmpty(key))
                    {
                        var result = TryDecryptWithKey(encryptedValue, key);
                        if (!string.IsNullOrEmpty(result) && result != encryptedValue && result != "[ACCESS DENIED]")
                        {
                            Console.WriteLine($"Successfully decrypted with key pattern: {key.Substring(0, Math.Min(20, key.Length))}...");
                            return result;
                        }
                    }
                }

                Console.WriteLine("All decryption attempts failed");
                return encryptedValue; // Return original if decryption fails
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Decryption error: {ex.Message}");
                return encryptedValue;
            }
        }

        private static string TryDecryptWithKey(string cipherText, string key)
        {
            try
            {
                // Check if it's valid Base64 and long enough for AES encryption
                var encryptedBytes = Convert.FromBase64String(cipherText);
                if (encryptedBytes.Length < 16)
                    return null;

                // Normalize key length to 32 bytes for AES
                string normalizedKey = key;
                if (normalizedKey.Length < 32)
                    normalizedKey = normalizedKey.PadRight(32, '0');
                else if (normalizedKey.Length > 32)
                    normalizedKey = normalizedKey.Substring(0, 32);

                using (var aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(normalizedKey);

                    // Extract IV from the beginning of the encrypted data
                    var iv = new byte[16];
                    Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);
                    aes.IV = iv;

                    // Extract encrypted data
                    var encryptedData = new byte[encryptedBytes.Length - iv.Length];
                    Buffer.BlockCopy(encryptedBytes, iv.Length, encryptedData, 0, encryptedData.Length);

                    using (var decryptor = aes.CreateDecryptor())
                    {
                        var decryptedBytes = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
            catch
            {
                return null; // This key failed
            }
        }
    }
}
