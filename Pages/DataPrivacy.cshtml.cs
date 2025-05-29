using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Barangay.Pages;

public class DataPrivacyModel : PageModel
{
    private readonly ILogger<DataPrivacyModel> _logger;

    public DataPrivacyModel(ILogger<DataPrivacyModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation("Data Privacy page accessed at {time}", DateTime.UtcNow);
    }
} 