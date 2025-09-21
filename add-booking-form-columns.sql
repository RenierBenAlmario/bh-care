-- Set proper options
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

-- Add necessary columns for the booking form if they don't exist

-- Add AttachmentPath column to Appointments table if it doesn't exist
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'AttachmentPath'
)
BEGIN
    ALTER TABLE Appointments ADD AttachmentPath NVARCHAR(500) NULL;
    PRINT 'Added AttachmentPath column to Appointments table';
END
ELSE
BEGIN
    PRINT 'AttachmentPath column already exists in Appointments table';
END

-- Add Reason column to Appointments table if it doesn't exist
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'Reason'
)
BEGIN
    ALTER TABLE Appointments ADD Reason NVARCHAR(MAX) NULL;
    PRINT 'Added Reason column to Appointments table';
END
ELSE
BEGIN
    PRINT 'Reason column already exists in Appointments table';
END

-- Add PreferredTime column to Appointments table if it doesn't exist
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'PreferredTime'
)
BEGIN
    ALTER TABLE Appointments ADD PreferredTime NVARCHAR(50) NULL;
    PRINT 'Added PreferredTime column to Appointments table';
END
ELSE
BEGIN
    PRINT 'PreferredTime column already exists in Appointments table';
END

-- Add Notes column to Appointments table if it doesn't exist
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'Notes'
)
BEGIN
    ALTER TABLE Appointments ADD Notes NVARCHAR(MAX) NULL;
    PRINT 'Added Notes column to Appointments table';
END
ELSE
BEGIN
    PRINT 'Notes column already exists in Appointments table';
END

PRINT 'Database update for booking form completed successfully.'; 