-- Drop the table if it exists to ensure a clean creation
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserDocuments')
BEGIN
    DROP TABLE [UserDocuments];
END

-- Create the UserDocuments table with all necessary fields
CREATE TABLE [UserDocuments](
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [FilePath] NVARCHAR(MAX) NOT NULL,
    [FileName] NVARCHAR(255) NOT NULL,
    [FileSize] BIGINT NOT NULL,
    [ContentType] NVARCHAR(100) NOT NULL,
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    [ApprovedAt] DATETIME2 NULL,
    [ApprovedBy] NVARCHAR(256) NULL,
    [UploadDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_UserDocuments_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
);

-- Add index for better query performance
CREATE INDEX [IX_UserDocuments_UserId] ON [UserDocuments]([UserId]);

-- Verify table was created
SELECT 'UserDocuments table created successfully' AS Result; 