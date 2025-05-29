using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Barangay.Pages.Account
{
    public class CheckboxTestModel : PageModel
    {
        private readonly ILogger<CheckboxTestModel> _logger;

        public CheckboxTestModel(ILogger<CheckboxTestModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public bool Checkbox1 { get; set; }

        [BindProperty]
        public bool Checkbox2 { get; set; }

        public void OnGet()
        {
            // Check if success parameter is present
            if (Request.Query.ContainsKey("success"))
            {
                TempData["SuccessMessage"] = "Form submitted successfully!";
            }
        }

        public IActionResult OnPost()
        {
            _logger.LogInformation($"Form submitted - Checkbox1: {Checkbox1}, Checkbox2: {Checkbox2}");

            if (!Checkbox1)
            {
                ModelState.AddModelError("Checkbox1", "You must agree to the terms and conditions");
            }

            if (!Checkbox2)
            {
                ModelState.AddModelError("Checkbox2", "You must confirm your information is correct");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            return RedirectToPage("./CheckboxTest", new { success = true });
        }
    }
}