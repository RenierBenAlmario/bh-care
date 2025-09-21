-- Simple check for column existence with printouts
PRINT 'Checking if UserNumber column exists';

-- Check for the column existence
DECLARE @ColumnExists BIT = 0;

SELECT @ColumnExists = 1 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'UserNumber';

IF @ColumnExists = 1
BEGIN
    PRINT 'SUCCESS: UserNumber column exists in AspNetUsers table';
    
    -- Display some sample data
    PRINT 'Sample data:';
    SELECT TOP 5 UserName, UserNumber FROM AspNetUsers ORDER BY UserNumber;
END
ELSE
BEGIN
    PRINT 'FAILURE: UserNumber column does NOT exist in AspNetUsers table';
END 