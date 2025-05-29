-- Drop existing procedures to avoid errors
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_CreatePrescription')
    DROP PROCEDURE sp_CreatePrescription;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_UpdatePrescription')
    DROP PROCEDURE sp_UpdatePrescription;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetPatientPrescriptions')
    DROP PROCEDURE sp_GetPatientPrescriptions;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetDoctorPrescriptions')
    DROP PROCEDURE sp_GetDoctorPrescriptions;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetPrescriptionDetails')
    DROP PROCEDURE sp_GetPrescriptionDetails;
GO

-- Insert a new prescription and its medications
CREATE PROCEDURE sp_CreatePrescription
    @PatientId INT,
    @DoctorId INT,
    @Status NVARCHAR(50),
    @Notes NVARCHAR(MAX),
    @MedicationsJSON NVARCHAR(MAX) -- JSON array with medication details
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Insert the prescription header
        DECLARE @PrescriptionId INT;
        DECLARE @Now DATETIME = GETDATE();
        
        INSERT INTO Prescriptions (
            PatientId, 
            DoctorId, 
            Status, 
            PrescriptionDate, 
            Notes, 
            CreatedAt, 
            UpdatedAt
        )
        VALUES (
            @PatientId, 
            @DoctorId, 
            @Status, 
            @Now, 
            @Notes, 
            @Now, 
            @Now
        );
        
        SET @PrescriptionId = SCOPE_IDENTITY();
        
        -- Insert medications from JSON
        INSERT INTO PrescriptionMedications (
            MedicationName,
            Dosage,
            Frequency,
            Duration,
            Instructions,
            PrescriptionId,
            MedicalRecordId,
            CreatedAt,
            UpdatedAt
        )
        SELECT
            JSON_VALUE(m.value, '$.medicationName'),
            JSON_VALUE(m.value, '$.dosage'),
            JSON_VALUE(m.value, '$.frequency'),
            JSON_VALUE(m.value, '$.duration'),
            JSON_VALUE(m.value, '$.instructions'),
            @PrescriptionId,
            CAST(JSON_VALUE(m.value, '$.medicalRecordId') AS INT),
            @Now,
            @Now
        FROM OPENJSON(@MedicationsJSON) AS m;
        
        COMMIT TRANSACTION;
        
        -- Return the created prescription ID
        SELECT @PrescriptionId AS PrescriptionId, 1 AS Success, 'Prescription created successfully' AS Message;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        
        -- Return error
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END;
GO

-- Update an existing prescription and its medications
CREATE PROCEDURE sp_UpdatePrescription
    @PrescriptionId INT,
    @Status NVARCHAR(50),
    @Notes NVARCHAR(MAX),
    @MedicationsJSON NVARCHAR(MAX) -- JSON array with medication details
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Update the prescription header
        DECLARE @Now DATETIME = GETDATE();
        
        UPDATE Prescriptions
        SET 
            Status = @Status,
            Notes = @Notes,
            UpdatedAt = @Now
        WHERE 
            Id = @PrescriptionId;
        
        -- Delete existing medications
        DELETE FROM PrescriptionMedications 
        WHERE PrescriptionId = @PrescriptionId;
        
        -- Insert updated medications from JSON
        INSERT INTO PrescriptionMedications (
            MedicationName,
            Dosage,
            Frequency,
            Duration,
            Instructions,
            PrescriptionId,
            MedicalRecordId,
            CreatedAt,
            UpdatedAt
        )
        SELECT
            JSON_VALUE(m.value, '$.medicationName'),
            JSON_VALUE(m.value, '$.dosage'),
            JSON_VALUE(m.value, '$.frequency'),
            JSON_VALUE(m.value, '$.duration'),
            JSON_VALUE(m.value, '$.instructions'),
            @PrescriptionId,
            CAST(JSON_VALUE(m.value, '$.medicalRecordId') AS INT),
            @Now,
            @Now
        FROM OPENJSON(@MedicationsJSON) AS m;
        
        COMMIT TRANSACTION;
        
        -- Return success
        SELECT 1 AS Success, 'Prescription updated successfully' AS Message;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        
        -- Return error
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END;
GO

-- Fetch prescriptions by patient
CREATE PROCEDURE sp_GetPatientPrescriptions
    @PatientId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get prescription headers
    SELECT 
        p.Id,
        p.PatientId,
        p.DoctorId,
        s.Name AS DoctorName, -- Assuming this joins to your StaffMembers table
        p.Status,
        p.PrescriptionDate,
        p.Notes,
        p.CreatedAt,
        p.UpdatedAt
    FROM 
        Prescriptions p
    LEFT JOIN 
        StaffMembers s ON p.DoctorId = s.Id
    WHERE 
        p.PatientId = @PatientId
    ORDER BY 
        p.PrescriptionDate DESC;
    
    -- Get medications for all prescriptions of this patient
    SELECT 
        pm.Id,
        pm.PrescriptionId,
        pm.MedicationName,
        pm.Dosage,
        pm.Frequency,
        pm.Duration,
        pm.Instructions,
        pm.MedicalRecordId
    FROM 
        PrescriptionMedications pm
    INNER JOIN 
        Prescriptions p ON pm.PrescriptionId = p.Id
    WHERE 
        p.PatientId = @PatientId
    ORDER BY 
        p.PrescriptionDate DESC, pm.Id;
END;
GO

-- Fetch prescriptions by doctor
CREATE PROCEDURE sp_GetDoctorPrescriptions
    @DoctorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get prescription headers
    SELECT 
        p.Id,
        p.PatientId,
        pt.Name AS PatientName, -- Assuming this joins to your Patients table
        p.DoctorId,
        p.Status,
        p.PrescriptionDate,
        p.Notes,
        p.CreatedAt,
        p.UpdatedAt
    FROM 
        Prescriptions p
    LEFT JOIN 
        Patients pt ON p.PatientId = pt.Id
    WHERE 
        p.DoctorId = @DoctorId
    ORDER BY 
        p.PrescriptionDate DESC;
    
    -- Get medications for all prescriptions by this doctor
    SELECT 
        pm.Id,
        pm.PrescriptionId,
        pm.MedicationName,
        pm.Dosage,
        pm.Frequency,
        pm.Duration,
        pm.Instructions,
        pm.MedicalRecordId
    FROM 
        PrescriptionMedications pm
    INNER JOIN 
        Prescriptions p ON pm.PrescriptionId = p.Id
    WHERE 
        p.DoctorId = @DoctorId
    ORDER BY 
        p.PrescriptionDate DESC, pm.Id;
END;
GO

-- Get a single prescription with its medications
CREATE PROCEDURE sp_GetPrescriptionDetails
    @PrescriptionId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get prescription header
    SELECT 
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
    LEFT JOIN 
        Patients pt ON p.PatientId = pt.Id
    LEFT JOIN 
        StaffMembers s ON p.DoctorId = s.Id
    WHERE 
        p.Id = @PrescriptionId;
    
    -- Get medications for this prescription
    SELECT 
        pm.Id,
        pm.MedicationName,
        pm.Dosage,
        pm.Frequency,
        pm.Duration,
        pm.Instructions,
        pm.MedicalRecordId,
        mr.DiagnosisCode,
        mr.DiagnosisDescription
    FROM 
        PrescriptionMedications pm
    LEFT JOIN 
        MedicalRecords mr ON pm.MedicalRecordId = mr.Id
    WHERE 
        pm.PrescriptionId = @PrescriptionId
    ORDER BY 
        pm.Id;
END;
GO

PRINT 'Prescription stored procedures created successfully.' 