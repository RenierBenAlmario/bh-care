-- Complete Database Update Script
-- This script combines all necessary database fixes in one file

PRINT 'Starting comprehensive database update...'

-- 1. Add missing columns to AspNetUsers table
PRINT '1. Adding missing columns to AspNetUsers table...'
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'MiddleName' AND object_id = OBJECT_ID('AspNetUsers'))
BEGIN
    ALTER TABLE [AspNetUsers] ADD [MiddleName] nvarchar(max) NULL;
    PRINT '  Added MiddleName column to AspNetUsers table'
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Status' AND object_id = OBJECT_ID('AspNetUsers'))
BEGIN
    ALTER TABLE [AspNetUsers] ADD [Status] nvarchar(max) NULL;
    PRINT '  Added Status column to AspNetUsers table'
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Suffix' AND object_id = OBJECT_ID('AspNetUsers'))
BEGIN
    ALTER TABLE [AspNetUsers] ADD [Suffix] nvarchar(max) NULL;
    PRINT '  Added Suffix column to AspNetUsers table'
END

-- 2. Fix cascade paths issue in Messages table
PRINT '2. Fixing cascade paths issue in Messages table...'
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Messages_AspNetUsers_SenderId')
BEGIN
    ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_SenderId]
    PRINT '  Dropped FK_Messages_AspNetUsers_SenderId constraint'
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Messages_AspNetUsers_ReceiverId')
BEGIN
    ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId]
    PRINT '  Dropped FK_Messages_AspNetUsers_ReceiverId constraint'
END

-- Re-create the foreign key constraints with NO ACTION instead of CASCADE
ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_SenderId] 
    FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
PRINT '  Added FK_Messages_AspNetUsers_SenderId constraint with NO ACTION'

ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId] 
    FOREIGN KEY ([ReceiverId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
PRINT '  Added FK_Messages_AspNetUsers_ReceiverId constraint with NO ACTION'

-- 3. Create AppointmentAttachments table if it doesn't exist
PRINT '3. Creating AppointmentAttachments table if it doesn''t exist...'
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AppointmentAttachments')
BEGIN
    CREATE TABLE [AppointmentAttachments] (
        [Id] int NOT NULL IDENTITY,
        [AppointmentId] int NOT NULL,
        [FileName] nvarchar(max) NOT NULL,
        [OriginalFileName] nvarchar(max) NOT NULL,
        [ContentType] nvarchar(max) NOT NULL,
        [FilePath] nvarchar(max) NOT NULL,
        [UploadedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_AppointmentAttachments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AppointmentAttachments_Appointments_AppointmentId] 
            FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE CASCADE
    );
    PRINT '  Created AppointmentAttachments table'
END

-- 4. Create AppointmentFiles table if it doesn't exist
PRINT '4. Creating AppointmentFiles table if it doesn''t exist...'
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AppointmentFiles')
BEGIN
    CREATE TABLE [AppointmentFiles] (
        [Id] int NOT NULL IDENTITY,
        [AppointmentId] int NOT NULL,
        [FileName] nvarchar(max) NOT NULL,
        [OriginalFileName] nvarchar(max) NOT NULL,
        [ContentType] nvarchar(max) NOT NULL,
        [FilePath] nvarchar(max) NOT NULL,
        [UploadedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_AppointmentFiles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AppointmentFiles_Appointments_AppointmentId] 
            FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE CASCADE
    );
    PRINT '  Created AppointmentFiles table'
END

-- 5. Add MedicationId column to PrescriptionMedications if it doesn't exist
PRINT '5. Adding MedicationId column to PrescriptionMedications if it doesn''t exist...'
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'MedicationId' AND object_id = OBJECT_ID('PrescriptionMedications'))
BEGIN
    ALTER TABLE [PrescriptionMedications] ADD [MedicationId] int NOT NULL DEFAULT 0;
    PRINT '  Added MedicationId column to PrescriptionMedications table'
END

-- 6. Add Medications column to MedicalRecords if it doesn't exist
PRINT '6. Adding Medications column to MedicalRecords if it doesn''t exist...'
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Medications' AND object_id = OBJECT_ID('MedicalRecords'))
BEGIN
    ALTER TABLE [MedicalRecords] ADD [Medications] nvarchar(max) NOT NULL DEFAULT N'';
    PRINT '  Added Medications column to MedicalRecords table'
END

-- 7. Add RecordDate column to MedicalRecords if it doesn't exist
PRINT '7. Adding RecordDate column to MedicalRecords if it doesn''t exist...'
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'RecordDate' AND object_id = OBJECT_ID('MedicalRecords'))
BEGIN
    ALTER TABLE [MedicalRecords] ADD [RecordDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
    PRINT '  Added RecordDate column to MedicalRecords table'
END

-- 8. Create indexes for foreign keys
PRINT '8. Creating indexes for foreign keys...'
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_StaffMembers_UserId' AND object_id = OBJECT_ID('StaffMembers'))
BEGIN
    CREATE INDEX [IX_StaffMembers_UserId] ON [StaffMembers] ([UserId]);
    PRINT '  Created index IX_StaffMembers_UserId'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PrescriptionMedications_MedicationId' AND object_id = OBJECT_ID('PrescriptionMedications'))
BEGIN
    CREATE INDEX [IX_PrescriptionMedications_MedicationId] ON [PrescriptionMedications] ([MedicationId]);
    PRINT '  Created index IX_PrescriptionMedications_MedicationId'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AppointmentAttachments_AppointmentId' AND object_id = OBJECT_ID('AppointmentAttachments'))
BEGIN
    CREATE INDEX [IX_AppointmentAttachments_AppointmentId] ON [AppointmentAttachments] ([AppointmentId]);
    PRINT '  Created index IX_AppointmentAttachments_AppointmentId'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AppointmentFiles_AppointmentId' AND object_id = OBJECT_ID('AppointmentFiles'))
BEGIN
    CREATE INDEX [IX_AppointmentFiles_AppointmentId] ON [AppointmentFiles] ([AppointmentId]);
    PRINT '  Created index IX_AppointmentFiles_AppointmentId'
END

-- 9. Mark all migrations as applied
PRINT '9. Marking migrations as applied...'
-- First, make sure we have the __EFMigrationsHistory table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
    PRINT '  Created __EFMigrationsHistory table'
END

-- Insert the latest migration if it doesn't exist
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250511084736_AddMissingAspNetUsersColumns')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250511084736_AddMissingAspNetUsersColumns', N'8.0.2');

-- 10. Verify database schema
PRINT '10. Verifying database schema...'
-- Check AspNetUsers columns
SELECT 
    'AspNetUsers' AS TableName,
    COUNT(*) AS ColumnCount,
    SUM(CASE WHEN name IN ('MiddleName', 'Status', 'Suffix') THEN 1 ELSE 0 END) AS RequiredColumnsCount
FROM 
    sys.columns 
WHERE 
    object_id = OBJECT_ID('AspNetUsers');

-- Check Messages foreign keys
SELECT 
    fk.name AS 'Foreign Key', 
    OBJECT_NAME(fk.parent_object_id) AS 'Table',
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS 'Column',
    OBJECT_NAME(fk.referenced_object_id) AS 'Referenced Table',
    fk.delete_referential_action_desc AS 'Delete Action'
FROM 
    sys.foreign_keys fk
INNER JOIN 
    sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
WHERE 
    OBJECT_NAME(fk.parent_object_id) = 'Messages'
ORDER BY 
    fk.name;

PRINT 'Database update completed successfully!' 