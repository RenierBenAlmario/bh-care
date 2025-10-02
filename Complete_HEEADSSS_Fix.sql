-- Complete HEEADSSS Assessment Type Mismatch Fix
-- Convert Age and AppointmentId from int to nvarchar for encryption

PRINT 'Starting final HEEADSSS Assessment type conversion...';

-- Rename AgeNew to Age
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'HEEADSSSAssessments' AND COLUMN_NAME = 'AgeNew')
BEGIN
    EXEC sp_rename '[dbo].[HEEADSSSAssessments].AgeNew', 'Age', 'COLUMN';
    PRINT 'Renamed AgeNew to Age';
END

-- Rename AppointmentIdNew to AppointmentId
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'HEEADSSSAssessments' AND COLUMN_NAME = 'AppointmentIdNew')
BEGIN
    EXEC sp_rename '[dbo].[HEEADSSSAssessments].AppointmentIdNew', 'AppointmentId', 'COLUMN';
    PRINT 'Renamed AppointmentIdNew to AppointmentId';
END

-- Verify the migration
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'HEEADSSSAssessments' 
    AND COLUMN_NAME IN ('Age', 'AppointmentId')
ORDER BY COLUMN_NAME;

PRINT 'HEEADSSS Assessment type conversion completed successfully!';
PRINT 'Age and AppointmentId are now nvarchar fields ready for encryption.';
