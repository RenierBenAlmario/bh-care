using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using BCrypt.Net;

namespace Barangay.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ResetPasswordModel> _logger;

        public ResetPasswordModel(IConfiguration configuration, ILogger<ResetPasswordModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [BindProperty]
        public string? Token { get; set; }

        [BindProperty]
        public string? NewPassword { get; set; }

        [BindProperty]
        public string? ConfirmPassword { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(NewPassword) || NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return Page();
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string getUserQuery = "SELECT Email FROM Users WHERE VerificationToken = @Token AND TokenExpiry > @Now";
                    string? email = null;
                    using (SqlCommand command = new SqlCommand(getUserQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Token", Token);
                        command.Parameters.AddWithValue("@Now", DateTime.UtcNow);

                        var result = await command.ExecuteScalarAsync();
                        if (result != null)
                        {
                            email = result.ToString();
                        }
                    }

                    if (string.IsNullOrEmpty(email))
                    {
                        ModelState.AddModelError(string.Empty, "Invalid or expired token.");
                        return Page();
                    }

                    string hashedPassword = HashPassword(NewPassword);

                    string updatePasswordQuery = "UPDATE Users SET PasswordHash = @Password, VerificationToken = NULL, TokenExpiry = NULL WHERE Email = @Email";
                    using (SqlCommand command = new SqlCommand(updatePasswordQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Password", hashedPassword);
                        command.Parameters.AddWithValue("@Email", email);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                TempData["SuccessMessage"] = "Your password has been reset successfully.";
                return RedirectToPage("/Account/Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for token: {Token}", Token);
                ModelState.AddModelError(string.Empty, "An error occurred while resetting your password.");
                return Page();
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    } // ✅ Added missing closing brace for the class
}
