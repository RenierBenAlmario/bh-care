-- Mark all migrations as applied
-- This script will insert all migration records into the __EFMigrationsHistory table

-- First, make sure we have the __EFMigrationsHistory table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
    PRINT 'Created __EFMigrationsHistory table'
END

-- Insert all migration records if they don't exist
-- Initial migrations
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250424173237_InitialCreate')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250424173237_InitialCreate', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250426063930_UpdatePrescriptionSchema')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250426063930_UpdatePrescriptionSchema', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250428154514_AddNameToPatient')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250428154514_AddNameToPatient', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250429102719_AddHeightAndBloodTypeToPatient')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250429102719_AddHeightAndBloodTypeToPatient', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250429103542_AddReportRelatedTables')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250429103542_AddReportRelatedTables', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250430155358_FixTimeSpanHandling')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250430155358_FixTimeSpanHandling', N'8.0.2');

-- Additional migrations
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250501072124_AddDependentFields')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250501072124_AddDependentFields', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250501072455_FixTimeSpanAndAgeHandling')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250501072455_FixTimeSpanAndAgeHandling', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250501163902_FixNavigationPropertiesAndReferences')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250501163902_FixNavigationPropertiesAndReferences', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250502053106_AppointmentModelUpdate')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250502053106_AppointmentModelUpdate', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250502053121_FixAppointmentValidation')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250502053121_FixAppointmentValidation', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250502053134_NewMigrationName')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250502053134_NewMigrationName', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250502115714_AddAppointmentTimeInput')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250502115714_AddAppointmentTimeInput', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250502120152_AddAppointmentTimeInputV2')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250502120152_AddAppointmentTimeInputV2', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250502162642_AddPrescriptionMedicationsToMedicalRecords')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250502162642_AddPrescriptionMedicationsToMedicalRecords', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250507111754_FixApplicationUserProperties')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250507111754_FixApplicationUserProperties', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250507112314_FixMessageUserRelationships')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250507112314_FixMessageUserRelationships', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250507121025_AddMiddleNameAndSuffixToApplicationUser')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250507121025_AddMiddleNameAndSuffixToApplicationUser', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250507123012_AddMiddleNameAndSuffixColumns')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250507123012_AddMiddleNameAndSuffixColumns', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250508150111_MakeLastNameAndSuffixOptional')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250508150111_MakeLastNameAndSuffixOptional', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250508150438_MakeLastNameAndSuffixNullable')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250508150438_MakeLastNameAndSuffixNullable', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250508151056_UpdateNullableLastNameAndSuffix')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250508151056_UpdateNullableLastNameAndSuffix', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250508155707_AddStatusAndFixMiddleName')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250508155707_AddStatusAndFixMiddleName', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250508155825_AddStatusToApplicationUser')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250508155825_AddStatusToApplicationUser', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250508164043_AddMissingStatusColumn')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250508164043_AddMissingStatusColumn', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250511082823_UpdateDatabase_20250511_162811')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250511082823_UpdateDatabase_20250511_162811', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250511083240_FixCascadePaths')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250511083240_FixCascadePaths', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250603000001_UpdateBirthDateToNonNullable')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250603000001_UpdateBirthDateToNonNullable', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250603000002_FixPrescriptionMedicationRelationship')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250603000002_FixPrescriptionMedicationRelationship', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250603000003_FixPatientFamilyMemberRelationship')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250603000003_FixPatientFamilyMemberRelationship', N'8.0.2');

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250603000004_FixPatientForeignKeyConstraints')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250603000004_FixPatientForeignKeyConstraints', N'8.0.2');

-- Print the current migration history
SELECT [MigrationId], [ProductVersion] FROM [__EFMigrationsHistory] ORDER BY [MigrationId];

PRINT 'All migrations have been marked as applied.' 