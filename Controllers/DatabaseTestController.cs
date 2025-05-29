using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Barangay.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Barangay.Models;
using System.Linq;

namespace Barangay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseTestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DatabaseTestController> _logger;

        public DatabaseTestController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<DatabaseTestController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                _logger.LogInformation("Testing database connection");

                // Test basic database connection
                bool canConnect = await _context.Database.CanConnectAsync();
                _logger.LogInformation($"Can connect to database: {canConnect}");

                // Get database name
                var connection = _context.Database.GetDbConnection();
                string dbName = connection.Database;
                string dataSource = connection.DataSource;
                _logger.LogInformation($"Connected to database: {dbName} on server: {dataSource}");

                // Count users
                int userCount = await _userManager.Users.CountAsync();
                _logger.LogInformation($"Total user count: {userCount}");

                // Count pending users
                int pendingUsers = await _userManager.Users.CountAsync(u => u.Status == "Pending");
                _logger.LogInformation($"Pending user count: {pendingUsers}");

                // Check documents
                int docCount = await _context.UserDocuments.CountAsync();
                _logger.LogInformation($"Total document count: {docCount}");
                
                // Get connection string (safe version without credentials)
                string connectionString = connection.ConnectionString;
                string safeConnectionString = "Connection to: " + dataSource + "/" + dbName;

                // Check for missing columns using raw SQL
                var columnCheck = await _context.Database.ExecuteSqlRawAsync(
                    "IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'HasAgreedToTerms' AND Object_ID = Object_ID('dbo.AspNetUsers')) " +
                    "SELECT 1 ELSE SELECT 0");
                _logger.LogInformation($"HasAgreedToTerms column exists: {columnCheck == 1}");

                return Ok(new
                {
                    success = true,
                    canConnect,
                    databaseName = dbName,
                    dataSource,
                    userCount,
                    pendingUsers,
                    docCount,
                    connectionInfo = safeConnectionString
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing database connection");
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }
        
        [HttpGet("users")]
        public async Task<IActionResult> GetRecentUsers()
        {
            try
            {
                var recentUsers = await _userManager.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
                    .Select(u => new 
                    {
                        u.Id,
                        u.UserName,
                        u.Email,
                        u.Status,
                        u.IsActive,
                        u.CreatedAt
                    })
                    .ToListAsync();
                    
                return Ok(recentUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent users");
                return StatusCode(500, new { error = ex.Message });
            }
        }
        
        [HttpGet("documents")]
        public async Task<IActionResult> GetRecentDocuments()
        {
            try
            {
                var recentDocs = await _context.UserDocuments
                    .OrderByDescending(d => d.Id)
                    .Take(5)
                    .Select(d => new
                    {
                        d.Id,
                        d.UserId,
                        d.FileName,
                        d.Status
                    })
                    .ToListAsync();
                    
                return Ok(recentDocs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent documents");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
} 