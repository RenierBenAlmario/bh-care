-- RegisterUserTest.sql
-- SQL Script to diagnose and test user registration issues
-- Run this script to verify database state and manually create a test user

-- Step 1: Check Database Schema
PRINT '======== CHECKING DATABASE SCHEMA ========';

-- Check AspNetUsers table
IF OBJECT_ID('AspNetUsers', 'U') IS NOT NULL
BEGIN
    PRINT '✓ AspNetUsers table exists';
    
    -- Check required columns
    DECLARE @missingColumns NVARCHAR(MAX) = '';
    
    IF COL_LENGTH('AspNetUsers', 'Id') IS NULL
        SET @missingColumns = @missingColumns + 'Id, ';
    IF COL_LENGTH('AspNetUsers', 'UserName') IS NULL
        SET @missingColumns = @missingColumns + 'UserName, ';
    IF COL_LENGTH('AspNetUsers', 'Email') IS NULL
        SET @missingColumns = @missingColumns + 'Email, ';
    IF COL_LENGTH('AspNetUsers', 'Status') IS NULL
        SET @missingColumns = @missingColumns + 'Status, ';
    IF COL_LENGTH('AspNetUsers', 'CreatedAt') IS NULL
        SET @missingColumns = @missingColumns + 'CreatedAt, ';
        
    IF LEN(@missingColumns) > 0
    BEGIN
        SET @missingColumns = LEFT(@missingColumns, LEN(@missingColumns) - 1); -- Remove trailing comma
        PRINT '❌ Missing columns in AspNetUsers: ' + @missingColumns;
    END
    ELSE
    BEGIN
        PRINT '✓ All required columns exist in AspNetUsers';
    END
    
    -- Show sample data
    PRINT 'Sample user data:';
    SELECT TOP 5 Id, UserName, Email, Status, CreatedAt FROM AspNetUsers;
    
    -- Show status distribution
    PRINT 'Status distribution:';
    SELECT Status, COUNT(*) AS Count 
    FROM AspNetUsers 
    GROUP BY Status;
END
ELSE
BEGIN
    PRINT '❌ AspNetUsers table does NOT exist!';
END

-- Check UserDocuments table
IF OBJECT_ID('UserDocuments', 'U') IS NOT NULL
BEGIN
    PRINT '✓ UserDocuments table exists';
    
    -- Check required columns
    DECLARE @missingDocCols NVARCHAR(MAX) = '';
    
    IF COL_LENGTH('UserDocuments', 'Id') IS NULL
        SET @missingDocCols = @missingDocCols + 'Id, ';
    IF COL_LENGTH('UserDocuments', 'UserId') IS NULL
        SET @missingDocCols = @missingDocCols + 'UserId, ';
    IF COL_LENGTH('UserDocuments', 'FileName') IS NULL
        SET @missingDocCols = @missingDocCols + 'FileName, ';
    IF COL_LENGTH('UserDocuments', 'ContentType') IS NULL
        SET @missingDocCols = @missingDocCols + 'ContentType, ';
        
    IF LEN(@missingDocCols) > 0
    BEGIN
        SET @missingDocCols = LEFT(@missingDocCols, LEN(@missingDocCols) - 1); -- Remove trailing comma
        PRINT '❌ Missing columns in UserDocuments: ' + @missingDocCols;
    END
    ELSE
    BEGIN
        PRINT '✓ All required columns exist in UserDocuments';
    END
    
    -- Show sample data
    PRINT 'Sample document data:';
    SELECT TOP 5 Id, UserId, FileName, ContentType FROM UserDocuments;
END
ELSE
BEGIN
    PRINT '❌ UserDocuments table does NOT exist!';
END

