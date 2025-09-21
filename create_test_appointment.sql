-- Get the doctor's ID
DECLARE @DoctorId NVARCHAR(450);
SELECT @DoctorId = Id FROM AspNetUsers WHERE Email = 'kc123@email.com';

-- Get a patient ID (or use a specific one if you know it)
DECLARE @PatientId NVARCHAR(450);
SELECT TOP 1 @PatientId = Id FROM AspNetUsers WHERE Email != 'kc123@email.com';

-- Insert a test appointment for today
INSERT INTO Appointments (
    PatientId,
    DoctorId,
    PatientName,
    Gender,
    ContactNumber,
    AppointmentDate,
    AppointmentTime,
    Description,
    ReasonForVisit,
    Status,
    AgeValue,
    CreatedAt,
    UpdatedAt,
    Type,
    ApplicationUserId
)
VALUES (
    @PatientId,
    @DoctorId,
    'Test Patient',
    'Male',
    '123456789',
    CAST(GETDATE() AS DATE),  -- Today's date
    '14:30:00',               -- 2:30 PM
    'Test Appointment',
    'Regular Checkup',
    0,                        -- 0 is Pending in AppointmentStatus enum
    35,
    GETDATE(),
    GETDATE(),
    'Consultation',
    @PatientId
);

-- Insert an upcoming appointment for tomorrow
INSERT INTO Appointments (
    PatientId,
    DoctorId,
    PatientName,
    Gender,
    ContactNumber,
    AppointmentDate,
    AppointmentTime,
    Description,
    ReasonForVisit,
    Status,
    AgeValue,
    CreatedAt,
    UpdatedAt,
    Type,
    ApplicationUserId
)
VALUES (
    @PatientId,
    @DoctorId,
    'Another Test Patient',
    'Female',
    '987654321',
    DATEADD(DAY, 1, CAST(GETDATE() AS DATE)),  -- Tomorrow's date
    '10:00:00',               -- 10:00 AM
    'Follow-up Appointment',
    'Follow-up',
    0,                        -- 0 is Pending in AppointmentStatus enum
    42,
    GETDATE(),
    GETDATE(),
    'Follow-up',
    @PatientId
);

-- Select the appointments for this doctor to verify
SELECT * FROM Appointments WHERE DoctorId = @DoctorId ORDER BY AppointmentDate, AppointmentTime; 