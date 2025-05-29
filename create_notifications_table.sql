-- Check if Notifications table exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Notifications')
BEGIN
    -- Create Notifications table with all required columns
    CREATE TABLE [dbo].[Notifications] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Title] NVARCHAR(MAX) NOT NULL,
        [Message] NVARCHAR(MAX) NOT NULL,
        [Type] NVARCHAR(50) NOT NULL,
        [Link] NVARCHAR(MAX) NULL,
        [UserId] NVARCHAR(450) NULL,
        [RecipientId] NVARCHAR(450) NULL,
        [CreatedAt] DATETIME2 NOT NULL,
        [ReadAt] DATETIME2 NULL,
        [IsRead] BIT NOT NULL DEFAULT 0
    );

    PRINT 'Notifications table created successfully.';
END
ELSE
BEGIN
    PRINT 'Notifications table already exists.';

    -- Check if Link column exists and add it if it doesn't
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Notifications]') AND name = 'Link')
    BEGIN
        ALTER TABLE [dbo].[Notifications] ADD [Link] NVARCHAR(MAX) NULL;
        PRINT 'Added Link column to Notifications table';
    END
    ELSE
    BEGIN
        PRINT 'Link column already exists';
    END

    -- Check if ReadAt column exists and add it if it doesn't
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Notifications]') AND name = 'ReadAt')
    BEGIN
        ALTER TABLE [dbo].[Notifications] ADD [ReadAt] DATETIME2 NULL;
        PRINT 'Added ReadAt column to Notifications table';
    END
    ELSE
    BEGIN
        PRINT 'ReadAt column already exists';
    END

    -- Check if IsRead column exists and add it if it doesn't
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Notifications]') AND name = 'IsRead')
    BEGIN
        ALTER TABLE [dbo].[Notifications] ADD [IsRead] BIT NOT NULL DEFAULT 0;
        PRINT 'Added IsRead column to Notifications table';
    END
    ELSE
    BEGIN
        PRINT 'IsRead column already exists';
    END
END 