-- Investigate Notification Issues

-- 1. Check if there are any hardcoded notifications or test data in the Notifications table
SELECT TOP 20 * FROM Notifications ORDER BY CreatedAt DESC;

-- 2. Check connection string info from ConnectionStrings table (if exists)
IF OBJECT_ID('ConnectionStrings', 'U') IS NOT NULL
BEGIN
    SELECT * FROM ConnectionStrings;
END
ELSE
BEGIN
    PRINT 'ConnectionStrings table does not exist';
END

-- 3. Check if we have multiple databases by seeing what databases exist on this server
SELECT name, database_id, create_date
FROM sys.databases
ORDER BY name;

-- 4. Check AspNetUsers table for any records regardless of Status
SELECT TOP 20
    Id, UserName, Email, Status, IsActive, CreatedAt
FROM AspNetUsers
ORDER BY CreatedAt DESC;

-- 5. Check for database triggers that might affect notification counts
SELECT * FROM sys.triggers;

-- 6. Check if there are any stored procedures related to notifications
SELECT name, type_desc FROM sys.objects 
WHERE type = 'P' AND name LIKE '%Notification%';

-- 7. Check if the table structure and columns exist
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE 
FROM 
    INFORMATION_SCHEMA.COLUMNS
WHERE 
    TABLE_NAME = 'Notifications';
    
-- 8. Check for user activity that might trigger notifications
SELECT TOP 20 
    u.UserName, 
    u.Email,
    n.Message,
    n.CreatedAt
FROM Notifications n
LEFT JOIN AspNetUsers u ON n.Message LIKE '%' + u.UserName + '%'
ORDER BY n.CreatedAt DESC; 