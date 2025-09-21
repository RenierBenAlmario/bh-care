-- VitalSigns Table String Conversion Migration Script
-- This script converts integer and decimal columns to nvarchar for proper encryption support
-- Run this script to update the VitalSigns table structure

USE [Barangay];
GO

-- Create backup table before making changes
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VitalSigns_Backup_StringConversion')
BEGIN
    SELECT * INTO VitalSigns_Backup_StringConversion FROM VitalSigns;
    PRINT 'Created VitalSigns_Backup_StringConversion table';
END
ELSE
BEGIN
    PRINT 'Backup table already exists';
END
GO

-- Add new nvarchar columns for the converted fields
ALTER TABLE VitalSigns 
ADD 
    Temperature_New NVARCHAR(50) NULL,
    HeartRate_New NVARCHAR(50) NULL,
    RespiratoryRate_New NVARCHAR(50) NULL,
    SpO2_New NVARCHAR(50) NULL,
    Weight_New NVARCHAR(50) NULL,
    Height_New NVARCHAR(50) NULL;
GO

PRINT 'Added new nvarchar columns to VitalSigns table';
GO

-- Copy and convert data from old columns to new columns
UPDATE VitalSigns 
SET 
    Temperature_New = CASE 
        WHEN Temperature IS NOT NULL THEN CAST(Temperature AS NVARCHAR(50))
        ELSE NULL 
    END,
    HeartRate_New = CASE 
        WHEN HeartRate IS NOT NULL THEN CAST(HeartRate AS NVARCHAR(50))
        ELSE NULL 
    END,
    RespiratoryRate_New = CASE 
        WHEN RespiratoryRate IS NOT NULL THEN CAST(RespiratoryRate AS NVARCHAR(50))
        ELSE NULL 
    END,
    SpO2_New = CASE 
        WHEN SpO2 IS NOT NULL THEN CAST(SpO2 AS NVARCHAR(50))
        ELSE NULL 
    END,
    Weight_New = CASE 
        WHEN Weight IS NOT NULL THEN CAST(Weight AS NVARCHAR(50))
        ELSE NULL 
    END,
    Height_New = CASE 
        WHEN Height IS NOT NULL THEN CAST(Height AS NVARCHAR(50))
        ELSE NULL 
    END;
GO

PRINT 'Data converted and copied to new columns';
GO

-- Drop old columns
ALTER TABLE VitalSigns DROP COLUMN Temperature;
ALTER TABLE VitalSigns DROP COLUMN HeartRate;
ALTER TABLE VitalSigns DROP COLUMN RespiratoryRate;
ALTER TABLE VitalSigns DROP COLUMN SpO2;
ALTER TABLE VitalSigns DROP COLUMN Weight;
ALTER TABLE VitalSigns DROP COLUMN Height;
GO

PRINT 'Old columns dropped';
GO

-- Rename new columns to original names
EXEC sp_rename 'VitalSigns.Temperature_New', 'Temperature', 'COLUMN';
EXEC sp_rename 'VitalSigns.HeartRate_New', 'HeartRate', 'COLUMN';
EXEC sp_rename 'VitalSigns.RespiratoryRate_New', 'RespiratoryRate', 'COLUMN';
EXEC sp_rename 'VitalSigns.SpO2_New', 'SpO2', 'COLUMN';
EXEC sp_rename 'VitalSigns.Weight_New', 'Weight', 'COLUMN';
EXEC sp_rename 'VitalSigns.Height_New', 'Height', 'COLUMN';
GO

PRINT 'New columns renamed to original names';
GO

-- Verify the table structure
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'VitalSigns' 
ORDER BY ORDINAL_POSITION;
GO

PRINT 'Migration completed successfully!';
PRINT 'All numeric fields have been converted to NVARCHAR for encryption support.';
PRINT 'Backup table VitalSigns_Backup_StringConversion contains the original data.';
GO

-- Optional: Add indexes for better performance on string columns
CREATE INDEX IX_VitalSigns_Temperature ON VitalSigns(Temperature);
CREATE INDEX IX_VitalSigns_HeartRate ON VitalSigns(HeartRate);
CREATE INDEX IX_VitalSigns_RespiratoryRate ON VitalSigns(RespiratoryRate);
CREATE INDEX IX_VitalSigns_SpO2 ON VitalSigns(SpO2);
CREATE INDEX IX_VitalSigns_Weight ON VitalSigns(Weight);
CREATE INDEX IX_VitalSigns_Height ON VitalSigns(Height);
GO

PRINT 'Performance indexes added for string columns';
GO

