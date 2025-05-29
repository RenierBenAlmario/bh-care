-- Identify and fix specific records with NULL string values that cause GetString errors
USE [Barangay];

-- First backup the table structure and data
IF OBJECT_ID('dbo.UserDocuments_Backup', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserDocuments_Backup];

SELECT * INTO [dbo].[UserDocuments_Backup] FROM [dbo].[UserDocuments];
PRINT 'UserDocuments table backed up to UserDocuments_Backup';

-- Print table statistics
PRINT 'Total records in UserDocuments:';
SELECT COUNT(*) FROM [dbo].[UserDocuments];

-- Look for records that could cause GetString() errors
PRINT 'Records with problematic string columns:';
SELECT * FROM [dbo].[UserDocuments]
WHERE [FilePath] IS NULL 
   OR [FileName] IS NULL 
   OR [ContentType] IS NULL 
   OR [Status] IS NULL
   OR [UserId] IS NULL
   OR [ApprovedBy] IS NULL;

-- Check for blank strings that may also be causing issues
PRINT 'Records with empty string columns:';
SELECT * FROM [dbo].[UserDocuments]
WHERE [FilePath] = '' 
   OR [FileName] = '' 
   OR [ContentType] = '' 
   OR [Status] = ''
   OR [UserId] = '';

-- Analyze the specific columns with potential issues
PRINT 'Column content sampling:';
SELECT DISTINCT 
    SUBSTRING([FilePath], 1, 20) as FilePath_Sample,
    SUBSTRING([FileName], 1, 20) as FileName_Sample,
    SUBSTRING([ContentType], 1, 20) as ContentType_Sample,
    SUBSTRING([Status], 1, 20) as Status_Sample,
    SUBSTRING([UserId], 1, 20) as UserId_Sample,
    SUBSTRING(ISNULL([ApprovedBy],'NULL'), 1, 20) as ApprovedBy_Sample
FROM [dbo].[UserDocuments];

-- Recreate the table with NOT NULL constraints where appropriate
PRINT 'Fixing NULL values in all string columns...';

-- Fix document FilePath
UPDATE [dbo].[UserDocuments] SET [FilePath] = '/uploads/unknown.pdf' WHERE [FilePath] IS NULL OR [FilePath] = '';

-- Fix FileName
UPDATE [dbo].[UserDocuments] SET [FileName] = 'unknown.pdf' WHERE [FileName] IS NULL OR [FileName] = '';

-- Fix ContentType
UPDATE [dbo].[UserDocuments] SET [ContentType] = 'application/pdf' WHERE [ContentType] IS NULL OR [ContentType] = '';

-- Fix Status
UPDATE [dbo].[UserDocuments] SET [Status] = 'Unknown' WHERE [Status] IS NULL OR [Status] = '';

-- Fix ApprovedBy - This can be NULL in database design, but empty string is better for C# code
UPDATE [dbo].[UserDocuments] SET [ApprovedBy] = '' WHERE [ApprovedBy] IS NULL;

-- Check if there are any remaining issues
PRINT 'Checking for any remaining NULL issues:';
SELECT * FROM [dbo].[UserDocuments]
WHERE [FilePath] IS NULL 
   OR [FileName] IS NULL 
   OR [ContentType] IS NULL 
   OR [Status] IS NULL
   OR [UserId] IS NULL
   OR [ApprovedBy] IS NULL; 