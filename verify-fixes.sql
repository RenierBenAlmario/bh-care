-- SQL script to verify the fixes applied to the database
USE [Barangay];

-- 1. Verify Message-User foreign key constraint definitions
SELECT 
    fk.name AS ForeignKeyName, 
    OBJECT_NAME(fk.parent_object_id) AS TableName,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ColumnName,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTableName,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumnName,
    delete_referential_action_desc AS DeleteAction
FROM 
    sys.foreign_keys AS fk
INNER JOIN 
    sys.foreign_key_columns AS fkc 
        ON fk.object_id = fkc.constraint_object_id
WHERE 
    OBJECT_NAME(fk.parent_object_id) = 'Messages';

-- 2. Verify there are no NULL values in critical AspNetUsers columns
SELECT 
    'Email NULL Count' AS Column_Check,
    COUNT(*) AS NullCount
FROM 
    [AspNetUsers]
WHERE 
    [Email] IS NULL
UNION
SELECT 
    'UserName NULL Count' AS Column_Check,
    COUNT(*) AS NullCount
FROM 
    [AspNetUsers]
WHERE 
    [UserName] IS NULL
UNION
SELECT 
    'NormalizedEmail NULL Count' AS Column_Check,
    COUNT(*) AS NullCount
FROM 
    [AspNetUsers]
WHERE 
    [NormalizedEmail] IS NULL
UNION
SELECT 
    'NormalizedUserName NULL Count' AS Column_Check,
    COUNT(*) AS NullCount
FROM 
    [AspNetUsers]
WHERE 
    [NormalizedUserName] IS NULL;

-- 3. Check for any empty string values in Email/UserName (which indicates our fixes applied)
SELECT 
    'Email Empty String Count' AS Column_Check,
    COUNT(*) AS Count
FROM 
    [AspNetUsers]
WHERE 
    [Email] = ''
UNION
SELECT 
    'UserName Empty String Count' AS Column_Check,
    COUNT(*) AS Count
FROM 
    [AspNetUsers]
WHERE 
    [UserName] = '';

-- 4. Check migration history to confirm our fix has been recorded
SELECT 
    [MigrationId], 
    [ProductVersion]
FROM 
    [__EFMigrationsHistory]
WHERE 
    [MigrationId] = 'FixMessageUserRelationships';

-- 5. Verify UserId foreign key is correctly defined in the Patients table
SELECT 
    fk.name AS ForeignKeyName, 
    OBJECT_NAME(fk.parent_object_id) AS TableName,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ColumnName,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTableName,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumnName,
    delete_referential_action_desc AS DeleteAction
FROM 
    sys.foreign_keys AS fk
INNER JOIN 
    sys.foreign_key_columns AS fkc 
        ON fk.object_id = fkc.constraint_object_id
WHERE 
    OBJECT_NAME(fk.parent_object_id) = 'Patients'
    OR OBJECT_NAME(fk.referenced_object_id) = 'Patients';

PRINT 'Verification complete. Check the results to ensure all fixes were properly applied.';
GO 