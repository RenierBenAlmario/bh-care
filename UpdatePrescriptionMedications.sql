-- Add missing MedicationName column to PrescriptionMedications table
-- Run this script to fix the error "Invalid column name 'MedicationName'"

USE Barangay;

-- Check if MedicationName column exists, if not add it
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'PrescriptionMedications' AND COLUMN_NAME = 'MedicationName')
BEGIN
    PRINT 'Adding MedicationName column to PrescriptionMedications table...';
    ALTER TABLE PrescriptionMedications
    ADD MedicationName NVARCHAR(255) NOT NULL DEFAULT 'Unknown Medication';
    
    PRINT 'MedicationName column added successfully.';
    
    -- Note: After adding this column, medication names will need to be set 
    -- in the application code when creating new prescription medications
END
ELSE
BEGIN
    PRINT 'MedicationName column already exists in PrescriptionMedications table.';
END 