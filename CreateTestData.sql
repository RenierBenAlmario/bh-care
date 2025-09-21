-- Create test appointments data
-- First make DoctorId column nullable
IF EXISTS (SELECT 1 FROM sys.columns 
          WHERE Name = 'DoctorId'
          AND Object_ID = Object_ID('Appointments')
          AND is_nullable = 0)
BEGIN
    ALTER TABLE Appointments ALTER COLUMN DoctorId NVARCHAR(450) NULL;
    PRINT 'Modified DoctorId column to allow NULL values';
END

IF NOT EXISTS (SELECT TOP 1 * FROM Appointments WHERE PatientName = 'Renier Ben Perez Almario')
BEGIN
    DECLARE @PatientId NVARCHAR(450);
    
    -- Select a valid user ID from the AspNetUsers table
    SELECT TOP 1 @PatientId = Id FROM AspNetUsers;
    
    -- Insert test appointment
    INSERT INTO Appointments (
        PatientId, 
        PatientName, 
        Gender, 
        ContactNumber, 
        AppointmentDate, 
        AppointmentTime, 
        ReasonForVisit, 
        Status, 
        Description, 
        AgeValue, 
        CreatedAt, 
        UpdatedAt
    )
    VALUES (
        @PatientId,  
        'Renier Ben Perez Almario', 
        'Male', 
        '09913333498', 
        DATEADD(day, 1, GETDATE()), -- Tomorrow's date
        '08:00:00', 
        'Medical Consultation', 
        0,  -- 0 = Pending in AppointmentStatus enum
        'Test appointment for development', 
        22, 
        GETDATE(), 
        GETDATE()
    );
    
    PRINT 'Test appointment created successfully.';
END
ELSE
BEGIN
    PRINT 'Test appointment already exists.';
END

-- Check the inserted data
SELECT * FROM Appointments WHERE PatientName = 'Renier Ben Perez Almario'; 