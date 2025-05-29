-- Check if the columns already exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Notifications]') AND name = 'Link')
BEGIN
    -- Add Link column to Notifications table
    ALTER TABLE [dbo].[Notifications] ADD [Link] NVARCHAR(MAX) NULL;
    PRINT 'Added Link column to Notifications table';
END
ELSE
BEGIN
    PRINT 'Link column already exists';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Notifications]') AND name = 'ReadAt')
BEGIN
    -- Add ReadAt column to Notifications table
    ALTER TABLE [dbo].[Notifications] ADD [ReadAt] DATETIME2 NULL;
    PRINT 'Added ReadAt column to Notifications table';
END
ELSE
BEGIN
    PRINT 'ReadAt column already exists';
END

-- Update the migration history to mark this migration as applied
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250513114024_AddNotificationLinkAndReadAt')
BEGIN
    -- Get the current product version
    DECLARE @CurrentProductVersion NVARCHAR(MAX);
    SELECT TOP 1 @CurrentProductVersion = [ProductVersion] FROM [__EFMigrationsHistory] ORDER BY [MigrationId] DESC;
    
    -- Insert migration record
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250513114024_AddNotificationLinkAndReadAt', @CurrentProductVersion);
    PRINT 'Added migration record to __EFMigrationsHistory';
END
ELSE
BEGIN
    PRINT 'Migration already applied';
END 