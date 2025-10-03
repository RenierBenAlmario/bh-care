using System;
using System.Security.Cryptography;
using System.Text;

namespace NCDDecryptor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== NCD Assessment Data Decryptor ===");
            Console.WriteLine();

            // Your encrypted values from the NCD form
            string assessmentDateEncrypted = "cAdh29hGNTITgPcwjlYn3t9xsc/bh62dnk6Tfz4Qe6HgutHqCpzWE6x1Qg03AR5/";
            string idNoEncrypted = "PePxvYwe4qsP6OxMNaAMGCSkCiDCMfflmv5geXmuul0=";

            Console.WriteLine("Decrypting NCD Assessment Data:");
            Console.WriteLine("=================================");
            Console.WriteLine();

            // Decrypt Assessment Date
            Console.WriteLine("Assessment Date:");
            Console.WriteLine($"Encrypted: {assessmentDateEncrypted}");
            Console.WriteLine($"Decrypted: {Decrypt(assessmentDateEncrypted)}");
            Console.WriteLine();

            // Decrypt ID No
            Console.WriteLine("ID No:");
            Console.WriteLine($"Encrypted: {idNoEncrypted}");
            Console.WriteLine($"Decrypted: {Decrypt(idNoEncrypted)}");
            Console.WriteLine();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static string Decrypt(string encryptedValue)
        {
            if (string.IsNullOrEmpty(encryptedValue))
                return encryptedValue;

            try
            {
                // Common encryption keys used in healthcare applications
                string[] possibleKeys = {
                    "YourStrongEncryptionKeyHere1234567890123456", // Default/Legacy
                    "BHCARE_ENCRYPTION_KEY_FOR_PRODUCTION_SYSTEM", // Production
                    "BarangayHealthCareEncryptionSystem2024", // System-specific
                    "NCDAssessmentEncryptionKey2024ForPH" // NCD-specific
                };

                foreach (var key in possibleKeys)
                {
                    var result = TryDecryptWithKey(encryptedValue, key);
                    if (!string.IsNullOrEmpty(result) && result != encryptedValue && result.Length > 0)
                    {
                        return result;
                    }
                }

                return "[DECRYPTION FAILED]";
            }
            catch (Exception ex)
            {
                return $"[ERROR: {ex.Message}]";
            }
        }

        static string TryDecryptWithKey(string cipherText, string key)
        {
            try
            {
                // Check if it's valid Base64 format
                var encryptedBytes = Convert.FromBase64String(cipherText);
                if (encryptedBytes.Length < 16) // Too short for AES IV + data
                    return null;

                // Normalize key to 32 bytes for AES-256
                string normalizedKey = NormalizeKey(key);

                using (var aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(normalizedKey);

                    // Extract IV from the beginning of encrypted data
                    var iv = new byte[16];
                    Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);
                    aes.IV = iv;

                    // Extract encrypted data
                    var encryptedData = new byte[encryptedBytes.Length - iv.Length];
                    Buffer.BlockCopy(encryptedBytes, iv.Length, encryptedData, 0, encryptedData.Length);

                    using (var decryptor = aes.CreateDecryptor())
                    {
                        var decryptedBytes = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                        string result = Encoding.UTF8.GetString(decryptedBytes);
                        
                        // Validate the result looks reasonable (not just random characters)
                        if (IsValidText(result))
                        {
                            return result;
                        }
                    }
                }
            }
            catch
            {
                // This key didn't work, try next one
            }

            return null;
        }

        static string NormalizeKey(string key)
        {
            if (key.Length < 32)
                return key.PadRight(32, '0');
            else if (key.Length > 32)
                return key.Substring(0, 32);
            return key;
        }

        static bool IsValidText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            // Check if it contains printable characters and looks like readable data
            foreach (char c in text)
            {
                if (char.IsControl(c) && c != '\n' && c != '\r' && c != '\t')
                    return false;
            }

            return true;
        }
    }
}