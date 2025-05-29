-- This script will ensure the VitalSigns table has the correct structure

-- First, test if we can insert data directly
BEGIN TRY
    INSERT INTO VitalSigns (PatientId, Temperature, BloodPressure, HeartRate, RespiratoryRate, SpO2, Weight, Height, RecordedAt, Notes)
    VALUES ('test-patient-id', 37.5, '120/80', 75, 18, 98.5, 70.5, 175.0, GETDATE(), 'Test vital signs');

    PRINT 'Successfully inserted a test record into VitalSigns table';
    
    -- Clean up the test record
    DELETE FROM VitalSigns WHERE PatientId = 'test-patient-id';
END TRY
BEGIN CATCH
    PRINT 'Error inserting into VitalSigns table: ' + ERROR_MESSAGE();
    
    -- Let's check if we need to recreate the table with proper structure
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'VitalSigns')
    BEGIN
        PRINT 'VitalSigns table exists, but has issues. Backing up and recreating.';
        
        -- Backup any existing data
        IF OBJECT_ID('tempdb..#VitalSignsBackup') IS NOT NULL
            DROP TABLE #VitalSignsBackup;
            
        SELECT * INTO #VitalSignsBackup FROM VitalSigns;
        
        -- Drop the existing table
        DROP TABLE VitalSigns;
        
        -- Create the table with proper structure
        CREATE TABLE VitalSigns (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            PatientId NVARCHAR(450) NOT NULL,
            Temperature DECIMAL(5,2) NULL,
            BloodPressure NVARCHAR(20) NULL,
            HeartRate INT NULL,
            RespiratoryRate INT NULL,
            SpO2 DECIMAL(5,2) NULL,
            Weight DECIMAL(5,2) NULL,
            Height DECIMAL(5,2) NULL,
            RecordedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
            Notes NVARCHAR(1000) NULL
        );
        
        -- Try to restore data if any existed
        IF (SELECT COUNT(*) FROM #VitalSignsBackup) > 0
        BEGIN
            PRINT 'Restoring data from backup';
            
            -- Insert data back
            INSERT INTO VitalSigns (PatientId, Temperature, BloodPressure, HeartRate, RespiratoryRate, SpO2, Weight, Height, RecordedAt, Notes)
            SELECT PatientId, Temperature, BloodPressure, HeartRate, RespiratoryRate, SpO2, Weight, Height, RecordedAt, Notes
            FROM #VitalSignsBackup;
        END
        
        -- Test insert again
        INSERT INTO VitalSigns (PatientId, Temperature, BloodPressure, HeartRate, RespiratoryRate, SpO2, Weight, Height, RecordedAt, Notes)
        VALUES ('test-patient-id', 37.5, '120/80', 75, 18, 98.5, 70.5, 175.0, GETDATE(), 'Test vital signs');
        
        PRINT 'Successfully recreated VitalSigns table and inserted test record';
        
        -- Clean up the test record
        DELETE FROM VitalSigns WHERE PatientId = 'test-patient-id';
    END
END CATCH

PRINT 'Script completed'; 