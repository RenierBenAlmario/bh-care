-- Show current database
SELECT DB_NAME() AS 'Current Database';

-- List all tables
PRINT '=== All Tables ===';
SELECT 
    t.name AS 'TableName',
    SCHEMA_NAME(t.schema_id) AS 'SchemaName'
FROM 
    sys.tables t
ORDER BY 
    t.name;

-- Check if AspNetUsers exists (to verify Identity tables are present)
PRINT '=== Identity Tables Check ===';
IF OBJECT_ID('AspNetUsers', 'U') IS NOT NULL
    PRINT 'AspNetUsers table exists - Identity framework is properly installed'
ELSE
    PRINT 'AspNetUsers table does not exist - Identity framework may not be properly installed';

-- Check database connection info
PRINT '=== Database Connection Info ===';
SELECT 
    @@SERVERNAME AS 'Server',
    @@VERSION AS 'SQL Server Version';

-- Look for any table that might contain appointment data
PRINT '=== Tables that might contain appointment data ===';
SELECT 
    t.name AS 'TableName'
FROM 
    sys.tables t
WHERE 
    t.name LIKE '%Appoint%' OR
    t.name LIKE '%Booking%' OR
    t.name LIKE '%Schedule%' OR
    t.name LIKE '%Patient%' OR
    t.name LIKE '%Doctor%'
ORDER BY 
    t.name; 