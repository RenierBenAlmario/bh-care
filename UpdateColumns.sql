-- Add the missing columns to AspNetUsers table
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

BEGIN TRY
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AspNetUsers') AND name = 'MiddleName')
    BEGIN
        ALTER TABLE AspNetUsers ADD MiddleName nvarchar(max) NOT NULL DEFAULT '';
        PRINT 'Added MiddleName column to AspNetUsers table';
    END
    ELSE
    BEGIN
        PRINT 'MiddleName column already exists in AspNetUsers table';
    END
END TRY
BEGIN CATCH
    PRINT 'Error adding MiddleName column: ' + ERROR_MESSAGE();
END CATCH

BEGIN TRY
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AspNetUsers') AND name = 'Suffix')
    BEGIN
        ALTER TABLE AspNetUsers ADD Suffix nvarchar(max) NOT NULL DEFAULT '';
        PRINT 'Added Suffix column to AspNetUsers table';
    END
    ELSE
    BEGIN
        PRINT 'Suffix column already exists in AspNetUsers table';
    END
END TRY
BEGIN CATCH
    PRINT 'Error adding Suffix column: ' + ERROR_MESSAGE();
END CATCH 