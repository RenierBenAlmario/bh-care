-- Check if the column exists
PRINT 'Starting UserNumber column creation process...';

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE Name = N'UserNumber' AND Object_ID = Object_ID(N'AspNetUsers'))
BEGIN
    PRINT 'Column does not exist. Adding it...';
    
    -- Add the UserNumber column
    ALTER TABLE AspNetUsers ADD UserNumber INT NOT NULL DEFAULT 0;
    PRINT 'UserNumber column added';
    
    -- Number existing users
    PRINT 'Numbering existing users...';
    WITH NumberedUsers AS (
        SELECT Id, ROW_NUMBER() OVER (ORDER BY CreatedAt) AS RowNum
        FROM AspNetUsers
    )
    UPDATE AspNetUsers
    SET UserNumber = NumberedUsers.RowNum
    FROM NumberedUsers
    WHERE AspNetUsers.Id = NumberedUsers.Id;
    PRINT 'Users numbered successfully';
    
    -- Create trigger for new users
    PRINT 'Setting up trigger...';
    IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_AspNetUsers_AssignUserNumber')
    BEGIN
        DROP TRIGGER TR_AspNetUsers_AssignUserNumber;
        PRINT 'Existing trigger dropped';
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
    PRINT 'Trigger created successfully';
    
    PRINT 'UserNumber column added successfully';
    
    -- Verify the column exists
    IF EXISTS(SELECT * FROM sys.columns WHERE Name = N'UserNumber' AND Object_ID = Object_ID(N'AspNetUsers'))
        PRINT 'Verification: UserNumber column exists';
    ELSE
        PRINT 'Verification FAILED: UserNumber column does NOT exist';
END
ELSE
BEGIN
    PRINT 'UserNumber column already exists';
    
    -- Show the table name to verify we're looking at the right table
    SELECT TOP 1 TABLE_NAME 
    FROM INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_NAME = 'AspNetUsers';
END 