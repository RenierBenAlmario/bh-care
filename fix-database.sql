-- Enable QUOTED_IDENTIFIER
SET QUOTED_IDENTIFIER ON;
GO

-- Add the Name column to AspNetUsers table if it doesn't exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'Name')
BEGIN
    ALTER TABLE [AspNetUsers]
    ADD [Name] NVARCHAR(256) NOT NULL DEFAULT '';
    PRINT 'Name column added to AspNetUsers table';
END
ELSE
BEGIN
    PRINT 'Name column already exists in AspNetUsers table';
END

-- Check if FullName column exists and add it if not
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'FullName')
BEGIN
    ALTER TABLE [AspNetUsers]
    ADD [FullName] NVARCHAR(256) NOT NULL DEFAULT '';
    PRINT 'FullName column added to AspNetUsers table';
END
ELSE
BEGIN
    PRINT 'FullName column already exists in AspNetUsers table';
END

-- Update any NULL values in Name column
UPDATE [AspNetUsers]
SET [Name] = CONCAT(FirstName, ' ', LastName)
WHERE [Name] IS NULL OR [Name] = '';

-- Update any NULL values in FullName column
UPDATE [AspNetUsers]
SET [FullName] = [Name]
WHERE [FullName] IS NULL;

-- Make sure FullName and Name are synchronized
UPDATE [AspNetUsers]
SET [FullName] = [Name]
WHERE [FullName] <> [Name];

PRINT 'Database columns synchronized successfully'; 