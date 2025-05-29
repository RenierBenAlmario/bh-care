-- Database Structure Verification Script
-- Run this after applying migrations to verify the correct structure

-- Check if migration history table exists and show latest migrations
PRINT 'Checking migration history...'
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__EFMigrationsHistory')
BEGIN
    SELECT TOP 10 MigrationId, ProductVersion 
    FROM __EFMigrationsHistory 
    ORDER BY MigrationId DESC;
END
ELSE
BEGIN
    PRINT 'Migration history table does not exist!';
END

-- Check all tables in the database
PRINT 'Listing all tables in the database...'
SELECT t.TABLE_NAME, 
       COUNT(c.COLUMN_NAME) AS ColumnCount
FROM INFORMATION_SCHEMA.TABLES t
JOIN INFORMATION_SCHEMA.COLUMNS c ON t.TABLE_NAME = c.TABLE_NAME
WHERE t.TABLE_TYPE = 'BASE TABLE'
GROUP BY t.TABLE_NAME
ORDER BY t.TABLE_NAME;

-- Verify specific required tables
PRINT 'Verifying required tables exist...'
DECLARE @RequiredTables TABLE (TableName NVARCHAR(100))
INSERT INTO @RequiredTables VALUES
('AspNetUsers'), ('AspNetRoles'), ('AspNetUserRoles'),
('Patients'), ('Appointments'), ('MedicalRecords'),
('Prescriptions'), ('PrescriptionMedications'), ('VitalSigns'),
('Messages'), ('Notifications'), ('UserDocuments');

SELECT r.TableName, 
       CASE WHEN t.TABLE_NAME IS NULL THEN 'MISSING' ELSE 'EXISTS' END AS Status
FROM @RequiredTables r
LEFT JOIN INFORMATION_SCHEMA.TABLES t ON r.TableName = t.TABLE_NAME AND t.TABLE_TYPE = 'BASE TABLE';

-- Check UserDocuments table structure (from MigrationManager.cs)
PRINT 'Checking UserDocuments table structure...'
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserDocuments')
BEGIN
    SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'UserDocuments'
    ORDER BY ORDINAL_POSITION;
    
    -- Check for any NULL values that should not be NULL
    SELECT 'UserDocuments_FileName_NULL_count' AS CheckName, COUNT(*) AS NullCount
    FROM UserDocuments WHERE FileName IS NULL;
    
    SELECT 'UserDocuments_FilePath_NULL_count' AS CheckName, COUNT(*) AS NullCount
    FROM UserDocuments WHERE FilePath IS NULL;
    
    SELECT 'UserDocuments_Status_NULL_count' AS CheckName, COUNT(*) AS NullCount
    FROM UserDocuments WHERE Status IS NULL;
END
ELSE
BEGIN
    PRINT 'UserDocuments table does not exist!';
END

-- Check foreign key relationships (focus on cascade delete settings)
PRINT 'Checking foreign key relationships with ON DELETE CASCADE...'
SELECT 
    fk.name AS ForeignKeyName,
    OBJECT_NAME(fk.parent_object_id) AS ParentTable,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ParentColumn,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumn,
    CASE fk.delete_referential_action
        WHEN 0 THEN 'NO_ACTION'
        WHEN 1 THEN 'CASCADE'
        WHEN 2 THEN 'SET_NULL'
        WHEN 3 THEN 'SET_DEFAULT'
    END AS OnDelete
FROM 
    sys.foreign_keys fk
INNER JOIN 
    sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
WHERE 
    fk.delete_referential_action = 1; -- CASCADE

-- Check database connection string from appsettings.json
PRINT 'Connection string in appsettings.json is set to:'
PRINT 'Server=DESKTOP-NU53VS3;Database=Barangay;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True'; 