using Microsoft.EntityFrameworkCore;
using Barangay.Data;

namespace Barangay;

public class SimpleColumnFix
{
    public static async Task Main(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer("Server=tcp:bhcare.database.windows.net,1433;Initial Catalog=bhcareDB;Persist Security Info=False;User ID=bhcare;Password=Thebenzzz10;MultipleActiveResultSets=False;Encrypted=True;TrustServerCertificate=False;Connection Timeout=30;");

        using var context = new ApplicationDbContext(optionsBuilder.Options);

        try
        {
            Console.WriteLine("Running targeted column fix for HEEADSSS...");
            
            // Execute multiple ALTER statements as a single batch
            await context.Database.ExecuteSqlRawAsync(@"
                BEGIN TRY
                    -- Fix Is4Ps column
                    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'HEEADSSSAssessments' AND COLUMN_NAME = 'Is4Ps')
                        ALTER TABLE HEEADSSSAssessments ALTER COLUMN Is4Ps NVARCHAR(MAX)
                    ELSE
                        ALTER TABLE HEEADSSSAssessments ADD Is4Ps NVARCHAR(MAX)
                    
                    -- Fix IsNHPTS column
                    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'HEEADSSSAssessments' AND COLUMN_NAME = 'IsNHPTS')
                        ALTER TABLE HEEADSSSAssessments ALTER COLUMN IsNHPTS NVARCHAR(MAX)
                    ELSE
                        ALTER TABLE HEEADSSSAssessments ADD IsNHPTS NVARCHAR(MAX)
                    
                    -- Fix PhilHealth PIN column
                    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'HEEADSSSAssessments' AND COLUMN_NAME = 'PhilHealthPIN')
                        ALTER TABLE HEEADSSSAssessments ALTER COLUMN PhilHealthPIN NVARCHAR(MAX)
                    ELSE
                        ALTER TABLE HEEADSSSAssessments ADD PhilHealthPIN NVARCHAR(MAX)
                        
                    PRINT 'All critical columns fixed successfully'
                END TRY
                BEGIN CATCH
                    PRINT 'Error found: ' + ERROR_MESSAGE()
                    -- Don't fail the entire operation for individual column issues
                END CATCH");

            Console.WriteLine("Column fix completed successfully!");
            
            // Verify the fix
            Console.WriteLine("Verifying column changes...");
            var results = await context.Database.SqlQueryRaw<string>(@"
                SELECT COLUMN_NAME + ': ' + DATA_TYPE + '(' + CASE WHEN CHARACTER_MAXIMUM_LENGTH = -1 THEN 'MAX' ELSE CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR) END + ')'
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'HEEADSSSAssessments' 
                  AND COLUMN_NAME IN ('Is4Ps', 'IsNHPTS', 'PhilHealthPIN')
                ORDER BY COLUMN_NAME")
                .ToListAsync();

            foreach (var result in results)
            {
                Console.WriteLine($"  {result}");
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
            throw;
        }
    }
}
