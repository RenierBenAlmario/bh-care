using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Barangay.Models;
using Barangay.Data;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace Barangay.Pages.Account
{
    public class SignUpModel : PageModel
    {
        private readonly ILogger<SignUpModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SignUpModel(
            ILogger<SignUpModel> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Username is required")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
            [Display(Name = "Username")]
            [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Username can only contain letters, numbers, underscores, and hyphens.")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Email address is required")]
            [EmailAddress(ErrorMessage = "Invalid email address format")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "First name is required")]
            [Display(Name = "First Name")]
            [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
            public string FirstName { get; set; }

            [Display(Name = "Middle Name")]
            [StringLength(50, ErrorMessage = "Middle name cannot exceed 50 characters.")]
            public string? MiddleName { get; set; }

            [Required(ErrorMessage = "Last name is required")]
            [Display(Name = "Last Name")]
            [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
            public string LastName { get; set; }

            [Display(Name = "Suffix")]
            [StringLength(10, ErrorMessage = "Suffix cannot exceed 10 characters.")]
            public string? Suffix { get; set; }

            [Required(ErrorMessage = "Contact number is required")]
            [Display(Name = "Contact Number")]
            [RegularExpression(@"^(09|\+639)\d{9}$", ErrorMessage = "Contact number must be in format 09XXXXXXXXX or +639XXXXXXXXX")]
            public string ContactNumber { get; set; }

            [Required(ErrorMessage = "Birth date is required")]
            [DataType(DataType.Date)]
            [Display(Name = "Birth Date")]
            public DateTime BirthDate { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", 
                ErrorMessage = "Password must be at least 8 characters and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            // Guardian consent properties
            [Display(Name = "Guardian's First Name")]
            [StringLength(50, ErrorMessage = "Guardian's first name cannot exceed 50 characters.")]
            public string? GuardianFirstName { get; set; }
            
            [Display(Name = "Guardian's Last Name")]
            [StringLength(50, ErrorMessage = "Guardian's last name cannot exceed 50 characters.")]
            public string? GuardianLastName { get; set; }

            [Display(Name = "Guardian's Residency Proof")]
            public IFormFile? GuardianResidencyProof { get; set; }

            [Required(ErrorMessage = "Residency proof document is required")]
            [Display(Name = "Residency Proof")]
            public IFormFile ResidencyProof { get; set; }

            [Required(ErrorMessage = "You must agree to the data privacy terms")]
            [Display(Name = "Agree to Terms")]
            [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the data privacy terms")]
            public bool AgreeToTerms { get; set; }

            [Required(ErrorMessage = "You must confirm your residency")]
            [Display(Name = "Confirm Residency")]
            [Range(typeof(bool), "true", "true", ErrorMessage = "You must confirm your residency in Barangay 161")]
            public bool ConfirmResidency { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // If the user is under 18, try to pre-populate guardian information
            var userId = User?.Identity?.Name;
            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    // Use direct SQL to query for guardian information
                    using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
                    {
                        await connection.OpenAsync();
                        
                        var query = @"
                            SELECT GuardianFirstName, GuardianLastName 
                            FROM GuardianInformation 
                            WHERE UserId = @UserId";
                        
                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@UserId", userId);
                            
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    Input = new InputModel
                                    {
                                        GuardianFirstName = reader.GetString(0),
                                        GuardianLastName = reader.GetString(1)
                                    };
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error pre-populating guardian information for user {UserId}", userId);
                }
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Calculate age using the reference date (May 29, 2025)
                var referenceDate = new DateTime(2025, 5, 29);
                var age = referenceDate.Year - Input.BirthDate.Year;
                if (Input.BirthDate.Date > referenceDate.AddYears(-age)) age--;

                // Check if guardian consent is required
                if (age < 18)
                {
                    // Clear any existing guardian-related model errors
                    foreach (var key in ModelState.Keys.Where(k => k.Contains("Guardian")).ToList())
                    {
                        ModelState.Remove(key);
                    }

                    // Validate guardian information
                    if (string.IsNullOrWhiteSpace(Input.GuardianFirstName))
                    {
                        ModelState.AddModelError("Input.GuardianFirstName", "Guardian's first name is required for users under 18.");
                    }
                    
                    if (string.IsNullOrWhiteSpace(Input.GuardianLastName))
                    {
                        ModelState.AddModelError("Input.GuardianLastName", "Guardian's last name is required for users under 18.");
                    }
                    
                    if (Input.GuardianResidencyProof == null)
                    {
                        ModelState.AddModelError("Input.GuardianResidencyProof", "Guardian's residency proof is required for users under 18.");
                    }
                    else
                    {
                        // Validate file type
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                        var extension = Path.GetExtension(Input.GuardianResidencyProof.FileName).ToLowerInvariant();
                        
                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("Input.GuardianResidencyProof", 
                                "Invalid file type. Allowed types are: JPG, JPEG, PNG, PDF.");
                        }
                        
                        // Validate file size (max 5MB)
                        if (Input.GuardianResidencyProof.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("Input.GuardianResidencyProof", 
                                "File size exceeds the maximum limit of 5MB.");
                        }
                    }

                    if (!ModelState.IsValid)
                    {
                        return Page();
                    }
                }

                // Validate residency proof
                if (Input.ResidencyProof == null)
                {
                    ModelState.AddModelError("Input.ResidencyProof", "Residency proof document is required.");
                    return Page();
                }
                else
                {
                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                    var extension = Path.GetExtension(Input.ResidencyProof.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("Input.ResidencyProof", 
                            "Invalid file type. Allowed types are: JPG, JPEG, PNG, PDF.");
                        return Page();
                    }
                    
                    // Validate file size (max 5MB)
                    if (Input.ResidencyProof.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Input.ResidencyProof", 
                            "File size exceeds the maximum limit of 5MB.");
                        return Page();
                    }
                }

                // Create user
                var user = new ApplicationUser
                {
                    UserName = Input.Username,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    MiddleName = Input.MiddleName ?? string.Empty,
                    LastName = Input.LastName,
                    Suffix = Input.Suffix ?? string.Empty,
                    PhoneNumber = Input.ContactNumber,
                    BirthDate = Input.BirthDate,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    JoinDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Handle guardian consent if required
                    if (age < 18 && Input.GuardianResidencyProof != null)
                    {
                        try
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await Input.GuardianResidencyProof.CopyToAsync(memoryStream);
                                var guardianData = memoryStream.ToArray();
                                
                                // Use direct SQL instead of Entity Framework
                                // Check if the user already has guardian info
                                var checkSql = "SELECT COUNT(1) FROM GuardianInformation WHERE UserId = @UserId";
                                var checkParam = new SqlParameter("@UserId", user.Id);
                                var exists = await _context.Database.ExecuteSqlRawAsync(checkSql, checkParam) > 0;
                                
                                if (exists)
                                {
                                    // Update existing record
                                    var updateSql = @"
                                        UPDATE GuardianInformation 
                                        SET GuardianFirstName = @FirstName, 
                                            GuardianLastName = @LastName, 
                                            ResidencyProof = @Proof,
                                            CreatedAt = @CreatedAt
                                        WHERE UserId = @UserId";
                                    
                                    await _context.Database.ExecuteSqlRawAsync(updateSql,
                                        new SqlParameter("@FirstName", Input.GuardianFirstName),
                                        new SqlParameter("@LastName", Input.GuardianLastName),
                                        new SqlParameter("@Proof", guardianData),
                                        new SqlParameter("@CreatedAt", DateTime.UtcNow),
                                        new SqlParameter("@UserId", user.Id));
                                }
                                else
                                {
                                    // Insert new record
                                    var insertSql = @"
                                        INSERT INTO GuardianInformation (UserId, GuardianFirstName, GuardianLastName, ResidencyProof, CreatedAt)
                                        VALUES (@UserId, @FirstName, @LastName, @Proof, @CreatedAt)";
                                    
                                    await _context.Database.ExecuteSqlRawAsync(insertSql,
                                        new SqlParameter("@UserId", user.Id),
                                        new SqlParameter("@FirstName", Input.GuardianFirstName),
                                        new SqlParameter("@LastName", Input.GuardianLastName),
                                        new SqlParameter("@Proof", guardianData),
                                        new SqlParameter("@CreatedAt", DateTime.UtcNow));
                                }
                                
                                _logger.LogInformation("Guardian information saved successfully for user {UserId}", user.Id);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error saving guardian information for user {UserId}", user.Id);
                            // Continue with the registration process even if guardian info fails
                        }
                    }

                    // Add to User role
                    await _userManager.AddToRoleAsync(user, "User");

                    // Handle the user's residency proof
                    if (Input.ResidencyProof != null)
                    {
                        try
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await Input.ResidencyProof.CopyToAsync(memoryStream);
                                
                                // Determine file type based on content type
                                string fileType = DetermineFileType(Input.ResidencyProof.ContentType);
                                
                                var document = new UserDocument
                                {
                                    UserId = user.Id,
                                    FileName = Input.ResidencyProof.FileName,
                                    ContentType = Input.ResidencyProof.ContentType,
                                    FilePath = $"/uploads/{user.Id}/{Input.ResidencyProof.FileName}",
                                    FileType = fileType, // Set the file type explicitly
                                    FileSize = Input.ResidencyProof.Length,
                                    UploadDate = DateTime.UtcNow,
                                    Status = "Pending"
                                };
                                
                                _context.UserDocuments.Add(document);
                                await _context.SaveChangesAsync();
                                
                                _logger.LogInformation("Residency proof document saved successfully for user {UserId}", user.Id);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error saving residency proof document for user {UserId}", user.Id);
                            ModelState.AddModelError(string.Empty, "Error saving residency proof. Please try again.");
                            return Page();
                        }
                    }

                    // Redirect to waiting for approval page
                    return RedirectToPage("./WaitingForApproval");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
            }

            return Page();
        }
        
        // Helper method to determine file type based on content type
        private string DetermineFileType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
                return "document"; // Default value to avoid NULL
                
            if (contentType.StartsWith("image/"))
                return "image";
            else if (contentType == "application/pdf")
                return "pdf";
            else if (contentType.Contains("word") || contentType.Contains("doc"))
                return "document";
            else
                return "other"; // Default value for unknown types
        }
    }
} 