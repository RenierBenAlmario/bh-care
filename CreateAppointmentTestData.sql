-- Insert test data into Appointments table
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
    'de8857a1-c84c-4674-9d73-9f4664cadabe',  -- Use a real PatientId from your database
    'Renier Ben Perez Almario', 
    'Male', 
    '09913333498', 
    '2025-04-06', 
    '08:00:00', 
    'Medical Consultation', 
    0,  -- 0 = Pending in AppointmentStatus enum
    'addssd', 
    22, 
    GETDATE(), 
    GETDATE()
); 