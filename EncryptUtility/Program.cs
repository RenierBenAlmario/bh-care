using System;
using System.Security.Cryptography;
using System.Text;

namespace EncryptUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            string key = "BHCARE_DataEncryption_Key_2024_Secure_32Chars";
            
            // Encrypt "dsadsa" (the child name that needs encryption)
            string encryptedChildName = EncryptString("dsadsa", key);
            Console.WriteLine($"Encrypted ChildName: {encryptedChildName}");
            
            // Encrypt other common values
            string encryptedDate1 = EncryptString("2025-01-21 00:00:00.0000000", key);
            string encryptedDate2 = EncryptString("2025-09-18", key);
            string encryptedEmail1 = EncryptString("renierbenalma@gmail.com", key);
            string encryptedEmail2 = EncryptString("jelayquilatan@gmail.com", key);
            string encryptedPlaceOfBirth = EncryptString("dasd", key);
            string encryptedAddress1 = EncryptString("sasdasd", key);
            string encryptedAddress2 = EncryptString("sdasd", key);
            string encryptedAddress3 = EncryptString("298 asdkjad", key);
            string encryptedMotherName = EncryptString("dasd", key);
            string encryptedFatherName = EncryptString("dasd", key);
            string encryptedSex = EncryptString("Male", key);
            string encryptedHeight = EncryptString("30", key);
            string encryptedWeight = EncryptString("32", key);
            string encryptedHealthCenter = EncryptString("Baesa Health Center", key);
            string encryptedBarangay = EncryptString("161", key);
            string encryptedContactNumber = EncryptString("09913933498", key);
            
            Console.WriteLine($"Encrypted Date1: {encryptedDate1}");
            Console.WriteLine($"Encrypted Date2: {encryptedDate2}");
            Console.WriteLine($"Encrypted Email1: {encryptedEmail1}");
            Console.WriteLine($"Encrypted Email2: {encryptedEmail2}");
            Console.WriteLine($"Encrypted PlaceOfBirth: {encryptedPlaceOfBirth}");
            Console.WriteLine($"Encrypted Address1: {encryptedAddress1}");
            Console.WriteLine($"Encrypted Address2: {encryptedAddress2}");
            Console.WriteLine($"Encrypted Address3: {encryptedAddress3}");
            Console.WriteLine($"Encrypted MotherName: {encryptedMotherName}");
            Console.WriteLine($"Encrypted FatherName: {encryptedFatherName}");
            Console.WriteLine($"Encrypted Sex: {encryptedSex}");
            Console.WriteLine($"Encrypted Height: {encryptedHeight}");
            Console.WriteLine($"Encrypted Weight: {encryptedWeight}");
            Console.WriteLine($"Encrypted HealthCenter: {encryptedHealthCenter}");
            Console.WriteLine($"Encrypted Barangay: {encryptedBarangay}");
            Console.WriteLine($"Encrypted ContactNumber: {encryptedContactNumber}");
        }
        
        static string EncryptString(string plainText, string key)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;
                
            using (var aes = Aes.Create())
            {
                // Ensure key is exactly 32 bytes for AES-256
                var keyBytes = Encoding.UTF8.GetBytes(key);
                if (keyBytes.Length != 32)
                {
                    Array.Resize(ref keyBytes, 32);
                }
                aes.Key = keyBytes;
                aes.GenerateIV();
                
                using (var encryptor = aes.CreateEncryptor())
                {
                    var plainBytes = Encoding.UTF8.GetBytes(plainText);
                    var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    
                    var result = new byte[aes.IV.Length + encryptedBytes.Length];
                    Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
                    Buffer.BlockCopy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);
                    
                    return Convert.ToBase64String(result);
                }
            }
        }
    }
}