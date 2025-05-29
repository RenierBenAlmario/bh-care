-- Create index for linking if not exists
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PrescriptionMedications_MedicalRecordId' AND object_id = OBJECT_ID('PrescriptionMedications'))
BEGIN
    CREATE INDEX IX_PrescriptionMedications_MedicalRecordId ON PrescriptionMedications(MedicalRecordId);
    PRINT 'Index created for MedicalRecordId in PrescriptionMedications table.';
END
GO

-- Drop existing procedure if it exists
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetMedicalRecordPrescriptions')
    DROP PROCEDURE sp_GetMedicalRecordPrescriptions;
GO

-- Get prescriptions by medical record
CREATE PROCEDURE sp_GetMedicalRecordPrescriptions
    @MedicalRecordId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get unique prescriptions that contain medications for this medical record
    SELECT DISTINCT
        p.Id,
        p.PatientId,
        pt.Name AS PatientName,
        p.DoctorId,
        s.Name AS DoctorName,
        p.Status,
        p.PrescriptionDate,
        p.Notes,
        p.CreatedAt,
        p.UpdatedAt
    FROM 
        Prescriptions p
    INNER JOIN 
        PrescriptionMedications pm ON p.Id = pm.PrescriptionId
    LEFT JOIN 
        Patients pt ON p.PatientId = pt.Id
    LEFT JOIN 
        StaffMembers s ON p.DoctorId = s.Id
    WHERE 
        pm.MedicalRecordId = @MedicalRecordId
    ORDER BY 
        p.PrescriptionDate DESC;
    
    -- Get medications associated with this medical record
    SELECT 
        pm.Id,
        pm.PrescriptionId,
        pm.MedicationName,
        pm.Dosage,
        pm.Frequency,
        pm.Duration,
        pm.Instructions
    FROM 
        PrescriptionMedications pm
    WHERE 
        pm.MedicalRecordId = @MedicalRecordId
    ORDER BY 
        pm.Id;
END;
GO

PRINT 'Prescription-to-Medical Record linking features created successfully.' 