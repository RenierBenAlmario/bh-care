using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Barangay.Services;
using System.Linq;
using System.Collections.Generic;

namespace Barangay.Middleware
{
    public class DataEncryptionMiddleware
    {
        private readonly RequestDelegate _next;

        public DataEncryptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IDataEncryptionService encryptionService)
        {
            // Skip encryption middleware for certain paths
            if (ShouldSkipEncryption(context.Request.Path))
            {
                await _next(context);
                return;
            }

            // Store original response body stream
            var originalResponseBodyStream = context.Response.Body;

            try
            {
                // Create a new memory stream for the response
                using var responseBodyStream = new MemoryStream();
                context.Response.Body = responseBodyStream;

                // Continue to the next middleware
                await _next(context);

                // After the response is generated, decrypt sensitive data for authorized users
                if (context.Response.StatusCode == 200 && encryptionService.CanUserDecrypt(context.User))
                {
                    var responseBody = await GetResponseBody(responseBodyStream);
                    
                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        // Decrypt sensitive data in the response
                        var decryptedResponse = DecryptResponseData(responseBody, context.User, encryptionService);
                        
                        // Write the decrypted response back to the original stream
                        var decryptedBytes = Encoding.UTF8.GetBytes(decryptedResponse);
                        await originalResponseBodyStream.WriteAsync(decryptedBytes, 0, decryptedBytes.Length);
                    }
                }
                else
                {
                    // Copy the response to the original stream
                    responseBodyStream.Seek(0, SeekOrigin.Begin);
                    await responseBodyStream.CopyToAsync(originalResponseBodyStream);
                }
            }
            finally
            {
                // Restore the original response body stream
                context.Response.Body = originalResponseBodyStream;
            }
        }

        private bool ShouldSkipEncryption(PathString path)
        {
            var skipPaths = new[]
            {
                "/Account/Login",
                "/Account/Register",
                "/Account/Logout",
                "/css/",
                "/js/",
                "/lib/",
                "/images/",
                "/favicon.ico"
            };

            return skipPaths.Any(skipPath => path.StartsWithSegments(skipPath));
        }

        private async Task<string> GetResponseBody(MemoryStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            return await reader.ReadToEndAsync();
        }

        private string DecryptResponseData(string responseBody, System.Security.Claims.ClaimsPrincipal user, IDataEncryptionService encryptionService)
        {
            try
            {
                // Parse JSON response
                using var document = JsonDocument.Parse(responseBody);
                var root = document.RootElement;

                // Recursively decrypt sensitive fields
                var decryptedJson = DecryptJsonElement(root, user, encryptionService);

                return JsonSerializer.Serialize(decryptedJson, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
            catch
            {
                // If JSON parsing fails, return original response
                return responseBody;
            }
        }

        private JsonElement DecryptJsonElement(JsonElement element, System.Security.Claims.ClaimsPrincipal user, IDataEncryptionService encryptionService)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    var obj = new Dictionary<string, object>();
                    foreach (var property in element.EnumerateObject())
                    {
                        var key = property.Name;
                        var value = DecryptJsonElement(property.Value, user, encryptionService);
                        
                        // Check if this is a sensitive field that should be decrypted
                        if (IsSensitiveField(key))
                        {
                            if (value.ValueKind == JsonValueKind.String)
                            {
                                var decryptedValue = encryptionService.DecryptForUser(value.GetString() ?? "", user);
                                obj[key] = decryptedValue;
                            }
                            else
                            {
                                obj[key] = value;
                            }
                        }
                        else
                        {
                            obj[key] = value;
                        }
                    }
                    return JsonSerializer.SerializeToElement(obj);

                case JsonValueKind.Array:
                    var array = new List<object>();
                    foreach (var item in element.EnumerateArray())
                    {
                        array.Add(DecryptJsonElement(item, user, encryptionService));
                    }
                    return JsonSerializer.SerializeToElement(array);

                default:
                    return element;
            }
        }

        private bool IsSensitiveField(string fieldName)
        {
            var sensitiveFields = new[]
            {
                "FullName", "Name", "FirstName", "MiddleName", "LastName",
                "Address", "ContactNumber", "EmergencyContact", "EmergencyContactNumber",
                "Email", "Diagnosis", "Treatment", "Notes", "MedicalHistory",
                "CurrentMedications", "Allergies", "Prescription", "Instructions",
                "ChiefComplaint", "Medications", "PhilHealthId", "PatientName",
                "DependentFullName", "Description", "ReasonForVisit"
            };

            return sensitiveFields.Contains(fieldName, StringComparer.OrdinalIgnoreCase);
        }
    }
}
