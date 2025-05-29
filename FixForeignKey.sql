-- Fix the duplicate foreign key constraint issue
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

-- Try to drop the existing constraint if it exists
BEGIN TRY
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_MedicalRecords_Patients_PatientId')
    BEGIN
        ALTER TABLE MedicalRecords DROP CONSTRAINT FK_MedicalRecords_Patients_PatientId;
        PRINT 'Dropped existing FK_MedicalRecords_Patients_PatientId constraint';
    END
END TRY
BEGIN CATCH
    PRINT 'Error dropping foreign key: ' + ERROR_MESSAGE();
END CATCH

-- Fix any orphaned records in MedicalRecords that don't have valid PatientId
BEGIN TRY
    DELETE FROM MedicalRecords 
    WHERE PatientId NOT IN (SELECT UserId FROM Patients);
    PRINT 'Removed orphaned MedicalRecords';
END TRY
BEGIN CATCH
    PRINT 'Error removing orphaned records: ' + ERROR_MESSAGE();
END CATCH

-- Recreate the foreign key constraint with the correct options
BEGIN TRY
    ALTER TABLE MedicalRecords 
    ADD CONSTRAINT FK_MedicalRecords_Patients_PatientId 
    FOREIGN KEY (PatientId) REFERENCES Patients(UserId) ON DELETE NO ACTION;
    PRINT 'Added FK_MedicalRecords_Patients_PatientId constraint';
END TRY
BEGIN CATCH
    PRINT 'Error recreating foreign key: ' + ERROR_MESSAGE();
END CATCH 