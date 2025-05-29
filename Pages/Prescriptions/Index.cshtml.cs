using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Barangay.Models;
using Barangay.Data;

namespace Barangay.Pages.Prescriptions
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public class Prescription
        {
            public string Id { get; set; }
            public DateTime Date { get; set; }
            public string Medication { get; set; }
            public string Dosage { get; set; }
            public string Instructions { get; set; }
            public string Doctor { get; set; }
            public int RefillsRemaining { get; set; }
            public DateTime ExpirationDate { get; set; }
        }

        public class MedicationTime
        {
            public string Time { get; set; }
            public int Count { get; set; }
            public List<string> Medications { get; set; } = new List<string>();
        }

        public List<Prescription> ActivePrescriptions { get; set; } = new List<Prescription>();
        public List<Prescription> PastPrescriptions { get; set; } = new List<Prescription>();
        public List<MedicationTime> MedicationSchedule { get; set; } = new List<MedicationTime>();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Sample data for active prescriptions
            ActivePrescriptions = new List<Prescription>
            {
                new Prescription
                {
                    Id = "1",
                    Date = DateTime.Now.AddDays(-15),
                    Medication = "Amoxicillin",
                    Dosage = "500mg",
                    Instructions = "Take 1 capsule 3 times daily with food",
                    Doctor = "Dr. Emily Johnson",
                    RefillsRemaining = 2,
                    ExpirationDate = DateTime.Now.AddDays(15)
                },
                new Prescription
                {
                    Id = "2",
                    Date = DateTime.Now.AddDays(-10),
                    Medication = "Ibuprofen",
                    Dosage = "400mg",
                    Instructions = "Take 1 tablet every 6 hours as needed for pain",
                    Doctor = "Dr. Mark Smith",
                    RefillsRemaining = 1,
                    ExpirationDate = DateTime.Now.AddDays(20)
                }
            };

            // Sample data for past prescriptions
            PastPrescriptions = new List<Prescription>
            {
                new Prescription
                {
                    Id = "3",
                    Date = DateTime.Now.AddDays(-60),
                    Medication = "Ciprofloxacin",
                    Dosage = "250mg",
                    Instructions = "Take 1 tablet twice daily",
                    Doctor = "Dr. Emily Johnson",
                    RefillsRemaining = 0,
                    ExpirationDate = DateTime.Now.AddDays(-30)
                },
                new Prescription
                {
                    Id = "4",
                    Date = DateTime.Now.AddDays(-90),
                    Medication = "Loratadine",
                    Dosage = "10mg",
                    Instructions = "Take 1 tablet daily",
                    Doctor = "Dr. Ben Jhone",
                    RefillsRemaining = 0,
                    ExpirationDate = DateTime.Now.AddDays(-60)
                }
            };

            // Sample data for medication schedule
            MedicationSchedule = new List<MedicationTime>
            {
                new MedicationTime
                {
                    Time = "Morning (8:00 AM)",
                    Count = 2,
                    Medications = new List<string> { "Amoxicillin 500mg", "Vitamin D 1000 IU" }
                },
                new MedicationTime
                {
                    Time = "Afternoon (2:00 PM)",
                    Count = 1,
                    Medications = new List<string> { "Amoxicillin 500mg" }
                },
                new MedicationTime
                {
                    Time = "Evening (8:00 PM)",
                    Count = 2,
                    Medications = new List<string> { "Amoxicillin 500mg", "Ibuprofen 400mg (as needed)" }
                }
            };

            return Page();
        }
    }
} 