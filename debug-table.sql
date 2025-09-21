-- Debug table issues
PRINT 'Checking database ownership and permissions';

-- Who am I?
SELECT SUSER_NAME() as 'Current User';

-- Check if I have permission to alter the table
PRINT 'Checking table permissions';
SELECT HAS_PERMS_BY_NAME('dbo.AspNetUsers', 'OBJECT', 'ALTER') as 'Can Alter AspNetUsers';

-- Check if the table is locked
PRINT 'Checking for locks on table';
SELECT * FROM sys.dm_tran_locks WHERE resource_associated_entity_id = 
    OBJECT_ID('dbo.AspNetUsers');

-- Try a different approach to add the column with better error handling
PRINT 'Attempting to add column with detailed error handling';
BEGIN TRY
    ALTER TABLE AspNetUsers ADD UserNumber INT NOT NULL DEFAULT 0;
    PRINT 'Column added successfully';
END TRY
BEGIN CATCH
    PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS VARCHAR);
    PRINT 'Error Severity: ' + CAST(ERROR_SEVERITY() AS VARCHAR);
    PRINT 'Error State: ' + CAST(ERROR_STATE() AS VARCHAR);
    PRINT 'Error Line: ' + CAST(ERROR_LINE() AS VARCHAR);
    PRINT 'Error Message: ' + ERROR_MESSAGE();
END CATCH 