-- Script to fix the Status column in the Appointments table
-- This script will convert the Status column from nvarchar to int if needed

-- First, check if the Status column is nvarchar
IF EXISTS (
    SELECT 1
    FROM sys.columns c
    JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('Appointments')
    AND c.name = 'Status'
    AND t.name = 'nvarchar'
)
BEGIN
    PRINT 'Status column is nvarchar. Converting to int...';
    
    -- Create a backup of the Appointments table
    SELECT * INTO Appointments_Backup FROM Appointments;
    
    -- Print a message
    PRINT 'Backup created as Appointments_Backup';
    
    -- Update the Status values to match the enum values
    UPDATE Appointments SET 
        Status = 
            CASE 
                WHEN Status = 'Pending' THEN '0'
                WHEN Status = 'Confirmed' THEN '1'
                WHEN Status = 'InProgress' THEN '2'
                WHEN Status = 'Completed' THEN '3'
                WHEN Status = 'Cancelled' THEN '4'
                WHEN Status = 'Urgent' THEN '5'
                WHEN Status = 'NoShow' THEN '6'
                ELSE '0' -- Default to Pending if unknown
            END;
    
    -- Alter the column type
    ALTER TABLE Appointments ALTER COLUMN Status INT NOT NULL;
    
    PRINT 'Status column successfully converted to int.';
END
ELSE
BEGIN
    PRINT 'Status column is already the correct type. No changes needed.';
END 