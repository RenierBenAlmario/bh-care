using System;
using System.IO;
using System.Threading.Tasks;
using Barangay.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Barangay.Pages.User
{
    public class ExecuteSqlModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ExecuteSqlModel> _logger;

        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public ExecuteSqlModel(ApplicationDbContext context, ILogger<ExecuteSqlModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _logger.LogInformation("Starting SQL execution");
                
                // Add missing columns
                await _context.Database.ExecuteSqlRawAsync(@"
                    -- Add missing columns from the error message
                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'ChestPain')
                    BEGIN
                        ALTER TABLE NCDRiskAssessments ADD ChestPain NVARCHAR(100) NULL;
                    END

                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'ChestPainLocation')
                    BEGIN
                        ALTER TABLE NCDRiskAssessments ADD ChestPainLocation NVARCHAR(100) NULL;
                    END

                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'ChestPainValue')
                    BEGIN
                        ALTER TABLE NCDRiskAssessments ADD ChestPainValue INT NULL;
                    END

                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'HasAsthma')
                    BEGIN
                        ALTER TABLE NCDRiskAssessments ADD HasAsthma BIT NULL;
                    END

                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'HasDifficultyBreathing')
                    BEGIN
                        ALTER TABLE NCDRiskAssessments ADD HasDifficultyBreathing BIT NULL;
                    END

                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'HasNoRegularExercise')
                    BEGIN
                        ALTER TABLE NCDRiskAssessments ADD HasNoRegularExercise BIT NULL;
                    END

                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'HealthFacility')
                    BEGIN
                        ALTER TABLE NCDRiskAssessments ADD HealthFacility NVARCHAR(100) NULL;
                    END

                    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'HighSaltIntake')
                    BEGIN
                        ALTER TABLE NCDRiskAssessments ADD HighSaltIntake BIT NULL;
                    END

                    -- Make CancerType column nullable
                    ALTER TABLE NCDRiskAssessments ALTER COLUMN CancerType NVARCHAR(100) NULL;
                ");

                _logger.LogInformation("SQL executed successfully");
                Success = true;
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing SQL");
                Success = false;
                ErrorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    ErrorMessage += $" Inner exception: {ex.InnerException.Message}";
                }
                return Page();
            }
        }
    }
} 