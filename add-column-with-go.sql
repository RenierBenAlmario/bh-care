-- Check if column exists
PRINT 'Checking if UserNumber column exists';
IF EXISTS(SELECT * FROM sys.columns 
          WHERE Name = N'UserNumber' AND Object_ID = Object_ID(N'AspNetUsers'))
BEGIN
    PRINT 'UserNumber column already exists';
END
ELSE
BEGIN
    PRINT 'UserNumber column does not exist';
    PRINT 'Adding column...';
    
    -- Add the column
    ALTER TABLE AspNetUsers ADD UserNumber INT NOT NULL DEFAULT 0;
    PRINT 'Column should be added';
END
GO

-- Check if column exists now
IF EXISTS(SELECT * FROM sys.columns 
          WHERE Name = N'UserNumber' AND Object_ID = Object_ID(N'AspNetUsers'))
BEGIN
    PRINT 'Verification: UserNumber column exists';
    
    -- Update with row numbers
    WITH NumberedUsers AS (
        SELECT Id, ROW_NUMBER() OVER (ORDER BY CreatedAt) AS RowNum
        FROM AspNetUsers
    )
    UPDATE AspNetUsers
    SET UserNumber = NumberedUsers.RowNum
    FROM NumberedUsers
    WHERE AspNetUsers.Id = NumberedUsers.Id;
    
    PRINT 'Users have been numbered';
END
ELSE
BEGIN
    PRINT 'ERROR: UserNumber column still does not exist';
END
GO 