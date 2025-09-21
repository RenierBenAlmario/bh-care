using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using Barangay.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Barangay.Pages.Admin
{
    [Authorize(Policy = "AccessAdminDashboard")]
    public class UserDetailsModel : AdminPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserDetailsModel> _logger;
        private readonly IDataEncryptionService _encryptionService;
        
        public UserDetailsModel(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService,
            ILogger<UserDetailsModel> logger,
            IDataEncryptionService encryptionService) 
            : base(notificationService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _encryptionService = encryptionService;
        }
        
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }
        
        public ApplicationUser UserDetails { get; set; }
        public List<UserDocument> UserDocuments { get; set; } = new();
        public GuardianInformation Guardian { get; set; }
        public List<string> UserRoles { get; set; } = new();
        public bool IsMinor { get; set; }
        public string ErrorMessage { get; set; }
        
        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                ErrorMessage = "No user ID provided";
                return Page();
            }
            
            // Load user with includes
            UserDetails = await _context.Users
                .Include(u => u.UserDocuments)
                .FirstOrDefaultAsync(u => u.Id == Id);
                
            if (UserDetails == null)
            {
                ErrorMessage = "User not found";
                return Page();
            }

            // Decrypt user data for authorized users
            UserDetails = UserDetails.DecryptSensitiveData(_encryptionService, User);
            
            // Manually decrypt Email since it's not marked with [Encrypted] attribute
            if (!string.IsNullOrEmpty(UserDetails.Email) && _encryptionService.IsEncrypted(UserDetails.Email))
            {
                UserDetails.Email = UserDetails.Email.DecryptForUser(_encryptionService, User);
            }
            
            // Manually decrypt PhoneNumber since it's not marked with [Encrypted] attribute
            if (!string.IsNullOrEmpty(UserDetails.PhoneNumber) && _encryptionService.IsEncrypted(UserDetails.PhoneNumber))
            {
                UserDetails.PhoneNumber = UserDetails.PhoneNumber.DecryptForUser(_encryptionService, User);
            }
            
            // Load user documents
            UserDocuments = UserDetails.UserDocuments?.ToList() ?? new List<UserDocument>();
            
            // Load user roles
            UserRoles = (await _userManager.GetRolesAsync(UserDetails)).ToList();
            
            // Check if user is a minor
            IsMinor = CalculateAge() < 18;
            
            // If minor, try to get guardian information
            if (IsMinor)
            {
                try
                {
                    Guardian = await _context.GuardianInformation
                        .AsNoTracking()
                        .Select(g => new GuardianInformation 
                        {
                            GuardianId = g.GuardianId,
                            UserId = g.UserId,
                            GuardianFirstName = g.GuardianFirstName ?? g.FirstName ?? "Unknown",
                            GuardianLastName = g.GuardianLastName ?? g.LastName ?? "Unknown",
                            ResidencyProofPath = g.ResidencyProofPath ?? "",
                            ConsentStatus = g.ConsentStatus ?? "Pending",
                            ProofType = g.ProofType ?? "GuardianResidencyProof",
                            CreatedAt = g.CreatedAt != default ? g.CreatedAt : DateTime.UtcNow
                        })
                        .FirstOrDefaultAsync(g => g.UserId == Id);
                    
                    _logger.LogInformation($"Loaded guardian information for user {Id}: {Guardian != null}");
                }
                catch (Exception ex)
                {
                    // Handle exception but continue loading the page
                    _logger.LogError(ex, $"Error loading guardian information for user {Id}");
                    Guardian = null;
                }
            }
            
            return Page();
        }
        
        public async Task<JsonResult> OnPostApproveAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Attempting to approve user with ID: {id}");
                
                // Find the user to approve
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"User not found with ID: {id}");
                    return new JsonResult(new { success = false, message = "User not found." });
                }
                
                // Get current admin
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return new JsonResult(new { success = false, message = "Admin user not found." });
                }
                
                // Update user status
                user.Status = "Verified";
                user.IsActive = true;
                
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    string errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    return new JsonResult(new { success = false, message = $"Failed to update user: {errors}" });
                }
                
                // Update document status if exists
                var documents = await _context.UserDocuments
                    .Where(d => d.UserId == id)
                    .ToListAsync();
                    
                foreach (var document in documents)
                {
                    document.Status = "Verified";
                    document.ApprovedAt = DateTime.UtcNow;
                    document.ApprovedBy = currentUser.Id;
                }
                
                // Assign patient role if needed
                if (!await _userManager.IsInRoleAsync(user, "PATIENT"))
                {
                    await _userManager.AddToRoleAsync(user, "PATIENT");
                }
                
                // Save changes
                await _context.SaveChangesAsync();
                
                // Create notification for user
                await _notificationService.CreateNotificationForUserAsync(
                    userId: id,
                    title: "Account Approved",
                    message: "Your account has been approved. You can now access all system features.",
                    type: "Success",
                    link: "/Index"
                );
                
                return new JsonResult(new { success = true, message = "User approved successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving user: {UserId}", id);
                return new JsonResult(new { success = false, message = "An error occurred while approving the user." });
            }
        }
        
        public async Task<JsonResult> OnPostRejectAsync(string id)
        {
            try
            {
                // Find the user to reject
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return new JsonResult(new { success = false, message = "User not found." });
                }
                
                // Update user status
                user.Status = "Rejected";
                var updateResult = await _userManager.UpdateAsync(user);
                
                if (!updateResult.Succeeded)
                {
                    string errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    return new JsonResult(new { success = false, message = $"Failed to update user: {errors}" });
                }
                
                // Update document status if exists
                var documents = await _context.UserDocuments
                    .Where(d => d.UserId == id)
                    .ToListAsync();
                    
                foreach (var document in documents)
                {
                    document.Status = "Rejected";
                }
                
                await _context.SaveChangesAsync();
                
                // Create notification for user
                await _notificationService.CreateNotificationForUserAsync(
                    userId: id,
                    title: "Account Rejected",
                    message: "Your account verification was not approved.",
                    type: "Danger",
                    link: "/Index"
                );
                
                return new JsonResult(new { success = true, message = "User rejected successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting user: {UserId}", id);
                return new JsonResult(new { success = false, message = "An error occurred while rejecting the user." });
            }
        }
        
        public int CalculateAge()
        {
            // Check if UserDetails is null or has no birth date
            if (UserDetails == null) 
                return 0;
            
            DateTime? birthDateNullable = DateTime.TryParse(UserDetails.BirthDate, out var parsedBirthDate) ? parsedBirthDate : null;
            if (birthDateNullable == null) 
                return 0;
                
            DateTime birthDate = birthDateNullable.Value;
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            
            // Adjust age if birthday hasn't occurred yet this year
            if (birthDate.Date > today.AddYears(-age))
                age--;
                
            return age;
        }
        
        public string GetStatusBadgeClass()
        {
            if (UserDetails == null)
                return "secondary";
                
            string status = UserDetails.Status;
            if (string.IsNullOrEmpty(status))
                return "secondary";
                
            return status.ToLower() switch
            {
                "verified" => "success",
                "pending" => "warning",
                "rejected" => "danger",
                "inactive" => "secondary",
                _ => "secondary"
            };
        }
        
        public string GetBirthDateAsString()
        {
            if (UserDetails == null)
                return "";
                
            DateTime? birthDateNullable = DateTime.TryParse(UserDetails.BirthDate, out var parsedBirthDate) ? parsedBirthDate : null;
            if (birthDateNullable == null)
                return "";
                
            return birthDateNullable.Value.ToString("MM/dd/yyyy");
        }
        
        public string GetLastActiveAsString()
        {
            if (UserDetails == null)
                return "";
                
            DateTime? lastActiveNullable = UserDetails.LastActive;
            if (lastActiveNullable == null)
                return "";
                
            return lastActiveNullable.Value.ToString("MM/dd/yyyy");
        }
        
        public string GetUserTypeDisplay()
        {
            if (UserDetails == null)
                return "Standard";
                
            // Use ToString() directly since UserType is not nullable
            string userTypeString = UserDetails.UserType.ToString();
            return string.IsNullOrEmpty(userTypeString) ? "Standard" : userTypeString;
        }
    }
} 