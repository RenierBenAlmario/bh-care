-- This script fixes the Gender column size issue in the Appointments table
USE Barangay;

-- Verify we're in the correct database
PRINT 'Current database: ' + DB_NAME();

-- Check the current size of the Gender column
PRINT 'Current Gender column size:';
SELECT 
    c.name AS 'ColumnName',
    t.name AS 'DataType',
    c.max_length AS 'MaxLength',
    c.is_nullable AS 'IsNullable'
FROM 
    sys.columns c
JOIN 
    sys.types t ON c.user_type_id = t.user_type_id
WHERE 
    c.object_id = OBJECT_ID('Appointments')
    AND c.name = 'Gender';

-- Alter the Gender column to allow longer values (50 characters) and be nullable
ALTER TABLE Appointments ALTER COLUMN Gender NVARCHAR(50) NULL;
PRINT 'Gender column altered to NVARCHAR(50) NULL';

-- Verify the change
PRINT 'Updated Gender column size:';
SELECT 
    c.name AS 'ColumnName',
    t.name AS 'DataType',
    c.max_length AS 'MaxLength',
    c.is_nullable AS 'IsNullable'
FROM 
    sys.columns c
JOIN 
    sys.types t ON c.user_type_id = t.user_type_id
WHERE 
    c.object_id = OBJECT_ID('Appointments')
    AND c.name = 'Gender';

-- Update any existing "Not Specified" values to fit within the column size
UPDATE Appointments
SET Gender = 'Unknown'
WHERE Gender = 'Not Specified';
PRINT 'Updated existing Gender values that might be too long';

-- Ensure we have a default value for NULL Gender values
ALTER TABLE Appointments ADD CONSTRAINT DF_Appointments_Gender DEFAULT 'Unknown' FOR Gender;
PRINT 'Added default constraint for Gender column';

PRINT 'Fix completed successfully'; 