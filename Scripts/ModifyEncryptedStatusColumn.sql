-- Set the correct options for the script
SET QUOTED_IDENTIFIER ON;
GO

-- Modify EncryptedStatus column to allow NULL values
ALTER TABLE [dbo].[AspNetUsers]
ALTER COLUMN [EncryptedStatus] NVARCHAR(MAX) NULL;

-- Update any NULL values to empty string to maintain data consistency
UPDATE [dbo].[AspNetUsers]
SET [EncryptedStatus] = ''
WHERE [EncryptedStatus] IS NULL; 