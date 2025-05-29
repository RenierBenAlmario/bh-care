using System.Threading.Tasks;
using Barangay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;

namespace Barangay.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<UserManagementController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(string? searchTerm = null, string? status = "all")
        {
            try
        {
                var query = _userManager.Users
                    .Include(u => u.UserDocuments)
                    .AsQueryable();

                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
            {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(u => 
                        u.FullName.ToLower().Contains(searchTerm) || 
                        u.Email.ToLower().Contains(searchTerm));
            }

                // Apply status filter
                if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
            {
                    query = query.Where(u => u.Status.ToLower() == status.ToLower());
            }

                var users = await query
                    .OrderByDescending(u => u.CreatedAt)
                .Select(u => new
                {
                    u.Id,
                        u.FullName,
                    u.Email,
                    u.PhoneNumber,
                        u.CreatedAt,
                        u.Status,
                        Documents = u.UserDocuments.Select(d => new
                        {
                            d.Id,
                            d.FileName,
                            d.FilePath,
                            d.ContentType
                        }).ToList()
                })
                .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return StatusCode(500, "An error occurred while getting users");
            }
        }

        [HttpPost("approve/{userId}")]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                // Update user status
                user.Status = "Verified";
                user.IsActive = true;
                
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest("Failed to update user status");
                }
                
                // Update any associated documents
                var documents = await _context.UserDocuments
                    .Where(d => d.UserId == userId)
                    .ToListAsync();
                    
                foreach (var doc in documents)
                {
                    doc.Status = "Verified";
                    _context.UserDocuments.Update(doc);
                }

                    await _context.SaveChangesAsync();
                
                return Ok(new { message = "User approved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving user {UserId}", userId);
                return StatusCode(500, "An error occurred while approving the user");
            }
        }

        [HttpPost("reject/{userId}")]
        public async Task<IActionResult> RejectUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                // Update user status
                user.Status = "Rejected";
                user.IsActive = false;
                
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest("Failed to update user status");
                }

                // Update any associated documents
                var documents = await _context.UserDocuments
                    .Where(d => d.UserId == userId)
                    .ToListAsync();

                foreach (var doc in documents)
                {
                    doc.Status = "Rejected";
                    _context.UserDocuments.Update(doc);
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "User rejected successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting user {UserId}", userId);
                return StatusCode(500, "An error occurred while rejecting the user");
            }
        }
    }
} 