-- Script to add missing columns to the Notifications table
-- Run this script to fix the "Invalid column name 'Link'" and "Invalid column name 'ReadAt'" errors

-- Add Link column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Notifications]') AND name = 'Link')
BEGIN
    ALTER TABLE [dbo].[Notifications] ADD [Link] nvarchar(255) NULL;
    PRINT 'Added Link column to Notifications table.';
END
ELSE
BEGIN
    PRINT 'Link column already exists in Notifications table.';
END

-- Add ReadAt column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Notifications]') AND name = 'ReadAt')
BEGIN
    ALTER TABLE [dbo].[Notifications] ADD [ReadAt] datetime2 NULL;
    PRINT 'Added ReadAt column to Notifications table.';
END
ELSE
BEGIN
    PRINT 'ReadAt column already exists in Notifications table.';
END

-- Update ReadAt values based on IsRead flag
UPDATE [dbo].[Notifications]
SET [ReadAt] = GETDATE()
WHERE [IsRead] = 1 AND [ReadAt] IS NULL;

PRINT 'Database migration completed successfully.'; 