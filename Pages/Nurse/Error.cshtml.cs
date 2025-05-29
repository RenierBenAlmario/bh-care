using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Barangay.Pages.Nurse
{
    public class ErrorModel : PageModel
    {
        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        public string ErrorMessage { get; set; } = "An unexpected error occurred.";

        public void OnGet(string? message = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ErrorMessage = message;
            }
            
            _logger.LogError("Error page displayed with message: {Message}", ErrorMessage);
        }
    }
}