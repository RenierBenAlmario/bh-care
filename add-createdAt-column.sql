-- Check if the column exists first to avoid errors
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'StaffMembers' AND COLUMN_NAME = 'CreatedAt'
)
BEGIN
    -- Add the CreatedAt column with a default value of current date/time
    ALTER TABLE StaffMembers
    ADD CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE();
    
    PRINT 'CreatedAt column added successfully to StaffMembers table.';
END
ELSE
BEGIN
    PRINT 'CreatedAt column already exists in StaffMembers table.';
END 