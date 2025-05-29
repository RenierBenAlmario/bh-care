-- Fix NULL DateTime values in AspNetUsers table
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

-- Fix NULL DateTime values in AspNetUsers table
BEGIN TRY
    -- Check if the columns exist and update only if they do
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'BirthDate')
    BEGIN
        UPDATE AspNetUsers 
        SET BirthDate = COALESCE(BirthDate, '1900-01-01')
        WHERE BirthDate IS NULL;
        PRINT 'Fixed NULL BirthDate values in AspNetUsers';
    END

    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'CreatedAt')
    BEGIN
        UPDATE AspNetUsers 
        SET CreatedAt = COALESCE(CreatedAt, GETDATE())
        WHERE CreatedAt IS NULL;
        PRINT 'Fixed NULL CreatedAt values in AspNetUsers';
    END

    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'LastActive')
    BEGIN
        UPDATE AspNetUsers 
        SET LastActive = COALESCE(LastActive, GETDATE())
        WHERE LastActive IS NULL;
        PRINT 'Fixed NULL LastActive values in AspNetUsers';
    END

    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'JoinDate')
    BEGIN
        UPDATE AspNetUsers 
        SET JoinDate = COALESCE(JoinDate, GETDATE())
        WHERE JoinDate IS NULL;
        PRINT 'Fixed NULL JoinDate values in AspNetUsers';
    END
END TRY
BEGIN CATCH
    PRINT 'Error fixing DateTime NULL values in AspNetUsers: ' + ERROR_MESSAGE();
END CATCH

-- Fix NULL DateTime values in Patients table
BEGIN TRY
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Patients' AND COLUMN_NAME = 'BirthDate')
    BEGIN
        UPDATE Patients
        SET BirthDate = COALESCE(BirthDate, '1900-01-01')
        WHERE BirthDate IS NULL;
        PRINT 'Fixed NULL BirthDate values in Patients';
    END
END TRY
BEGIN CATCH
    PRINT 'Error fixing Patients DateTime NULL values: ' + ERROR_MESSAGE();
END CATCH

-- Fix NULL DateTime values in MedicalRecords table
BEGIN TRY
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MedicalRecords' AND COLUMN_NAME = 'CreatedAt')
    BEGIN
        UPDATE MedicalRecords
        SET CreatedAt = COALESCE(CreatedAt, GETDATE())
        WHERE CreatedAt IS NULL;
        PRINT 'Fixed NULL CreatedAt values in MedicalRecords';
    END

    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MedicalRecords' AND COLUMN_NAME = 'UpdatedAt')
    BEGIN
        UPDATE MedicalRecords
        SET UpdatedAt = COALESCE(UpdatedAt, GETDATE())
        WHERE UpdatedAt IS NULL;
        PRINT 'Fixed NULL UpdatedAt values in MedicalRecords';
    END
END TRY
BEGIN CATCH
    PRINT 'Error fixing MedicalRecords DateTime NULL values: ' + ERROR_MESSAGE();
END CATCH

-- Fix NULL DateTime values in Appointments table
BEGIN TRY
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'AppointmentDate')
    BEGIN
        UPDATE Appointments
        SET AppointmentDate = COALESCE(AppointmentDate, GETDATE())
        WHERE AppointmentDate IS NULL;
        PRINT 'Fixed NULL AppointmentDate values in Appointments';
    END

    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'CreatedAt')
    BEGIN
        UPDATE Appointments
        SET CreatedAt = COALESCE(CreatedAt, GETDATE())
        WHERE CreatedAt IS NULL;
        PRINT 'Fixed NULL CreatedAt values in Appointments';
    END

    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'UpdatedAt')
    BEGIN
        UPDATE Appointments
        SET UpdatedAt = COALESCE(UpdatedAt, GETDATE())
        WHERE UpdatedAt IS NULL;
        PRINT 'Fixed NULL UpdatedAt values in Appointments';
    END
END TRY
BEGIN CATCH
    PRINT 'Error fixing Appointments DateTime NULL values: ' + ERROR_MESSAGE();
END CATCH 