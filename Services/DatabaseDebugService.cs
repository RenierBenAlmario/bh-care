using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;

namespace Barangay.Services
{
    public interface IDatabaseDebugService
    {
        Task<(bool success, string message)> TryDatabaseOperation(Func<Task> operation, string operationName);
        void LogDatabaseError(Exception ex, string operationName);
    }

    public class DatabaseDebugService : IDatabaseDebugService
    {
        private readonly ILogger<DatabaseDebugService> _logger;
        private readonly ApplicationDbContext _context;

        public DatabaseDebugService(ILogger<DatabaseDebugService> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<(bool success, string message)> TryDatabaseOperation(Func<Task> operation, string operationName)
        {
            try
            {
                await operation();
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Database operation '{operationName}' completed successfully");
                return (true, "Operation completed successfully");
            }
            catch (DbUpdateException ex)
            {
                LogDatabaseError(ex, operationName);
                return (false, $"Database update error: {GetUserFriendlyErrorMessage(ex)}");
            }
            catch (Exception ex)
            {
                LogDatabaseError(ex, operationName);
                return (false, $"An error occurred: {GetUserFriendlyErrorMessage(ex)}");
            }
        }

        public void LogDatabaseError(Exception ex, string operationName)
        {
            _logger.LogError(ex, $"Error during database operation '{operationName}'");

            if (ex is DbUpdateException dbEx)
            {
                _logger.LogError("Database error details:");
                foreach (var entry in dbEx.Entries)
                {
                    _logger.LogError($"Entity Type: {entry.Entity.GetType().Name}");
                    _logger.LogError($"State: {entry.State}");
                    foreach (var prop in entry.Properties)
                    {
                        _logger.LogError($"Property: {prop.Metadata.Name}, Value: {prop.CurrentValue}");
                    }
                }
            }
        }

        private string GetUserFriendlyErrorMessage(Exception ex)
        {
            if (ex is DbUpdateException dbEx)
            {
                // Check for common database errors
                if (dbEx.InnerException?.Message.Contains("duplicate") ?? false)
                {
                    return "A record with this information already exists.";
                }
                if (dbEx.InnerException?.Message.Contains("foreign key") ?? false)
                {
                    return "The referenced record does not exist.";
                }
                if (dbEx.InnerException?.Message.Contains("null") ?? false)
                {
                    return "Required information is missing.";
                }
            }

            // Default generic message
            return "There was a problem saving your information. Please try again.";
        }
    }
} 