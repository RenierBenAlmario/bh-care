-- Set proper options
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

-- Check if CreatedAt column exists in UserPermissions table
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[UserPermissions]') AND name = 'CreatedAt')
BEGIN
    PRINT 'CreatedAt column already exists in UserPermissions table';
END
ELSE
BEGIN
    -- Add CreatedAt column if it doesn't exist
    ALTER TABLE UserPermissions ADD CreatedAt datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
    PRINT 'Added CreatedAt column to UserPermissions table';
END

-- Check if UpdatedAt column exists in UserPermissions table
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[UserPermissions]') AND name = 'UpdatedAt')
BEGIN
    PRINT 'UpdatedAt column already exists in UserPermissions table';
END
ELSE
BEGIN
    -- Add UpdatedAt column if it doesn't exist
    ALTER TABLE UserPermissions ADD UpdatedAt datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
    PRINT 'Added UpdatedAt column to UserPermissions table';
END

-- Update the __EFMigrationsHistory table to mark the migrations as completed
IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20250529091817_UpdateSchema')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20250529091817_UpdateSchema', '8.0.2');
    PRINT 'Marked 20250529091817_UpdateSchema migration as completed';
END

IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20250529103232_AddGuardianInformationColumns')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20250529103232_AddGuardianInformationColumns', '8.0.2');
    PRINT 'Marked 20250529103232_AddGuardianInformationColumns migration as completed';
END

IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20250529114300_MakeDoctorIdNullable')
BEGIN
    -- Make DoctorId nullable in Appointments table
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Appointments]') AND name = 'DoctorId' AND is_nullable = 0)
    BEGIN
        ALTER TABLE Appointments ALTER COLUMN DoctorId nvarchar(450) NULL;
        PRINT 'Made DoctorId nullable in Appointments table';
    END
    
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20250529114300_MakeDoctorIdNullable', '8.0.2');
    PRINT 'Marked 20250529114300_MakeDoctorIdNullable migration as completed';
END

-- Add foreign key constraint for NCDRiskAssessments to Appointments
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_NCDRiskAssessments_Appointments_AppointmentId' AND parent_object_id = OBJECT_ID('NCDRiskAssessments'))
BEGIN
    -- Create index if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_NCDRiskAssessments_AppointmentId' AND object_id = OBJECT_ID('NCDRiskAssessments'))
    BEGIN
        CREATE INDEX IX_NCDRiskAssessments_AppointmentId ON NCDRiskAssessments (AppointmentId);
        PRINT 'Created index IX_NCDRiskAssessments_AppointmentId';
    END
    
    -- Add foreign key constraint
    ALTER TABLE NCDRiskAssessments 
    ADD CONSTRAINT FK_NCDRiskAssessments_Appointments_AppointmentId 
    FOREIGN KEY (AppointmentId) 
    REFERENCES Appointments (Id);
    PRINT 'Added foreign key constraint FK_NCDRiskAssessments_Appointments_AppointmentId';
END

-- Add foreign key constraint for Appointments to AspNetUsers (DoctorId)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointments_AspNetUsers_DoctorId' AND parent_object_id = OBJECT_ID('Appointments'))
BEGIN
    ALTER TABLE Appointments 
    ADD CONSTRAINT FK_Appointments_AspNetUsers_DoctorId 
    FOREIGN KEY (DoctorId) 
    REFERENCES AspNetUsers (Id);
    PRINT 'Added foreign key constraint FK_Appointments_AspNetUsers_DoctorId';
END

PRINT 'Database update completed successfully'; 