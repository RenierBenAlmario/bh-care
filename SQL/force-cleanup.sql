-- Force Database Cleanup Script
-- This script removes all data except the system admin account
-- Admin account: admin@example.com (preserved)

USE [bhcareDB]
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

PRINT 'Starting force database cleanup...';
PRINT 'Preserving system admin account: admin@example.com';

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- Get admin user ID for reference
    DECLARE @AdminUserId NVARCHAR(450);
    SELECT @AdminUserId = Id FROM AspNetUsers WHERE NormalizedEmail = 'ADMIN@EXAMPLE.COM';
    
    IF @AdminUserId IS NULL
    BEGIN
        PRINT 'ERROR: Admin account not found! Aborting cleanup for safety.';
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    PRINT 'Admin account found with ID: ' + @AdminUserId;
    
    -- Disable all foreign key constraints
    PRINT 'Disabling foreign key constraints...';
    DECLARE @sql NVARCHAR(MAX) = '';
    SELECT @sql = @sql + 'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(name) + ' NOCHECK CONSTRAINT ALL; '
    FROM sys.tables;
    EXEC sp_executesql @sql;
    
    -- Clear all data from all tables except AspNetUsers
    PRINT 'Clearing all data...';
    
    -- Clear all tables except AspNetUsers
    DECLARE @tableName NVARCHAR(128);
    DECLARE table_cursor CURSOR FOR
        SELECT name FROM sys.tables WHERE name != 'AspNetUsers' AND name != 'AspNetRoles' AND name != 'AspNetRoleClaims';
    
    OPEN table_cursor;
    FETCH NEXT FROM table_cursor INTO @tableName;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @sql = 'DELETE FROM ' + QUOTENAME(@tableName);
        EXEC sp_executesql @sql;
        PRINT 'Cleared ' + @tableName;
        FETCH NEXT FROM table_cursor INTO @tableName;
    END
    
    CLOSE table_cursor;
    DEALLOCATE table_cursor;
    
    -- Clear all users except admin
    PRINT 'Clearing all users except admin...';
    DELETE FROM AspNetUsers WHERE Id != @AdminUserId;
    PRINT 'Cleared all users (preserved admin account)';
    
    -- Reset admin user number to 1
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'UserNumber')
    BEGIN
        UPDATE AspNetUsers SET UserNumber = 1 WHERE Id = @AdminUserId;
        PRINT 'Reset admin user number to 1';
    END
    
    -- Re-enable all foreign key constraints
    PRINT 'Re-enabling foreign key constraints...';
    SET @sql = '';
    SELECT @sql = @sql + 'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(name) + ' WITH CHECK CHECK CONSTRAINT ALL; '
    FROM sys.tables;
    EXEC sp_executesql @sql;
    
    -- Verify admin account is still intact
    PRINT 'Verifying admin account integrity...';
    
    DECLARE @AdminEmail NVARCHAR(256);
    DECLARE @AdminStatus NVARCHAR(50);
    DECLARE @TotalUsers INT;
    
    SELECT @AdminEmail = Email, @AdminStatus = Status 
    FROM AspNetUsers 
    WHERE Id = @AdminUserId;
    
    SELECT @TotalUsers = COUNT(*) FROM AspNetUsers;
    
    IF @AdminEmail IS NOT NULL AND @TotalUsers = 1
    BEGIN
        PRINT 'Admin account verified: ' + @AdminEmail + ' (Status: ' + ISNULL(@AdminStatus, 'Unknown') + ')';
        PRINT 'Total users remaining: ' + CAST(@TotalUsers AS NVARCHAR);
    END
    ELSE
    BEGIN
        PRINT 'ERROR: Admin account verification failed!';
        PRINT 'Admin Email: ' + ISNULL(@AdminEmail, 'NULL');
        PRINT 'Total Users: ' + CAST(@TotalUsers AS NVARCHAR);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    -- Final cleanup statistics
    PRINT 'Cleanup completed successfully!';
    PRINT 'Database has been reset to initial state with only the system admin account preserved.';
    PRINT 'Admin account: ' + @AdminEmail;
    PRINT 'All other user accounts, patient data, appointments, and forms have been cleared.';
    
    COMMIT TRANSACTION;
    
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
        
    PRINT 'ERROR: Database cleanup failed!';
    PRINT 'Error Message: ' + ERROR_MESSAGE();
    PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR);
    PRINT 'Error Severity: ' + CAST(ERROR_SEVERITY() AS NVARCHAR);
    PRINT 'Error State: ' + CAST(ERROR_STATE() AS NVARCHAR);
    PRINT 'Error Line: ' + CAST(ERROR_LINE() AS NVARCHAR);
    
    -- Re-enable foreign key constraints in case of error
    SET @sql = '';
    SELECT @sql = @sql + 'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(name) + ' WITH CHECK CHECK CONSTRAINT ALL; '
    FROM sys.tables;
    EXEC sp_executesql @sql;
END CATCH

PRINT 'Database cleanup script execution completed.';
