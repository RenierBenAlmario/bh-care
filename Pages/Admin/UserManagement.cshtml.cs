using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using Barangay.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Barangay.Pages.Admin
{
    // Extension methods for common utility functions
    public static class DateTimeExtensions
    {
        // Calculate age based on a reference date (for consistent age calculation)
        public static int CalculateAge(this DateTime birthDate, DateTime referenceDate)
        {
            int age = referenceDate.Year - birthDate.Year;
            
            // Adjust age if birthday hasn't occurred yet in the reference year
            if (birthDate.Date > referenceDate.AddYears(-age))
                age--;
                
            return age;
        }

        
        // Check if a person is a minor (under 18) based on a reference date
        public static bool IsMinor(this DateTime birthDate, DateTime referenceDate)
        {
            return birthDate.CalculateAge(referenceDate) < 18;
        }
    }

    [Authorize(Policy = "AccessAdminDashboard")]
    [ValidateAntiForgeryToken]
    public class UserManagementModel : AdminPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserManagementModel> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IDataEncryptionService _encryptionService;

        public UserManagementModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService,
            ILogger<UserManagementModel> logger,
            IWebHostEnvironment environment,
            IDataEncryptionService encryptionService)
            : base(notificationService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _environment = environment;
            _encryptionService = encryptionService;
        }

        public List<ApplicationUser> Users { get; set; } = new();
        public List<UserDocument> UserDocuments { get; set; } = new();
        public List<GuardianInformation> GuardianInformation { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public string LastNameFilter { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string SuffixFilter { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; } = "all";
        
        public int TotalUsers { get; set; }
        public int PendingUsers { get; set; }
        public int VerifiedUsers { get; set; }
        public int RejectedUsers { get; set; }
        
        public bool HasError { get; set; }
        public string? ErrorMessage { get; set; }

        // Model for batch operations
        public class BatchOperationModel
        {
            public List<string> UserIds { get; set; } = new();
        }

        // Model for batch operation response
        public class BatchOperationResponse
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
            public int ProcessedCount { get; set; }
            public int FailedCount { get; set; }
            public List<string>? Errors { get; set; } = new();
        }

        // DTO for UpdateUserStatus JSON payload
        public class UpdateUserStatusRequest
        {
            public string? UserId { get; set; }
            public string? Status { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string status = "all")
        {
            try
            {
                StatusFilter = status;

                // Get the IDs of users with special roles to exclude
                var excludedRoleNames = new[] { "Admin", "System Administrator", "Admin Staff", "System Admin", "Staff Admin", "Doctor", "Nurse", "Head Nurse", "Head Doctor" };
                var excludedRoles = await _context.Roles
                    .Where(r => excludedRoleNames.Contains(r.Name))
                    .ToListAsync();
                    
                var excludedUserIds = await _context.UserRoles
                    .Where(ur => excludedRoles.Select(r => r.Id).Contains(ur.RoleId))
                    .Select(ur => ur.UserId)
                    .ToListAsync();

                // Build the query
                var query = _userManager.Users
                    .Include(u => u.UserDocuments)
                    .Where(u => !excludedUserIds.Contains(u.Id));

                // Apply status filter
                if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
                {
                    query = query.Where(u => u.Status.ToLower() == status.ToLower());
                }

                // Get users
                Users = await query
                    .OrderByDescending(u => u.CreatedAt)
                    .ToListAsync();

                // Decrypt user data for authorized users
                foreach (var user in Users)
                {
                    user.DecryptSensitiveData(_encryptionService, User);
                    
                    // Manually decrypt Email since it's not marked with [Encrypted] attribute
                    if (!string.IsNullOrEmpty(user.Email) && _encryptionService.IsEncrypted(user.Email))
                    {
                        user.Email = user.Email.DecryptForUser(_encryptionService, User);
                    }
                    
                    // Manually decrypt PhoneNumber since it's not marked with [Encrypted] attribute
                    if (!string.IsNullOrEmpty(user.PhoneNumber) && _encryptionService.IsEncrypted(user.PhoneNumber))
                    {
                        user.PhoneNumber = user.PhoneNumber.DecryptForUser(_encryptionService, User);
                    }
                }
                    
                // Reference date for age calculation
                var referenceDate = new DateTime(2025, 6, 16);
                
                // Find users under 18 years old
                var underageUserIds = Users
                    .Where(u => DateTime.TryParse(u.BirthDate, out var birthDate) && birthDate.IsMinor(referenceDate))
                    .Select(u => u.Id)
                    .ToList();
                
                _logger.LogInformation($"Found {underageUserIds.Count} users under 18 years old");
                
                // Load guardian information only for underage users
                if (underageUserIds.Any())
                {
                    try
                    {
                        // Check if GuardianInformation table exists and has data
                        GuardianInformation = await _context.GuardianInformation
                            .Where(g => underageUserIds.Contains(g.UserId))
                            .AsNoTracking() // Add this to prevent tracking issues
                            .Select(g => new GuardianInformation 
                            {
                                GuardianId = g.GuardianId,
                                UserId = g.UserId,
                                GuardianFirstName = g.GuardianFirstName ?? g.FirstName ?? "Unknown", // Handle nulls
                                GuardianLastName = g.LastName ?? "Unknown", // Handle nulls
                                ResidencyProofPath = g.ResidencyProofPath ?? "",
                                ConsentStatus = g.ConsentStatus ?? "Pending",
                                ProofType = g.ProofType ?? "GuardianResidencyProof",
                                CreatedAt = g.CreatedAt != default ? g.CreatedAt : DateTime.UtcNow
                            })
                            .ToListAsync();
                            
                        _logger.LogInformation($"Loaded {GuardianInformation.Count} guardian information records");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error loading guardian information");
                        GuardianInformation = new List<GuardianInformation>();
                    }
                }
                else
                {
                    GuardianInformation = new List<GuardianInformation>();
                }

                // Get counts for different statuses (excluding special roles)
                TotalUsers = await query.CountAsync();
                PendingUsers = await query.CountAsync(u => u.Status.ToLower() == "pending");
                VerifiedUsers = await query.CountAsync(u => u.Status.ToLower() == "verified");
                RejectedUsers = await query.CountAsync(u => u.Status.ToLower() == "rejected");

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user management page");
                TempData["ErrorMessage"] = "An error occurred while loading the page. Please try again.";
                return Page();
            }
        }
        
        public async Task<IActionResult> OnPostApproveAsync(string id, string notes)
        {
            try
            {
                _logger.LogInformation($"Attempting to approve user with ID: {id}");
                
                // Verify current user has admin role
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogWarning("Current user not found during approval process");
                    TempData["ErrorMessage"] = "Authentication error. Please login again.";
                    return RedirectToPage();
                }
                
                _logger.LogInformation($"Admin user: {currentUser.Email} attempting to approve user");
                
                // Find the user to approve
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"User not found with ID: {id}");
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToPage();
                }
                
                _logger.LogInformation($"Found user to approve: {user.Email}, Current Status: {user.Status}");
                
                // Check for residency proof
                UserDocument? document = null;
                try
                {
                    document = await _context.UserDocuments
                        .FirstOrDefaultAsync(d => d.UserId == id);
                    
                    if (document == null)
                    {
                        _logger.LogWarning($"No residency proof document found for user: {user.Email}");
                        TempData["WarningMessage"] = "Proceeding with approval despite missing residency proof document.";
                    }
                    else
                    {
                        _logger.LogInformation($"Found residency document for user: {document.FileName}");
                        // Update document status
                        document.Status = "Verified";
                        document.ApprovedAt = DateTime.UtcNow;
                        document.ApprovedBy = currentUser.Id;
                        
                        // Ensure no NULL values
                        document.FileName = document.FileName ?? "";
                        document.FilePath = document.FilePath ?? "";
                        document.ContentType = document.ContentType ?? "application/octet-stream";
                        document.Status = document.Status ?? "Verified";
                        document.ApprovedBy = document.ApprovedBy ?? "";
                        
                        _context.UserDocuments.Update(document);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing document for user: {user.Email}");
                    TempData["WarningMessage"] = "Unable to process residency document, but proceeding with approval.";
                }

                // Check if user is underage and automatically approve guardian consent if exists
                DateTime? birthDate = DateTime.TryParse(user.BirthDate, out var parsedBirthDate) ? parsedBirthDate : null;
                if (birthDate != null)
                {
                    // Use reference date of today to calculate age
                    var today = DateTime.Today;
                    int age = today.Year - ((DateTime)birthDate).Year;
                    
                    // Adjust age if birthday hasn't occurred yet this year
                    if (((DateTime)birthDate).Date > today.AddYears(-age)) 
                        age--;
                    
                    // If user is under 18, approve guardian consent
                    if (age < 18)
                    {
                        _logger.LogInformation($"User {user.Email} is underage ({age} years old). Checking for guardian information.");
                        
                        try
                        {
                            // Find guardian information
                            var guardian = await _context.GuardianInformation
                                .FirstOrDefaultAsync(g => g.UserId == id);
                                
                            if (guardian != null)
                            {
                                _logger.LogInformation($"Found guardian information for user {user.Email}. Updating consent status to Approved.");
                                
                                // Update guardian consent status to Approved
                                guardian.ConsentStatus = "Approved";
                                _context.GuardianInformation.Update(guardian);
                                
                                // Create notification for guardian consent approval
                                await _notificationService.CreateNotificationForUserAsync(
                                    userId: id,
                                    title: "Guardian Consent Approved",
                                    message: "Your guardian consent has been approved along with your account verification.",
                                    type: "Success",
                                    link: "/Account/Profile"
                                );
                            }
                            else
                            {
                                _logger.LogWarning($"No guardian information found for underage user {user.Email}");
                                TempData["WarningMessage"] = "User is underage but no guardian information found. Account approved anyway.";
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error processing guardian information for underage user: {user.Email}");
                            TempData["WarningMessage"] = "Error processing guardian information, but proceeding with approval.";
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"User {user.Email} is not underage ({age} years old). No guardian consent needed.");
                    }
                }

                // Update user status
                user.Status = "Verified";
                user.IsActive = true;
                user.EncryptedStatus = "Active";
                
                _logger.LogInformation($"Attempting to update user status to: Verified");
                var updateResult = await _userManager.UpdateAsync(user);
                
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        _logger.LogError($"User update error: {error.Code} - {error.Description}");
                    }
                    TempData["ErrorMessage"] = "Failed to update user status: " + 
                        string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    return RedirectToPage();
                }
                
                _logger.LogInformation($"Successfully updated user status for {user.Email}");
                
                // Update patient status if exists
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.UserId == id);
                    
                if (patient != null)
                {
                    _logger.LogInformation($"Updating associated patient record for user: {user.Email}");
                    patient.Status = "Active";
                    _context.Patients.Update(patient);
                }
                
                // Update document status
                if (document != null)
                {
                    document.Status = "Approved";
                    document.ApprovedAt = DateTime.UtcNow;
                    document.ApprovedBy = User.Identity?.Name ?? "System";
                    _logger.LogInformation($"Updating document status to Approved, approved by: {User.Identity?.Name ?? "System"}");
                    
                    if (!string.IsNullOrEmpty(notes))
                    {
                        _logger.LogInformation($"Admin notes for approval: {notes}");
                    }
                    
                    _context.UserDocuments.Update(document);
                }
                else
                {
                    _logger.LogInformation("No document found to update for approval");
                }
                
                // Assign role if needed
                if (!await _userManager.IsInRoleAsync(user, "PATIENT"))
                {
                    _logger.LogInformation($"Assigning PATIENT role to user: {user.Email}");
                    var roleResult = await _userManager.AddToRoleAsync(user, "PATIENT");
                    
                    if (!roleResult.Succeeded)
                    {
                        foreach (var error in roleResult.Errors)
                        {
                            _logger.LogWarning($"Role assignment error: {error.Code} - {error.Description}");
                        }
                        // Continue even if role assignment fails - log but don't return
                    }
                }
                
                _logger.LogInformation($"Saving changes to database");
                await _context.SaveChangesAsync();

                // Create notification
                _logger.LogInformation($"Creating notification for user approval");
                await _notificationService.CreateNotificationAsync(
                    title: "User Approved",
                    message: $"User {user.UserName} ({user.FirstName} {user.LastName}) has been approved and verified.",
                    type: "Success",
                    link: "/Admin/UserManagement"
                );
                
                // Create notification for the approved user
                await _notificationService.CreateNotificationForUserAsync(
                    userId: id,
                    title: "Account Approved",
                    message: "Your account has been approved. You can now access all system features.",
                    type: "Success",
                    link: "/Index"
                );
                
                // Update pending users count for notification badge
                PendingUsers = await _userManager.Users
                    .CountAsync(u => u.Status == "Pending");
                    
                ViewData["PendingUsersCount"] = PendingUsers;
                
                // Set success message
                TempData["SuccessMessage"] = "User has been approved successfully.";
                TempData["UpdateNotificationBadge"] = "true"; // Flag to update notification badge on redirect
                
                _logger.LogInformation($"User approval process completed successfully for: {user.Email}");
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user approval process for UserId {UserId}: {ErrorMessage}", 
                    id, ex.Message);
                    
                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner exception: {InnerError}", ex.InnerException.Message);
                }
                
                _logger.LogError("Stack trace: {StackTrace}", ex.StackTrace);
                
                TempData["ErrorMessage"] = "An error occurred while approving the user. Please try again.";
                return RedirectToPage();
            }
        }

        // Returns a JSON response with guardian proof and metadata for the modal
        public async Task<IActionResult> OnGetGuardianProofAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return new JsonResult(new { success = false, message = "Missing userId." });
                }

                var guardian = await _context.GuardianInformation
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.UserId == userId);

                if (guardian == null)
                {
                    return new JsonResult(new { success = false, message = "No guardian information found." });
                }

                string? proofPath = null;
                bool hasProof = false;

                // Prefer stored file path if it exists under wwwroot
                if (!string.IsNullOrWhiteSpace(guardian.ResidencyProofPath))
                {
                    try
                    {
                        var relative = guardian.ResidencyProofPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                        var absolute = Path.Combine(_environment.WebRootPath, relative);
                        if (System.IO.File.Exists(absolute))
                        {
                            proofPath = guardian.ResidencyProofPath;
                            hasProof = true;
                        }
                    }
                    catch { /* ignore and fall back */ }
                }

                // Fallback to bytes via API endpoint if available
                if (!hasProof && guardian.ResidencyProof != null && guardian.ResidencyProof.Length > 0)
                {
                    proofPath = $"/api/Admin/GetGuardianProof/{guardian.GuardianId}";
                    hasProof = true;
                }

                return new JsonResult(new {
                    success = true,
                    guardianFirstName = guardian.GuardianFirstName ?? guardian.FirstName ?? string.Empty,
                    guardianLastName = guardian.GuardianLastName ?? guardian.LastName ?? string.Empty,
                    consentStatus = guardian.ConsentStatus ?? "Pending",
                    proofType = guardian.ProofType ?? "GuardianResidencyProof",
                    createdAt = guardian.CreatedAt,
                    hasProof,
                    proofPath = proofPath ?? string.Empty
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnGetGuardianProofAsync for user {UserId}", userId);
                return new JsonResult(new { success = false, message = "Error loading guardian information." });
            }
        }

        // Handles fetch('/Admin/UserManagement?handler=UpdateGuardianConsent') JSON requests
        public async Task<IActionResult> OnPostUpdateGuardianConsentAsync()
        {
            try
            {
                // Read JSON body
                string body;
                using (var reader = new StreamReader(Request.Body))
                {
                    body = await reader.ReadToEndAsync();
                }

                if (string.IsNullOrWhiteSpace(body))
                {
                    return new JsonResult(new { success = false, message = "Empty request body." });
                }

                var payload = JsonSerializer.Deserialize<UpdateUserStatusRequest>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (payload == null || string.IsNullOrWhiteSpace(payload.UserId) || string.IsNullOrWhiteSpace(payload.Status))
                {
                    return new JsonResult(new { success = false, message = "Invalid payload. Missing userId or status." });
                }

                var userId = payload.UserId.Trim();
                var newStatus = payload.Status.Trim();

                var guardian = await _context.GuardianInformation.FirstOrDefaultAsync(g => g.UserId == userId);
                if (guardian == null)
                {
                    return new JsonResult(new { success = false, message = "Guardian record not found." });
                }

                // Normalize and set status
                switch (newStatus.ToLowerInvariant())
                {
                    case "approved":
                        guardian.ConsentStatus = "Approved";
                        break;
                    case "rejected":
                        guardian.ConsentStatus = "Rejected";
                        break;
                    case "pending":
                        guardian.ConsentStatus = "Pending";
                        break;
                    default:
                        return new JsonResult(new { success = false, message = $"Unsupported status '{newStatus}'." });
                }

                _context.GuardianInformation.Update(guardian);
                await _context.SaveChangesAsync();

                return new JsonResult(new { success = true, message = $"Guardian consent {guardian.ConsentStatus.ToLower()} successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateGuardianConsent handler");
                return new JsonResult(new { success = false, message = "An error occurred while updating guardian consent." });
            }
        }

        public int CalculateAge(DateTime birthDate, DateTime referenceDate)
        {
            int age = referenceDate.Year - birthDate.Year;
            
            // Adjust age if birthday hasn't occurred yet in the reference year
            if (birthDate.Date > referenceDate.AddYears(-age))
                age--;
                
            return age;
        }

        // Handles fetch('/Admin/UserManagement?handler=UpdateUserStatus') JSON requests
        public async Task<IActionResult> OnPostUpdateUserStatusAsync()
        {
            try
            {
                // Read JSON body
                string body;
                using (var reader = new StreamReader(Request.Body))
                {
                    body = await reader.ReadToEndAsync();
                }

                if (string.IsNullOrWhiteSpace(body))
                {
                    return new JsonResult(new { success = false, message = "Empty request body." });
                }

                var payload = JsonSerializer.Deserialize<UpdateUserStatusRequest>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (payload == null || string.IsNullOrWhiteSpace(payload.UserId) || string.IsNullOrWhiteSpace(payload.Status))
                {
                    return new JsonResult(new { success = false, message = "Invalid payload. Missing userId or status." });
                }

                var user = await _userManager.FindByIdAsync(payload.UserId);
                if (user == null)
                {
                    return new JsonResult(new { success = false, message = "User not found." });
                }

                var status = payload.Status.Trim();
                _logger.LogInformation($"UpdateUserStatus handler: setting user {user.Email} to status '{status}'");

                switch (status.ToLowerInvariant())
                {
                    case "verified":
                        user.Status = "Verified";
                        user.IsActive = true;
                        user.EncryptedStatus = "Active";
                        // If the user is under 18, auto-approve guardian consent
                        try
                        {
                            if (user.BirthDate != default)
                            {
                                var today = DateTime.Today;
                                var userBirthDate = DateTime.TryParse(user.BirthDate, out var parsedBirthDate) ? parsedBirthDate : DateTime.MinValue;
                                var age = today.Year - userBirthDate.Year;
                                if (userBirthDate.Date > today.AddYears(-age)) age--;

                                if (age < 18)
                                {
                                    var guardian = await _context.GuardianInformation.FirstOrDefaultAsync(g => g.UserId == user.Id);
                                    if (guardian != null)
                                    {
                                        guardian.ConsentStatus = "Approved";
                                        _context.GuardianInformation.Update(guardian);
                                        _logger.LogInformation("Auto-approved guardian consent for underage user {UserId}", user.Id);
                                    }
                                    else
                                    {
                                        _logger.LogWarning("No guardian record found to auto-approve for underage user {UserId}", user.Id);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to auto-approve guardian consent for user {UserId}", user.Id);
                        }
                        break;
                    case "rejected":
                        user.Status = "Rejected";
                        user.IsActive = false;
                        user.EncryptedStatus = "Inactive";
                        break;
                    case "pending":
                        user.Status = "Pending";
                        user.IsActive = false;
                        user.EncryptedStatus = "Inactive";
                        break;
                    default:
                        return new JsonResult(new { success = false, message = $"Unsupported status '{status}'." });
                }

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to update user {UserId}: {Errors}", user.Id, errors);
                    return new JsonResult(new { success = false, message = $"Failed to update user: {errors}" });
                }

                // Update related patient row if exists
                try
                {
                    var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);
                    if (patient != null)
                    {
                        patient.Status = user.IsActive ? "Active" : "Inactive";
                        _context.Patients.Update(patient);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Unable to update patient status for user {UserId}", user.Id);
                }

                // Assign PATIENT role on verify (best-effort)
                if (status.Equals("verified", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        if (!await _userManager.IsInRoleAsync(user, "PATIENT"))
                        {
                            var roleResult = await _userManager.AddToRoleAsync(user, "PATIENT");
                            if (!roleResult.Succeeded)
                            {
                                _logger.LogWarning("Failed to assign PATIENT role to {UserId}", user.Id);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error assigning PATIENT role to {UserId}", user.Id);
                    }
                }

                await _context.SaveChangesAsync();

                var successMessage = status.Equals("verified", StringComparison.OrdinalIgnoreCase)
                    ? "User has been approved successfully."
                    : status.Equals("rejected", StringComparison.OrdinalIgnoreCase)
                        ? "User has been rejected."
                        : "User status updated.";

                return new JsonResult(new { success = true, message = successMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateUserStatus handler");
                return new JsonResult(new { success = false, message = "An error occurred while updating user status." });
            }
        }

        public async Task<IActionResult> OnPostDeleteUserAsync()
        {
            try
            {
                // Read JSON body
                string body;
                using (var reader = new StreamReader(Request.Body))
                {
                    body = await reader.ReadToEndAsync();
                }

                if (string.IsNullOrWhiteSpace(body))
                {
                    return new JsonResult(new { success = false, message = "Empty request body." });
                }

                var payload = JsonSerializer.Deserialize<UpdateUserStatusRequest>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (payload == null || string.IsNullOrWhiteSpace(payload.UserId))
                {
                    return new JsonResult(new { success = false, message = "Invalid payload. Missing userId." });
                }

                var userId = payload.UserId.Trim();
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new JsonResult(new { success = false, message = "User not found." });
                }

                _logger.LogInformation($"Deleting user: {user.Email} (ID: {userId})");

                // Delete related data first (to avoid foreign key constraints)
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Delete user documents
                    var userDocuments = await _context.UserDocuments.Where(d => d.UserId == userId).ToListAsync();
                    _context.UserDocuments.RemoveRange(userDocuments);

                    // Delete guardian information
                    var guardianInfo = await _context.GuardianInformation.Where(g => g.UserId == userId).ToListAsync();
                    _context.GuardianInformation.RemoveRange(guardianInfo);

                    // Delete patient records
                    var patients = await _context.Patients.Where(p => p.UserId == userId).ToListAsync();
                    _context.Patients.RemoveRange(patients);

                    // Delete appointments
                    var appointments = await _context.Appointments.Where(a => a.PatientId == userId).ToListAsync();
                    _context.Appointments.RemoveRange(appointments);

                    // Delete medical records
                    var medicalRecords = await _context.MedicalRecords.Where(m => m.PatientId == userId).ToListAsync();
                    _context.MedicalRecords.RemoveRange(medicalRecords);

                    // Delete prescriptions (with error handling for schema mismatch)
                    try
                    {
                        var prescriptions = await _context.Prescriptions.Where(p => p.PatientId == userId).ToListAsync();
                        _context.Prescriptions.RemoveRange(prescriptions);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete prescriptions for user {UserId}, continuing with other deletions", userId);
                    }

                    // Delete assessments
                    var ncdAssessments = await _context.NCDRiskAssessments.Where(n => n.UserId == userId).ToListAsync();
                    _context.NCDRiskAssessments.RemoveRange(ncdAssessments);

                    var heeadsssAssessments = await _context.HEEADSSSAssessments.Where(h => h.UserId == userId).ToListAsync();
                    _context.HEEADSSSAssessments.RemoveRange(heeadsssAssessments);

                    // Delete notifications
                    var notifications = await _context.Notifications.Where(n => n.UserId == userId).ToListAsync();
                    _context.Notifications.RemoveRange(notifications);

                    // Delete Identity-related data
                    try
                    {
                        // Delete user claims
                        var userClaims = await _context.UserClaims.Where(c => c.UserId == userId).ToListAsync();
                        _context.UserClaims.RemoveRange(userClaims);

                        // Delete user roles
                        var userRoles = await _context.UserRoles.Where(r => r.UserId == userId).ToListAsync();
                        _context.UserRoles.RemoveRange(userRoles);

                        // Delete user logins
                        var userLogins = await _context.UserLogins.Where(l => l.UserId == userId).ToListAsync();
                        _context.UserLogins.RemoveRange(userLogins);

                        // Delete user tokens
                        var userTokens = await _context.UserTokens.Where(t => t.UserId == userId).ToListAsync();
                        _context.UserTokens.RemoveRange(userTokens);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete Identity-related data for user {UserId}, continuing with user deletion", userId);
                    }

                    // Save changes to related data
                    await _context.SaveChangesAsync();

                    // Delete the user
                    var deleteResult = await _userManager.DeleteAsync(user);
                    if (!deleteResult.Succeeded)
                    {
                        var errors = string.Join(", ", deleteResult.Errors.Select(e => e.Description));
                        _logger.LogError("Failed to delete user {UserId}: {Errors}", userId, errors);
                        await transaction.RollbackAsync();
                        return new JsonResult(new { success = false, message = $"Failed to delete user: {errors}" });
                    }

                    await transaction.CommitAsync();
                    _logger.LogInformation($"User {user.Email} deleted successfully");
                    return new JsonResult(new { success = true, message = "User account deleted successfully." });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error deleting user {UserId}", userId);
                    return new JsonResult(new { success = false, message = "An error occurred while deleting the user account." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteUser handler");
                return new JsonResult(new { success = false, message = "An error occurred while deleting the user account." });
            }
        }
    }
}