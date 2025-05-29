using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System;

namespace Barangay.Pages.Account
{
    public class VerifyEmailModel : PageModel
    {
        private readonly ILogger<VerifyEmailModel> _logger;
        private readonly IConfiguration _configuration;

        public VerifyEmailModel(ILogger<VerifyEmailModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public string Message { get; set; } = string.Empty;

        public IActionResult OnGet(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                Message = "Invalid token.";
                return Page();
            }

            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("Database connection string is missing.");
                Message = "Internal server error. Please try again later.";
                return Page();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE Users SET EmailVerified = 1 WHERE VerificationToken = @Token AND EmailVerified = 0";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Token", token);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Message = "Email verified successfully!";
                        }
                        else
                        {
                            Message = "Invalid or expired token, or email already verified.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while verifying email: " + ex.Message);
                Message = "An error occurred while verifying your email.";
            }

            return Page();
        }
    }
}
