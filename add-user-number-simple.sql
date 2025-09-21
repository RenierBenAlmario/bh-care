-- Set proper options
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

-- Add UserNumber column to AspNetUsers table if it doesn't exist
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'UserNumber' AND Object_ID = Object_ID(N'AspNetUsers'))
BEGIN
    -- Add the UserNumber column
    ALTER TABLE AspNetUsers ADD UserNumber INT NOT NULL DEFAULT 0;
    PRINT 'Added UserNumber column to AspNetUsers table';
END
ELSE
BEGIN
    PRINT 'UserNumber column already exists in AspNetUsers table';
END
GO

PRINT 'Column addition completed successfully.'; 