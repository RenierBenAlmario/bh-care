using System;
using System.Reflection;
using System.Security.Claims;
using Barangay.Attributes;
using Barangay.Services;
using System.Linq;

namespace Barangay.Extensions
{
    public static class EncryptionExtensions
    {
        /// <summary>
        /// Encrypts all properties marked with [Encrypted] attribute on an object
        /// </summary>
        public static T EncryptSensitiveData<T>(this T obj, IDataEncryptionService encryptionService) where T : class
        {
            if (obj == null || encryptionService == null)
                return obj;

            Console.WriteLine($"EncryptSensitiveData called for {obj.GetType().Name}");
            Console.WriteLine($"Total properties found: {typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Length}");

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var encryptedProperties = properties.Where(p => p.GetCustomAttribute<EncryptedAttribute>() != null).ToList();
            Console.WriteLine($"Properties with [Encrypted] attribute: {encryptedProperties.Count}");
            
            foreach (var property in encryptedProperties)
            {
                Console.WriteLine($"Processing encrypted property: {property.Name} of type {property.PropertyType}");
                if (property.CanWrite)
                {
                    var value = property.GetValue(obj);
                    if (value != null)
                    {
                        var stringValue = value.ToString();
                        if (!string.IsNullOrEmpty(stringValue))
                        {
                            Console.WriteLine($"Encrypting {property.Name}: {stringValue}");
                            var encryptedValue = encryptionService.Encrypt(stringValue);
                            Console.WriteLine($"Encrypted {property.Name}: {encryptedValue}");
                            
                            // Handle different property types
                            if (property.PropertyType == typeof(string))
                            {
                                property.SetValue(obj, encryptedValue);
                            }
                            else if (property.PropertyType == typeof(int?) || property.PropertyType == typeof(int))
                            {
                                // For numeric types, we need to store the encrypted string
                                // This requires changing the database schema to use string columns for encrypted numeric values
                                // For now, we'll store the encrypted value as a string representation
                                property.SetValue(obj, encryptedValue);
                            }
                            else if (property.PropertyType == typeof(decimal?) || property.PropertyType == typeof(decimal))
                            {
                                // For decimal types, store encrypted value as string
                                property.SetValue(obj, encryptedValue);
                            }
                            else
                            {
                                // For other types, try to convert back to original type
                                try
                                {
                                    var convertedValue = Convert.ChangeType(encryptedValue, property.PropertyType);
                                    property.SetValue(obj, convertedValue);
                                }
                                catch
                                {
                                    // If conversion fails, store as string
                                    property.SetValue(obj, encryptedValue);
                                }
                            }
                        }
                    }
                }
            }

            return obj;
        }

        /// <summary>
        /// Decrypts all properties marked with [Encrypted] attribute on an object for authorized users
        /// </summary>
        public static T DecryptSensitiveData<T>(this T obj, IDataEncryptionService encryptionService, ClaimsPrincipal user) where T : class
        {
            if (obj == null || encryptionService == null || !encryptionService.CanUserDecrypt(user))
                return obj;

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var property in properties)
            {
                if (property.GetCustomAttribute<EncryptedAttribute>() != null && property.CanWrite)
                {
                    var value = property.GetValue(obj)?.ToString();
                    if (!string.IsNullOrEmpty(value) && encryptionService.IsEncrypted(value))
                    {
                        var decryptedValue = encryptionService.DecryptForUser(value, user);
                        property.SetValue(obj, decryptedValue);
                    }
                }
            }

            return obj;
        }

        /// <summary>
        /// Decrypts a single encrypted string for authorized users
        /// </summary>
        public static string DecryptForUser(this string encryptedText, IDataEncryptionService encryptionService, ClaimsPrincipal user)
        {
            if (string.IsNullOrEmpty(encryptedText) || encryptionService == null)
                return encryptedText;

            if (!encryptionService.CanUserDecrypt(user))
                return "[ACCESS DENIED]";

            return encryptionService.DecryptForUser(encryptedText, user);
        }

        /// <summary>
        /// Encrypts a single string
        /// </summary>
        public static string EncryptText(this string plainText, IDataEncryptionService encryptionService)
        {
            if (string.IsNullOrEmpty(plainText) || encryptionService == null)
                return plainText;

            return encryptionService.Encrypt(plainText);
        }

