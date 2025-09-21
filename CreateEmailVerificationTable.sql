USE [Barangay]
GO

-- Check if table already exists
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmailVerifications]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EmailVerifications] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Email] NVARCHAR(255) NOT NULL,
        [VerificationCode] NVARCHAR(10) NOT NULL,
        [ExpiryTime] DATETIME2 NOT NULL,
        [IsVerified] BIT NOT NULL DEFAULT(0),
        [CreatedAt] DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
        [VerifiedAt] DATETIME2 NULL
    );

    -- Create index on Email
    CREATE UNIQUE INDEX [IX_EmailVerifications_Email] ON [dbo].[EmailVerifications]([Email]);
    
    PRINT 'EmailVerifications table created successfully.';
END
ELSE
BEGIN
    PRINT 'EmailVerifications table already exists.';
END 