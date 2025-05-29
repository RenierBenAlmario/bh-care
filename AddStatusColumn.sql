-- Set the correct options
SET QUOTED_IDENTIFIER ON;
GO

-- Check if Status column exists and drop it
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'Status')
BEGIN
    ALTER TABLE [dbo].[AspNetUsers] DROP COLUMN [Status];
    PRINT 'Existing Status column dropped.';
END

-- Add Status column with default value 'Pending'
ALTER TABLE [dbo].[AspNetUsers] ADD [Status] nvarchar(max) NOT NULL DEFAULT 'Pending';
PRINT 'Status column added successfully.';

-- Script to add Status column to AspNetUsers table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AspNetUsers') AND name = 'Status')
BEGIN
    ALTER TABLE dbo.AspNetUsers ADD Status nvarchar(50) NOT NULL DEFAULT 'Pending';
    PRINT 'Status column added to AspNetUsers table.';
END
ELSE
BEGIN
    PRINT 'Status column already exists in AspNetUsers table.';
END
GO 