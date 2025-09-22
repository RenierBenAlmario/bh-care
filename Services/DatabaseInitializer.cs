using Microsoft.EntityFrameworkCore;
using Barangay.Data;

namespace Barangay.Services
{
    public class DatabaseInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(ApplicationDbContext context, ILogger<DatabaseInitializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                // Check if UrlTokens table exists by trying to query it
                bool tableExists = false;
                try
                {
                    await _context.Database.ExecuteSqlRawAsync("SELECT TOP 1 Id FROM UrlTokens");
                    tableExists = true;
                }
                catch
                {
                    tableExists = false;
                }

                if (!tableExists)
                {
                    _logger.LogInformation("UrlTokens table does not exist. Creating...");
                    
                    // Create the table using raw SQL
                    await _context.Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE [dbo].[UrlTokens](
                            [Id] [int] IDENTITY(1,1) NOT NULL,
                            [Token] [nvarchar](500) NOT NULL,
                            [ResourceType] [nvarchar](50) NOT NULL,
                            [ResourceId] [nvarchar](450) NOT NULL,
                            [OriginalUrl] [nvarchar](500) NOT NULL,
                            [CreatedAt] [datetime2](7) NOT NULL,
                            [ExpiresAt] [datetime2](7) NOT NULL,
                            [IsUsed] [bit] NOT NULL,
                            [UsedAt] [datetime2](7) NULL,
                            [UserAgent] [nvarchar](256) NULL,
                            [ClientIp] [nvarchar](45) NULL,
                            CONSTRAINT [PK_UrlTokens] PRIMARY KEY CLUSTERED ([Id] ASC)
                        )");

                    // Create indexes
                    await _context.Database.ExecuteSqlRawAsync(@"
                        CREATE UNIQUE NONCLUSTERED INDEX [IX_UrlTokens_Token] ON [dbo].[UrlTokens] ([Token] ASC)");
                    
                    await _context.Database.ExecuteSqlRawAsync(@"
                        CREATE NONCLUSTERED INDEX [IX_UrlTokens_ExpiresAt] ON [dbo].[UrlTokens] ([ExpiresAt] ASC)");
                    
                    await _context.Database.ExecuteSqlRawAsync(@"
                        CREATE NONCLUSTERED INDEX [IX_UrlTokens_IsUsed] ON [dbo].[UrlTokens] ([IsUsed] ASC)");
                    
                    await _context.Database.ExecuteSqlRawAsync(@"
                        CREATE NONCLUSTERED INDEX [IX_UrlTokens_ResourceId] ON [dbo].[UrlTokens] ([ResourceId] ASC)");
                    
                    await _context.Database.ExecuteSqlRawAsync(@"
                        CREATE NONCLUSTERED INDEX [IX_UrlTokens_ResourceType] ON [dbo].[UrlTokens] ([ResourceType] ASC)");

                    // Create foreign key constraint
                    await _context.Database.ExecuteSqlRawAsync(@"
                        ALTER TABLE [dbo].[UrlTokens] 
                        ADD CONSTRAINT [FK_UrlTokens_AspNetUsers_ResourceId] 
                        FOREIGN KEY([ResourceId]) REFERENCES [dbo].[AspNetUsers] ([Id]) 
                        ON DELETE NO ACTION");

                    _logger.LogInformation("UrlTokens table created successfully");
                }
                else
                {
                    _logger.LogInformation("UrlTokens table already exists");
                    
                    // Check if we need to fix column names
                    bool hasIpAddressColumn = false;
                    bool hasClientIpColumn = false;
                    
                    try
                    {
                        await _context.Database.ExecuteSqlRawAsync("SELECT IpAddress FROM UrlTokens WHERE 1=0");
                        hasIpAddressColumn = true;
                    }
                    catch
                    {
                        hasIpAddressColumn = false;
                    }
                    
                    try
                    {
                        await _context.Database.ExecuteSqlRawAsync("SELECT ClientIp FROM UrlTokens WHERE 1=0");
                        hasClientIpColumn = true;
                    }
                    catch
                    {
                        hasClientIpColumn = false;
                    }
                    
                    if (hasIpAddressColumn && !hasClientIpColumn)
                    {
                        _logger.LogInformation("Fixing column name from IpAddress to ClientIp...");
                        await _context.Database.ExecuteSqlRawAsync(@"
                            EXEC sp_rename 'UrlTokens.IpAddress', 'ClientIp', 'COLUMN'");
                        _logger.LogInformation("Column name fixed successfully");
                    }
                    else if (!hasIpAddressColumn && !hasClientIpColumn)
                    {
                        _logger.LogInformation("Adding missing ClientIp column...");
                        await _context.Database.ExecuteSqlRawAsync(@"
                            ALTER TABLE [dbo].[UrlTokens] 
                            ADD [ClientIp] [nvarchar](45) NULL");
                        _logger.LogInformation("ClientIp column added successfully");
                    }
                    else
                    {
                        _logger.LogInformation("UrlTokens table structure is correct");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing UrlTokens table");
                throw;
            }
        }
    }
}
