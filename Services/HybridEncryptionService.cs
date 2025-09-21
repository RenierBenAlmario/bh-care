using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Barangay.Services
{
    public class HybridEncryptionService
    {
        private readonly ILogger<HybridEncryptionService> _logger;
        private const int RSA_KEY_SIZE = 2048;
        private const int AES_KEY_SIZE = 256; // 32 bytes for AES-256
        private const int AES_IV_SIZE = 12; // 12 bytes for GCM mode

        public HybridEncryptionService(ILogger<HybridEncryptionService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Generates a new RSA key pair
        /// </summary>
        /// <returns>RSA key pair with public and private keys</returns>
        public RSAKeyPair GenerateRSAKeyPair()
        {
            try
            {
                using (var rsa = RSA.Create(RSA_KEY_SIZE))
                {
                    var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
                    var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
                    
                    return new RSAKeyPair
                    {
                        PublicKey = publicKey,
                        PrivateKey = privateKey,
                        KeyId = Guid.NewGuid().ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating RSA key pair");
                throw;
            }
        }

        /// <summary>
        /// Encrypts data using hybrid encryption (AES + RSA)
        /// </summary>
        /// <param name="plaintext">Data to encrypt</param>
        /// <param name="publicKey">RSA public key (Base64 encoded)</param>
        /// <returns>Encrypted data structure</returns>
        public HybridEncryptedData Encrypt(string plaintext, string publicKey)
        {
            if (string.IsNullOrEmpty(plaintext))
                return new HybridEncryptedData { EncryptedData = "", EncryptedKey = "", IV = "" };

            try
            {
                // Generate random AES key and IV
                var aesKey = new byte[AES_KEY_SIZE / 8]; // 32 bytes
                var iv = new byte[AES_IV_SIZE]; // 12 bytes
                
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(aesKey);
                    rng.GetBytes(iv);
                }

                // Encrypt data with AES-GCM
                var encryptedData = EncryptWithAES(plaintext, aesKey, iv);
                
                // Encrypt AES key with RSA
                var encryptedKey = EncryptAESKeyWithRSA(aesKey, publicKey);

                return new HybridEncryptedData
                {
                    EncryptedData = Convert.ToBase64String(encryptedData),
                    EncryptedKey = Convert.ToBase64String(encryptedKey),
                    IV = Convert.ToBase64String(iv),
                    Algorithm = "AES-256-GCM+RSA-2048"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during hybrid encryption");
                throw;
            }
        }

        /// <summary>
        /// Decrypts data using hybrid decryption (RSA + AES)
        /// </summary>
        /// <param name="encryptedData">Encrypted data structure</param>
        /// <param name="privateKey">RSA private key (Base64 encoded)</param>
        /// <returns>Decrypted plaintext</returns>
        public string Decrypt(HybridEncryptedData encryptedData, string privateKey)
        {
            if (encryptedData == null || string.IsNullOrEmpty(encryptedData.EncryptedData))
                return "";

            try
            {
                // Decrypt AES key with RSA
                var aesKey = DecryptAESKeyWithRSA(Convert.FromBase64String(encryptedData.EncryptedKey), privateKey);
                
                // Decrypt data with AES-GCM
                var iv = Convert.FromBase64String(encryptedData.IV);
                var cipherData = Convert.FromBase64String(encryptedData.EncryptedData);
                
                return DecryptWithAES(cipherData, aesKey, iv);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during hybrid decryption");
                throw new CryptographicException("Decryption failed. Invalid key or corrupted data.", ex);
            }
        }

        /// <summary>
        /// Encrypts data with AES-256-GCM
        /// </summary>
        private byte[] EncryptWithAES(string plaintext, byte[] key, byte[] iv)
        {
            using (var aes = new AesGcm(key, 16)) // Specify tag size explicitly
            {
                var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
                var ciphertext = new byte[plaintextBytes.Length];
                var tag = new byte[16]; // GCM tag size

                aes.Encrypt(iv, plaintextBytes, ciphertext, tag);

                // Combine ciphertext and tag
                var result = new byte[ciphertext.Length + tag.Length];
                Array.Copy(ciphertext, 0, result, 0, ciphertext.Length);
                Array.Copy(tag, 0, result, ciphertext.Length, tag.Length);

                return result;
            }
        }

        /// <summary>
        /// Decrypts data with AES-256-GCM
        /// </summary>
        private string DecryptWithAES(byte[] cipherData, byte[] key, byte[] iv)
        {
            using (var aes = new AesGcm(key, 16)) // Specify tag size explicitly
            {
                // Split ciphertext and tag
                var ciphertextLength = cipherData.Length - 16; // Subtract tag size
                var ciphertext = new byte[ciphertextLength];
                var tag = new byte[16];

                Array.Copy(cipherData, 0, ciphertext, 0, ciphertextLength);
                Array.Copy(cipherData, ciphertextLength, tag, 0, 16);

                var plaintext = new byte[ciphertextLength];
                aes.Decrypt(iv, ciphertext, tag, plaintext);

                return Encoding.UTF8.GetString(plaintext);
            }
        }

        /// <summary>
        /// Encrypts AES key with RSA
        /// </summary>
        private byte[] EncryptAESKeyWithRSA(byte[] aesKey, string publicKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
                return rsa.Encrypt(aesKey, RSAEncryptionPadding.OaepSHA256);
            }
        }

        /// <summary>
        /// Decrypts AES key with RSA
        /// </summary>
        private byte[] DecryptAESKeyWithRSA(byte[] encryptedKey, string privateKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
                return rsa.Decrypt(encryptedKey, RSAEncryptionPadding.OaepSHA256);
            }
        }

        /// <summary>
        /// Validates if a string is a valid RSA private key
        /// </summary>
        public bool IsValidRSAPrivateKey(string privateKey)
        {
            try
            {
                if (string.IsNullOrEmpty(privateKey))
                    return false;

                using (var rsa = RSA.Create())
                {
                    rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates if a string is a valid RSA public key
        /// </summary>
        public bool IsValidRSAPublicKey(string publicKey)
        {
            try
            {
                if (string.IsNullOrEmpty(publicKey))
                    return false;

                using (var rsa = RSA.Create())
                {
                    rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// RSA key pair structure
    /// </summary>
    public class RSAKeyPair
    {
        public string PublicKey { get; set; } = "";
        public string PrivateKey { get; set; } = "";
        public string KeyId { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Hybrid encrypted data structure
    /// </summary>
    public class HybridEncryptedData
    {
        public string EncryptedData { get; set; } = "";
        public string EncryptedKey { get; set; } = "";
        public string IV { get; set; } = "";
        public string Algorithm { get; set; } = "";
        public string KeyId { get; set; } = "";
    }
}
