-- SQL script to fix Patient-related foreign key constraints
-- Only use this if Entity Framework migrations fail

-- Step 1: Apply the FixApplicationUserProperties migration manually if it's not applying automatically

-- First, check if the migration has already been applied
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250507111754_FixApplicationUserProperties')
BEGIN
    -- Drop existing foreign keys that are causing issues with the Patient table
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VitalSigns_Patients_PatientId')
        ALTER TABLE [VitalSigns] DROP CONSTRAINT [FK_VitalSigns_Patients_PatientId];
        
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_FamilyMembers_Patients_PatientId')
        ALTER TABLE [FamilyMembers] DROP CONSTRAINT [FK_FamilyMembers_Patients_PatientId];
        
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointments_Patients_PatientUserId')
        ALTER TABLE [Appointments] DROP CONSTRAINT [FK_Appointments_Patients_PatientUserId];
        
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_MedicalRecords_Patients_PatientId')
        ALTER TABLE [MedicalRecords] DROP CONSTRAINT [FK_MedicalRecords_Patients_PatientId];
        
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Prescriptions_Patients_PatientId')
        ALTER TABLE [Prescriptions] DROP CONSTRAINT [FK_Prescriptions_Patients_PatientId];

    -- Make PatientId columns nullable
    IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'PatientId' AND Object_ID = Object_ID('VitalSigns'))
        ALTER TABLE [VitalSigns] ALTER COLUMN [PatientId] nvarchar(450) NULL;
    
    IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'PatientId' AND Object_ID = Object_ID('FamilyMembers'))
        ALTER TABLE [FamilyMembers] ALTER COLUMN [PatientId] nvarchar(450) NULL;
    
    IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'PatientId' AND Object_ID = Object_ID('MedicalRecords'))
        ALTER TABLE [MedicalRecords] ALTER COLUMN [PatientId] nvarchar(450) NULL;
    
    IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'PatientId' AND Object_ID = Object_ID('Prescriptions'))
        ALTER TABLE [Prescriptions] ALTER COLUMN [PatientId] nvarchar(450) NULL;

    -- Fix the issue with UserId and ID columns in the AspNetUsers table
    -- First, add the migration record to prevent EF Core from trying to apply it
    IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250507111754_FixApplicationUserProperties')
        INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES ('20250507111754_FixApplicationUserProperties', '7.0.5');
END

-- Step 2: Apply the remaining fixes for foreign key constraints

-- Clean up invalid references
UPDATE VitalSigns SET PatientId = NULL WHERE PatientId NOT IN (SELECT UserId FROM Patients);
UPDATE FamilyMembers SET PatientId = NULL WHERE PatientId NOT IN (SELECT UserId FROM Patients);
UPDATE MedicalRecords SET PatientId = NULL WHERE PatientId NOT IN (SELECT UserId FROM Patients);
UPDATE Prescriptions SET PatientId = NULL WHERE PatientId NOT IN (SELECT UserId FROM Patients);

-- Re-add foreign key constraints
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VitalSigns_Patients_PatientId')
    ALTER TABLE [VitalSigns] ADD CONSTRAINT [FK_VitalSigns_Patients_PatientId] 
        FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE SET NULL;
    
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_FamilyMembers_Patients_PatientId')
    ALTER TABLE [FamilyMembers] ADD CONSTRAINT [FK_FamilyMembers_Patients_PatientId] 
        FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE SET NULL;
    
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointments_Patients_PatientUserId') AND
   EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'PatientUserId' AND Object_ID = Object_ID('Appointments'))
    ALTER TABLE [Appointments] ADD CONSTRAINT [FK_Appointments_Patients_PatientUserId] 
        FOREIGN KEY ([PatientUserId]) REFERENCES [Patients] ([UserId]) ON DELETE SET NULL;
    
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_MedicalRecords_Patients_PatientId')
    ALTER TABLE [MedicalRecords] ADD CONSTRAINT [FK_MedicalRecords_Patients_PatientId] 
        FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE SET NULL;
    
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Prescriptions_Patients_PatientId')
    ALTER TABLE [Prescriptions] ADD CONSTRAINT [FK_Prescriptions_Patients_PatientId] 
        FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE SET NULL;

-- Step 3: If you want to make the columns required again after all data is fixed
-- (Only run these after ensuring no NULL values remain)

-- Example if you want to make them required again:
-- ALTER TABLE [VitalSigns] ALTER COLUMN [PatientId] nvarchar(450) NOT NULL;
-- ALTER TABLE [FamilyMembers] ALTER COLUMN [PatientId] nvarchar(450) NOT NULL;
-- ALTER TABLE [MedicalRecords] ALTER COLUMN [PatientId] nvarchar(450) NOT NULL;
-- ALTER TABLE [Prescriptions] ALTER COLUMN [PatientId] nvarchar(450) NOT NULL; 