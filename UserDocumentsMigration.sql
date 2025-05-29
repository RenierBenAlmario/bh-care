-- Script to create the missing UserDocuments table

-- First check if the table already exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserDocuments')
BEGIN
    -- Create the UserDocuments table
    CREATE TABLE [UserDocuments] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserId] NVARCHAR(450) NOT NULL,
        [FileName] NVARCHAR(256) NOT NULL,
        [FilePath] NVARCHAR(256) NULL,
        [Status] NVARCHAR(20) NOT NULL DEFAULT ('Pending'),
        [ApprovedAt] DATETIME2 NULL,
        [ApprovedBy] NVARCHAR(256) NULL,
        [FileSize] BIGINT NOT NULL,
        [ContentType] NVARCHAR(100) NULL,
        CONSTRAINT [PK_UserDocuments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserDocuments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );

    -- Create an index on the UserId field for performance
    CREATE INDEX [IX_UserDocuments_UserId] ON [UserDocuments] ([UserId]);

    PRINT 'UserDocuments table created successfully';
END
ELSE
BEGIN
    PRINT 'UserDocuments table already exists';
END 