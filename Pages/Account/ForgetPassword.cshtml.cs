using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Barangay.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly ILogger<ForgotPasswordModel> _logger;
        private readonly IConfiguration _configuration;

        public ForgotPasswordModel(ILogger<ForgotPasswordModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [BindProperty]
        public string? Email { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError(string.Empty, "Email is required.");
                return Page();
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "";
            string resetToken = Guid.NewGuid().ToString();
            string userId = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    
                    // 🔥 FIX: Use the correct column name, e.g., "UserID"
                    string query = "SELECT UserId FROM Users WHERE Email = @Email";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", Email);
                        var result = await command.ExecuteScalarAsync();

                        if (result != null)
                        {
                            userId = result.ToString() ?? "";
                        }
                    }
                }

                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError(string.Empty, "Email not found.");
                    return Page();
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string insertTokenQuery = "UPDATE Users SET VerificationToken = @Token, TokenExpiry = @Expiry WHERE Email = @Email";


                    using (SqlCommand command = new SqlCommand(insertTokenQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Token", resetToken);
                        command.Parameters.AddWithValue("@Expiry", DateTime.UtcNow.AddHours(1));
                        command.Parameters.AddWithValue("@Email", Email);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                // TODO: Send email with reset link
                TempData["SuccessMessage"] = "A password reset link has been sent to your email.";
                return RedirectToPage("/Account/Login");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during password reset: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                return Page();
            }
        }
    }
}
