-- Check current database schema
USE [bhcareDB]
GO

-- Check if PrescriptionMedicines exists
SELECT 'PrescriptionMedicines table exists' AS Status
WHERE EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PrescriptionMedicines')
UNION ALL
SELECT 'PrescriptionMedicines table does NOT exist' AS Status
WHERE NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PrescriptionMedicines');

-- Check if PrescriptionMedications has Unit column
SELECT 'Unit column exists in PrescriptionMedications' AS Status
WHERE EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('PrescriptionMedications') 
    AND name = 'Unit'
)
UNION ALL
SELECT 'Unit column does NOT exist in PrescriptionMedications' AS Status
WHERE NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('PrescriptionMedications') 
    AND name = 'Unit'
);

-- Check if LabResults table exists
SELECT 'LabResults table exists' AS Status
WHERE EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LabResults')
UNION ALL
SELECT 'LabResults table does NOT exist' AS Status
WHERE NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LabResults');
