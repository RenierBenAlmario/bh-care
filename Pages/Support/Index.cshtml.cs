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

namespace Barangay.Pages.Support
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

        public class FAQ
        {
            public int Id { get; set; }
            public string Question { get; set; }
            public string Answer { get; set; }
            public string Category { get; set; }
        }

        public class SupportTicket
        {
            public string Id { get; set; }
            public string Subject { get; set; }
            public DateTime Date { get; set; }
            public string Status { get; set; }
            public string Description { get; set; }
        }

        public List<FAQ> FAQs { get; set; } = new List<FAQ>();
        public List<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Sample FAQ data
            FAQs = new List<FAQ>
            {
                new FAQ
                {
                    Id = 1,
                    Question = "How do I book an appointment?",
                    Answer = "You can book an appointment by clicking the 'Book an Appointment' button on your dashboard or by going to the Appointments section and clicking 'New Appointment'. Follow the prompts to select a doctor, date, and time that works for you.",
                    Category = "Appointments"
                },
                new FAQ
                {
                    Id = 2,
                    Question = "How can I view my medical records?",
                    Answer = "Your medical records can be accessed in the 'Medical Records' section of your dashboard. You can view your visit history, lab results, and other health information. You can also download or print these records for your personal use.",
                    Category = "Records"
                },
                new FAQ
                {
                    Id = 3,
                    Question = "Can I request prescription refills online?",
                    Answer = "Yes, you can request prescription refills through the 'Prescriptions' section. Find the prescription you need refilled, click the refill button (sync icon), and submit your request. The pharmacy will be notified and you'll receive a confirmation when your refill is ready.",
                    Category = "Prescriptions"
                },
                new FAQ
                {
                    Id = 4,
                    Question = "How do I update my personal information?",
                    Answer = "You can update your personal information in the 'Settings' section. Click on 'Profile' and then 'Edit' to modify your contact details, address, and other personal information. Remember to save your changes before exiting.",
                    Category = "Account"
                },
                new FAQ
                {
                    Id = 5,
                    Question = "What should I do if I need to cancel an appointment?",
                    Answer = "To cancel an appointment, go to the 'Appointments' section, find the appointment you wish to cancel, and click the cancel button (X icon). Please try to cancel at least 24 hours in advance so that other patients can use that time slot.",
                    Category = "Appointments"
                }
            };

            // Sample support tickets
            SupportTickets = new List<SupportTicket>
            {
                new SupportTicket
                {
                    Id = "TICKET-001",
                    Subject = "Cannot access medical records",
                    Date = DateTime.Now.AddDays(-5),
                    Status = "In Progress",
                    Description = "I'm trying to view my lab results from last month but I'm getting an error message."
                },
                new SupportTicket
                {
                    Id = "TICKET-002",
                    Subject = "Question about prescription renewal",
                    Date = DateTime.Now.AddDays(-15),
                    Status = "Closed",
                    Description = "I need help understanding how to renew my prescription online."
                }
            };

            return Page();
        }
    }
} 