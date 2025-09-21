-- Check if UserNumber column exists in AspNetUsers table
IF EXISTS(SELECT * FROM sys.columns WHERE Name = N'UserNumber' AND Object_ID = Object_ID(N'AspNetUsers'))
BEGIN
    PRINT 'UserNumber column exists in AspNetUsers table';
    
    -- Get sample data
    SELECT TOP 5 Id, UserName, Email, UserNumber
    FROM AspNetUsers
    ORDER BY UserNumber;
END
ELSE
BEGIN
    PRINT 'UserNumber column does not exist in AspNetUsers table';
END 