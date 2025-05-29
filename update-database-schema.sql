-- Update Database Schema Script
-- This script will update the database schema with necessary changes

-- First, make sure all foreign key constraints are set to NO ACTION for Messages table
-- Drop existing foreign key constraints if they exist
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Messages_AspNetUsers_SenderId')
BEGIN
    ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_SenderId]
    PRINT 'Dropped FK_Messages_AspNetUsers_SenderId constraint'
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Messages_AspNetUsers_ReceiverId')
BEGIN
    ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId]
    PRINT 'Dropped FK_Messages_AspNetUsers_ReceiverId constraint'
END

-- Re-create the foreign key constraints with NO ACTION instead of CASCADE
ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_SenderId] 
    FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
PRINT 'Added FK_Messages_AspNetUsers_SenderId constraint with NO ACTION'

ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId] 
    FOREIGN KEY ([ReceiverId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
PRINT 'Added FK_Messages_AspNetUsers_ReceiverId constraint with NO ACTION'

-- Create AppointmentAttachments table if it doesn't exist
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
    PRINT 'Created AppointmentAttachments table'
END

-- Create AppointmentFiles table if it doesn't exist
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
    PRINT 'Created AppointmentFiles table'
END

-- Create Users table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [Email] nvarchar(max) NOT NULL,
        [FirstName] nvarchar(max) NOT NULL,
        [LastName] nvarchar(max) NOT NULL,
        [Password] nvarchar(max) NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [FullName] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
    PRINT 'Created Users table'
END

-- Add MedicationId column to PrescriptionMedications if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'MedicationId' AND object_id = OBJECT_ID('PrescriptionMedications'))
BEGIN
    ALTER TABLE [PrescriptionMedications] ADD [MedicationId] int NOT NULL DEFAULT 0;
    PRINT 'Added MedicationId column to PrescriptionMedications table'
END

-- Add Medications column to MedicalRecords if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Medications' AND object_id = OBJECT_ID('MedicalRecords'))
BEGIN
    ALTER TABLE [MedicalRecords] ADD [Medications] nvarchar(max) NOT NULL DEFAULT N'';
    PRINT 'Added Medications column to MedicalRecords table'
END

-- Add RecordDate column to MedicalRecords if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'RecordDate' AND object_id = OBJECT_ID('MedicalRecords'))
BEGIN
    ALTER TABLE [MedicalRecords] ADD [RecordDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
    PRINT 'Added RecordDate column to MedicalRecords table'
END

-- Create indexes for foreign keys
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_StaffMembers_UserId' AND object_id = OBJECT_ID('StaffMembers'))
BEGIN
    CREATE INDEX [IX_StaffMembers_UserId] ON [StaffMembers] ([UserId]);
    PRINT 'Created index IX_StaffMembers_UserId'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PrescriptionMedications_MedicationId' AND object_id = OBJECT_ID('PrescriptionMedications'))
BEGIN
    CREATE INDEX [IX_PrescriptionMedications_MedicationId] ON [PrescriptionMedications] ([MedicationId]);
    PRINT 'Created index IX_PrescriptionMedications_MedicationId'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AppointmentAttachments_AppointmentId' AND object_id = OBJECT_ID('AppointmentAttachments'))
BEGIN
    CREATE INDEX [IX_AppointmentAttachments_AppointmentId] ON [AppointmentAttachments] ([AppointmentId]);
    PRINT 'Created index IX_AppointmentAttachments_AppointmentId'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AppointmentFiles_AppointmentId' AND object_id = OBJECT_ID('AppointmentFiles'))
BEGIN
    CREATE INDEX [IX_AppointmentFiles_AppointmentId] ON [AppointmentFiles] ([AppointmentId]);
    PRINT 'Created index IX_AppointmentFiles_AppointmentId'
END

PRINT 'Database schema update completed successfully!' 