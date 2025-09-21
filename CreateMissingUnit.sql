-- Fix missing Unit values in PrescriptionMedications table
-- This script will add default 'unit' values to existing records

USE Barangay;

-- Check if there are records with null or empty Unit values
PRINT 'Checking for PrescriptionMedications with missing Unit values...';

-- Count how many records need updating
DECLARE @MissingUnitCount INT;
SELECT @MissingUnitCount = COUNT(*) 
FROM PrescriptionMedications 
WHERE Unit IS NULL OR Unit = '';

PRINT 'Found ' + CAST(@MissingUnitCount AS NVARCHAR) + ' records with missing Unit values.';

-- Update records with missing Unit values to 'mg' as a default
IF @MissingUnitCount > 0
BEGIN
    UPDATE PrescriptionMedications
    SET Unit = 'mg'
    WHERE Unit IS NULL OR Unit = '';
    
    PRINT 'Updated ' + CAST(@MissingUnitCount AS NVARCHAR) + ' records with default Unit value ''mg''';
END
ELSE
BEGIN
    PRINT 'No records need updating.';
END

-- Update records with missing Frequency values
UPDATE PrescriptionMedications
SET Frequency = 'As prescribed'
WHERE Frequency IS NULL OR Frequency = '';

PRINT 'Finished updating missing values.'; 