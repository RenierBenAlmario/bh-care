using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System;
using System.IO;
using System.Threading.Tasks;
using Barangay.Data;
using Microsoft.AspNetCore.Authorization;

namespace Barangay.Pages.Admin
{
    [Authorize(Roles = "Admin,System Administrator")]
    public class RepairUserNumbersModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RepairUserNumbersModel> _logger;

        public string Message { get; set; }
        public bool IsError { get; set; } = false;
        public bool ColumnExists { get; set; } = false;
        public bool TriggerExists { get; set; } = false;
        public int UsersWithZeroNumber { get; set; } = 0;
        public int TotalUsers { get; set; } = 0;

        public RepairUserNumbersModel(
            ApplicationDbContext context,
            ILogger<RepairUserNumbersModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            await CheckColumnStatusAsync();
            await CountUsersAsync();
        }

        public async Task<IActionResult> OnPostVerifyColumnAsync()
        {
            await CheckColumnStatusAsync();
            await CountUsersAsync();
            
            Message = ColumnExists 
                ? "UserNumber column exists." 
                : "UserNumber column does NOT exist and should be created.";
                
            IsError = !ColumnExists;
            
            return Page();
        }

        public async Task<IActionResult> OnPostCreateColumnAsync()
        {
            try
            {
                await CheckColumnStatusAsync();
                
                if (!ColumnExists)
                {
                    // Run SQL to create column and trigger
                    string sql = GetAddUserNumberScript();
                    await _context.Database.ExecuteSqlRawAsync(sql);
                    Message = "Successfully created UserNumber column and trigger!";
                }
                else
                {
                    Message = "UserNumber column already exists.";
                }
                
                await CheckColumnStatusAsync();
                await CountUsersAsync();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating UserNumber column");
                Message = $"Error creating column: {ex.Message}";
                IsError = true;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostRepairNumbersAsync()
        {
            try
            {
                await CheckColumnStatusAsync();
                
                if (ColumnExists)
                {
                    // Run SQL to repair user numbers
                    string sql = @"
                        -- Number users with UserNumber = 0
                        WITH NumberedUsers AS (
                            SELECT Id, ROW_NUMBER() OVER (ORDER BY CreatedAt) 
                            + ISNULL((SELECT MAX(UserNumber) FROM AspNetUsers WHERE UserNumber > 0), 0) AS RowNum
                            FROM AspNetUsers
                            WHERE UserNumber = 0
                        )
                        UPDATE AspNetUsers
                        SET UserNumber = NumberedUsers.RowNum
                        FROM NumberedUsers
                        WHERE AspNetUsers.Id = NumberedUsers.Id;
                    ";
                    
                    await _context.Database.ExecuteSqlRawAsync(sql);
                    Message = "Successfully repaired user numbers!";
                }
                else
                {
                    Message = "UserNumber column doesn't exist. Create it first.";
                    IsError = true;
                }
                
                await CheckColumnStatusAsync();
                await CountUsersAsync();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error repairing user numbers");
                Message = $"Error repairing user numbers: {ex.Message}";
                IsError = true;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostRunFullRepairAsync()
        {
            try
            {
                // Check column status first
                await CheckColumnStatusAsync();
                
                // Create column if needed
                if (!ColumnExists)
                {
                    string sql = GetAddUserNumberScript();
                    await _context.Database.ExecuteSqlRawAsync(sql);
                }
                
                // Repair user numbers
                string repairSql = @"
                    -- Number users with UserNumber = 0
                    WITH NumberedUsers AS (
                        SELECT Id, ROW_NUMBER() OVER (ORDER BY CreatedAt) 
                        + ISNULL((SELECT MAX(UserNumber) FROM AspNetUsers WHERE UserNumber > 0), 0) AS RowNum
                        FROM AspNetUsers
                        WHERE UserNumber = 0
                    )
                    UPDATE AspNetUsers
                    SET UserNumber = NumberedUsers.RowNum
                    FROM NumberedUsers
                    WHERE AspNetUsers.Id = NumberedUsers.Id;
                ";
                
                await _context.Database.ExecuteSqlRawAsync(repairSql);
                
                Message = "Full repair completed successfully!";
                
                await CheckColumnStatusAsync();
                await CountUsersAsync();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running full repair");
                Message = $"Error running full repair: {ex.Message}";
                IsError = true;
                return Page();
            }
        }

        private async Task CheckColumnStatusAsync()
        {
            // Check if column exists
            try
            {
                var connectionString = _context.Database.GetConnectionString();
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Check if column exists
                    using (var command = new SqlCommand(
                        "IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'UserNumber') SELECT 1 ELSE SELECT 0", 
                        connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        ColumnExists = Convert.ToBoolean(result);
                    }
                    
                    // Check if trigger exists
                    if (ColumnExists)
                    {
                        using (var command = new SqlCommand(
                            "IF EXISTS(SELECT * FROM sys.triggers WHERE name = 'TR_AspNetUsers_AssignUserNumber') SELECT 1 ELSE SELECT 0", 
                            connection))
                        {
                            var result = await command.ExecuteScalarAsync();
                            TriggerExists = Convert.ToBoolean(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking column status");
                Message = $"Error checking column status: {ex.Message}";
                IsError = true;
            }
        }

        private async Task CountUsersAsync()
        {
            // Count users
            try
            {
                var connectionString = _context.Database.GetConnectionString();
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Count total users
                    using (var command = new SqlCommand("SELECT COUNT(*) FROM AspNetUsers", connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        TotalUsers = Convert.ToInt32(result);
                    }
                    
                    // Count users with zero UserNumber (if column exists)
                    if (ColumnExists)
                    {
                        using (var command = new SqlCommand("SELECT COUNT(*) FROM AspNetUsers WHERE UserNumber = 0", connection))
                        {
                            var result = await command.ExecuteScalarAsync();
                            UsersWithZeroNumber = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting users");
                UsersWithZeroNumber = -1;
                TotalUsers = -1;
            }
        }

        private string GetAddUserNumberScript()
        {
            return @"
                -- Check if the column exists
                IF NOT EXISTS(SELECT * FROM sys.columns 
                              WHERE Name = N'UserNumber' AND Object_ID = Object_ID(N'AspNetUsers'))
                BEGIN
                    -- Add the UserNumber column
                    ALTER TABLE AspNetUsers ADD UserNumber INT NOT NULL DEFAULT 0;
                    
                    -- Number existing users
                    WITH NumberedUsers AS (
                        SELECT Id, ROW_NUMBER() OVER (ORDER BY CreatedAt) AS RowNum
                        FROM AspNetUsers
                    )
                    UPDATE AspNetUsers
                    SET UserNumber = NumberedUsers.RowNum
                    FROM NumberedUsers
                    WHERE AspNetUsers.Id = NumberedUsers.Id;
                    
                    -- Create trigger for new users
                    IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_AspNetUsers_AssignUserNumber')
                        DROP TRIGGER TR_AspNetUsers_AssignUserNumber;
                        
                    EXEC('
                    CREATE TRIGGER TR_AspNetUsers_AssignUserNumber
                    ON AspNetUsers
                    AFTER INSERT
                    AS
                    BEGIN
                        DECLARE @MaxUserNumber int;
                        
                        -- Get the current maximum UserNumber
                        SELECT @MaxUserNumber = ISNULL(MAX(UserNumber), 0) FROM AspNetUsers;
                        
                        -- Update the inserted records with incremented UserNumber
                        UPDATE AspNetUsers
                        SET UserNumber = @MaxUserNumber + ROW_NUMBER() OVER (ORDER BY CreatedAt)
                        FROM AspNetUsers
                        INNER JOIN inserted ON AspNetUsers.Id = inserted.Id
                        WHERE AspNetUsers.UserNumber = 0;
                    END
                    ');
                    
                    -- Add index for better performance
                    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name=''IX_AspNetUsers_UserNumber'' AND object_id = OBJECT_ID(''AspNetUsers''))
                    BEGIN
                        CREATE NONCLUSTERED INDEX [IX_AspNetUsers_UserNumber] ON [AspNetUsers] ([UserNumber])
                    END
                END
            ";
        }
    }
} 