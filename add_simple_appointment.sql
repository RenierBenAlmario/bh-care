-- Get the doctor ID for kc123@email.com
DECLARE @DoctorId nvarchar(450);
SELECT @DoctorId = Id FROM AspNetUsers WHERE Email = 'kc123@email.com';

-- Insert a simple test appointment
INSERT INTO Appointments 
(
    PatientId,
    PatientName,
    DoctorId,
    AppointmentDate,
    AppointmentTime,
    AppointmentTimeInput,
    ReasonForVisit,
    Status,
    CreatedAt,
    UpdatedAt,
    Description,
    Type,
    AgeValue
)
VALUES 
(
    '35f1b87c-6870-4f50-8eb4-f367c0ca0fff', -- Use sample PatientId from your screenshot
    'Baggae Chunkie',                        -- PatientName from screenshot
    @DoctorId,
    GETDATE(),                               -- Today
    '10:00:00',                              -- 10 AM
    '10:00 AM',                              -- AppointmentTimeInput as string
    'Regular Checkup',
    0,                                       -- Pending status
    GETDATE(),
    GETDATE(),
    'Test appointment for doctor dashboard',
    'Consultation',
    35                                       -- Age value
);

-- Print the new appointment for verification
SELECT 'New appointment added with ID:' + CAST(SCOPE_IDENTITY() AS VARCHAR(10)) AS Result;

-- Show all appointments for this doctor
SELECT Id, PatientName, ReasonForVisit, Status, AppointmentDate, AppointmentTime 
FROM Appointments 
WHERE DoctorId = @DoctorId; 