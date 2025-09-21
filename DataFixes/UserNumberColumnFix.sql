-- Check if UserNumber column exists, if not add it
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'UserNumber')
BEGIN
    -- Add UserNumber column
    ALTER TABLE AspNetUsers ADD UserNumber INT NOT NULL DEFAULT 0;
    
    -- Populate UserNumber for existing users
    WITH NumberedUsers AS (
        SELECT Id, ROW_NUMBER() OVER (ORDER BY CreatedAt) AS RowNum
        FROM AspNetUsers
    )
    UPDATE AspNetUsers
    SET UserNumber = NumberedUsers.RowNum
    FROM NumberedUsers
    WHERE AspNetUsers.Id = NumberedUsers.Id;
    
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
            SET UserNumber = @MaxUserNumber + ROW_NUMBER() OVER (ORDER BY CreatedAt)
            FROM AspNetUsers
            INNER JOIN inserted ON AspNetUsers.Id = inserted.Id
            WHERE AspNetUsers.UserNumber = 0;
        END
    ');
    
    PRINT 'UserNumber column added and populated successfully.';
END
ELSE
BEGIN
    PRINT 'UserNumber column already exists.';
END 