-- Add Status column to AspNetUsers table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'Status')
BEGIN
    ALTER TABLE [dbo].[AspNetUsers] ADD [Status] nvarchar(max) NOT NULL DEFAULT 'Pending';
END

-- Ensure LastName is nullable
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'LastName')
BEGIN
    -- Check if LastName is already nullable
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'LastName' AND is_nullable = 0)
    BEGIN
        ALTER TABLE [dbo].[AspNetUsers] ALTER COLUMN [LastName] nvarchar(max) NULL;
    END
END

-- Ensure Suffix is nullable
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'Suffix')
BEGIN
    -- Check if Suffix is already nullable
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'Suffix' AND is_nullable = 0)
    BEGIN
        ALTER TABLE [dbo].[AspNetUsers] ALTER COLUMN [Suffix] nvarchar(max) NULL;
    END
END

-- Update existing users to have a Status value
UPDATE [dbo].[AspNetUsers]
SET [Status] = CASE 
                WHEN [EncryptedStatus] = 'Pending' THEN 'Pending'
                WHEN [EncryptedStatus] = 'Active' THEN 'Approved'
                ELSE 'Pending'
               END
WHERE [Status] IS NULL OR [Status] = ''; 