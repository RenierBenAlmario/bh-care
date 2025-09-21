-- Fix the naming mismatch between PrescriptionMedicines and PrescriptionMedications
-- Run this script to fix the error "Invalid object name 'PrescriptionMedicines'"

USE Barangay;

-- Check if there's a query that refers to PrescriptionMedicines
PRINT 'Updating queries that might be using PrescriptionMedicines instead of PrescriptionMedications';

-- Create a synonym to allow both table names to work
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'PrescriptionMedications')
    AND NOT EXISTS (SELECT * FROM sys.synonyms WHERE name = 'PrescriptionMedicines')
BEGIN
    PRINT 'Creating synonym PrescriptionMedicines for PrescriptionMedications';
    CREATE SYNONYM PrescriptionMedicines FOR PrescriptionMedications;
    PRINT 'Synonym created successfully. Now queries using either name will work.';
END
ELSE IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PrescriptionMedications')
BEGIN
    PRINT 'Error: PrescriptionMedications table does not exist. Please create the table first.';
END
ELSE
BEGIN
    PRINT 'Synonym PrescriptionMedicines already exists.';
END 