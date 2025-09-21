-- Set proper options
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

-- Add UserNumber column to AspNetUsers table if it doesn't exist
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'UserNumber'
)
BEGIN
    -- Add the UserNumber column
    ALTER TABLE AspNetUsers ADD UserNumber INT NOT NULL DEFAULT 0;
    PRINT 'Added UserNumber column to AspNetUsers table';

    -- Assign numbers to existing users based on a different column
    DECLARE @counter INT = 1;
    
    -- Cursor-based approach to number users
    DECLARE @userId NVARCHAR(450);
    DECLARE user_cursor CURSOR FOR 
    SELECT Id FROM AspNetUsers ORDER BY Id;
    
    OPEN user_cursor;
    FETCH NEXT FROM user_cursor INTO @userId;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        UPDATE AspNetUsers 
        SET UserNumber = @counter 
        WHERE Id = @userId;
        
        SET @counter = @counter + 1;
        FETCH NEXT FROM user_cursor INTO @userId;
    END
    
    CLOSE user_cursor;
    DEALLOCATE user_cursor;
    
    PRINT 'Assigned sequential numbers to existing users';

    -- Create trigger for new users
    IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_AspNetUsers_AssignUserNumber')   
        DROP TRIGGER TR_AspNetUsers_AssignUserNumber;

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
        SET UserNumber = @MaxUserNumber + ROW_NUMBER() OVER (ORDER BY Id)
        FROM AspNetUsers
        INNER JOIN inserted ON AspNetUsers.Id = inserted.Id
        WHERE AspNetUsers.UserNumber = 0;
    END
    ');
    PRINT 'Created auto-numbering trigger for new users';
END
ELSE
BEGIN
    PRINT 'UserNumber column already exists in AspNetUsers table';
END

PRINT 'AspNetUsers table update completed successfully.'; 