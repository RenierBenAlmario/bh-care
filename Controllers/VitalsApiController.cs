using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Data;
using Barangay.Services;
using Barangay.Extensions;

namespace Barangay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VitalsApiController : ControllerBase
    {
        private readonly EncryptedDbContext _context;
        private readonly ILogger<VitalsApiController> _logger;
        private readonly IDataEncryptionService _encryptionService;

        public VitalsApiController(EncryptedDbContext context, ILogger<VitalsApiController> logger, IDataEncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
            _encryptionService = encryptionService;
        }

        // POST: api/Vitals
        [HttpPost]
        public async Task<ActionResult<object>> PostVitalSigns([FromBody] VitalSignsRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.PatientId))
                {
                    return BadRequest("PatientId is required");
                }

                // Validate required fields and convert numeric values to strings
                if (string.IsNullOrEmpty(request.Temperature?.ToString()) || 
                    string.IsNullOrEmpty(request.RespiratoryRate?.ToString()) || 
                    string.IsNullOrEmpty(request.SpO2?.ToString()) || 
                    string.IsNullOrEmpty(request.Weight?.ToString()) || 
                    string.IsNullOrEmpty(request.Height?.ToString()))
                {
                    return BadRequest("All vital sign measurements are required");
                }

                // Create new VitalSign entity with string conversion
                var vitalSign = new VitalSign
                {
                    PatientId = request.PatientId,
                    Temperature = request.Temperature?.ToString(),
                    BloodPressure = request.BloodPressure,
                    HeartRate = request.HeartRate?.ToString(),
                    RespiratoryRate = request.RespiratoryRate?.ToString(),
                    SpO2 = request.SpO2?.ToString(),
                    Weight = request.Weight?.ToString(),
                    Height = request.Height?.ToString(),
                    RecordedAt = DateTime.Now,
                    Notes = request.Notes
                };

                // Add to context - EncryptedDbContext will handle encryption automatically
                _context.VitalSigns.Add(vitalSign);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully saved vital signs for patient {request.PatientId}");

                return Ok(new { success = true, message = "Vital signs recorded successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving vital signs");
                return StatusCode(500, new { success = false, message = $"Error saving vital signs: {ex.Message}" });
            }
        }

        // GET: api/Vitals/patient/{patientId}
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPatientVitalSigns(string patientId)
        {
            try
            {
                if (string.IsNullOrEmpty(patientId))
                {
                    return BadRequest("Patient ID is required");
                }

                var vitalSigns = await _context.VitalSigns
                    .Where(v => v.PatientId == patientId)
                    .OrderByDescending(v => v.RecordedAt)
                    .Take(10) // Limit to latest 10 records
                    .ToListAsync();

                // EncryptedDbContext will handle decryption automatically when loading

                var result = vitalSigns.Select(v => new
                {
                    Id = v.Id,
                    RecordedAt = v.RecordedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    BloodPressure = v.BloodPressure,
                    HeartRate = v.HeartRate,
                    Temperature = v.Temperature,
                    RespiratoryRate = v.RespiratoryRate,
                    SpO2 = v.SpO2,
                    Weight = v.Weight,
                    Height = v.Height,
                    Notes = v.Notes
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching vital signs for patient {PatientId}", patientId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }

    public class VitalSignsRequest
    {
        public string PatientId { get; set; } = string.Empty;
        public int? AppointmentId { get; set; }
        public string? BloodPressure { get; set; }
        public object? HeartRate { get; set; } // Can be string or numeric
        public object? Temperature { get; set; } // Can be string or numeric
        public object? RespiratoryRate { get; set; } // Can be string or numeric
        public object? SpO2 { get; set; } // Can be string or numeric
        public object? Weight { get; set; } // Can be string or numeric
        public object? Height { get; set; } // Can be string or numeric
        public string? Notes { get; set; }
    }
} 