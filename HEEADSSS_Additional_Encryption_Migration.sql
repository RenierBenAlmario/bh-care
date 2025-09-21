-- HEEADSSS Assessment Additional Encryption Migration
-- This script converts remaining int fields to encrypted string fields

PRINT 'Starting additional HEEADSSS Assessment encryption migration...';

-- Step 1: Add new encrypted string columns
ALTER TABLE [dbo].[HEEADSSSAssessments] ADD [AppointmentIdEncrypted] [nvarchar](max) NULL;
ALTER TABLE [dbo].[HEEADSSSAssessments] ADD [AgeEncrypted] [nvarchar](max) NULL;

PRINT 'Added new encrypted string columns for AppointmentId and Age';

-- Step 2: Migrate existing int data to encrypted strings
UPDATE [dbo].[HEEADSSSAssessments] SET [AppointmentIdEncrypted] = CAST([AppointmentId] AS nvarchar(max));
UPDATE [dbo].[HEEADSSSAssessments] SET [AgeEncrypted] = CAST([Age] AS nvarchar(max));

PRINT 'Migrated existing int data to encrypted strings';

-- Step 3: Drop old int columns
ALTER TABLE [dbo].[HEEADSSSAssessments] DROP COLUMN [AppointmentId];
ALTER TABLE [dbo].[HEEADSSSAssessments] DROP COLUMN [Age];

PRINT 'Dropped old int columns';

-- Step 4: Rename new columns to original names
EXEC sp_rename '[dbo].[HEEADSSSAssessments].[AppointmentIdEncrypted]', 'AppointmentId', 'COLUMN';
EXEC sp_rename '[dbo].[HEEADSSSAssessments].[AgeEncrypted]', 'Age', 'COLUMN';

PRINT 'Renamed new columns to original names';

-- Step 5: Verify the migration
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'HEEADSSSAssessments' 
    AND COLUMN_NAME IN ('Id', 'AppointmentId', 'HealthFacility', 'Age')
ORDER BY COLUMN_NAME;

PRINT 'Additional HEEADSSS Assessment encryption migration completed successfully!';
PRINT 'AppointmentId and Age are now encrypted string fields.';
PRINT 'Note: Id remains as int (primary key should not be encrypted).';
PRINT 'Note: HealthFacility is already nvarchar and will be encrypted by the [Encrypted] attribute.';
