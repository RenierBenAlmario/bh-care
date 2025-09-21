USE [Barangay]
GO

-- Create prescription for the existing medical record
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
    'eee7f324-6daa-4b50-ad64-b847c6015acc',
    'ea85984a-127e-4ab3-bbe0-e59bacada348',
    'dsad',
    7,
    'Prescription created for consultation on August 6, 2025',
    1,
    '2025-08-06',
    '2025-08-06',
    '2025-08-06'
);

-- Get the prescription ID
DECLARE @PrescriptionId INT = SCOPE_IDENTITY();
DECLARE @ParacetamolId INT;
DECLARE @AmoxicillinId INT;

-- Get medication IDs
SELECT @ParacetamolId = Id FROM Medications WHERE Name = 'Paracetamol';
SELECT @AmoxicillinId = Id FROM Medications WHERE Name = 'Amoxicillin';

-- Add prescription medications
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
        MedicationName
    )
    VALUES (
        1, -- MedicalRecordId
        @PrescriptionId,
        @ParacetamolId,
        '500mg',
        'Take 1 tablet every 4-6 hours as needed for pain or fever',
        '7 days',
        'Every 4-6 hours',
        'Paracetamol'
    );
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
        MedicationName
    )
    VALUES (
        1, -- MedicalRecordId
        @PrescriptionId,
        @AmoxicillinId,
        '500mg',
        'Take 1 capsule 3 times daily with meals',
        '7 days',
        '3 times daily',
        'Amoxicillin'
    );
END

-- Update medical record
UPDATE MedicalRecords 
SET 
    Medications = 'Paracetamol 500mg, Amoxicillin 500mg',
    Prescription = 'Prescription created for consultation on August 6, 2025. Patient prescribed Paracetamol for pain relief and Amoxicillin for bacterial infection.',
    UpdatedAt = GETDATE()
WHERE Id = 1;

PRINT 'Prescription created successfully!'; 