        /// <summary>
        /// Encrypts VitalSign data using dual-column approach
        /// </summary>
        public static Models.VitalSign EncryptVitalSignData(this Models.VitalSign vitalSign, IDataEncryptionService encryptionService)
        {
            if (vitalSign == null || encryptionService == null)
            {
                Console.WriteLine("EncryptVitalSignData: vitalSign or encryptionService is null");
                return vitalSign;
            }

            Console.WriteLine($"EncryptVitalSignData called - Temperature: {vitalSign.Temperature}, BloodPressure: {vitalSign.BloodPressure}");

            // Encrypt Temperature and store in regular field
            if (!string.IsNullOrEmpty(vitalSign.Temperature))
            {
                var encryptedTemp = encryptionService.Encrypt(vitalSign.Temperature);
                vitalSign.Temperature = encryptedTemp; // Store encrypted data in regular field
                vitalSign.EncryptedTemperature = encryptedTemp;
                Console.WriteLine($"Encrypted Temperature: {vitalSign.Temperature} -> {encryptedTemp}");
            }

            // Encrypt BloodPressure and store in regular field
            if (!string.IsNullOrEmpty(vitalSign.BloodPressure))
            {
                var encryptedBP = encryptionService.Encrypt(vitalSign.BloodPressure);
                vitalSign.BloodPressure = encryptedBP; // Store encrypted data in regular field
                vitalSign.EncryptedBloodPressure = encryptedBP;
            }

            // Encrypt HeartRate and store in regular field
            if (!string.IsNullOrEmpty(vitalSign.HeartRate))
            {
                var encryptedHR = encryptionService.Encrypt(vitalSign.HeartRate);
                vitalSign.HeartRate = encryptedHR; // Store encrypted data in regular field
                vitalSign.EncryptedHeartRate = encryptedHR;
            }

            // Encrypt RespiratoryRate and store in regular field
            if (!string.IsNullOrEmpty(vitalSign.RespiratoryRate))
            {
                var encryptedRR = encryptionService.Encrypt(vitalSign.RespiratoryRate);
                vitalSign.RespiratoryRate = encryptedRR; // Store encrypted data in regular field
                vitalSign.EncryptedRespiratoryRate = encryptedRR;
            }

            // Encrypt SpO2 and store in regular field
            if (!string.IsNullOrEmpty(vitalSign.SpO2))
            {
                var encryptedSpO2 = encryptionService.Encrypt(vitalSign.SpO2);
                vitalSign.SpO2 = encryptedSpO2; // Store encrypted data in regular field
                vitalSign.EncryptedSpO2 = encryptedSpO2;
            }

            // Encrypt Weight and store in regular field
            if (!string.IsNullOrEmpty(vitalSign.Weight))
            {
                var encryptedWeight = encryptionService.Encrypt(vitalSign.Weight);
                vitalSign.Weight = encryptedWeight; // Store encrypted data in regular field
                vitalSign.EncryptedWeight = encryptedWeight;
            }

            // Encrypt Height and store in regular field
            if (!string.IsNullOrEmpty(vitalSign.Height))
            {
                var encryptedHeight = encryptionService.Encrypt(vitalSign.Height);
                vitalSign.Height = encryptedHeight; // Store encrypted data in regular field
                vitalSign.EncryptedHeight = encryptedHeight;
            }

            // Encrypt Notes (already has [Encrypted] attribute, but let's handle it explicitly)
            if (!string.IsNullOrEmpty(vitalSign.Notes))
            {
                vitalSign.Notes = encryptionService.Encrypt(vitalSign.Notes);
            }

            return vitalSign;
        }

        /// <summary>
        /// Decrypts VitalSign data using dual-column approach
        /// </summary>
        public static Models.VitalSign DecryptVitalSignData(this Models.VitalSign vitalSign, IDataEncryptionService encryptionService, ClaimsPrincipal user)
        {
            if (vitalSign == null || encryptionService == null || !encryptionService.CanUserDecrypt(user))
                return vitalSign;

            // Decrypt Temperature from regular field
            if (!string.IsNullOrEmpty(vitalSign.Temperature))
            {
                vitalSign.Temperature = encryptionService.DecryptForUser(vitalSign.Temperature, user);
            }

            // Decrypt BloodPressure from regular field
            if (!string.IsNullOrEmpty(vitalSign.BloodPressure))
            {
                vitalSign.BloodPressure = encryptionService.DecryptForUser(vitalSign.BloodPressure, user);
            }

            // Decrypt HeartRate from regular field
            if (!string.IsNullOrEmpty(vitalSign.HeartRate))
            {
                vitalSign.HeartRate = encryptionService.DecryptForUser(vitalSign.HeartRate, user);
            }

            // Decrypt RespiratoryRate from regular field
            if (!string.IsNullOrEmpty(vitalSign.RespiratoryRate))
            {
                vitalSign.RespiratoryRate = encryptionService.DecryptForUser(vitalSign.RespiratoryRate, user);
            }

            // Decrypt SpO2 from regular field
            if (!string.IsNullOrEmpty(vitalSign.SpO2))
            {
                vitalSign.SpO2 = encryptionService.DecryptForUser(vitalSign.SpO2, user);
            }

            // Decrypt Weight from regular field
            if (!string.IsNullOrEmpty(vitalSign.Weight))
            {
                vitalSign.Weight = encryptionService.DecryptForUser(vitalSign.Weight, user);
            }

            // Decrypt Height from regular field
            if (!string.IsNullOrEmpty(vitalSign.Height))
            {
                vitalSign.Height = encryptionService.DecryptForUser(vitalSign.Height, user);
            }

            // Decrypt Notes
            if (!string.IsNullOrEmpty(vitalSign.Notes))
            {
                vitalSign.Notes = encryptionService.DecryptForUser(vitalSign.Notes, user);
            }

            return vitalSign;
        }
    }
}
