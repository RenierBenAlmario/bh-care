-- Add missing ValidUntil column to Prescriptions table
-- Run this script to fix the error "Invalid column name 'ValidUntil'"

USE Barangay;

-- Check if ValidUntil column exists, if not add it
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Prescriptions' AND COLUMN_NAME = 'ValidUntil')
BEGIN
    PRINT 'Adding ValidUntil column to Prescriptions table...';
    ALTER TABLE Prescriptions
    ADD ValidUntil DATETIME2 NULL;
    PRINT 'ValidUntil column added successfully.';
END
ELSE
BEGIN
    PRINT 'ValidUntil column already exists in Prescriptions table.';
END 