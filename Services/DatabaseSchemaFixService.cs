using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Barangay.Services
{
    public interface IDatabaseSchemaFixService
    {
        Task FixNCDRiskAssessmentSchemaAsync();
    }

    public class DatabaseSchemaFixService : IDatabaseSchemaFixService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseSchemaFixService> _logger;

        public DatabaseSchemaFixService(IConfiguration configuration, ILogger<DatabaseSchemaFixService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task FixNCDRiskAssessmentSchemaAsync()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // List of columns to add if they don't exist
                var columnsToAdd = new[]
                {
                    ("Pangalan", "NVARCHAR(100) NULL"),
                    ("EatsProcessedFood", "BIT NOT NULL DEFAULT 0"),
                    ("HighSaltIntake", "BIT NOT NULL DEFAULT 0"),
                    ("AlcoholFrequency", "NVARCHAR(50) NULL"),
                    ("AlcoholConsumption", "NVARCHAR(50) NULL"),
                    ("ExerciseDuration", "NVARCHAR(50) NULL"),
                    ("SmokingStatus", "NVARCHAR(50) NULL"),
                    ("FamilyOtherDiseaseDetails", "NVARCHAR(MAX) NULL"),
                    ("RiskStatus", "NVARCHAR(50) NULL"),
                    ("ChestPain", "NVARCHAR(100) NULL"),
                    ("ChestPainLocation", "NVARCHAR(100) NULL"),
                    ("ChestPainValue", "INT NULL"),
                    ("HasDifficultyBreathing", "BIT NOT NULL DEFAULT 0"),
                    ("HasAsthma", "BIT NOT NULL DEFAULT 0"),
                    ("HasNoRegularExercise", "BIT NOT NULL DEFAULT 0")
                };

                foreach (var (columnName, columnDefinition) in columnsToAdd)
                {
                    try
                    {
                        // Check if column exists
                        var checkColumnSql = @"
                            SELECT COUNT(*) 
                            FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_NAME = 'NCDRiskAssessments' 
                            AND COLUMN_NAME = @ColumnName";

                        using var checkCommand = new SqlCommand(checkColumnSql, connection);
                        checkCommand.Parameters.AddWithValue("@ColumnName", columnName);
                        
                        var result = await checkCommand.ExecuteScalarAsync();
                        var columnExists = result != null && result != DBNull.Value && Convert.ToInt32(result) > 0;

                        if (!columnExists)
                        {
                            // Add the column
                            var addColumnSql = $"ALTER TABLE NCDRiskAssessments ADD {columnName} {columnDefinition}";
                            using var addCommand = new SqlCommand(addColumnSql, connection);
                            await addCommand.ExecuteNonQueryAsync();
                            
                            _logger.LogInformation($"Added missing column: {columnName}");
                        }
                        else
                        {
                            _logger.LogInformation($"Column {columnName} already exists");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error adding column {columnName}: {ex.Message}");
                    }
                }

                _logger.LogInformation("NCDRiskAssessment schema fix completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fixing NCDRiskAssessment schema: {ex.Message}");
                throw;
            }
        }
    }
}
