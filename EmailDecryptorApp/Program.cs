using System;
using System.Text;
using System.Security.Cryptography;

namespace EmailDecryptorApp
{
    class Program
    {
        private static readonly string EncryptionKey = "BHCARE_DataEncryption_Key_2024_Secure_32Chars";

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            try
            {
                byte[] fullCipher = Convert.FromBase64String(cipherText);

                if (fullCipher.Length < 16) // IV size is 16 bytes
                {
                    Console.WriteLine($"Cipher too short: {fullCipher.Length} bytes");
                    return cipherText;
                }

                using (var aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);

                    // Extract IV from the beginning of the encrypted data
                    var iv = new byte[16];
                    Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                    aes.IV = iv;

                    // Extract encrypted data
                    var encryptedData = new byte[fullCipher.Length - iv.Length];
                    Buffer.BlockCopy(fullCipher, iv.Length, encryptedData, 0, encryptedData.Length);

                    using (var decryptor = aes.CreateDecryptor())
                    {
                        var decryptedBytes = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Decryption error: {ex.Message}");
                return cipherText;
            }
        }

        static void Main(string[] args)
        {
            // Encrypted names from the approval email
            string encryptedFirstName = "UjzSs4VzAiu1VQbFx+jUmmLM0M6QV3gKclQAWu2zm+I=";
            string encryptedLastName = "Uodekdugb82+8JR1oJktrHAwFhKDnHiQK7FDv9WyxtE=";
            
            Console.WriteLine("Decrypting names from approval email:");
            Console.WriteLine("=====================================");
            
            Console.WriteLine("Encrypted First Name:");
            Console.WriteLine(encryptedFirstName);
            Console.WriteLine();
            
            string decryptedFirstName = Decrypt(encryptedFirstName);
            Console.WriteLine("Decrypted First Name:");
            Console.WriteLine(decryptedFirstName);
            Console.WriteLine();
            
            Console.WriteLine("Encrypted Last Name:");
            Console.WriteLine(encryptedLastName);
            Console.WriteLine();
            
            string decryptedLastName = Decrypt(encryptedLastName);
            Console.WriteLine("Decrypted Last Name:");
            Console.WriteLine(decryptedLastName);
            Console.WriteLine();
            
            Console.WriteLine("Full Name:");
            Console.WriteLine($"{decryptedFirstName} {decryptedLastName}");
        }
    }
}