-- SQL script to clean up sample data from health records
-- This removes data from Vital Signs and Medical History

BEGIN TRY
    BEGIN TRANSACTION;
    
    PRINT 'Starting cleanup of sample health data...';
    
    -- Clear Vital Signs data
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'VitalSigns')
    BEGIN
        PRINT 'Clearing data from VitalSigns table...';
        DELETE FROM VitalSigns;
        PRINT 'Removed ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' records from VitalSigns table.';
    END
    ELSE
    BEGIN
        PRINT 'VitalSigns table not found.';
    END
    
    -- Clear Medical Records data
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'MedicalRecords')
    BEGIN
        PRINT 'Clearing data from MedicalRecords table...';
        DELETE FROM MedicalRecords;
        PRINT 'Removed ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' records from MedicalRecords table.';
    END
    ELSE
    BEGIN
        PRINT 'MedicalRecords table not found.';
    END
    
    -- Note: Laboratory Results appear to be generated dynamically in code
    -- and not stored in a dedicated database table
    
    COMMIT TRANSACTION;
    PRINT 'Successfully removed all sample health data.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
        
    PRINT 'Error occurred while cleaning up sample data:';
    PRINT ERROR_MESSAGE();
END CATCH 