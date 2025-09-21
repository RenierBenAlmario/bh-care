-- Insert test appointment for July 15, 2025
-- Run this script to ensure there's test data available

-- First get the doctor information to ensure appointment links to a valid doctor
DECLARE @DoctorId nvarchar(450);
DECLARE @DoctorName nvarchar(256);

-- Find a doctor user (try multiple methods)
SELECT TOP 1 @DoctorId = Id, @DoctorName = FirstName + ' ' + LastName 
FROM AspNetUsers
WHERE Id IN (
    SELECT UserId FROM AspNetUserRoles WHERE RoleId IN (
        SELECT Id FROM AspNetRoles WHERE Name = 'Doctor'
    )
)
ORDER BY FirstName;

-- If no doctor found, try another method
IF @DoctorId IS NULL
BEGIN
    SELECT TOP 1 @DoctorId = Id, @DoctorName = FirstName + ' ' + LastName 
    FROM AspNetUsers
    WHERE Email LIKE '%doctor%' OR UserName LIKE '%doctor%'
    ORDER BY Id;
END

PRINT 'Found doctor ID: ' + ISNULL(@DoctorId, 'NULL');
PRINT 'Found doctor name: ' + ISNULL(@DoctorName, 'NULL');

-- First check if appointment already exists
IF NOT EXISTS (
    SELECT 1 FROM [Barangay].[dbo].[Appointments] 
    WHERE CONVERT(date, AppointmentDate) = '2025-07-15' 
    AND AppointmentTime = '13:00:00'
)
BEGIN
    -- Insert a test appointment
    INSERT INTO [Barangay].[dbo].[Appointments] (
        Id,
        PatientId,
        DoctorId,
        PatientName,
        Doctor,
        AppointmentDate,
        AppointmentTime,
        Status,
        Description,
        Type,
        CreatedAt,
        UpdatedAt
    )
    VALUES (
        NEXT VALUE FOR [Barangay].[dbo].[AppointmentSequence],
        '1', -- Use an existing PatientId or create one
        @DoctorId, -- Use the found doctor ID
        'Test Patient',
        @DoctorName, -- Use the found doctor name
        '2025-07-15', -- July 15, 2025
        '13:00:00', -- 1:00 PM
        'Pending',
        'Test appointment for doctor dashboard',
        'Checkup',
        GETDATE(),
        GETDATE()
    );
    
    PRINT 'Test appointment inserted for July 15, 2025';
    
    -- Get the inserted appointment ID
    DECLARE @AppointmentId INT;
    SELECT @AppointmentId = Id FROM [Barangay].[dbo].[Appointments]
    WHERE CONVERT(date, AppointmentDate) = '2025-07-15' 
    AND AppointmentTime = '13:00:00';
    
    PRINT 'Appointment ID: ' + CAST(@AppointmentId AS VARCHAR(10));
    
    -- Insert test vital signs if they don't exist
    IF NOT EXISTS (SELECT 1 FROM [Barangay].[dbo].[VitalSigns] WHERE AppointmentId = @AppointmentId)
    BEGIN
        INSERT INTO [Barangay].[dbo].[VitalSigns] (
            AppointmentId,
            PatientId,
            Temperature,
            BloodPressure,
            HeartRate,
            RespiratoryRate,
            SpO2,
            Weight,
            Height,
            RecordedAt,
            RecordedBy,
            Notes
        )
        VALUES (
            @AppointmentId,
            '1', -- Use the same PatientId as in the appointment
            38.5, -- Abnormal temperature for testing
            '120/80',
            82,
            18,
            96,
            70.5,
            170,
            GETDATE(),
            'Test Nurse',
            'Test vital signs for doctor dashboard'
        );
        
        PRINT 'Test vital signs inserted';
    END
    
    -- Insert test HEADSSS assessment if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM [Barangay].[dbo].[HEEADSSSAssessments] WHERE AppointmentId = @AppointmentId)
    BEGIN
        INSERT INTO [Barangay].[dbo].[HEEADSSSAssessments] (
            AppointmentId,
            PatientId,
            FullName,
            Age,
            Gender,
            HomeFamilyProblems,
            SuicidalThoughts,
            IsConsulted,
            RecordedAt,
            RecordedBy
        )
        VALUES (
            @AppointmentId,
            '1', -- Use the same PatientId as in the appointment
            'Test Patient',
            30,
            'Male',
            'Family issues for testing',
            'Yes', -- Abnormal for testing
            0,
            GETDATE(),
            'Test Nurse'
        );
        
        PRINT 'Test HEADSSS assessment inserted';
    END
    
    -- Insert test NCD risk assessment if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM [Barangay].[dbo].[NCDRiskAssessments] WHERE AppointmentId = @AppointmentId)
    BEGIN
        INSERT INTO [Barangay].[dbo].[NCDRiskAssessments] (
            AppointmentId,
            PatientId,
            HasDiabetes,
            ChestPain,
            RecordedAt,
            RecordedBy
        )
        VALUES (
            @AppointmentId,
            '1', -- Use the same PatientId as in the appointment
            1, -- True for testing
            1, -- True for testing
            GETDATE(),
            'Test Nurse'
        );
        
        PRINT 'Test NCD risk assessment inserted';
    END
END
ELSE
BEGIN
    PRINT 'Test appointment for July 15, 2025 already exists';
    
    -- Update the existing appointment to ensure it has correct doctor information
    UPDATE [Barangay].[dbo].[Appointments]
    SET DoctorId = @DoctorId, 
        Doctor = @DoctorName
    WHERE CONVERT(date, AppointmentDate) = '2025-07-15' 
    AND AppointmentTime = '13:00:00';
    
    PRINT 'Updated doctor information in existing appointment';
END

-- Print debug information
SELECT 'All appointments for July 15, 2025:' as [Debug Info];
SELECT Id, PatientName, DoctorId, Doctor, AppointmentTime, Status, Type
FROM [Barangay].[dbo].[Appointments]
WHERE CONVERT(date, AppointmentDate) = '2025-07-15'; 