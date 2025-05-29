-- Add missing columns to AspNetUsers table if they don't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AspNetUsers') AND name = 'MiddleName')
BEGIN
    ALTER TABLE dbo.AspNetUsers
    ADD MiddleName nvarchar(max) NOT NULL DEFAULT '';
    PRINT 'Added MiddleName column to AspNetUsers table';
END
ELSE
BEGIN
    PRINT 'MiddleName column already exists in AspNetUsers table';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AspNetUsers') AND name = 'Suffix')
BEGIN
    ALTER TABLE dbo.AspNetUsers
    ADD Suffix nvarchar(max) NOT NULL DEFAULT '';
    PRINT 'Added Suffix column to AspNetUsers table';
END
ELSE
BEGIN
    PRINT 'Suffix column already exists in AspNetUsers table';
END 