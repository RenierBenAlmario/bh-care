using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Barangay.Services
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }

    public class EncryptionService : IEncryptionService
    {
        private readonly string _encryptionKey;

        public EncryptionService(IConfiguration configuration)
        {
            _encryptionKey = configuration["EncryptionKey"] ?? 
                throw new ArgumentNullException("Encryption key not found");
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            try
            {
                // Ensure the key is the correct size for the algorithm
                // For AES, key sizes are typically 128, 192, or 256 bits (16, 24, or 32 bytes)
                byte[] keyBytes = new byte[32]; // Using 256 bits (32 bytes)
                byte[] existingKeyBytes = Encoding.UTF8.GetBytes(_encryptionKey);
                
                // Copy the existing key bytes, padding or truncating as needed
                Array.Copy(existingKeyBytes, keyBytes, Math.Min(existingKeyBytes.Length, keyBytes.Length));
                
                // Continue with encryption using the properly sized key
                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = new byte[16]; // IV should be 16 bytes for AES
                    
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        memoryStream.Write(aes.IV, 0, aes.IV.Length);

                        using (ICryptoTransform encryptor = aes.CreateEncryptor())
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Encryption failed: {ex.Message}", ex);
            }
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            try
            {
                byte[] fullCipher;
                try
                {
                    fullCipher = Convert.FromBase64String(cipherText);
                }
                catch (FormatException)
                {
                    // Not a valid Base64 string, return the original text
                    Console.WriteLine($"Decrypt: Not a valid Base64 string: {cipherText.Substring(0, Math.Min(10, cipherText.Length))}...");
                    return cipherText;
                }

                if (fullCipher.Length < 16) // IV size is 16 bytes
                {
                    Console.WriteLine($"Decrypt: Cipher too short: {fullCipher.Length} bytes");
                    return cipherText; // Return original text if too short
                }

                // Try with the new key handling method first
                string result = TryDecryptWithKey(fullCipher, useStandardizedKey: true);
                
                // If decryption failed, try with the original key handling as fallback
                if (string.IsNullOrEmpty(result))
                {
                    Console.WriteLine("Decrypt: First method failed, trying fallback method");
                    result = TryDecryptWithKey(fullCipher, useStandardizedKey: false);
                }
                
                // If both methods failed, return the original text
                if (string.IsNullOrEmpty(result))
                {
                    Console.WriteLine("Decrypt: Both decryption methods failed");
                    return cipherText;
                }
                
                return result;
            }
            catch (Exception ex)
            {
                // Return the original text in case of any exception
                Console.WriteLine($"Decryption error: {ex.Message}");
                return cipherText;
            }
        }

        private string TryDecryptWithKey(byte[] fullCipher, bool useStandardizedKey)
        {
            try
            {
                byte[] iv = new byte[16];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);

                byte[] cipherBytes = new byte[fullCipher.Length - iv.Length];
                Array.Copy(fullCipher, iv.Length, cipherBytes, 0, cipherBytes.Length);

                using (Aes aes = Aes.Create())
                {
                    // Use either standardized key or original key based on parameter
                    if (useStandardizedKey)
                    {
                        byte[] keyBytes = new byte[32]; // Using 256 bits (32 bytes)
                        byte[] existingKeyBytes = Encoding.UTF8.GetBytes(_encryptionKey);
                        Array.Copy(existingKeyBytes, keyBytes, Math.Min(existingKeyBytes.Length, keyBytes.Length));
                        aes.Key = keyBytes;
                    }
                    else
                    {
                        aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                    }
                    
                    aes.IV = iv;

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (ICryptoTransform decryptor = aes.CreateDecryptor())
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            return Encoding.UTF8.GetString(memoryStream.ToArray());
                        }
                    }
                }
            }
            catch (CryptographicException)
            {
                return string.Empty; // Return empty string if decryption fails
            }
            catch (Exception)
            {
                return string.Empty; // Return empty string for any other exception
            }
        }
    }
}