-- Apply the migration directly using SQL
-- This is useful if you can't use the EF Core tooling

-- First check if the migration has already been applied
IF NOT EXISTS (SELECT 1 FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = '20250511120000_AddAgreedToTermsColumns')
BEGIN
    -- Add the migration record to __EFMigrationsHistory
    INSERT INTO [dbo].[__EFMigrationsHistory] 
    ([MigrationId], [ProductVersion])
    VALUES 
    ('20250511120000_AddAgreedToTermsColumns', '5.0.17');
    
    PRINT 'Migration record added to __EFMigrationsHistory';
END
ELSE
BEGIN
    PRINT 'Migration record already exists in __EFMigrationsHistory';
END

-- Add the missing columns to AspNetUsers table
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'HasAgreedToTerms' AND Object_ID = Object_ID('dbo.AspNetUsers'))
BEGIN
    ALTER TABLE [dbo].[AspNetUsers]
    ADD [HasAgreedToTerms] BIT NOT NULL DEFAULT 0;
    
    PRINT 'Added HasAgreedToTerms column to AspNetUsers table';
END
ELSE
BEGIN
    PRINT 'HasAgreedToTerms column already exists in AspNetUsers table';
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'AgreedAt' AND Object_ID = Object_ID('dbo.AspNetUsers'))
BEGIN
    ALTER TABLE [dbo].[AspNetUsers] 
    ADD [AgreedAt] DATETIME2 NULL;
    
    PRINT 'Added AgreedAt column to AspNetUsers table';
END
ELSE
BEGIN
    PRINT 'AgreedAt column already exists in AspNetUsers table';
END

-- Print verification statement
PRINT 'Migration completed. Please verify both columns exist in AspNetUsers table.' 