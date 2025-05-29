-- Verify Database Script
-- This script checks if the database is properly updated and working

PRINT 'Starting database verification...'

-- 1. Check if AspNetUsers table has the required columns
PRINT '1. Checking AspNetUsers table columns...'
SELECT 
    'AspNetUsers' AS TableName,
    COUNT(*) AS ColumnCount,
    SUM(CASE WHEN name IN ('MiddleName', 'Status', 'Suffix') THEN 1 ELSE 0 END) AS RequiredColumnsCount
FROM 
    sys.columns 
WHERE 
    object_id = OBJECT_ID('AspNetUsers');

-- 2. Check Messages table foreign key constraints
PRINT '2. Checking Messages table foreign key constraints...'
SELECT 
    fk.name AS 'Foreign Key', 
    OBJECT_NAME(fk.parent_object_id) AS 'Table',
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS 'Column',
    OBJECT_NAME(fk.referenced_object_id) AS 'Referenced Table',
    fk.delete_referential_action_desc AS 'Delete Action'
FROM 
    sys.foreign_keys fk
INNER JOIN 
    sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
WHERE 
    OBJECT_NAME(fk.parent_object_id) = 'Messages'
ORDER BY 
    fk.name;

-- 3. Check if all required tables exist
PRINT '3. Checking if all required tables exist...'
SELECT 
    name AS 'Table Name', 
    'Exists' AS 'Status'
FROM 
    sys.tables
WHERE 
    name IN ('AspNetUsers', 'Messages', 'Appointments', 'AppointmentAttachments', 'AppointmentFiles', 
             'MedicalRecords', 'Prescriptions', 'PrescriptionMedications', 'Medications')
ORDER BY 
    name;

-- 4. Check if all migrations are applied
PRINT '4. Checking if all migrations are applied...'
SELECT TOP 10
    [MigrationId], 
    [ProductVersion]
FROM 
    [__EFMigrationsHistory]
ORDER BY 
    [MigrationId] DESC;

-- 5. Check for any potential issues
PRINT '5. Checking for any potential issues...'
-- Check for tables without primary keys
SELECT 
    t.name AS 'Table Without Primary Key'
FROM 
    sys.tables t
LEFT JOIN 
    sys.indexes i ON t.object_id = i.object_id AND i.is_primary_key = 1
WHERE 
    i.object_id IS NULL
ORDER BY 
    t.name;

-- Check for foreign keys with CASCADE DELETE that might cause issues
SELECT 
    fk.name AS 'Potential Problem Foreign Key', 
    OBJECT_NAME(fk.parent_object_id) AS 'Table',
    OBJECT_NAME(fk.referenced_object_id) AS 'Referenced Table',
    fk.delete_referential_action_desc AS 'Delete Action'
FROM 
    sys.foreign_keys fk
WHERE 
    fk.delete_referential_action_desc = 'CASCADE' AND
    OBJECT_NAME(fk.parent_object_id) IN ('Messages')
ORDER BY 
    fk.name;

PRINT 'Database verification completed!' 