using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Barangay.Models;
using Barangay.Services;
using Barangay.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Antiforgery;
using System.Collections.Generic;

namespace Barangay.Pages.Account
{
    public static class HttpRequestExtensions
    {
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }

    public class SignUpModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SignUpModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IAntiforgery _antiforgery;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SignUpModel(
            UserManager<ApplicationUser> userManager,
            ILogger<SignUpModel> logger,
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            IAntiforgery antiforgery,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
            _environment = environment;
            _antiforgery = antiforgery;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

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

            [Required(ErrorMessage = "Age is required")]
            [Range(0, 120, ErrorMessage = "Age must be between 0 and 120")]
            [Display(Name = "Age")]
            public int Age { get; set; }
            
            [Required(ErrorMessage = "Birth date is required")]
            [Display(Name = "Birth Date")]
            [DataType(DataType.Date)]
            public DateTime BirthDate { get; set; }
            
            [Display(Name = "Guardian's First Name")]
            [StringLength(50, ErrorMessage = "Guardian's first name cannot exceed 50 characters.")]
            public string? GuardianFirstName { get; set; }
            
            [Display(Name = "Guardian's Last Name")]
            [StringLength(50, ErrorMessage = "Guardian's last name cannot exceed 50 characters.")]
            public string? GuardianLastName { get; set; }

            [Display(Name = "Guardian's Residency Proof")]
            public IFormFile? GuardianResidencyProof { get; set; }

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

            [Required(ErrorMessage = "Residency proof document is required")]
            [Display(Name = "Residency Proof")]
            public Microsoft.AspNetCore.Http.IFormFile ResidencyProof { get; set; }

            [Required(ErrorMessage = "You must agree to the data privacy terms")]
            [Display(Name = "Agree to Terms")]
            [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the data privacy terms")]
            public bool AgreeToTerms { get; set; } = false;

            [Required(ErrorMessage = "You must confirm your residency")]
            [Display(Name = "Confirm Residency")]
            [Range(typeof(bool), "true", "true", ErrorMessage = "You must confirm your residency in Barangay 161")]
            public bool ConfirmResidency { get; set; } = false;
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            
            // Initialize Input model with unchecked checkboxes
            Input = new InputModel
            {
                AgreeToTerms = false,
                ConfirmResidency = false
            };
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            return await ProcessRegistration(returnUrl);
        }

        public async Task<IActionResult> OnPostSubmitAsync(string returnUrl = null)
        {
            return await ProcessRegistration(returnUrl);
        }

        private async Task EnsureRoleExistsAndAssign(ApplicationUser user, string roleName)
        {
            try
            {
                // Check if role exists, create if it doesn't
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    _logger.LogInformation($"Role {roleName} does not exist. Creating it...");
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }

                // Assign role to user
                if (!await _userManager.IsInRoleAsync(user, roleName))
                {
                    var result = await _userManager.AddToRoleAsync(user, roleName);
                    if (!result.Succeeded)
                    {
                        _logger.LogWarning($"Failed to assign role {roleName} to user {user.Email}. Errors: {string.Join(", ", result.Errors)}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while managing role {roleName} for user {user.Email}");
                throw;
            }
        }

        private async Task<IActionResult> ProcessRegistration(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            // Calculate age using current date
            var today = DateTime.Today; // Use current date instead of hardcoded date
            var age = today.Year - Input.BirthDate.Year;
            if (Input.BirthDate > today.AddYears(-age)) age--;

            // File extension variables
            string fileExtension = null;
            string guardianFileExtension = null;
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };

            // Validate guardian information for users under 18
            if (age < 18)
            {
                if (string.IsNullOrWhiteSpace(Input.GuardianFirstName) || 
                    string.IsNullOrWhiteSpace(Input.GuardianLastName) || 
                    Input.GuardianResidencyProof == null)
                {
                    ModelState.AddModelError(string.Empty, "Guardian information is required for users under 18.");
                    return Page();
                }

                // Validate file type
                guardianFileExtension = Path.GetExtension(Input.GuardianResidencyProof.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(guardianFileExtension))
                {
                    ModelState.AddModelError(string.Empty, "Invalid file type. Please upload a JPG, JPEG, PNG, or PDF file.");
                    return Page();
                }
            }

            try
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Username,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    MiddleName = Input.MiddleName ?? "",
                    LastName = Input.LastName,
                    Suffix = Input.Suffix ?? "",
                    PhoneNumber = Input.ContactNumber,
                    BirthDate = Input.BirthDate,
                    CreatedAt = DateTime.UtcNow,
                    HasAgreedToTerms = Input.AgreeToTerms,
                    AgreedAt = DateTime.UtcNow,
                    Name = $"{Input.FirstName} {Input.LastName}"
                };
                
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {user.Email} created successfully");

                    // Save residency proof document for all users
                    if (Input.ResidencyProof != null)
                    {
                        try
                        {
                            // Ensure the uploads directory exists
                            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "residency_proofs");
                            Directory.CreateDirectory(uploadsFolder);

                            // Create unique filename with user ID and timestamp
                            fileExtension = Path.GetExtension(Input.ResidencyProof.FileName).ToLowerInvariant();
                            var uniqueFileName = $"{user.Id}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                            var relativePath = $"/uploads/residency_proofs/{uniqueFileName}";

                            // Save file to disk
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await Input.ResidencyProof.CopyToAsync(fileStream);
                            }

                            // Ensure UserDocuments table exists
                            try
                            {
                                // Check if UserDocuments table exists by trying to access it
                                bool tableExists = false;
                                try {
                                    tableExists = _context.Model.FindEntityType(typeof(UserDocument)) != null;
                                } catch {
                                    tableExists = false;
                                }
                                
                                if (!tableExists)
                                {
                                    _logger.LogWarning("UserDocuments table not found in model");
                                    
                                    // Store the file path in user's ProfilePicture field as fallback
                                    user.ProfilePicture = relativePath;
                                    await _userManager.UpdateAsync(user);
                                    _logger.LogInformation($"Saved file path to user's ProfilePicture: {relativePath}");
                                }
                                else
                                {
                                    // Create record in UserDocuments table
                                    var userDocument = new UserDocument
                                    {
                                        UserId = user.Id,
                                        FileName = Input.ResidencyProof.FileName,
                                        FilePath = relativePath,
                                        FileSize = Input.ResidencyProof.Length,
                                        ContentType = Input.ResidencyProof.ContentType,
                                        Status = "Pending",
                                        UploadDate = DateTime.UtcNow
                                    };

                                    _context.UserDocuments.Add(userDocument);
                                    await _context.SaveChangesAsync();
                                    
                                    _logger.LogInformation($"Saved residency proof document for user {user.Id}: {uniqueFileName}");
                                }
                            }
                            catch (Exception dbEx)
                            {
                                _logger.LogError(dbEx, "Error accessing UserDocuments table");
                                
                                // Store the file path in user's ProfilePicture field as fallback
                                user.ProfilePicture = relativePath;
                                await _userManager.UpdateAsync(user);
                                _logger.LogInformation($"Saved file path to user's ProfilePicture as fallback: {relativePath}");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error saving residency proof document for user {user.Id}");
                            // Continue registration process even if document saving fails
                        }
                    }

                    // Save guardian information and residency proof for users under 18
                    if (age < 18 && Input.GuardianResidencyProof != null)
                    {
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "guardian_proofs");
                        Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = $"{user.Id}_{DateTime.Now:yyyyMMddHHmmss}{guardianFileExtension}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await Input.GuardianResidencyProof.CopyToAsync(fileStream);
                        }

                        // Save guardian information in the database
                        var guardianInfo = new GuardianInformation
                        {
                            UserId = user.Id,
                            FirstName = Input.GuardianFirstName,
                            LastName = Input.GuardianLastName,
                            ResidencyProofPath = $"/uploads/guardian_proofs/{uniqueFileName}",
                            CreatedAt = DateTime.UtcNow
                        };

                        await _context.GuardianInformation.AddAsync(guardianInfo);
                        await _context.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] = "Registration submitted for admin approval. You will be able to log in after your account is approved.";
                    return RedirectToPage("/Account/Login");
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
    }
}
