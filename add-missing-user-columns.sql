-- Add Missing Columns to AspNetUsers Table Script
-- This script adds columns that exist in the ApplicationUser model but are missing from the AspNetUsers table

PRINT 'Starting addition of missing columns to AspNetUsers table...';

-- Check and add MiddleName column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'MiddleName')
BEGIN
    ALTER TABLE AspNetUsers ADD MiddleName nvarchar(max) NULL;
    PRINT 'Added MiddleName column to AspNetUsers table.';
END
ELSE
BEGIN
    PRINT 'MiddleName column already exists in AspNetUsers table.';
END

-- Check and add Suffix column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Suffix')
BEGIN
    ALTER TABLE AspNetUsers ADD Suffix nvarchar(max) NULL;
    PRINT 'Added Suffix column to AspNetUsers table.';
END
ELSE
BEGIN
    PRINT 'Suffix column already exists in AspNetUsers table.';
END

-- Check and add Status column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'Status')
BEGIN
    ALTER TABLE AspNetUsers ADD Status nvarchar(max) NULL;
    PRINT 'Added Status column to AspNetUsers table.';
END
ELSE
BEGIN
    PRINT 'Status column already exists in AspNetUsers table.';
END

-- Check and add HasAgreedToTerms column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'HasAgreedToTerms')
BEGIN
    ALTER TABLE AspNetUsers ADD HasAgreedToTerms bit NOT NULL DEFAULT 0;
    PRINT 'Added HasAgreedToTerms column to AspNetUsers table.';
END
ELSE
BEGIN
    PRINT 'HasAgreedToTerms column already exists in AspNetUsers table.';
END

-- Check and add AgreedAt column
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'AgreedAt')
BEGIN
    ALTER TABLE AspNetUsers ADD AgreedAt datetime2 NULL;
    PRINT 'Added AgreedAt column to AspNetUsers table.';
END
ELSE
BEGIN
    PRINT 'AgreedAt column already exists in AspNetUsers table.';
END

-- Add the migration entry to EF Migrations History
DECLARE @MigrationId nvarchar(150) = '20250630000002_AddMissingUserColumns';
DECLARE @ProductVersion nvarchar(32) = '8.0.2';

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = @MigrationId)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (@MigrationId, @ProductVersion);
    
    PRINT 'Migration record added to __EFMigrationsHistory table.';
END
ELSE
BEGIN
    PRINT 'Migration record already exists in __EFMigrationsHistory table.';
END

PRINT 'Missing columns addition process completed.'; 