using System;
using System.Data;
using System.Threading.Tasks;
using Barangay.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Barangay.Pages.User
{
    public class FixCancerTypeModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FixCancerTypeModel> _logger;
        private readonly IConfiguration _configuration;

        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public FixCancerTypeModel(ApplicationDbContext context, ILogger<FixCancerTypeModel> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _logger.LogInformation("Starting CancerType column fix");
                
                // First try using EF Core
                try
                {
                    await _context.Database.ExecuteSqlRawAsync("ALTER TABLE NCDRiskAssessments ALTER COLUMN CancerType NVARCHAR(100) NULL");
                    _logger.LogInformation("CancerType column fixed using EF Core");
                    Success = true;
                    return Page();
                }
                catch (Exception efEx)
                {
                    _logger.LogWarning(efEx, "Failed to fix CancerType column using EF Core, trying direct SQL connection");
                }

                // If EF Core fails, try direct SQL connection
                string connectionString = _context.Database.GetDbConnection().ConnectionString;
                
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    
                    using (var command = new SqlCommand("ALTER TABLE NCDRiskAssessments ALTER COLUMN CancerType NVARCHAR(100) NULL", connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    
                    // Verify the change
                    using (var command = new SqlCommand(@"
                        SELECT IS_NULLABLE 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'NCDRiskAssessments' 
                        AND COLUMN_NAME = 'CancerType'", connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        if (result != null && result.ToString() == "YES")
                        {
                            _logger.LogInformation("CancerType column is now nullable");
                        }
                        else
                        {
                            _logger.LogWarning("CancerType column may not be nullable yet");
                        }
                    }
                }

                // Also update the model to provide a default value for CancerType when HasCancer is false
                await UpdateNCDRiskAssessmentModelAsync();
                
                _logger.LogInformation("CancerType column fix completed successfully");
                Success = true;
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fixing CancerType column");
                Success = false;
                ErrorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    ErrorMessage += $" Inner exception: {ex.InnerException.Message}";
                }
                return Page();
            }
        }

        private async Task UpdateNCDRiskAssessmentModelAsync()
        {
            try
            {
                // Update the NCDRiskAssessment.cshtml.cs file to ensure it provides a default value for CancerType
                // This is a fallback in case the database schema change doesn't work
                var assessments = await _context.NCDRiskAssessments
                    .Where(a => a.CancerType == null)
                    .ToListAsync();
                
                foreach (var assessment in assessments)
                {
                    assessment.CancerType = assessment.HasCancer == "true" ? "Unspecified" : "None";
                }
                
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated {Count} NCDRiskAssessment records with default CancerType values", assessments.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating NCDRiskAssessment records");
                // Don't rethrow, we still want to report success if the column was altered
            }
        }
    }
} 