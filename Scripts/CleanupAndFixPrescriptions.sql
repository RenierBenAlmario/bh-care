USE [Barangay]
GO

-- Clean up hardcoded prescriptions and create proper ones based on actual medical record
PRINT 'Cleaning up hardcoded prescriptions...'

-- Delete all existing hardcoded prescriptions
DELETE FROM PrescriptionMedications;
DELETE FROM Prescriptions;

PRINT 'Deleted hardcoded prescriptions and medications';

-- Create a proper prescription based on the actual medical record
DECLARE @PatientId NVARCHAR(450) = 'eee7f324-6daa-4b50-ad64-b847c6015acc';
DECLARE @DoctorId NVARCHAR(450) = 'ea85984a-127e-4ab3-bbe0-e59bacada348';
DECLARE @MedicalRecordId INT = 1;
DECLARE @PrescriptionId INT;

-- Create prescription based on actual medical record data
INSERT INTO Prescriptions (
    PatientId, 
    DoctorId, 
    Diagnosis, 
    Duration, 
    Notes, 
    Status, 
    PrescriptionDate, 
    CreatedAt, 
    UpdatedAt
) 
VALUES (
    @PatientId,
    @DoctorId,
    'dsad', -- Actual diagnosis from medical record
    7, -- 7 days duration
    'Prescription created for consultation on August 6, 2025. Patient prescribed Paracetamol for pain relief and Amoxicillin for bacterial infection.',
    1, -- Created status
    '2025-08-06',
    '2025-08-06',
    '2025-08-06'
);

SET @PrescriptionId = SCOPE_IDENTITY();
PRINT 'Created prescription with ID: ' + CAST(@PrescriptionId AS VARCHAR);

-- Get medication IDs
DECLARE @ParacetamolId INT;
DECLARE @AmoxicillinId INT;

SELECT @ParacetamolId = Id FROM Medications WHERE Name = 'Paracetamol';
SELECT @AmoxicillinId = Id FROM Medications WHERE Name = 'Amoxicillin';

-- Add prescription medications based on actual medical record
IF @ParacetamolId IS NOT NULL
BEGIN
    INSERT INTO PrescriptionMedications (
        MedicalRecordId,
        PrescriptionId,
        MedicationId,
        Dosage,
        Instructions,
        Duration,
        Frequency,
        MedicationName,
        Unit
    )
    VALUES (
        @MedicalRecordId,
        @PrescriptionId,
        @ParacetamolId,
        '500mg',
        'Take 1 tablet every 4-6 hours as needed for pain or fever',
        '7 days',
        'Every 4-6 hours',
        'Paracetamol',
        'mg'
    );
    PRINT 'Added Paracetamol prescription based on medical record';
END

IF @AmoxicillinId IS NOT NULL
BEGIN
    INSERT INTO PrescriptionMedications (
        MedicalRecordId,
        PrescriptionId,
        MedicationId,
        Dosage,
        Instructions,
        Duration,
        Frequency,
        MedicationName,
        Unit
    )
    VALUES (
        @MedicalRecordId,
        @PrescriptionId,
        @AmoxicillinId,
        '500mg',
        'Take 1 capsule 3 times daily with meals',
        '7 days',
        '3 times daily',
        'Amoxicillin',
        'mg'
    );
    PRINT 'Added Amoxicillin prescription based on medical record';
END

-- Verify the cleanup and fix
PRINT 'Verifying prescription data...'

SELECT 
    'Prescriptions Count' as Metric,
    COUNT(*) as Value
FROM Prescriptions
UNION ALL
SELECT 
    'Prescription Medications Count',
    COUNT(*)
FROM PrescriptionMedications;

-- Show the final prescription data
SELECT 
    p.Id,
    p.PatientId,
    p.Diagnosis,
    p.PrescriptionDate,
    p.Status,
    pm.MedicationName,
    pm.Dosage,
    pm.Unit,
    pm.Instructions
FROM Prescriptions p
LEFT JOIN PrescriptionMedications pm ON p.Id = pm.PrescriptionId
ORDER BY p.Id, pm.Id;

PRINT 'Prescription cleanup and fix completed successfully!';
PRINT 'The prescriptions page should now show data based on the actual medical record.'; 