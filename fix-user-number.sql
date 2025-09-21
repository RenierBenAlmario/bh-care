-- Set required options
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- Check if UserNumber column exists in AspNetUsers table
PRINT 'Checking if UserNumber column exists...';
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'UserNumber' AND Object_ID = Object_ID(N'AspNetUsers'))
BEGIN
    PRINT 'UserNumber column does not exist. Adding it now...';
    
    -- Add the UserNumber column
    ALTER TABLE AspNetUsers ADD UserNumber INT NOT NULL DEFAULT 0;
    PRINT 'Added UserNumber column to AspNetUsers table';

    -- Number existing users
    WITH NumberedUsers AS (
        SELECT Id, ROW_NUMBER() OVER (ORDER BY CreatedAt) AS RowNum
        FROM AspNetUsers
    )
    UPDATE AspNetUsers
    SET UserNumber = NumberedUsers.RowNum
    FROM NumberedUsers
    WHERE AspNetUsers.Id = NumberedUsers.Id;
    PRINT 'Assigned UserNumber values to existing users';

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
END
ELSE
BEGIN
    PRINT 'UserNumber column already exists in AspNetUsers table';
END
GO 