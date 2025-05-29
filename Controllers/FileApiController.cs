using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Barangay.Controllers
{
    [Route("api/files")]
    [ApiController]
    [Authorize]
    public class FileApiController : ControllerBase
    {
        private readonly ILogger<FileApiController> _logger;
        private readonly string _uploadsDirectory;

        public FileApiController(ILogger<FileApiController> logger)
        {
            _logger = logger;
            _uploadsDirectory = Path.Combine("wwwroot", "uploads");
        }

        [HttpGet("{filename}")]
        public async Task<IActionResult> GetFile(string filename, [FromQuery] bool download = false, [FromQuery] bool view = false)
        {
            try
            {
                // Sanitize filename to prevent directory traversal
                filename = Path.GetFileName(filename);
                
                if (string.IsNullOrEmpty(filename))
                {
                    return BadRequest("Invalid filename");
                }

                // Check common locations where files might be stored
                string[] possiblePaths = {
                    Path.Combine(_uploadsDirectory, "residency_proofs", filename),
                    Path.Combine(_uploadsDirectory, "documents", filename),
                    Path.Combine(_uploadsDirectory, "uploads", filename),
                    Path.Combine(_uploadsDirectory, filename)
                };

                string? filePath = null;
                foreach (var path in possiblePaths)
                {
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);
                    if (System.IO.File.Exists(fullPath))
                    {
                        filePath = fullPath;
                        break;
                    }
                }

                if (filePath == null)
                {
                    _logger.LogWarning("File not found: {FileName}", filename);
                    return NotFound("File not found");
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var contentType = GetContentType(filePath);

                // If view parameter is true, open in browser
                if (view)
                {
                    return File(fileBytes, contentType);
                }

                // If download parameter is true, force download
                if (download)
                {
                    return File(fileBytes, contentType, filename);
                }

                // Default: return file for preview
                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving file: {FileName}", filename);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("info/{filename}")]
        public IActionResult GetFileInfo(string filename)
        {
            try
            {
                // Sanitize filename to prevent directory traversal
                filename = Path.GetFileName(filename);
                
                if (string.IsNullOrEmpty(filename))
                {
                    return BadRequest("Invalid filename");
                }

                // Check common locations where files might be stored
                string[] possiblePaths = {
                    Path.Combine(_uploadsDirectory, "residency_proofs", filename),
                    Path.Combine(_uploadsDirectory, "documents", filename),
                    Path.Combine(_uploadsDirectory, "uploads", filename),
                    Path.Combine(_uploadsDirectory, filename)
                };

                string? filePath = null;
                foreach (var path in possiblePaths)
                {
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);
                    if (System.IO.File.Exists(fullPath))
                    {
                        filePath = fullPath;
                        break;
                    }
                }

                if (filePath == null)
                {
                    return NotFound("File not found");
                }

                var fileInfo = new FileInfo(filePath);
                
                return Ok(new
                {
                    FileName = fileInfo.Name,
                    FileSize = fileInfo.Length,
                    ContentType = GetContentType(filePath),
                    LastModified = fileInfo.LastWriteTime
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving file info: {FileName}", filename);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        private string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".svg" => "image/svg+xml",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
    }
} 