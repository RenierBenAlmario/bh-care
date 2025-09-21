-- Check if the AspNetUsers table exists
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUsers')
BEGIN
    PRINT 'AspNetUsers table exists';
    
    -- List all columns
    PRINT 'Columns in AspNetUsers table:';
    SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'AspNetUsers';
END
ELSE
BEGIN
    PRINT 'AspNetUsers table does NOT exist';
    
    -- List all tables to see what's available
    PRINT 'Available tables:';
    SELECT TABLE_NAME 
    FROM INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_TYPE = 'BASE TABLE';
END 