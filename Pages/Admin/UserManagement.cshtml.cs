using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Barangay.Pages.Admin
{
    [Authorize(Policy = "AccessAdminDashboard")]
    [ValidateAntiForgeryToken]
    public class UserManagementModel : AdminPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserManagementModel> _logger;
        private readonly IWebHostEnvironment _environment;

        public UserManagementModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService,
            ILogger<UserManagementModel> logger,
            IWebHostEnvironment environment)
            : base(notificationService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _environment = environment;
        }

        public List<ApplicationUser> Users { get; set; } = new();
        public List<UserDocument> UserDocuments { get; set; } = new();
        
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

        public async Task<IActionResult> OnGetAsync(string status = "all")
        {
            try
            {
                StatusFilter = status;

                // Get the IDs of users with special roles to exclude
                var excludedRoleNames = new[] { "System Admin", "Staff Admin", "Doctor", "Nurse" };
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
        
        public async Task<IActionResult> OnPostRejectAsync(string id, string notes)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                
                if (user == null)
                {
                    return NotFound();
                }

                // Update user status
                user.Status = "Rejected";
                await _userManager.UpdateAsync(user);
                
                // Update document status if exists
                try
                {
                    var document = await _context.UserDocuments
                        .FirstOrDefaultAsync(d => d.UserId == id);
                        
                    if (document != null)
                    {
                        document.Status = "Rejected";
                        
                        // Ensure no NULL values
                        document.FileName = document.FileName ?? "";
                        document.FilePath = document.FilePath ?? "";
                        document.ContentType = document.ContentType ?? "application/octet-stream";
                        document.Status = document.Status ?? "Rejected";
                        document.ApprovedBy = document.ApprovedBy ?? "";
                        
                        _context.UserDocuments.Update(document);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating document for rejected user: {user.Email}");
                    // Continue without failing
                }
                
                await _context.SaveChangesAsync();
                
                // Update pending users count for notification badge
                PendingUsers = await _userManager.Users
                    .CountAsync(u => u.Status == "Pending");
                    
                ViewData["PendingUsersCount"] = PendingUsers;
                
                // Set success message
                TempData["SuccessMessage"] = "User has been rejected successfully.";
                TempData["UpdateNotificationBadge"] = "true"; // Flag to update notification badge on redirect
                
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting user {UserId}", id);
                TempData["ErrorMessage"] = "An error occurred while rejecting the user. Please try again.";
                return RedirectToPage();
            }
        }
        
        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                
                if (user == null)
                {
                    return NotFound();
                }
                
                // Delete associated patient records first
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.UserId == id);
                    
                if (patient != null)
                {
                    _context.Patients.Remove(patient);
                    await _context.SaveChangesAsync();
                }
                
                // Delete user
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "User has been deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete user: " + string.Join(", ", result.Errors.Select(e => e.Description));
                }
                
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the user. Please try again.";
                return RedirectToPage();
            }
        }
        
        public IActionResult OnGetRetryAsync()
        {
            return RedirectToPage();
        }

        // This API endpoint is called by admin-user-management.js to get user data
        public async Task<IActionResult> OnGetUsersAsync(string filter = "all", int page = 1, string search = "")
        {
            const int pageSize = 10;
            
            try
            {
                _logger.LogInformation($"Loading users with filter: {filter}, page: {page}, search: '{search}'");
                
                // Ensure UserDocuments table exists first
                await EnsureUserDocumentsTableExistsAsync();
                
                // Get all pending users count first (for the badge)
                var pendingCount = await _userManager.Users
                    .Where(u => u.Status == "Pending")
                    .CountAsync();
                
                _logger.LogInformation($"Found {pendingCount} pending users");
                
                // Start with all users
                var usersQuery = _userManager.Users.AsQueryable();
                
                // Get list of admin staff user IDs to exclude from pending approval
                var adminStaffRoleId = await _context.Roles
                    .Where(r => r.Name == "Admin Staff")
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();

                var adminStaffUserIds = new List<string>();
                if (!string.IsNullOrEmpty(adminStaffRoleId))
                {
                    adminStaffUserIds = await _context.UserRoles
                        .Where(ur => ur.RoleId == adminStaffRoleId)
                        .Select(ur => ur.UserId)
                        .ToListAsync();
                    
                    _logger.LogInformation($"Found {adminStaffUserIds.Count} admin staff users to exclude from approval");
                }
                
                // Apply filter if not "all"
                if (filter.ToLower() == "pending")
                {
                    _logger.LogInformation("Filtering for pending users only");
                    usersQuery = usersQuery.Where(u => u.Status == "Pending");
                    
                    // Exclude admin staff users from pending filter
                    if (adminStaffUserIds.Any())
                    {
                        usersQuery = usersQuery.Where(u => !adminStaffUserIds.Contains(u.Id));
                    }
                }
                else if (filter.ToLower() == "verified")
                {
                    usersQuery = usersQuery.Where(u => u.Status == "Verified" || u.Status == "Approved");
                }
                else if (filter.ToLower() == "rejected")
                {
                    usersQuery = usersQuery.Where(u => u.Status == "Rejected");
                }
                
                // Apply search if provided
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    usersQuery = usersQuery.Where(u => 
                        (u.UserName != null && u.UserName.ToLower().Contains(search)) ||
                        (u.Email != null && u.Email.ToLower().Contains(search)) ||
                        (u.FirstName != null && u.FirstName.ToLower().Contains(search)) ||
                        (u.LastName != null && u.LastName.ToLower().Contains(search)) ||
                        (u.PhoneNumber != null && u.PhoneNumber.ToLower().Contains(search))
                    );
                }
                
                // Calculate total count and pages
                var totalCount = await usersQuery.CountAsync();
                _logger.LogInformation($"Total matching users: {totalCount}");
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                
                // Get user IDs for the current page
                var userIds = await usersQuery
                    .OrderByDescending(u => u.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => u.Id)
                    .ToListAsync();
                
                _logger.LogInformation($"Retrieved {userIds.Count} user IDs for page {page}");
                
                // Get the full user objects for these IDs
                List<ApplicationUser> users = new List<ApplicationUser>();
                try {
                    users = await _userManager.Users
                        .Where(u => userIds.Contains(u.Id))
                        .ToListAsync();
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "Error retrieving user details: {Message}", ex.Message);
                    // Return empty users list but don't fail completely
                    users = new List<ApplicationUser>();
                    
                    // Try to get users one by one to avoid failing completely
                    foreach (var userId in userIds) {
                        try {
                            var user = await _userManager.FindByIdAsync(userId);
                            if (user != null) {
                                users.Add(user);
                            }
                        }
                        catch (Exception userEx) {
                            _logger.LogError(userEx, "Error retrieving user with ID {UserId}: {Message}", userId, userEx.Message);
                        }
                    }
                }
                
                // Get documents for these users
                List<UserDocument> documents = new List<UserDocument>();
                try
                {
                    documents = await _context.UserDocuments
                        .Where(d => userIds.Contains(d.UserId))
                        .ToListAsync();
                    
                    _logger.LogInformation($"Retrieved {documents.Count} documents for the users");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving user documents");
                    // Continue with empty documents list
                }
                
                // Map to response objects
                var result = users.Select(user => {
                    try {
                        // Ensure all string properties have default values to prevent null errors
                        string userName = user.UserName ?? "";
                        string email = user.Email ?? "";
                        string phoneNumber = user.PhoneNumber ?? "N/A";
                        string status = user.Status ?? "Unknown";
                        DateTime createdAt = user.CreatedAt;
                        
                        // Find document for this user
                        var doc = documents.FirstOrDefault(d => d.UserId == user.Id);
                        
                        // Create full name with null checks
                        string firstName = user.FirstName ?? "";
                        string middleName = user.MiddleName ?? "";
                        string lastName = user.LastName ?? "";
                        string suffix = user.Suffix ?? "";
                        
                        string fullName = firstName;
                        if (!string.IsNullOrEmpty(middleName))
                            fullName += " " + middleName;
                        if (!string.IsNullOrEmpty(lastName))
                            fullName += " " + lastName;
                        if (!string.IsNullOrEmpty(suffix))
                            fullName += ", " + suffix;
                        
                        if (string.IsNullOrWhiteSpace(fullName))
                            fullName = user.FullName ?? ""; // Use FullName as fallback with null check
                            
                        if (string.IsNullOrWhiteSpace(fullName))
                            fullName = userName; // Use UserName as last resort
                        
                        bool hasLegacyDocument = !string.IsNullOrEmpty(user.ProfilePicture) && 
                                               user.ProfilePicture.Contains("/uploads/residency_proofs/");
                        
                        return new
                        {
                            id = user.Id,
                            username = userName,
                            fullName = fullName,
                            email = email,
                            contactNumber = phoneNumber,
                            status = status,
                            registeredOn = createdAt,
                            // Document info
                            hasDocument = doc != null || hasLegacyDocument,
                            documentId = doc?.Id ?? 0,
                            documentName = doc?.FileName ?? (hasLegacyDocument ? "Legacy Document" : "No document"),
                            documentPath = doc?.FilePath ?? "", // Alternative property name for compatibility
                            filePath = doc?.FilePath ?? "", // Alternative property name for compatibility
                            documentType = doc?.ContentType ?? "application/octet-stream",
                            // Include ProfilePicture for legacy document handling
                            profilePicture = user.ProfilePicture ?? ""
                        };
                    }
                    catch (Exception ex) {
                        _logger.LogError(ex, "Error mapping user data for user {UserId}: {Message}", user.Id, ex.Message);
                        // Return a minimal object with default values to prevent null errors
                        return new {
                            id = user.Id,
                            username = user.UserName ?? "",
                            fullName = user.FullName ?? user.UserName ?? "Unknown User",
                            email = user.Email ?? "",
                            contactNumber = "N/A",
                            status = "Unknown",
                            registeredOn = DateTime.Now,
                            hasDocument = false,
                            documentId = 0,
                            documentName = "No document",
                            documentPath = "",
                            filePath = "",
                            documentType = "application/octet-stream",
                            profilePicture = ""
                        };
                    }
                }).ToList();
                
                _logger.LogInformation($"Returning {result.Count} users for display");
                
                return new JsonResult(new
                {
                    users = result,
                    totalPages,
                    pendingCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users: {Message}", ex.Message);
                
                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner exception: {Message}", ex.InnerException.Message);
                }
                
                return new JsonResult(new
                {
                    users = new List<object>(),
                    totalPages = 0,
                    pendingCount = 0,
                    error = $"Error loading users: {ex.Message}"
                });
            }
        }

        // This method handles user status updates from the AJAX calls
        public async Task<IActionResult> OnPostUpdateUserStatusAsync([FromBody] UpdateUserStatusModel model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.Status))
                {
                    return new JsonResult(new { success = false, error = "Invalid request data" });
                }

                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return new JsonResult(new { success = false, error = "User not found" });
                }

                // Update user status
                user.Status = model.Status;
                user.IsActive = model.Status.ToLower() == "verified";

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new JsonResult(new { success = false, error = $"Failed to update user: {errors}" });
                }

                // Update associated documents if any
                var documents = await _context.UserDocuments
                    .Where(d => d.UserId == model.UserId)
                    .ToListAsync();

                foreach (var doc in documents)
                {
                    doc.Status = model.Status;
                    _context.UserDocuments.Update(doc);
                }

                await _context.SaveChangesAsync();

                return new JsonResult(new { 
                    success = true, 
                    message = $"User status updated to {model.Status} successfully" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user status");
                return new JsonResult(new { 
                    success = false, 
                    error = "An error occurred while updating the user status" 
                });
            }
        }
        
        public class UpdateUserStatusModel
        {
            public string UserId { get; set; }
            public string Status { get; set; }
        }

        // Add a method to migrate legacy documents from ProfilePicture to UserDocuments
        public async Task<IActionResult> OnPostMigrateDocumentAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Attempting to migrate legacy document for user ID: {id}");
                
                // Find user with the legacy document
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"User not found with ID: {id}");
                    return new JsonResult(new { success = false, message = "User not found" });
                }
                
                // Check if user has a ProfilePicture to migrate
                if (string.IsNullOrEmpty(user.ProfilePicture) || !user.ProfilePicture.Contains("/uploads/residency_proofs/"))
                {
                    _logger.LogWarning($"No valid ProfilePicture to migrate for user: {user.Email}");
                    return new JsonResult(new { success = false, message = "No valid document to migrate" });
                }
                
                // Check if a document already exists in UserDocuments
                var existingDoc = await _context.UserDocuments
                    .FirstOrDefaultAsync(d => d.UserId == id);
                
                if (existingDoc != null)
                {
                    _logger.LogWarning($"Document already exists in UserDocuments for user: {user.Email}");
                    return new JsonResult(new { success = false, message = "Document already exists in the new system" });
                }
                
                // Get file details
                string? filePath = user.ProfilePicture;
                string? fileName = Path.GetFileName(filePath);
                string physicalPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                
                // Check if file exists
                if (!System.IO.File.Exists(physicalPath))
                {
                    _logger.LogWarning($"File does not exist at path: {physicalPath}");
                    // Create document entry anyway but mark as missing
                    var missingDoc = new UserDocument
                    {
                        UserId = user.Id,
                        FileName = Path.GetFileName(filePath) ?? "unknown.pdf",
                        FilePath = filePath ?? "",
                        Status = "Missing",
                        FileSize = 0,
                        ContentType = "application/octet-stream",
                        ApprovedBy = ""
                    };
                    
                    _context.UserDocuments.Add(missingDoc);
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation($"Created missing document record for user: {user.Email}");
                    
                    return new JsonResult(new { 
                        success = true, 
                        message = "Document migrated (file missing)", 
                        documentId = missingDoc.Id 
                    });
                }
                
                // Get file info
                var fileInfo = new FileInfo(physicalPath);
                string contentType = GetContentType(Path.GetExtension(physicalPath));
                
                // Create new UserDocument
                var userDocument = new UserDocument
                {
                    UserId = user.Id,
                    FileName = Path.GetFileName(filePath) ?? "unknown.pdf",
                    FilePath = filePath ?? "",
                    Status = "Pending",
                    FileSize = fileInfo.Length,
                    ContentType = contentType ?? "application/octet-stream",
                    ApprovedBy = ""
                };
                
                _context.UserDocuments.Add(userDocument);
                
                // Clear ProfilePicture
                user.ProfilePicture = null;
                await _userManager.UpdateAsync(user);
                
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Successfully migrated document for user: {user.Email}, Document ID: {userDocument.Id}");
                
                return new JsonResult(new { 
                    success = true, 
                    message = "Document migrated successfully",
                    documentId = userDocument.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error migrating document");
                return new JsonResult(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
        
        // Helper method to determine content type based on file extension
        private string GetContentType(string extension)
        {
            return extension.ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };
        }

        // Find or add the LoadNotificationsAsync method
        new private async Task LoadNotificationsAsync()
        {
            try
            {
                // Get unread notifications
                var notifications = await _notificationService.GetUnreadNotificationsAsync(User.Identity?.Name);
                
                // Set notification data for the view
                ViewData["Notifications"] = notifications;
                ViewData["NotificationCount"] = notifications?.Count ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading notifications");
                // Set empty notifications to prevent page from breaking
                ViewData["Notifications"] = new List<Notification>();
                ViewData["NotificationCount"] = 0;
            }
        }

        private async Task EnsureUserDocumentsTableExistsAsync()
        {
            try
            {
                // Check if the UserDocuments table exists
                bool tableExists = false;
                try
                {
                    // Try to access the table - if it throws an exception, the table doesn't exist or has issues
                    tableExists = await _context.UserDocuments.AnyAsync();
                    _logger.LogInformation("UserDocuments table exists and is accessible");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error accessing UserDocuments table: {Message}", ex.Message);
                    tableExists = false;
                }

                if (!tableExists)
                {
                    _logger.LogWarning("Creating UserDocuments table...");
                    
                    // Create the table if it doesn't exist
                    var sql = @"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserDocuments')
                        BEGIN
                            CREATE TABLE [UserDocuments](
                                [Id] INT IDENTITY(1,1) PRIMARY KEY,
                                [UserId] NVARCHAR(450) NOT NULL,
                                [FilePath] NVARCHAR(256) NOT NULL DEFAULT (''),
                                [FileName] NVARCHAR(255) NOT NULL DEFAULT (''),
                                [FileSize] BIGINT NOT NULL DEFAULT (0),
                                [ContentType] NVARCHAR(100) NOT NULL DEFAULT ('application/octet-stream'),
                                [Status] NVARCHAR(50) NOT NULL DEFAULT ('Pending'),
                                [ApprovedAt] DATETIME2 NULL,
                                [ApprovedBy] NVARCHAR(256) NULL DEFAULT (''),
                                [UploadDate] DATETIME2 NOT NULL DEFAULT GETDATE()),
                                CONSTRAINT [FK_UserDocuments_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
                            );
                        END
                    ";
                    
                    await _context.Database.ExecuteSqlRawAsync(sql);
                    _logger.LogInformation("UserDocuments table created successfully");
                    
                    // Now check if there are any users with legacy documents in ProfilePicture that need migration
                    await MigrateLegacyDocumentsAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ensuring UserDocuments table exists");
                throw; // Re-throw to be caught by the caller
            }
        }

        private async Task MigrateLegacyDocumentsAsync()
        {
            try
            {
                _logger.LogInformation("Checking for legacy documents in ProfilePicture...");
                
                // Get users with legacy documents in ProfilePicture
                var usersWithLegacyDocs = await _userManager.Users
                    .Where(u => !string.IsNullOrEmpty(u.ProfilePicture) && 
                                u.ProfilePicture.Contains("/uploads/residency_proofs/"))
                    .ToListAsync();
                    
                _logger.LogInformation($"Found {usersWithLegacyDocs.Count} users with legacy documents");
                
                if (usersWithLegacyDocs.Any())
                {
                    foreach (var user in usersWithLegacyDocs)
                    {
                        // Check if a document already exists in UserDocuments
                        try
                        {
                            bool documentExists = await _context.UserDocuments
                                .AnyAsync(d => d.UserId == user.Id);
                                
                            if (!documentExists)
                            {
                                _logger.LogInformation($"Migrating legacy document for user {user.UserName}");
                                
                                // Get file details
                                string? filePath = user.ProfilePicture;
                                string? fileName = Path.GetFileName(filePath);
                                
                                // Get the physical path to check file existence and size
                                string webRootPath = _environment.WebRootPath;
                                string physicalPath = Path.Combine(webRootPath, filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                                
                                long fileSize = 0;
                                bool fileExists = false;
                                
                                // Check if file exists and get its size
                                if (System.IO.File.Exists(physicalPath))
                                {
                                    var fileInfo = new FileInfo(physicalPath);
                                    fileSize = fileInfo.Length;
                                    fileExists = true;
                                    _logger.LogInformation($"Found physical file at {physicalPath}, size: {fileSize} bytes");
                                }
                                else
                                {
                                    _logger.LogWarning($"Physical file not found at path: {physicalPath}");
                                }
                                
                                // Create document entry even if physical file might be missing
                                var userDoc = new UserDocument
                                {
                                    UserId = user.Id,
                                    FileName = string.IsNullOrEmpty(fileName) ? "unknown.pdf" : fileName,
                                    FilePath = string.IsNullOrEmpty(filePath) ? "" : filePath,
                                    Status = fileExists ? "Pending" : "Missing",
                                    FileSize = fileSize,
                                    ContentType = string.IsNullOrEmpty(fileName) ? "application/octet-stream" : GetContentType(Path.GetExtension(fileName)),
                                    UploadDate = DateTime.UtcNow,
                                    ApprovedBy = ""
                                };
                                
                                // Save the document to the table
                                _context.UserDocuments.Add(userDoc);
                                
                                // Don't clear ProfilePicture until we're sure the document is saved
                                _logger.LogInformation($"Created UserDocument record for user {user.UserName}");
                            }
                        }
                        catch (Exception docEx)
                        {
                            _logger.LogError(docEx, $"Error checking or creating document for user {user.UserName}");
                            // Continue with next user
                        }
                    }
                    
                    // Save all changes
                    await _context.SaveChangesAsync();
                    
                    // Now clear the ProfilePicture fields after successful save
                    foreach (var user in usersWithLegacyDocs)
                    {
                        bool documentExists = await _context.UserDocuments
                            .AnyAsync(d => d.UserId == user.Id);
                            
                        if (documentExists)
                        {
                            user.ProfilePicture = null;
                            await _userManager.UpdateAsync(user);
                            _logger.LogInformation($"Cleared ProfilePicture for user {user.UserName} after successful migration");
                        }
                    }
                    
                    _logger.LogInformation("Legacy document migration completed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error migrating legacy documents");
                // Don't throw - this is a best-effort migration
            }
        }

        public async Task<IActionResult> OnPostBatchApproveAsync([FromBody] BatchOperationModel model)
        {
            if (model?.UserIds == null || !model.UserIds.Any())
            {
                return new JsonResult(new BatchOperationResponse 
                { 
                    Success = false, 
                    Message = "No users selected for approval"
                });
            }

            _logger.LogInformation($"Batch approval requested for {model.UserIds.Count} users");
            
            var response = new BatchOperationResponse
            {
                Success = true,
                Message = "Batch approval successful",
                ProcessedCount = 0,
                FailedCount = 0,
                Errors = new List<string>()
            };

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return new JsonResult(new BatchOperationResponse 
                { 
                    Success = false, 
                    Message = "Authentication error. Please login again."
                });
            }

            foreach (var userId in model.UserIds)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        response.FailedCount++;
                        response.Errors.Add($"User not found with ID: {userId}");
                        continue;
                    }

                    if (user.Status != "Pending")
                    {
                        response.FailedCount++;
                        response.Errors.Add($"User {user.UserName} is not in pending status");
                        continue;
                    }

                    // Update user status
                    user.Status = "Verified";
                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        // Update document status if exists
                        await UpdateDocumentStatusAsync(userId, "Approved", currentUser.Id);
                        
                        // Send notification to user using CreateNotificationForUserAsync
                        await _notificationService.CreateNotificationForUserAsync(
                            userId: userId,
                            title: "Account Approved",
                            message: "Your account has been verified. You can now access all features of the system.",
                            type: "Success",
                            link: "/Index"
                        );

                        response.ProcessedCount++;
                    }
                    else
                    {
                        response.FailedCount++;
                        response.Errors.Add($"Failed to update status for {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error approving user with ID: {userId}");
                    response.FailedCount++;
                    response.Errors.Add($"Error processing user {userId}: {ex.Message}");
                }
            }

            if (response.FailedCount > 0)
            {
                response.Success = response.ProcessedCount > 0;
                response.Message = $"Approved {response.ProcessedCount} users. Failed to approve {response.FailedCount} users.";
            }

            return new JsonResult(response);
        }

        public async Task<IActionResult> OnPostBatchRejectAsync([FromBody] BatchOperationModel model)
        {
            if (model?.UserIds == null || !model.UserIds.Any())
            {
                return new JsonResult(new BatchOperationResponse 
                { 
                    Success = false, 
                    Message = "No users selected for rejection"
                });
            }

            _logger.LogInformation($"Batch rejection requested for {model.UserIds.Count} users");
            
            var response = new BatchOperationResponse
            {
                Success = true,
                Message = "Batch rejection successful",
                ProcessedCount = 0,
                FailedCount = 0,
                Errors = new List<string>()
            };

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return new JsonResult(new BatchOperationResponse 
                { 
                    Success = false, 
                    Message = "Authentication error. Please login again."
                });
            }

            foreach (var userId in model.UserIds)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        response.FailedCount++;
                        response.Errors.Add($"User not found with ID: {userId}");
                        continue;
                    }

                    if (user.Status != "Pending")
                    {
                        response.FailedCount++;
                        response.Errors.Add($"User {user.UserName} is not in pending status");
                        continue;
                    }

                    // Update user status
                    user.Status = "Rejected";
                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        // Update document status if exists
                        await UpdateDocumentStatusAsync(userId, "Rejected", currentUser.Id);
                        
                        // Send notification to user using CreateNotificationForUserAsync
                        await _notificationService.CreateNotificationForUserAsync(
                            userId: userId,
                            title: "Account Rejected",
                            message: "Your account verification has been rejected. Please contact support for more information.",
                            type: "Danger",
                            link: "/Index"
                        );

                        response.ProcessedCount++;
                    }
                    else
                    {
                        response.FailedCount++;
                        response.Errors.Add($"Failed to update status for {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error rejecting user with ID: {userId}");
                    response.FailedCount++;
                    response.Errors.Add($"Error processing user {userId}: {ex.Message}");
                }
            }

            if (response.FailedCount > 0)
            {
                response.Success = response.ProcessedCount > 0;
                response.Message = $"Rejected {response.ProcessedCount} users. Failed to reject {response.FailedCount} users.";
            }

            return new JsonResult(response);
        }

        private async Task UpdateDocumentStatusAsync(string userId, string status, string approvedBy)
        {
            try
            {
                if (_context.UserDocuments == null)
                {
                    _logger.LogWarning("UserDocuments DbSet is null during document status update");
                    return;
                }

                var document = await _context.UserDocuments
                    .FirstOrDefaultAsync(d => d.UserId == userId);

                if (document != null)
                {
                    document.Status = status;
                    document.ApprovedBy = approvedBy;
                    document.ApprovedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Updated document status to {status} for user {userId}");
                }
                else
                {
                    _logger.LogInformation($"No document found for user {userId} during status update");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating document status for user {userId}");
                // Continue processing even if document update fails
            }
        }
    }
} 