using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Barangay.Pages;

public class TermsModel : PageModel
{
    private readonly ILogger<TermsModel> _logger;

    public TermsModel(ILogger<TermsModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation("Terms of Use page accessed at {time}", DateTime.UtcNow);
    }
} 