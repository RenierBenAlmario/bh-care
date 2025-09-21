-- Copy any existing medications from medical records to prescriptions
-- This script will extract medication names from MedicalRecords and create PrescriptionMedication entries

USE Barangay;

PRINT 'Starting medication sync process...';

-- Create a temporary table to store all prescriptions that need medications
CREATE TABLE #PrescriptionsToUpdate (
    PrescriptionId INT,
    MedicalRecordId INT,
    MedicationsText NVARCHAR(MAX)
);

-- Find all prescriptions with no medications but linked to medical records that have medications
INSERT INTO #PrescriptionsToUpdate (PrescriptionId, MedicalRecordId, MedicationsText)
SELECT 
    p.Id AS PrescriptionId,
    mr.Id AS MedicalRecordId,
    mr.Medications
FROM Prescriptions p
JOIN MedicalRecords mr ON mr.AppointmentId = p.Id
LEFT JOIN PrescriptionMedications pm ON pm.PrescriptionId = p.Id
WHERE 
    mr.Medications IS NOT NULL 
    AND mr.Medications <> ''
    AND NOT EXISTS (
        SELECT 1 
        FROM PrescriptionMedications 
        WHERE PrescriptionId = p.Id
    );

-- Display count of prescriptions to update
DECLARE @Count INT = (SELECT COUNT(*) FROM #PrescriptionsToUpdate);
PRINT 'Found ' + CAST(@Count AS NVARCHAR) + ' prescriptions to update with medications';

-- For each prescription that needs medications, parse the medications text and add entries
DECLARE @PrescriptionId INT, @MedicalRecordId INT, @MedicationsText NVARCHAR(MAX);
DECLARE @MedicationName NVARCHAR(255);
DECLARE @MedicationId INT;

-- Cursor to process each prescription
DECLARE prescription_cursor CURSOR FOR 
    SELECT PrescriptionId, MedicalRecordId, MedicationsText 
    FROM #PrescriptionsToUpdate;

OPEN prescription_cursor;
FETCH NEXT FROM prescription_cursor INTO @PrescriptionId, @MedicalRecordId, @MedicationsText;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT 'Processing prescription ID: ' + CAST(@PrescriptionId AS NVARCHAR);
    
    -- Split medication names by comma
    DECLARE @Position INT = 1;
    DECLARE @NextComma INT;
    
    WHILE @Position <= LEN(@MedicationsText)
    BEGIN
        SET @NextComma = CHARINDEX(',', @MedicationsText, @Position);
        
        IF @NextComma = 0
            SET @NextComma = LEN(@MedicationsText) + 1;
        
        SET @MedicationName = LTRIM(RTRIM(SUBSTRING(@MedicationsText, @Position, @NextComma - @Position)));
        
        IF LEN(@MedicationName) > 0
        BEGIN
            -- Find or create medication
            SELECT @MedicationId = Id FROM Medications WHERE Name = @MedicationName;
            
            IF @MedicationId IS NULL
            BEGIN
                INSERT INTO Medications (Name, Description) 
                VALUES (@MedicationName, 'Added from medical record');
                
                SET @MedicationId = SCOPE_IDENTITY();
                PRINT '  - Created new medication: ' + @MedicationName;
            END
            
            -- Add prescription medication link
            INSERT INTO PrescriptionMedications (
                PrescriptionId,
                MedicationId,
                MedicationName,
                MedicalRecordId,
                Dosage,
                Unit,
                Frequency,
                Duration,
                Instructions
            )
            VALUES (
                @PrescriptionId,
                @MedicationId,
                @MedicationName,
                @MedicalRecordId,
                'As prescribed',
                'mg',
                'Once daily',
                'As directed',
                'Take as directed by physician'
            );
            
            PRINT '  - Added medication: ' + @MedicationName;
        END
        
        SET @Position = @NextComma + 1;
    END
    
    FETCH NEXT FROM prescription_cursor INTO @PrescriptionId, @MedicalRecordId, @MedicationsText;
END

CLOSE prescription_cursor;
DEALLOCATE prescription_cursor;

-- Clean up
DROP TABLE #PrescriptionsToUpdate;

PRINT 'Medication sync complete.'; 