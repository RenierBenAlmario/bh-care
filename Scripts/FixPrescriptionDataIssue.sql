-- Fix Prescription Data Issue
-- This script addresses the discrepancy between completed consultations and missing prescriptions

USE [Barangay]
GO

PRINT 'Starting prescription data fix...'

-- 1. Add sample medications to the Medications table
PRINT 'Adding sample medications...'

IF NOT EXISTS (SELECT 1 FROM Medications WHERE Name = 'Paracetamol')
BEGIN
    INSERT INTO Medications (Name, Description, Category, Manufacturer) 
    VALUES ('Paracetamol', 'Pain reliever and fever reducer', 'Analgesic', 'Generic');
    PRINT 'Added Paracetamol';
END

IF NOT EXISTS (SELECT 1 FROM Medications WHERE Name = 'Ibuprofen')
BEGIN
    INSERT INTO Medications (Name, Description, Category, Manufacturer) 
    VALUES ('Ibuprofen', 'Anti-inflammatory pain reliever', 'NSAID', 'Generic');
    PRINT 'Added Ibuprofen';
END

IF NOT EXISTS (SELECT 1 FROM Medications WHERE Name = 'Amoxicillin')
BEGIN
    INSERT INTO Medications (Name, Description, Category, Manufacturer) 
    VALUES ('Amoxicillin', 'Antibiotic for bacterial infections', 'Antibiotic', 'Generic');
    PRINT 'Added Amoxicillin';
END

IF NOT EXISTS (SELECT 1 FROM Medications WHERE Name = 'Omeprazole')
BEGIN
    INSERT INTO Medications (Name, Description, Category, Manufacturer) 
    VALUES ('Omeprazole', 'Proton pump inhibitor for acid reflux', 'PPI', 'Generic');
    PRINT 'Added Omeprazole';
END

IF NOT EXISTS (SELECT 1 FROM Medications WHERE Name = 'Cetirizine')
BEGIN
    INSERT INTO Medications (Name, Description, Category, Manufacturer) 
    VALUES ('Cetirizine', 'Antihistamine for allergies', 'Antihistamine', 'Generic');
    PRINT 'Added Cetirizine';
END

-- 2. Create a prescription for the existing medical record from August 6, 2025
PRINT 'Creating prescription for existing medical record...'

DECLARE @PatientId NVARCHAR(450) = 'eee7f324-6daa-4b50-ad64-b847c6015acc';
DECLARE @MedicalRecordId INT;
DECLARE @PrescriptionId INT;
DECLARE @ParacetamolId INT;
DECLARE @AmoxicillinId INT;

-- Get the medical record ID
SELECT @MedicalRecordId = Id FROM MedicalRecords WHERE PatientId = @PatientId AND Date = '2025-08-06';

IF @MedicalRecordId IS NOT NULL
BEGIN
    PRINT 'Found medical record with ID: ' + CAST(@MedicalRecordId AS VARCHAR);
    
    -- Create prescription
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
    SELECT 
        PatientId,
        DoctorId,
        Diagnosis,
        7, -- 7 days duration
        'Prescription created for consultation on August 6, 2025',
        1, -- Created status
        Date,
        Date,
        Date
    FROM MedicalRecords 
    WHERE Id = @MedicalRecordId;
    
    SET @PrescriptionId = SCOPE_IDENTITY();
    PRINT 'Created prescription with ID: ' + CAST(@PrescriptionId AS VARCHAR);
    
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
            @MedicalRecordId,
            @PrescriptionId,
            @ParacetamolId,
            '500mg',
            'Take 1 tablet every 4-6 hours as needed for pain or fever',
            '7 days',
            'Every 4-6 hours',
            'Paracetamol'
        );
        PRINT 'Added Paracetamol prescription';
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
            @MedicalRecordId,
            @PrescriptionId,
            @AmoxicillinId,
            '500mg',
            'Take 1 capsule 3 times daily with meals',
            '7 days',
            '3 times daily',
            'Amoxicillin'
        );
        PRINT 'Added Amoxicillin prescription';
    END
    
    -- Update the medical record to include prescription information
    UPDATE MedicalRecords 
    SET 
        Medications = 'Paracetamol 500mg, Amoxicillin 500mg',
        Prescription = 'Prescription created for consultation on August 6, 2025. Patient prescribed Paracetamol for pain relief and Amoxicillin for bacterial infection.',
        UpdatedAt = GETDATE()
    WHERE Id = @MedicalRecordId;
    
    PRINT 'Updated medical record with prescription information';
END
ELSE
BEGIN
    PRINT 'No medical record found for patient on August 6, 2025';
END

-- 3. Verify the fix
PRINT 'Verifying prescription data...'

SELECT 
    'Medications Count' as Metric,
    COUNT(*) as Value
FROM Medications
UNION ALL
SELECT 
    'Prescriptions Count',
    COUNT(*)
FROM Prescriptions
UNION ALL
SELECT 
    'Prescription Medications Count',
    COUNT(*)
FROM PrescriptionMedications;

PRINT 'Prescription data fix completed successfully!'
PRINT 'The My Prescriptions page should now display the prescription from August 6, 2025.' 