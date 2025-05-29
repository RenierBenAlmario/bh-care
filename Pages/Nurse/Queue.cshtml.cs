using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Barangay.Data;
using Barangay.Models;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse")]
    public class QueueModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<QueueModel> _logger;

        public QueueModel(ApplicationDbContext context, ILogger<QueueModel> logger)
        {
            _context = context;
            _logger = logger;
            QueueItems = new List<QueueItemViewModel>();
        }

        public class QueueItemViewModel
        {
            public int Id { get; set; }
            public string PatientId { get; set; } = string.Empty;
            public string PatientName { get; set; } = string.Empty;
            public TimeSpan? AppointmentTime { get; set; }
            public string DoctorName { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public int QueueNumber { get; set; }
            public string WaitingTime { get; set; } = string.Empty;
        }

        public List<QueueItemViewModel> QueueItems { get; set; }
        public int WaitingCount { get; set; }
        public int InProgressCount { get; set; }
        public int CompletedCount { get; set; }
        public int TotalCount { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }
    }
} 