-- Step 2: Check Foreign Key Relationships
PRINT '======== CHECKING RELATIONSHIPS ========';
IF OBJECT_ID('UserDocuments', 'U') IS NOT NULL AND OBJECT_ID('AspNetUsers', 'U') IS NOT NULL
BEGIN
    -- Check if foreign key exists
    IF EXISTS (
        SELECT * 
        FROM sys.foreign_keys 
        WHERE parent_object_id = OBJECT_ID('UserDocuments')
        AND referenced_object_id = OBJECT_ID('AspNetUsers')
    )
    BEGIN
        PRINT '✓ Foreign key relationship exists between UserDocuments and AspNetUsers';
    END
    ELSE
    BEGIN
        PRINT '❌ No foreign key relationship between UserDocuments and AspNetUsers';
    END
    
    -- Check for orphaned records
    DECLARE @orphanedCount INT;
    SELECT @orphanedCount = COUNT(*) 
    FROM UserDocuments ud
    LEFT JOIN AspNetUsers u ON ud.UserId = u.Id
    WHERE u.Id IS NULL;
    
    IF @orphanedCount > 0
        PRINT '❌ Found ' + CAST(@orphanedCount AS VARCHAR) + ' orphaned document records';
    ELSE
        PRINT '✓ No orphaned document records found';
END

-- Step 3: Create Test User (Manually)
PRINT '======== TEST USER CREATION ========';
PRINT 'Would you like to create a test user? (This is simulated in SQL - actually creating a user requires ASP.NET Identity.)';
PRINT 'For a real test, use the RegistrationDebug API endpoint: /api/RegistrationDebug/test-create-user';

-- The following is a simulation of how user creation works in the system
-- In reality, ASP.NET Identity manages password hashing, user creation, etc.
DECLARE @testUserId NVARCHAR(450) = NEWID();
DECLARE @testEmail NVARCHAR(256) = 'test_' + REPLACE(CONVERT(VARCHAR(36), NEWID()), '-', '') + '@example.com';
DECLARE @testUserName NVARCHAR(256) = @testEmail;
DECLARE @now DATETIME2 = GETUTCDATE();

SELECT 
    @testUserId AS 'TestUserId',
    @testEmail AS 'TestEmail',
    @testUserName AS 'TestUserName',
    'Pending' AS 'Status'


-- Step 4: Verify Indexes
PRINT '======== CHECKING INDEXES ========';

-- Check AspNetUsers indexes
PRINT 'AspNetUsers Indexes:';
SELECT 
    i.name AS IndexName,
    i.type_desc AS IndexType,
    STRING_AGG(c.name, ', ') WITHIN GROUP (ORDER BY ic.key_ordinal) AS IndexColumns
FROM 
    sys.indexes i
INNER JOIN 
    sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
INNER JOIN 
    sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
WHERE 
    i.object_id = OBJECT_ID('AspNetUsers')
GROUP BY 
    i.name, i.type_desc
ORDER BY 
    i.name;

-- Step 5: Check ASP.NET Identity tables
PRINT '======== CHECKING ASP.NET IDENTITY TABLES ========';
DECLARE @identityTables TABLE (TableName NVARCHAR(128));
INSERT INTO @identityTables (TableName)
VALUES 
    ('AspNetRoles'),
    ('AspNetUserRoles'),
    ('AspNetUserClaims'),
    ('AspNetUserLogins'),
    ('AspNetUserTokens'),
    ('AspNetRoleClaims');
    
DECLARE @missingIdentityTables NVARCHAR(MAX) = '';
DECLARE @tableName NVARCHAR(128);
DECLARE @tableExists BIT;

DECLARE tableCursor CURSOR FOR 
SELECT TableName FROM @identityTables;

OPEN tableCursor;
FETCH NEXT FROM tableCursor INTO @tableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF OBJECT_ID(@tableName, 'U') IS NULL
        SET @missingIdentityTables = @missingIdentityTables + @tableName + ', ';
        
    FETCH NEXT FROM tableCursor INTO @tableName;
END

CLOSE tableCursor;
DEALLOCATE tableCursor;

IF LEN(@missingIdentityTables) > 0
BEGIN
    SET @missingIdentityTables = LEFT(@missingIdentityTables, LEN(@missingIdentityTables) - 1); -- Remove trailing comma
    PRINT '❌ Missing ASP.NET Identity tables: ' + @missingIdentityTables;
END
ELSE
    PRINT '✓ All required ASP.NET Identity tables exist';

-- Summary
PRINT '======== SUMMARY ========';
PRINT 'Database structure check complete. Next steps:';
PRINT '1. If any tables or columns are missing, ensure EF Core migrations have been applied';
PRINT '2. Test user creation via RegistrationDebug API endpoint';
PRINT '3. Check application logs for any exceptions during registration';
PRINT '4. Verify user status values match frontend filtering expectations'; 