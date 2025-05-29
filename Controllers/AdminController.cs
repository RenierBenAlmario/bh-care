using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Barangay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using System.Diagnostics;
using System.IO;
using Barangay.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Barangay.Controllers
{
    [Authorize(Policy = "AccessAdminDashboard")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment webHostEnvironment,
            ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        [HttpPut]
        [Route("api/Admin/ApproveUser/{id}")]
        public async Task<IActionResult> ApproveUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            
            if (user == null)
            {
                return NotFound();
            }

            // Update user status
            user.EncryptedStatus = "Active";
            await _userManager.UpdateAsync(user);
            
            // Update patient status if exists
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == id);
                
            if (patient != null)
            {
                patient.Status = "Active";
                _context.Patients.Update(patient);
                await _context.SaveChangesAsync();
            }
            
            return Ok(new { message = "User approved successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> UserManagement()
        {
            try
            {
                // Get the current time in Philippine Standard Time (UTC+8)
                var philippineTime = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila")
                );

                // Define staff roles to exclude
                var staffRoles = new HashSet<string> {
                    "System Administrator",
                    "Admin Staff",
                    "Doctor",
                    "Nurse"
                };

                // Get all users excluding staff members with their documents
                var users = await _userManager.Users
                    .Where(u => u.IsActive)
                    .Include(u => u.UserDocuments)
                        .ThenInclude(d => d.Approver)
                    .Select(u => new UserData
                    {
                        Id = u.Id,
                        Email = u.Email,
                        Name = u.Name,
                        Status = u.Status,
                        CreatedAt = u.CreatedAt,
                        LastActive = u.LastActive,
                        IsActive = u.IsActive,
                        PhilHealthId = u.PhilHealthId,
                        Gender = u.Gender,
                        BirthDate = u.BirthDate,
                        Address = u.Address,
                        Documents = u.UserDocuments.OrderByDescending(d => d.UploadDate).ToList()
                    })
                    .ToListAsync();

                // Filter out users with staff roles
                var filteredUsers = new List<UserData>();
                foreach (var user in users)
                {
                    var userRoles = await _userManager.GetRolesAsync(
                        await _userManager.FindByIdAsync(user.Id)
                    );
                    
                    if (!userRoles.Any(role => staffRoles.Contains(role)))
                    {
                        filteredUsers.Add(user);
                    }
                }

                ViewBag.CurrentDateTime = philippineTime;
                return View(filteredUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user management data");
                TempData["ErrorMessage"] = "An error occurred while loading the user data.";
                return View(new List<UserData>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AdminDashboard()
        {
            // Get the current time in Philippine Standard Time (UTC+8)
            var philippineTime = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila")
            );

            // Mock staff data for demonstration
            var staffList = new List<StaffData>
            {
                new StaffData
                {
                    Id = "1",
                    Name = "Dr. Juan Dela Cruz",
                    Email = "juan.delacruz@example.com",
                    Role = "Doctor",
                    Department = "General Medicine",
                    Specialization = "Family Medicine",
                    ContactNumber = "+63 912 345 6789",
                    WorkingDays = "Monday-Friday",
                    WorkingHours = "8:00 AM - 5:00 PM",
                    JoinDate = new DateTime(2024, 1, 15),
                    IsActive = true,
                    MaxDailyPatients = 20,
                    LicenseNumber = "MD123456"
                },
                new StaffData
                {
                    Id = "2",
                    Name = "Nurse Maria Santos",
                    Email = "maria.santos@example.com",
                    Role = "Nurse",
                    Department = "Primary Care",
                    Specialization = "General Nursing",
                    ContactNumber = "+63 923 456 7890",
                    WorkingDays = "Monday-Saturday",
                    WorkingHours = "7:00 AM - 4:00 PM",
                    JoinDate = new DateTime(2024, 2, 1),
                    IsActive = true,
                    MaxDailyPatients = 30,
                    LicenseNumber = "RN789012"
                }
            };

            ViewBag.CurrentDateTime = philippineTime;
            return View(staffList);
        }

        [HttpGet]
        public IActionResult AddStaff()
        {
            return View();
        }

        [HttpGet]
        public IActionResult StaffDetail(string id)
        {
            return View();
        }

        [HttpGet]
        public IActionResult EditStaff(string id)
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Reports()
        {
            // Get the current time in Philippine Standard Time (UTC+8)
            var philippineTime = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila")
            );

            // Get report data asynchronously
            var reportData = await GenerateReportDataAsync();

            var viewModel = new AdminReportsViewModel
            {
                CurrentDateTime = philippineTime,
                ReportData = reportData
            };

            // Store report data in TempData for PDF generation
            TempData.Set("ReportData", reportData);

            return View(viewModel);
        }

        private async Task<ReportData> GenerateReportDataAsync()
        {
            // Get data from database asynchronously
            var totalRegistrations = await _context.Users.CountAsync();
            var totalConsultations = await _context.Appointments
                .Where(a => a.Status == AppointmentStatus.Completed)
                .CountAsync();

            var consultationsByType = await _context.Appointments
                .Where(a => a.Status == AppointmentStatus.Completed)
                .GroupBy(a => a.Type)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Type, x => x.Count);

            return new ReportData
            {
                ReportDate = DateTime.UtcNow,
                TotalRegistrations = totalRegistrations,
                TotalConsultations = totalConsultations,
                ConsultationsByType = consultationsByType,
                // ... other properties
            };
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePdf([FromBody] List<string> chartImages)
        {
            try
            {
                var reportData = TempData.Get<ReportData>("ReportData");
                if (reportData == null)
                {
                    return BadRequest("Report data not found");
                }

                // Save base64 chart images as files
                var imagesPaths = new List<string>();
                for (int i = 0; i < chartImages.Count; i++)
                {
                    var base64Data = chartImages[i].Split(',')[1];
                    var imageBytes = Convert.FromBase64String(base64Data);
                    var imagePath = Path.Combine("wwwroot", "temp", $"chart_{i}.png");
                    await System.IO.File.WriteAllBytesAsync(imagePath, imageBytes);
                    imagesPaths.Add(imagePath);
                }

                reportData.ChartImagePaths = imagesPaths;

                // Generate LaTeX content
                var latexContent = GenerateLatexContent(reportData);
                var tempDir = Path.Combine(Path.GetTempPath(), "barangay_report");
                Directory.CreateDirectory(tempDir);

                // Save LaTeX content to file
                var texFile = Path.Combine(tempDir, "report.tex");
                await System.IO.File.WriteAllTextAsync(texFile, latexContent);

                // Run pdflatex to generate PDF
                var pdfFile = Path.Combine(tempDir, "report.pdf");
                var startInfo = new ProcessStartInfo
                {
                    FileName = "pdflatex",
                    Arguments = $"-output-directory={tempDir} {texFile}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(startInfo))
                {
                    await process!.WaitForExitAsync();
                }

                // Read the generated PDF
                var pdfBytes = await System.IO.File.ReadAllBytesAsync(pdfFile);

                // Cleanup
                Directory.Delete(tempDir, true);
                foreach (var imagePath in imagesPaths)
                {
                    System.IO.File.Delete(imagePath);
                }

                return File(pdfBytes, "application/pdf", "BarangayHealthReport.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest($"PDF generation failed: {ex.Message}");
            }
        }

        private string GenerateLatexContent(ReportData data)
        {
            return @"\documentclass[12pt]{article}
\usepackage[utf8]{inputenc}
\usepackage{graphicx}
\usepackage{geometry}
\usepackage{fancyhdr}
\usepackage{booktabs}
\usepackage{xcolor}
\usepackage{hyperref}
\usepackage{tocloft}

\geometry{a4paper, margin=1in}
\pagestyle{fancy}
\fancyhf{}
\renewcommand{\headrulewidth}{0.4pt}
\rhead{\thepage}
\lhead{Barangay Health Center}

\title{\textbf{Barangay Health Center\\Administrative Report}}
\author{Generated on " + data.ReportDate.ToString("MMMM d, yyyy") + @"}
\date{}

\begin{document}

\maketitle
\thispagestyle{empty}
\newpage

\tableofcontents
\newpage

\section{Patient Registrations}
\subsection{Monthly Registration Statistics}
\begin{figure}[h]
\centering
\includegraphics[width=0.8\textwidth]{" + data.ChartImagePaths[0] + @"}
\caption{Monthly Patient Registrations}
\end{figure}

\section{Consultations by Type}
\begin{figure}[h]
\centering
\includegraphics[width=0.8\textwidth]{" + data.ChartImagePaths[1] + @"}
\caption{Distribution of Consultation Types}
\end{figure}

\section{Barangay Health Index}
\begin{figure}[h]
\centering
\includegraphics[width=0.8\textwidth]{" + data.ChartImagePaths[2] + @"}
\caption{Health Index by Category}
\end{figure}

\section{Summary}
\begin{table}[h]
\centering
\begin{tabular}{lr}
\toprule
\textbf{Metric} & \textbf{Value} \\
\midrule
Total Registrations & " + data.TotalRegistrations + @" \\
Total Consultations & " + data.TotalConsultations + @" \\
Average Health Index & " + data.AverageHealthIndex.ToString("F1") + @"\% \\
\bottomrule
\end{tabular}
\caption{Key Performance Indicators}
\end{table}

\end{document}";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDocument(IFormFile file, string userId)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    TempData["ErrorMessage"] = "No file was uploaded.";
                    return RedirectToAction(nameof(UserManagement));
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction(nameof(UserManagement));
                }

                // Validate file size (max 10MB)
                if (file.Length > 10 * 1024 * 1024)
                {
                    TempData["ErrorMessage"] = "File size must be less than 10MB.";
                    return RedirectToAction(nameof(UserManagement));
                }

                // Validate file type
                var allowedTypes = new[] { "application/pdf", "image/jpeg", "image/png" };
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                {
                    TempData["ErrorMessage"] = "Only PDF, JPEG, and PNG files are allowed.";
                    return RedirectToAction(nameof(UserManagement));
                }

                // Create uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                // Generate unique filename with timestamp and GUID
                var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var uniqueFileName = $"{timestamp}_{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file to disk
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Create document record
                var document = new UserDocument
                {
                    UserId = userId,
                    FileName = file.FileName,
                    FilePath = $"/uploads/{uniqueFileName}",
                    Status = "Pending",
                    FileSize = file.Length,
                    ContentType = file.ContentType,
                    UploadDate = DateTime.UtcNow
                };

                _context.UserDocuments.Add(document);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Document uploaded successfully for user {userId}: {uniqueFileName}");
                TempData["SuccessMessage"] = "Document uploaded successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading document for user {userId}");
                TempData["ErrorMessage"] = "An error occurred while uploading the document.";
            }

            return RedirectToAction(nameof(UserManagement));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessDocument(int documentId, string action)
        {
            var document = await _context.UserDocuments.FindAsync(documentId);
            if (document == null)
            {
                return NotFound("Document not found.");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            document.Status = action == "approve" ? "Verified" : "Rejected";
            document.ApprovedAt = DateTime.UtcNow;
            document.ApprovedBy = currentUser.Id;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(UserManagement));
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var dashboardData = new AdminDashboardViewModel
                {
                    TotalStaff = await _context.Users.OfType<ApplicationUser>()
                        .Where(u => u.UserType != UserType.Patient)
                        .CountAsync(),
                    
                    DoctorCount = await _userManager.GetUsersInRoleAsync("Doctor")
                        .ContinueWith(t => t.Result.Count),
                    
                    NurseCount = await _userManager.GetUsersInRoleAsync("Nurse")
                        .ContinueWith(t => t.Result.Count),
                    
                    PendingAppointments = await _context.Appointments
                        .Where(a => a.Status == AppointmentStatus.Pending)
                        .CountAsync(),
                    
                    ActiveStaff = await _context.Users.OfType<ApplicationUser>()
                        .Where(u => u.UserType != UserType.Patient && u.IsActive)
                        .CountAsync(),

                    StaffMembers = await _context.Users.OfType<ApplicationUser>()
                        .Where(u => u.UserType != UserType.Patient)
                        .Select(u => new StaffMemberViewModel
                        {
                            Id = u.Id,
                            Name = u.Name,
                            Role = u.UserType.ToString(),
                            Department = "General",
                            Status = u.IsActive ? "Active" : "Inactive",
                            LastActive = u.LastActive
                        })
                        .ToListAsync()
                };

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStaffDetails(string id)
        {
            try
            {
                var staff = await _context.Users.OfType<ApplicationUser>()
                    .Include(u => u.UserDocuments)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (staff == null)
                    return NotFound();

                var roles = await _userManager.GetRolesAsync(staff);

                var staffDetails = new StaffDetailsViewModel
                {
                    Id = staff.Id,
                    FullName = $"{staff.FirstName} {staff.LastName}",
                    Email = staff.Email,
                    PhoneNumber = staff.PhoneNumber,
                    Role = roles.FirstOrDefault() ?? "No Role Assigned",
                    Department = "General",
                    Status = staff.IsActive ? "Active" : "Inactive",
                    RegistrationDate = staff.JoinDate,
                    LastActive = staff.LastActive,
                    Documents = staff.UserDocuments?.Select(d => new DocumentViewModel
                    {
                        Name = d.FileName,
                        Type = d.FileType,
                        UploadDate = d.UploadDate
                    }).ToList() ?? new List<DocumentViewModel>()
                };

                return PartialView("_StaffDetails", staffDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching staff details");
                return StatusCode(500, "Error loading staff details");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStaffStatus(string id)
        {
            try
            {
                var staff = await _context.Users.OfType<ApplicationUser>()
                    .FirstOrDefaultAsync(u => u.Id == id);
                    
                if (staff == null)
                    return NotFound();

                staff.IsActive = !staff.IsActive;
                staff.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Json(new { success = true, newStatus = staff.IsActive ? "Active" : "Inactive" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling staff status");
                return StatusCode(500, "Error updating staff status");
            }
        }
    }

    public class AdminDashboardViewModel
    {
        public int TotalStaff { get; set; }
        public int DoctorCount { get; set; }
        public int NurseCount { get; set; }
        public int PendingAppointments { get; set; }
        public int ActiveStaff { get; set; }
        public List<StaffMemberViewModel> StaffMembers { get; set; }
    }

    public class StaffMemberViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }
        public DateTime? LastActive { get; set; }
    }

    public class StaffDetailsViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastActive { get; set; }
        public List<DocumentViewModel> Documents { get; set; }
    }

    public class DocumentViewModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime UploadDate { get; set; }
    }
}