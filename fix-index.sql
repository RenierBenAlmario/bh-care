-- Set required options
SET QUOTED_IDENTIFIER ON;
GO

-- Create index for BookedByUserId if it doesn't exist and if the column exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Appointments]') AND name = 'BookedByUserId')
    AND NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_BookedByUserId' AND object_id = OBJECT_ID('Appointments'))
BEGIN
    CREATE INDEX [IX_Appointments_BookedByUserId] ON [Appointments] ([BookedByUserId]) WHERE [BookedByUserId] IS NOT NULL;
    PRINT 'Created index IX_Appointments_BookedByUserId';
END
ELSE
BEGIN
    PRINT 'Index already exists or BookedByUserId column does not exist';
END
GO 