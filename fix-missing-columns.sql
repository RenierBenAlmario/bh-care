-- SQL Script to add missing columns to AspNetUsers table
-- This addresses the errors reported in the application log

PRINT 'Adding missing columns to AspNetUsers table...'

-- Check if AgreedAt column exists and add it if it doesn't
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'AgreedAt')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [AgreedAt] DATETIME2 NULL;
    PRINT 'AgreedAt column added';
END
ELSE
BEGIN
    PRINT 'AgreedAt column already exists';
END

-- Check if HasAgreedToTerms column exists and add it if it doesn't
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'HasAgreedToTerms')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [HasAgreedToTerms] BIT NOT NULL DEFAULT (0);
    PRINT 'HasAgreedToTerms column added';
END
ELSE
BEGIN
    PRINT 'HasAgreedToTerms column already exists';
END

-- Check if MiddleName column exists and add it if it doesn't
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'MiddleName')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [MiddleName] NVARCHAR(MAX) NULL;
    PRINT 'MiddleName column added';
END
ELSE
BEGIN
    PRINT 'MiddleName column already exists';
END

-- Check if Status column exists and add it if it doesn't
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'Status')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [Status] NVARCHAR(50) NOT NULL DEFAULT ('Pending');
    PRINT 'Status column added';
END
ELSE
BEGIN
    PRINT 'Status column already exists';
END

-- Check if Suffix column exists and add it if it doesn't
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'Suffix')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [Suffix] NVARCHAR(50) NULL;
    PRINT 'Suffix column added';
END
ELSE
BEGIN
    PRINT 'Suffix column already exists';
END

PRINT 'All missing columns have been added to AspNetUsers table.' 