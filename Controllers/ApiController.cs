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

        [HttpPost("decrypt")]
        public async Task<IActionResult> DecryptFamilyRecords([FromBody] DecryptRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.FamilyId) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { message = "FamilyId and Password are required" });
                }

                // For now, return a mock response since the actual decryption logic needs to be implemented
                // This matches the expected response format from the frontend
                var response = new
                {
                    familyName = $"Family {request.FamilyId}",
                    contentHtml = GenerateMockContentHtml(request.FamilyId),
                    token = Guid.NewGuid().ToString(),
                    ttlSeconds = 300 // 5 minutes
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during decryption", error = ex.Message });
            }
        }

        private string GenerateMockContentHtml(string familyId)
        {
            return $@"
                <div class='decrypted-family-content'>
                    <div class='family-header'>
                        <h3>Family {familyId} Health Records</h3>
                        <p class='text-muted'>Successfully decrypted on {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
                    </div>
                    <div class='record-sections'>
                        <div class='record-section'>
                            <h4><i class='fas fa-syringe'></i> Immunization Records</h4>
                            <p>Sample immunization data for Family {familyId}</p>
                        </div>
                        <div class='record-section'>
                            <h4><i class='fas fa-user-md'></i> HEEADSSS Assessments</h4>
                            <p>Sample HEEADSSS data for Family {familyId}</p>
                        </div>
                        <div class='record-section'>
                            <h4><i class='fas fa-heartbeat'></i> NCD Risk Assessments</h4>
                            <p>Sample NCD data for Family {familyId}</p>
                        </div>
                    </div>
                </div>";
        }
    }

    public class DecryptRequest
    {
        public string FamilyId { get; set; }
        public string Password { get; set; }
    }
} 