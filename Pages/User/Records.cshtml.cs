using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Microsoft.Extensions.Logging;
using Barangay.Extensions;
using Barangay.Services;
using System.Linq;

namespace Barangay.Pages.User
{
    [Authorize]
    public class RecordsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RecordsModel> _logger;
        private readonly IDataEncryptionService _encryptionService;
        private readonly IEncryptionService _legacyEncryptionService;

        public RecordsModel(ApplicationDbContext context, ILogger<RecordsModel> logger, IDataEncryptionService encryptionService, IEncryptionService legacyEncryptionService)
        {
            _context = context;
            _logger = logger;
            _encryptionService = encryptionService;
            _legacyEncryptionService = legacyEncryptionService;
        }

        public class VitalSignRecord
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public double Temperature { get; set; }
            public string BloodPressure { get; set; }
            public int HeartRate { get; set; }
            public double? Weight { get; set; }
            public double? Height { get; set; }
        }

        public class MedicalHistoryRecord
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public string Doctor { get; set; }
            public string Diagnosis { get; set; }
            public string Treatment { get; set; }
        }

        public List<VitalSignRecord> VitalSigns { get; set; } = new List<VitalSignRecord>();
        public List<MedicalHistoryRecord> MedicalHistory { get; set; } = new List<MedicalHistoryRecord>();
        public List<Barangay.Models.LabResult> LabResults { get; set; } = new List<Barangay.Models.LabResult>();

        private string TryDecrypt(string encryptedValue)
        {
            if (string.IsNullOrEmpty(encryptedValue))
                return encryptedValue;

            // First try the main encryption service
            try
            {
                if (_encryptionService.IsEncrypted(encryptedValue))
                {
                    return _encryptionService.DecryptForUser(encryptedValue, User);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to decrypt with main service: {ex.Message}");
            }

            // Try with legacy encryption service
            try
            {
                return _legacyEncryptionService.Decrypt(encryptedValue);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to decrypt with legacy service: {ex.Message}");
                return encryptedValue; // Return original if both fail
            }
        }

        public async Task OnGetAsync()
        {
            try
            {
                var userId = User.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found");
                    return;
                }

                // Check if user can decrypt
                var canDecrypt = _encryptionService.CanUserDecrypt(User);
                _logger.LogInformation($"User {userId} can decrypt: {canDecrypt}");
                _logger.LogInformation($"User roles: {string.Join(", ", User.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role).Select(c => c.Value))}");

                // Test decryption with a sample string
                var testString = "JGtrXVoE41c7YBdC";
                var isEncrypted = _encryptionService.IsEncrypted(testString);
                _logger.LogInformation($"Test string '{testString}' is encrypted: {isEncrypted}");
                if (isEncrypted)
                {
                    try
                    {
                        var decrypted = _encryptionService.DecryptForUser(testString, User);
                        _logger.LogInformation($"Decrypted test string: {decrypted}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to decrypt test string: {ex.Message}");
                    }
                }
                else
                {
                    // Try with legacy encryption service
                    try
                    {
                        var decrypted = _legacyEncryptionService.Decrypt(testString);
                        _logger.LogInformation($"Decrypted test string with legacy service: {decrypted}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to decrypt test string with legacy service: {ex.Message}");
                    }
                }

                // Load vital signs from database
                var vitalSigns = await _context.VitalSigns
                    .Where(v => v.PatientId == userId)
                    .OrderByDescending(v => v.RecordedAt)
                    .ToListAsync();

                // Decrypt vital signs data before creating view model
                foreach (var vitalSign in vitalSigns)
                {
                    _logger.LogInformation($"Before decryption - Temperature: {vitalSign.Temperature}, BloodPressure: {vitalSign.BloodPressure}");
                    vitalSign.DecryptVitalSignData(_encryptionService, User);
                    _logger.LogInformation($"After decryption - Temperature: {vitalSign.Temperature}, BloodPressure: {vitalSign.BloodPressure}");
                }

                VitalSigns = vitalSigns.Select(v => new VitalSignRecord
                {
                    Id = v.Id,
                    Date = v.RecordedAt,
                    Temperature = !string.IsNullOrEmpty(v.Temperature) && double.TryParse(TryDecrypt(v.Temperature), out var temp) ? temp : 0,
                    BloodPressure = TryDecrypt(v.BloodPressure) ?? "N/A",
                    HeartRate = !string.IsNullOrEmpty(v.HeartRate) && int.TryParse(TryDecrypt(v.HeartRate), out var hr) ? hr : 0,
                    Weight = !string.IsNullOrEmpty(v.Weight) && double.TryParse(TryDecrypt(v.Weight), out var weight) ? weight : null,
                    Height = !string.IsNullOrEmpty(v.Height) && double.TryParse(TryDecrypt(v.Height), out var height) ? height : null
                }).ToList();

                // Load medical records from database
                var medicalRecords = await _context.MedicalRecords
                    .Where(m => m.PatientId == userId)
                    .Include(m => m.Doctor)
                    .OrderByDescending(m => m.Date)
                    .ToListAsync();

                // Decrypt medical records before creating view model
                foreach (var record in medicalRecords)
                {
                    _logger.LogInformation($"Before decryption - Diagnosis: {record.Diagnosis}, Treatment: {record.Treatment}");
                    record.DecryptSensitiveData(_encryptionService, User);
                    _logger.LogInformation($"After decryption - Diagnosis: {record.Diagnosis}, Treatment: {record.Treatment}");
                }

                MedicalHistory = medicalRecords.Select(m => new MedicalHistoryRecord
                {
                    Id = m.Id,
                    Date = m.Date,
                    Doctor = m.Doctor?.FullName ?? "Unknown",
                    Diagnosis = TryDecrypt(m.Diagnosis),
                    Treatment = TryDecrypt(m.Treatment)
                }).ToList();

                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading medical records");
            }
        }

        public async Task<IActionResult> OnPostDeleteMedicalRecordAsync(int id)
        {
            try
            {
                var userId = User.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found during medical record deletion");
                    return NotFound();
                }

                // Find the medical record and verify it belongs to the current user
                var medicalRecord = await _context.MedicalRecords
                    .FirstOrDefaultAsync(m => m.Id == id && m.PatientId == userId);

                if (medicalRecord == null)
                {
                    _logger.LogWarning("Medical record {Id} not found or doesn't belong to user {UserId}", id, userId);
                    return NotFound();
                }

                // Delete the medical record
                _context.MedicalRecords.Remove(medicalRecord);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Medical record {Id} deleted successfully for user {UserId}", id, userId);
                TempData["SuccessMessage"] = "Medical record deleted successfully.";
                
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting medical record {Id}", id);
                TempData["ErrorMessage"] = "Error deleting medical record. Please try again.";
                return RedirectToPage();
            }
        }
    }
} 