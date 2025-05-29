using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Barangay.Controllers
{
    public class TestsController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<TestsController> _logger;

        public TestsController(IWebHostEnvironment environment, ILogger<TestsController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public IActionResult UploadTest()
        {
            return View("~/Tests/TestFileUpload.html");
        }

        [HttpPost]
        public async Task<IActionResult> UploadTest(Microsoft.AspNetCore.Http.IFormFile testFile)
        {
            _logger.LogInformation($"File upload test received: {testFile?.FileName ?? "null"}");
            
            if (testFile == null || testFile.Length == 0)
            {
                return Content("File not selected or empty");
            }

            try
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "test");
                Directory.CreateDirectory(uploadsFolder); // Create directory if it doesn't exist
                
                string uniqueFileName = $"test_{Path.GetFileName(testFile.FileName)}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                _logger.LogInformation($"Saving file to: {filePath}");
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await testFile.CopyToAsync(fileStream);
                }
                
                return Content($"File uploaded successfully to {filePath}");
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error uploading file: {ex.Message}");
                return Content($"Error: {ex.Message}");
            }
        }
    }
} 