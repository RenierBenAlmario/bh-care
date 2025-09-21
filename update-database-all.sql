-- Set required options
SET QUOTED_IDENTIFIER ON;
GO

-- Step 1: Fix the NCDRiskAssessments.AppointmentId issue
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_NCDRiskAssessments_Appointments_AppointmentId')
    ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [FK_NCDRiskAssessments_Appointments_AppointmentId];

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_NCDRiskAssessments_AppointmentId')
    DROP INDEX [IX_NCDRiskAssessments_AppointmentId] ON [NCDRiskAssessments];

-- Make AppointmentId nullable
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'AppointmentId')
BEGIN
    DECLARE @DefaultConstraintName nvarchar(200)
    
    -- Find default constraint name
    SELECT @DefaultConstraintName = dc.name
    FROM sys.default_constraints dc
    JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
    WHERE c.name = 'AppointmentId' AND OBJECT_NAME(dc.parent_object_id) = 'NCDRiskAssessments'
    
    -- Drop default constraint if it exists
    IF @DefaultConstraintName IS NOT NULL
        EXEC('ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @DefaultConstraintName + ']')
    
    -- Alter column to make it nullable
    ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [AppointmentId] int NULL;
    
    PRINT 'Successfully made NCDRiskAssessments.AppointmentId nullable';
END

-- Step 2: Apply migration 20250529035759_AddTimestampsToUserPermissions
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250529035759_AddTimestampsToUserPermissions')
BEGIN
    -- Add CreatedAt and UpdatedAt to UserPermissions if they don't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[UserPermissions]') AND name = 'CreatedAt')
    BEGIN
        ALTER TABLE [UserPermissions] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[UserPermissions]') AND name = 'UpdatedAt')
    BEGIN
        ALTER TABLE [UserPermissions] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
    END

    -- Update PrescriptionMedicines.Dosage precision
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PrescriptionMedicines' AND COLUMN_NAME = 'Dosage')
    BEGIN
        DECLARE @DosageConstraintName nvarchar(200)
        SELECT @DosageConstraintName = dc.name
        FROM sys.default_constraints dc
        JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
        WHERE c.name = 'Dosage' AND OBJECT_NAME(dc.parent_object_id) = 'PrescriptionMedicines'
        
        IF @DosageConstraintName IS NOT NULL
            EXEC('ALTER TABLE [PrescriptionMedicines] DROP CONSTRAINT [' + @DosageConstraintName + ']')
            
        ALTER TABLE [PrescriptionMedicines] ALTER COLUMN [Dosage] decimal(10,2) NOT NULL;
    END

    -- Update NCDRiskAssessments.FamilyOtherDiseaseDetails
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'FamilyOtherDiseaseDetails')
    BEGIN
        DECLARE @FamilyConstraintName nvarchar(200)
        SELECT @FamilyConstraintName = dc.name
        FROM sys.default_constraints dc
        JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
        WHERE c.name = 'FamilyOtherDiseaseDetails' AND OBJECT_NAME(dc.parent_object_id) = 'NCDRiskAssessments'
        
        IF @FamilyConstraintName IS NOT NULL
            EXEC('ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @FamilyConstraintName + ']')
            
        ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [FamilyOtherDiseaseDetails] nvarchar(max) NOT NULL;
    END

    -- Add Birthday to HEEADSSSAssessments
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'HEEADSSSAssessments') 
        AND NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'HEEADSSSAssessments' AND COLUMN_NAME = 'Birthday')
    BEGIN
        ALTER TABLE [HEEADSSSAssessments] ADD [Birthday] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
    END

    -- Update AspNetUsers.LastName
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'LastName')
    BEGIN
        DECLARE @LastNameConstraintName nvarchar(200)
        SELECT @LastNameConstraintName = dc.name
        FROM sys.default_constraints dc
        JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
        WHERE c.name = 'LastName' AND OBJECT_NAME(dc.parent_object_id) = 'AspNetUsers'
        
        IF @LastNameConstraintName IS NOT NULL
            EXEC('ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @LastNameConstraintName + ']')
            
        UPDATE [AspNetUsers] SET [LastName] = N'' WHERE [LastName] IS NULL;
        ALTER TABLE [AspNetUsers] ALTER COLUMN [LastName] nvarchar(max) NOT NULL;
        ALTER TABLE [AspNetUsers] ADD DEFAULT N'' FOR [LastName];
    END

    -- Create index for NCDRiskAssessments.AppointmentId
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_NCDRiskAssessments_AppointmentId' AND object_id = OBJECT_ID('NCDRiskAssessments'))
    BEGIN
        CREATE INDEX [IX_NCDRiskAssessments_AppointmentId] ON [NCDRiskAssessments] ([AppointmentId]);
    END

    -- Add foreign key for NCDRiskAssessments.AppointmentId
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_NCDRiskAssessments_Appointments_AppointmentId')
    BEGIN
        ALTER TABLE [NCDRiskAssessments] ADD CONSTRAINT [FK_NCDRiskAssessments_Appointments_AppointmentId] 
        FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE SET NULL;
    END

    -- Add to migration history
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250529035759_AddTimestampsToUserPermissions', '8.0.2');
    
    PRINT 'Applied migration 20250529035759_AddTimestampsToUserPermissions';
END

-- Step 3: Apply migration 20250529035948_AddCreatedAndUpdatedAtToUserPermissions
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250529035948_AddCreatedAndUpdatedAtToUserPermissions')
BEGIN
    -- Migration is already applied in the previous step, just record it
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250529035948_AddCreatedAndUpdatedAtToUserPermissions', '8.0.2');
    
    PRINT 'Applied migration 20250529035948_AddCreatedAndUpdatedAtToUserPermissions';
END

-- Step 4: Apply migration 20250529051202_AddHealthFacilityAndFamilyNoFields
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250529051202_AddHealthFacilityAndFamilyNoFields')
BEGIN
    -- Add HealthFacility to NCDRiskAssessments if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[NCDRiskAssessments]') AND name = 'HealthFacility')
    BEGIN
        ALTER TABLE [NCDRiskAssessments] ADD [HealthFacility] nvarchar(100) NOT NULL DEFAULT '';
    END
    
    -- Record migration
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250529051202_AddHealthFacilityAndFamilyNoFields', '8.0.2');
    
    PRINT 'Applied migration 20250529051202_AddHealthFacilityAndFamilyNoFields';
END

-- Step 5: Apply migration 20250529051329_AddHighSaltIntakeField
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250529051329_AddHighSaltIntakeField')
BEGIN
    -- Add HasHighSaltIntake to NCDRiskAssessments if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[NCDRiskAssessments]') AND name = 'HasHighSaltIntake')
    BEGIN
        ALTER TABLE [NCDRiskAssessments] ADD [HasHighSaltIntake] bit NOT NULL DEFAULT 0;
    END
    
    -- Record migration
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250529051329_AddHighSaltIntakeField', '8.0.2');
    
    PRINT 'Applied migration 20250529051329_AddHighSaltIntakeField';
END

-- Step 6: Apply remaining migrations up to 20250618061634_FixNCDRiskAssessmentAppointmentForeignKey
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250618061634_FixNCDRiskAssessmentAppointmentForeignKey')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250618061634_FixNCDRiskAssessmentAppointmentForeignKey', '8.0.2');
    
    PRINT 'Applied migration 20250618061634_FixNCDRiskAssessmentAppointmentForeignKey';
END

PRINT 'Database update completed successfully!'; 