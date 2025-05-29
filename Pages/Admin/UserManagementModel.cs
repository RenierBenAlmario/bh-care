using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Barangay.Data;
using Barangay.Models;
using Microsoft.Data.SqlClient;

namespace Barangay.Pages.Admin
{
    [Authorize]
    public class UserManagementModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserManagementModel> _logger;

        public UserManagementModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<UserManagementModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public List<ApplicationUser> Users { get; set; } = new();
        public List<GuardianInformation> GuardianInformation { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Get all users with their guardian information
                Users = await _context.Users
                    .OrderByDescending(u => u.JoinDate)
                    .ToListAsync();

                // Get guardian information for users under 18
                var today = new DateTime(2025, 5, 29);
                var minBirthDate = today.AddYears(-18);
                
                var userIds = Users
                    .Where(u => u.BirthDate > minBirthDate)
                    .Select(u => u.Id)
                    .ToList();

                if (userIds.Any())
                {
                    // Use direct SQL to retrieve guardian information
                    GuardianInformation = new List<GuardianInformation>();
                    
                    foreach (var userId in userIds)
                    {
                        using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_context.Database.GetConnectionString()))
                        {
                            await connection.OpenAsync();
                            
                            var query = @"
                                SELECT GuardianId, UserId, GuardianFirstName, GuardianLastName, CreatedAt 
                                FROM GuardianInformation 
                                WHERE UserId = @UserId";
                            
                            using (var command = new Microsoft.Data.SqlClient.SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@UserId", userId);
                                
                                using (var reader = await command.ExecuteReaderAsync())
                                {
                                    if (await reader.ReadAsync())
                                    {
                                        var guardianInfo = new GuardianInformation
                                        {
                                            GuardianId = reader.GetInt32(0),
                                            UserId = reader.GetString(1),
                                            GuardianFirstName = reader.GetString(2),
                                            GuardianLastName = reader.GetString(3),
                                            CreatedAt = reader.GetDateTime(4)
                                            // Not loading ResidencyProof here to save memory
                                        };
                                        
                                        GuardianInformation.Add(guardianInfo);
                                    }
                                }
                            }
                        }
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user management page");
                return Page();
            }
        }

        [HttpGet]
        public async Task<IActionResult> OnGetResidencyProofAsync(string userId)
        {
            try
            {
                byte[] residencyProof = null;
                
                using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_context.Database.GetConnectionString()))
                {
                    await connection.OpenAsync();
                    
                    var query = "SELECT ResidencyProof FROM GuardianInformation WHERE UserId = @UserId";
                    
                    using (var command = new Microsoft.Data.SqlClient.SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        
                        var result = await command.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            residencyProof = (byte[])result;
                        }
                    }
                }

                if (residencyProof == null)
                {
                    return NotFound();
                }

                return File(residencyProof, "image/jpeg");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving residency proof for user {UserId}", userId);
                return NotFound();
            }
        }
    }
} 