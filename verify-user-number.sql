-- Check if the UserNumber column exists
IF EXISTS(SELECT * FROM sys.columns 
          WHERE Name = N'UserNumber' AND Object_ID = Object_ID(N'AspNetUsers'))
BEGIN
    PRINT 'UserNumber column exists';
    
    -- Show some data to verify it's set correctly
    SELECT TOP 5 Id, UserName, UserNumber FROM AspNetUsers ORDER BY UserNumber;
END
ELSE
BEGIN
    PRINT 'UserNumber column does NOT exist';
END 