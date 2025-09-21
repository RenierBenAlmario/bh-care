using System;
using System.Security.Cryptography;
using System.Text;

class VitalSignsDecryptor
{
    private static readonly string _encryptionKey = "BHCARE_DataEncryption_Key_2024_Secure_32Chars";

    static void Main()
    {
        Console.WriteLine("=== Vital Signs Decryptor ===\n");

        // The encrypted vital signs from the consultation interface
        var encryptedVitalSigns = new[]
        {
            ("Blood Pressure", "eJ40XPbpYtmMxkwQpbPb31OwKHLxGKx2I+JrQpVWUdk="),
            ("Temperature", "LfUSUfoAmvwtsCkee2AGwzRo7oAhJCBJErlajqldgsw="),
            ("Heart Rate", "uyayL1QUmdbeMHnTWPGCVxEc0AoDzAdYpbMFUrZetko="),
            ("SpO2", "ufoulqxU5fEp9sUtx4Hva9o0iMWQZffGxlvWEquDkKk="),
            ("Weight", "5K9Tcuitrd49KRvZ4UU1nUTHH2wDk1c2LwGWd0+l1iU="),
            ("Height", "UmpUfl7p/gxcwmhq0agujzc2llcNqpXaHVxgAbhETM4=")
        };

        foreach (var (name, encryptedValue) in encryptedVitalSigns)
        {
            try
            {
                string decryptedValue = Decrypt(encryptedValue);
                Console.WriteLine($"{name}: {decryptedValue}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{name}: [DECRYPTION ERROR: {ex.Message}]");
            }
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    static string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        try
        {
            var encryptedBytes = Convert.FromBase64String(cipherText);

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);

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
        catch (Exception ex)
        {
            throw new Exception($"Decryption failed: {ex.Message}");
        }
    }
}
