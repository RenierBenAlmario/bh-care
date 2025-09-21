-- Set required options
SET QUOTED_IDENTIFIER ON;
GO

-- Check if Appointments table exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Appointments')
BEGIN
    PRINT 'Error: Appointments table does not exist';
    RETURN;
END

-- Apply migration 20250626061924_BookingFormUpdates
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250626061924_BookingFormUpdates')
BEGIN
    -- Add BookingForm columns if they don't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Appointments]') AND name = 'RelationshipToPatient')
    BEGIN
        ALTER TABLE [Appointments] ADD [RelationshipToPatient] nvarchar(100) NULL;
        PRINT 'Added RelationshipToPatient column to Appointments table';
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Appointments]') AND name = 'BookedByUserId')
    BEGIN
        ALTER TABLE [Appointments] ADD [BookedByUserId] nvarchar(450) NULL;
        PRINT 'Added BookedByUserId column to Appointments table';
    END
    
    -- Record migration
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250626061924_BookingFormUpdates', '8.0.2');
    
    PRINT 'Applied migration 20250626061924_BookingFormUpdates';
END

-- Create index for BookedByUserId if it doesn't exist and if the column exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Appointments]') AND name = 'BookedByUserId')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_BookedByUserId' AND object_id = OBJECT_ID('Appointments'))
    BEGIN
        CREATE INDEX [IX_Appointments_BookedByUserId] ON [Appointments] ([BookedByUserId]) WHERE [BookedByUserId] IS NOT NULL;
        PRINT 'Created index IX_Appointments_BookedByUserId';
    END
END
ELSE
BEGIN
    PRINT 'Warning: BookedByUserId column does not exist in Appointments table';
END

-- Add foreign key for BookedByUserId if it doesn't exist and if the column exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Appointments]') AND name = 'BookedByUserId')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointments_AspNetUsers_BookedByUserId')
    BEGIN
        ALTER TABLE [Appointments] ADD CONSTRAINT [FK_Appointments_AspNetUsers_BookedByUserId] 
        FOREIGN KEY ([BookedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE SET NULL;
        PRINT 'Added foreign key FK_Appointments_AspNetUsers_BookedByUserId';
    END
END

-- Apply migration 20250629115500_AddBookingRelationshipColumns
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250629115500_AddBookingRelationshipColumns')
BEGIN
    -- Add BookingForm relationship columns if they don't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Appointments]') AND name = 'BookingNotes')
    BEGIN
        ALTER TABLE [Appointments] ADD [BookingNotes] nvarchar(max) NULL;
        PRINT 'Added BookingNotes column to Appointments table';
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Appointments]') AND name = 'BookerContactNumber')
    BEGIN
        ALTER TABLE [Appointments] ADD [BookerContactNumber] nvarchar(20) NULL;
        PRINT 'Added BookerContactNumber column to Appointments table';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Appointments]') AND name = 'BookerName')
    BEGIN
        ALTER TABLE [Appointments] ADD [BookerName] nvarchar(200) NULL;
        PRINT 'Added BookerName column to Appointments table';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Appointments]') AND name = 'BookerEmail')
    BEGIN
        ALTER TABLE [Appointments] ADD [BookerEmail] nvarchar(200) NULL;
        PRINT 'Added BookerEmail column to Appointments table';
    END
    
    -- Record migration
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250629115500_AddBookingRelationshipColumns', '8.0.2');
    
    PRINT 'Applied migration 20250629115500_AddBookingRelationshipColumns';
END

PRINT 'All pending migrations have been applied successfully!';
GO 