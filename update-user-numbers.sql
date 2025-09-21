-- Set proper options
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

-- Update UserNumber values for existing users
IF EXISTS(SELECT * FROM sys.columns WHERE Name = N'UserNumber' AND Object_ID = Object_ID(N'AspNetUsers'))
BEGIN
    PRINT 'Updating UserNumber values for existing users...';
    
    -- Number existing users
    WITH NumberedUsers AS (
        SELECT Id, ROW_NUMBER() OVER (ORDER BY CreatedAt) AS RowNum
        FROM AspNetUsers
    )
    UPDATE AspNetUsers
    SET UserNumber = NumberedUsers.RowNum
    FROM NumberedUsers
    WHERE AspNetUsers.Id = NumberedUsers.Id;
    
    PRINT 'UserNumber values updated successfully';
    
    -- Create trigger for new users
    IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_AspNetUsers_AssignUserNumber')
    BEGIN
        DROP TRIGGER TR_AspNetUsers_AssignUserNumber;
        PRINT 'Dropped existing trigger';
    END

    EXEC('
    CREATE TRIGGER TR_AspNetUsers_AssignUserNumber
    ON AspNetUsers
    AFTER INSERT
    AS
    BEGIN
        DECLARE @MaxUserNumber int;

        -- Get the current maximum UserNumber
        SELECT @MaxUserNumber = ISNULL(MAX(UserNumber), 0) FROM AspNetUsers;

        -- Update the inserted records with incremented UserNumber
        UPDATE AspNetUsers
        SET UserNumber = @MaxUserNumber + ROW_NUMBER() OVER (ORDER BY CreatedAt)
        FROM AspNetUsers
        INNER JOIN inserted ON AspNetUsers.Id = inserted.Id
        WHERE AspNetUsers.UserNumber = 0;
    END
    ');
    
    PRINT 'Created trigger TR_AspNetUsers_AssignUserNumber';
    
    -- Show sample data
    PRINT 'Sample data:';
    SELECT TOP 5 Id, UserName, Email, UserNumber
    FROM AspNetUsers
    ORDER BY UserNumber;
END
ELSE
BEGIN
    PRINT 'UserNumber column does not exist in AspNetUsers table';
END 