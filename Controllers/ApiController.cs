using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.IO;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using Barangay.Extensions;
using System.Linq;
using System;
using Microsoft.AspNetCore.Hosting;

namespace Barangay.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly IDataEncryptionService _encryptionService;
        
        public ApiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment, IDataEncryptionService encryptionService)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
            _encryptionService = encryptionService;
        }
        
        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(int id)
        {
            // Get the document from the database
            var document = await _context.UserDocuments.FindAsync(id);
            
            if (document == null)
                return NotFound();
                
            // In a real app, we would get the file path from the database
            // For this example, we'll return sample files based on the file type
            string filePath;
            string contentType;
            
            if (document.ContentType?.ToLower().Contains("pdf") == true)
            {
                filePath = Path.Combine(_environment.WebRootPath, "images", "sample-document-preview.pdf");
                contentType = "application/pdf";
            }
            else if (document.ContentType?.ToLower().Contains("image") == true)
            {
                filePath = Path.Combine(_environment.WebRootPath, "images", "preview-placeholder.jpg");
                contentType = "image/jpeg";
            }
            else
            {
                // Default to PDF
                filePath = Path.Combine(_environment.WebRootPath, "images", "sample-document-preview.pdf");
                contentType = "application/pdf";
            }
            
            if (!System.IO.File.Exists(filePath))
                return NotFound();
                
            return PhysicalFile(filePath, contentType, document.FileName);
        }

        [HttpGet("staff/{id}")]
        public async Task<IActionResult> GetStaffDetails(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Staff member not found" });
                }

                // Decrypt user data for authorized users
                user = user.DecryptSensitiveData(_encryptionService, User);
                
                // Manually decrypt PhoneNumber since it's not marked with [Encrypted] attribute
                if (!string.IsNullOrEmpty(user.PhoneNumber) && _encryptionService.IsEncrypted(user.PhoneNumber))
                {
                    user.PhoneNumber = user.PhoneNumber.DecryptForUser(_encryptionService, User);
                }

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);
                string role = roles.FirstOrDefault() ?? "Unknown";

                // Create response object
                var staffDetails = new
                {
                    id = user.Id,
                    name = $"{user.FirstName} {user.LastName}",
                    email = user.Email,
                    phoneNumber = user.PhoneNumber,
                    role = role,
                    department = "General", // Default department
                    isActive = user.IsActive,
                    lastActive = user.LastActive,
                    joinDate = user.JoinDate,
                    specialization = user.Specialization
                };

                return Ok(staffDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving staff details", error = ex.Message });
            }
        }
    }
} 