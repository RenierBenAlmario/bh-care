-- Final database fix script for Barangay Health Center
USE [Barangay];
GO

PRINT 'Starting comprehensive database fixes...';
GO

-- 1. Fix Messages table relationships to avoid cyclic cascades
PRINT 'Fixing Messages table relationships...';
-- Drop existing foreign keys if they exist
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Messages_AspNetUsers_SenderId')
    ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_SenderId];
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Messages_AspNetUsers_ReceiverId')
    ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId];
GO

-- Re-add them with proper delete behavior
ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_SenderId] 
    FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId] 
    FOREIGN KEY ([ReceiverId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
GO

PRINT 'Message table relationships fixed successfully.';
GO

-- 2. Comprehensive fix for NULL values in AspNetUsers
PRINT 'Fixing NULL values in AspNetUsers table...';
UPDATE [AspNetUsers]
SET 
    [UserName] = COALESCE([UserName], CASE 
                                         WHEN [Email] IS NOT NULL THEN [Email] 
                                         ELSE CONCAT('user_', [Id]) 
                                      END),
    [NormalizedUserName] = UPPER(COALESCE([UserName], CASE 
                                                        WHEN [Email] IS NOT NULL THEN [Email] 
                                                        ELSE CONCAT('user_', [Id]) 
                                                     END)),
    [Email] = COALESCE([Email], CASE 
                                  WHEN [UserName] IS NOT NULL THEN [UserName] 
                                  ELSE CONCAT('user_', [Id], '@example.com') 
                               END),
    [NormalizedEmail] = UPPER(COALESCE([Email], CASE 
                                                  WHEN [UserName] IS NOT NULL THEN [UserName] 
                                                  ELSE CONCAT('user_', [Id], '@example.com') 
                                               END)),
    [SecurityStamp] = COALESCE([SecurityStamp], CONVERT(NVARCHAR(36), NEWID())),
    [ConcurrencyStamp] = COALESCE([ConcurrencyStamp], CONVERT(NVARCHAR(36), NEWID())),
    [PasswordHash] = COALESCE([PasswordHash], '$2a$11$aPNTKTAc2nYBVwhT.wnbJewz.YAXxlwT5CBDvkwpnPO4RVmdYg4w.'),
    [FirstName] = COALESCE([FirstName], ''),
    [LastName] = COALESCE([LastName], ''),
    [FullName] = COALESCE([FullName], CONCAT(COALESCE([FirstName], ''), ' ', COALESCE([LastName], ''))),
    [Gender] = COALESCE([Gender], ''),
    [Address] = COALESCE([Address], ''),
    [ProfilePicture] = COALESCE([ProfilePicture], ''),
    [PhilHealthId] = COALESCE([PhilHealthId], ''),
    [EncryptedFullName] = COALESCE([EncryptedFullName], ''),
    [EncryptedStatus] = COALESCE([EncryptedStatus], ''),
    [WorkingHours] = COALESCE([WorkingHours], ''),
    [Specialization] = COALESCE([Specialization], ''),
    [JoinDate] = CASE WHEN [JoinDate] = '0001-01-01 00:00:00.0000000' OR [JoinDate] IS NULL THEN GETDATE() ELSE [JoinDate] END,
    [LastActive] = CASE WHEN [LastActive] = '0001-01-01 00:00:00.0000000' OR [LastActive] IS NULL THEN GETDATE() ELSE [LastActive] END,
    [EmailConfirmed] = COALESCE([EmailConfirmed], 1),
    [PhoneNumberConfirmed] = COALESCE([PhoneNumberConfirmed], 0),
    [TwoFactorEnabled] = COALESCE([TwoFactorEnabled], 0),
    [LockoutEnabled] = COALESCE([LockoutEnabled], 1),
    [AccessFailedCount] = COALESCE([AccessFailedCount], 0)
WHERE [Id] IN (
    SELECT [Id] FROM [AspNetUsers]
);
GO

PRINT 'NULL values in AspNetUsers fixed successfully.';
GO

-- 3. Fix additional entity relationships with proper ON DELETE behaviors
PRINT 'Fixing additional entity relationships...';

-- Fix Patient-FamilyMember relationship
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_FamilyMembers_Patients_PatientId')
    ALTER TABLE [FamilyMembers] DROP CONSTRAINT [FK_FamilyMembers_Patients_PatientId];
GO

ALTER TABLE [FamilyMembers] ADD CONSTRAINT [FK_FamilyMembers_Patients_PatientId]
    FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION;
GO

-- Fix Patient-VitalSigns relationship
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VitalSigns_Patients_PatientId')
    ALTER TABLE [VitalSigns] DROP CONSTRAINT [FK_VitalSigns_Patients_PatientId];
GO

ALTER TABLE [VitalSigns] ADD CONSTRAINT [FK_VitalSigns_Patients_PatientId]
    FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION;
GO

-- Fix Patient-MedicalRecords relationship
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_MedicalRecords_Patients_PatientId')
    ALTER TABLE [MedicalRecords] DROP CONSTRAINT [FK_MedicalRecords_Patients_PatientId];
GO

ALTER TABLE [MedicalRecords] ADD CONSTRAINT [FK_MedicalRecords_Patients_PatientId]
    FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION;
GO

-- Fix Patient-Prescriptions relationship
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Prescriptions_Patients_PatientId')
    ALTER TABLE [Prescriptions] DROP CONSTRAINT [FK_Prescriptions_Patients_PatientId];
GO

ALTER TABLE [Prescriptions] ADD CONSTRAINT [FK_Prescriptions_Patients_PatientId]
    FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION;
GO

PRINT 'Additional entity relationships fixed successfully.';
GO

-- 4. Record that we applied these fixes in migrations history
PRINT 'Recording migration history...';
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250603000006_FixMessageUserRelationships')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250603000006_FixMessageUserRelationships', '8.0.0');
END
GO

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250603000007_FixAspNetUserNullValues')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250603000007_FixAspNetUserNullValues', '8.0.0');
END
GO

PRINT 'All fixes have been successfully applied.';
GO 