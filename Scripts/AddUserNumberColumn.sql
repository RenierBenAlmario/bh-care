-- Script to add UserNumber column to AspNetUsers table
-- This script is safe to run multiple times as it checks if the column exists first

-- Check if the column exists
IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE Name = N'UserNumber' AND Object_ID = Object_ID(N'AspNetUsers'))
BEGIN
    -- Add the UserNumber column
    ALTER TABLE AspNetUsers ADD UserNumber INT NOT NULL DEFAULT 0;
    
    -- Number existing users
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
    
    -- Add index for better performance
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_AspNetUsers_UserNumber' AND object_id = OBJECT_ID('AspNetUsers'))
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_AspNetUsers_UserNumber] ON [AspNetUsers] ([UserNumber])
    END
    
    PRINT 'UserNumber column and index added successfully';
END
ELSE
BEGIN
    -- If column exists but index doesn't, add the index
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_AspNetUsers_UserNumber' AND object_id = OBJECT_ID('AspNetUsers'))
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_AspNetUsers_UserNumber] ON [AspNetUsers] ([UserNumber])
        PRINT 'UserNumber index added successfully';
    END
    ELSE
    BEGIN
        PRINT 'UserNumber column and index already exist';
    END
END 