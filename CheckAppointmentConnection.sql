-- Print Appointments table schema
PRINT '=== Appointments Table Schema ===';
SELECT 
    c.name AS 'ColumnName',
    t.name AS 'DataType',
    c.max_length AS 'MaxLength',
    c.is_nullable AS 'IsNullable'
FROM 
    sys.columns c
JOIN 
    sys.types t ON c.user_type_id = t.user_type_id
WHERE 
    c.object_id = OBJECT_ID('Appointments')
ORDER BY 
    c.column_id;

-- Check if Appointments table has any records
PRINT '=== Appointments Count ===';
SELECT COUNT(*) AS 'TotalAppointments' FROM Appointments;

-- Check User connections to Appointments
PRINT '=== User Appointment Connections ===';
SELECT 
    u.UserName,
    u.Email,
    COUNT(a.Id) AS 'AppointmentCount'
FROM 
    AspNetUsers u
LEFT JOIN 
    Appointments a ON u.Id = a.PatientId
GROUP BY 
    u.UserName, u.Email
ORDER BY 
    COUNT(a.Id) DESC;

-- Get sample of recent appointments
PRINT '=== Recent Appointments Sample ===';
SELECT TOP 10
    a.Id,
    a.PatientName,
    a.Gender,
    a.AppointmentDate,
    a.AppointmentTime,
    a.Status,
    a.ReasonForVisit,
    u.UserName AS 'PatientUserName',
    d.UserName AS 'DoctorUserName'
FROM 
    Appointments a
LEFT JOIN 
    AspNetUsers u ON a.PatientId = u.Id
LEFT JOIN 
    AspNetUsers d ON a.DoctorId = d.Id
ORDER BY 
    a.CreatedAt DESC;

-- Check if there are appointments with missing user connections
PRINT '=== Orphaned Appointments ===';
SELECT COUNT(*) AS 'OrphanedAppointments'
FROM Appointments a
LEFT JOIN AspNetUsers u ON a.PatientId = u.Id
WHERE u.Id IS NULL;

-- Check connection to related tables
PRINT '=== Related Tables Connections ===';
-- Check if NCDRiskAssessments references Appointments
IF OBJECT_ID('NCDRiskAssessments', 'U') IS NOT NULL
BEGIN
    SELECT 
        'NCDRiskAssessments' AS 'Table',
        COUNT(*) AS 'RecordsWithAppointments'
    FROM 
        NCDRiskAssessments
    WHERE 
        AppointmentId IN (SELECT Id FROM Appointments);
END

-- Check if HEEADSSSAssessments references Appointments
IF OBJECT_ID('HEEADSSSAssessments', 'U') IS NOT NULL
BEGIN
    SELECT 
        'HEEADSSSAssessments' AS 'Table',
        COUNT(*) AS 'RecordsWithAppointments'
    FROM 
        HEEADSSSAssessments
    WHERE 
        AppointmentId IN (SELECT Id FROM Appointments);
END 