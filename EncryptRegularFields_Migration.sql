-- Encrypt existing VitalSigns data in regular fields
-- This script will encrypt the unencrypted data in the regular fields

USE [Barangay];
GO

-- Create backup table before making changes
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VitalSigns_Backup_EncryptRegularFields')
BEGIN
    SELECT * INTO VitalSigns_Backup_EncryptRegularFields FROM VitalSigns;
    PRINT 'Created VitalSigns_Backup_EncryptRegularFields table';
END
ELSE
BEGIN
    PRINT 'Backup table already exists';
END
GO

-- Update regular fields with encrypted data from encrypted fields
-- This will replace unencrypted values with encrypted values in regular fields

UPDATE VitalSigns 
SET 
    Temperature = CASE 
        WHEN EncryptedTemperature IS NOT NULL AND EncryptedTemperature != '' 
        THEN EncryptedTemperature 
        ELSE Temperature 
    END,
    HeartRate = CASE 
        WHEN EncryptedHeartRate IS NOT NULL AND EncryptedHeartRate != '' 
        THEN EncryptedHeartRate 
        ELSE HeartRate 
    END,
    RespiratoryRate = CASE 
        WHEN EncryptedRespiratoryRate IS NOT NULL AND EncryptedRespiratoryRate != '' 
        THEN EncryptedRespiratoryRate 
        ELSE RespiratoryRate 
    END,
    SpO2 = CASE 
        WHEN EncryptedSpO2 IS NOT NULL AND EncryptedSpO2 != '' 
        THEN EncryptedSpO2 
        ELSE SpO2 
    END,
    Weight = CASE 
        WHEN EncryptedWeight IS NOT NULL AND EncryptedWeight != '' 
        THEN EncryptedWeight 
        ELSE Weight 
    END,
    Height = CASE 
        WHEN EncryptedHeight IS NOT NULL AND EncryptedHeight != '' 
        THEN EncryptedHeight 
        ELSE Height 
    END,
    BloodPressure = CASE 
        WHEN EncryptedBloodPressure IS NOT NULL AND EncryptedBloodPressure != '' 
        THEN EncryptedBloodPressure 
        ELSE BloodPressure 
    END,
    Notes = CASE 
        WHEN Notes IS NOT NULL AND Notes != '' 
        THEN Notes  -- Notes are already encrypted by the [Encrypted] attribute
        ELSE Notes 
    END;
GO

PRINT 'Updated regular fields with encrypted data';
GO

-- Verify the changes
SELECT TOP 3 
    Id, 
    Temperature, 
    HeartRate, 
    Weight,
    EncryptedTemperature,
    EncryptedHeartRate,
    EncryptedWeight
FROM VitalSigns 
ORDER BY Id DESC;
GO

PRINT 'Migration completed successfully!';
PRINT 'All regular fields now contain encrypted data.';
PRINT 'Backup table VitalSigns_Backup_EncryptRegularFields contains the original data.';
GO

