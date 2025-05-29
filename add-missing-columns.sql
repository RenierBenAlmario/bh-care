-- Add missing columns to AspNetUsers table
-- This script adds the missing columns that are causing errors

-- Add MiddleName column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'MiddleName' AND object_id = OBJECT_ID('AspNetUsers'))
BEGIN
    ALTER TABLE [AspNetUsers] ADD [MiddleName] nvarchar(max) NULL;
    PRINT 'Added MiddleName column to AspNetUsers table'
END

-- Add Status column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Status' AND object_id = OBJECT_ID('AspNetUsers'))
BEGIN
    ALTER TABLE [AspNetUsers] ADD [Status] nvarchar(max) NULL;
    PRINT 'Added Status column to AspNetUsers table'
END

-- Add Suffix column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Suffix' AND object_id = OBJECT_ID('AspNetUsers'))
BEGIN
    ALTER TABLE [AspNetUsers] ADD [Suffix] nvarchar(max) NULL;
    PRINT 'Added Suffix column to AspNetUsers table'
END

-- Verify the columns were added
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE
FROM 
    INFORMATION_SCHEMA.COLUMNS 
WHERE 
    TABLE_NAME = 'AspNetUsers' AND 
    COLUMN_NAME IN ('MiddleName', 'Status', 'Suffix');

PRINT 'Missing columns have been added to AspNetUsers table'

-- Add ApplicationUserId and AttachmentsData columns to AppointmentAttachments table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AppointmentAttachments]') AND name = 'ApplicationUserId')
BEGIN
    ALTER TABLE [dbo].[AppointmentAttachments]
    ADD [ApplicationUserId] NVARCHAR(450) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AppointmentAttachments]') AND name = 'AttachmentsData')
BEGIN
    ALTER TABLE [dbo].[AppointmentAttachments]
    ADD [AttachmentsData] VARBINARY(MAX) NULL;
END

-- Add CreatedAt column to StaffMembers table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[StaffMembers]') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE [dbo].[StaffMembers]
    ADD [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE();
END

-- Add foreign key relationship for ApplicationUserId in AppointmentAttachments
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AppointmentAttachments_AspNetUsers_ApplicationUserId]'))
BEGIN
    ALTER TABLE [dbo].[AppointmentAttachments]
    ADD CONSTRAINT [FK_AppointmentAttachments_AspNetUsers_ApplicationUserId]
    FOREIGN KEY ([ApplicationUserId]) REFERENCES [dbo].[AspNetUsers] ([Id]);
END

-- Add index for ApplicationUserId in AppointmentAttachments
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AppointmentAttachments]') AND name = 'IX_AppointmentAttachments_ApplicationUserId')
BEGIN
    CREATE INDEX [IX_AppointmentAttachments_ApplicationUserId] ON [dbo].[AppointmentAttachments] ([ApplicationUserId]);
END

-- Add entry to __EFMigrationsHistory table to record these changes
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20240524_AddMissingColumns')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20240524_AddMissingColumns', '8.0.0');
END 