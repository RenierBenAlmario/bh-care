using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Barangay.Data;

namespace Barangay.Services
{
    /// <summary>
    /// This service runs at application startup and ensures the UserNumber column exists
    /// It runs before any other services to avoid "Invalid column name" errors
    /// </summary>
    public class UserNumberFixService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UserNumberFixService> _logger;
        private readonly IHostEnvironment _environment;
        private const int MaxRetries = 5;
        private const int RetryDelayMs = 1000;

        public UserNumberFixService(
            IServiceProvider serviceProvider,
            ILogger<UserNumberFixService> logger,
            IHostEnvironment environment)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _environment = environment;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting UserNumber fix service...");
            
            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    _logger.LogInformation($"Attempt {attempt} of {MaxRetries} to apply UserNumber fix");
                    
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    
                    // Check if UserNumber column exists
                    bool columnExists = false;
                    
                    using (var connection = new SqlConnection(dbContext.Database.GetConnectionString()))
                    {
                        await connection.OpenAsync(cancellationToken);
                        using var command = connection.CreateCommand();
                        command.CommandText = "IF EXISTS(SELECT * FROM sys.columns WHERE Name = N'UserNumber' AND Object_ID = Object_ID(N'AspNetUsers')) SELECT 1 ELSE SELECT 0";
                        var result = await command.ExecuteScalarAsync(cancellationToken);
                        columnExists = Convert.ToBoolean(result);
                    }
                    
                    if (!columnExists)
                    {
                        _logger.LogWarning("UserNumber column does not exist. Attempting to create it directly...");
                        
                        try
                        {
                            // Direct SQL approach without using migrations
                            await dbContext.Database.ExecuteSqlRawAsync(@"
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
                                    SET QUOTED_IDENTIFIER ON;
                                    SET ANSI_NULLS ON;
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
                                END
                            ", cancellationToken);
                            
                            // Check if it was added successfully
                            using (var connection = new SqlConnection(dbContext.Database.GetConnectionString()))
                            {
                                await connection.OpenAsync(cancellationToken);
                                using var checkCommand = connection.CreateCommand();
                                checkCommand.CommandText = "IF EXISTS(SELECT * FROM sys.columns WHERE Name = N'UserNumber' AND Object_ID = Object_ID(N'AspNetUsers')) SELECT 1 ELSE SELECT 0";
                                var checkResult = await checkCommand.ExecuteScalarAsync(cancellationToken);
                                bool columnAddedSuccessfully = Convert.ToBoolean(checkResult);
                                
                                if (columnAddedSuccessfully)
                                {
                                    _logger.LogInformation("UserNumber column added successfully");
                                }
                                else
                                {
                                    _logger.LogWarning("Could not add UserNumber column - will use fallback mechanism");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error trying to add UserNumber column directly. Will use fallback UserNumber");
                        }
                    }
                    else
                    {
                        _logger.LogInformation("UserNumber column already exists");

                        // ---------------------------------------------------------------------
                        // Ensure trigger was compiled with QUOTED_IDENTIFIER ON (SQL 1934 fix)
                        // ---------------------------------------------------------------------
                        await dbContext.Database.ExecuteSqlRawAsync(@"
                            IF EXISTS (
                                SELECT 1
                                FROM sys.triggers       t
                                JOIN sys.sql_modules    m ON t.object_id = m.object_id
                                WHERE t.name = N'TR_AspNetUsers_AssignUserNumber'
                                  AND m.uses_quoted_identifier = 0
                            )
                            BEGIN
                                PRINT 'Re-creating TR_AspNetUsers_AssignUserNumber with QUOTED_IDENTIFIER ON';

                                EXEC('DROP TRIGGER TR_AspNetUsers_AssignUserNumber;');

                                -- These SET options must be in a separate batch before CREATE TRIGGER
                                EXEC('SET QUOTED_IDENTIFIER ON; SET ANSI_NULLS ON;');

                                EXEC('CREATE TRIGGER TR_AspNetUsers_AssignUserNumber
ON AspNetUsers
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaxUserNumber int;
    SELECT @MaxUserNumber = ISNULL(MAX(UserNumber),0) FROM AspNetUsers;

    UPDATE u
    SET    UserNumber = @MaxUserNumber
          + ROW_NUMBER() OVER (ORDER BY i.CreatedAt)
    FROM   AspNetUsers u
    JOIN   inserted     i ON i.Id = u.Id
    WHERE  u.UserNumber = 0;
END
');
                            END
                        ");
                    }
                    
                    // Success - break out of retry loop
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error in UserNumber fix service (attempt {attempt} of {MaxRetries})");
                    
                    if (attempt < MaxRetries)
                    {
                        // Wait before retrying
                        await Task.Delay(RetryDelayMs, cancellationToken);
                    }
                }
            }
            
            return;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping UserNumber fix service...");
            return Task.CompletedTask;
        }
    }
} 