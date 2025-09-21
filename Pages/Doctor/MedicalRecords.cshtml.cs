using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Barangay.Data;
using Barangay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Barangay.Services;
using Barangay.Extensions;

namespace Barangay.Pages.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class MedicalRecordsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MedicalRecordsModel> _logger;
        private readonly IDataEncryptionService _encryptionService;

        public MedicalRecordsModel(ApplicationDbContext context, ILogger<MedicalRecordsModel> logger, IDataEncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
            _encryptionService = encryptionService;
        }

        public class MedicalRecordViewModel
        {
            public int Id { get; set; }
            public string PatientName { get; set; }
            public DateTime RecordDate { get; set; }
            public string RecordType { get; set; }
            public string Diagnosis { get; set; }
        }

        public List<MedicalRecordViewModel> MedicalRecords { get; set; } = new List<MedicalRecordViewModel>();

        public async Task OnGetAsync()
        {
            try
            {
                // Use a safer approach than raw SQL when dealing with prescription data
                var medicalRecords = await _context.MedicalRecords
                    .Include(m => m.Patient)
                    .ThenInclude(p => p.User)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(100)
                    .ToListAsync();

                // Decrypt medical records before creating view model
                foreach (var record in medicalRecords)
                {
                    record.DecryptSensitiveData(_encryptionService, User);
                }

                MedicalRecords = medicalRecords.Select(m => new MedicalRecordViewModel
                {
                    Id = m.Id,
                    PatientName = m.Patient?.User?.FullName ?? "Unknown Patient",
                    RecordDate = m.RecordDate,
                    RecordType = m.Type ?? "General", // Use Type property instead of RecordType
                    Diagnosis = m.Diagnosis ?? "No diagnosis recorded"
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medical records");
                MedicalRecords = new List<MedicalRecordViewModel>();
                
                // Add a message to TempData to show to the user
                TempData["ErrorMessage"] = "There was an error retrieving medical records. Please try again later.";
            }
        }
    }
} 