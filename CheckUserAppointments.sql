-- This script checks if the User/Appointments page can properly fetch appointments
USE Barangay;

PRINT 'Checking User/Appointments page connectivity...';

-- 1. Check if the Appointments table has the required columns for the page
PRINT '=== Appointments Table Required Columns ===';
SELECT
    CASE WHEN EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'Id' AND Object_ID = Object_ID('Appointments')) THEN 'Present' ELSE 'Missing' END AS 'Id',
    CASE WHEN EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'PatientId' AND Object_ID = Object_ID('Appointments')) THEN 'Present' ELSE 'Missing' END AS 'PatientId',
    CASE WHEN EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'AppointmentDate' AND Object_ID = Object_ID('Appointments')) THEN 'Present' ELSE 'Missing' END AS 'AppointmentDate',
    CASE WHEN EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'AppointmentTime' AND Object_ID = Object_ID('Appointments')) THEN 'Present' ELSE 'Missing' END AS 'AppointmentTime',
    CASE WHEN EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'Status' AND Object_ID = Object_ID('Appointments')) THEN 'Present' ELSE 'Missing' END AS 'Status',
    CASE WHEN EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'ReasonForVisit' AND Object_ID = Object_ID('Appointments')) THEN 'Present' ELSE 'Missing' END AS 'ReasonForVisit';

-- 2. Check if we can join the Appointments table with AspNetUsers (required for User/Appointments page)
PRINT '=== Testing Join Between Users and Appointments ===';
SELECT TOP 5
    u.UserName,
    u.Email,
    a.Id AS AppointmentId,
    a.AppointmentDate,
    a.AppointmentTime,
    a.Status,
    a.ReasonForVisit
FROM 
    AspNetUsers u
LEFT JOIN 
    Appointments a ON u.Id = a.PatientId
ORDER BY
    u.UserName;

-- 3. Check if our test appointment can be retrieved properly
PRINT '=== Test Appointment Retrieval ===';
SELECT TOP 5
    a.Id,
    a.PatientName,
    a.Gender,
    a.AppointmentDate,
    a.AppointmentTime,
    a.Status,
    a.ReasonForVisit,
    u.UserName AS PatientUserName,
    d.UserName AS DoctorUserName
FROM 
    Appointments a
LEFT JOIN 
    AspNetUsers u ON a.PatientId = u.Id
LEFT JOIN 
    AspNetUsers d ON a.DoctorId = d.Id
ORDER BY 
    a.CreatedAt DESC;

-- 4. Test the query that the User/Appointments page would use
PRINT '=== Simulating User/Appointments Page Query ===';
-- Get a test user ID to use
DECLARE @TestUserId NVARCHAR(450);
SELECT TOP 1 @TestUserId = Id FROM AspNetUsers;

-- Now run the query that would fetch appointments for this user
SELECT 
    a.Id,
    a.PatientName,
    a.AppointmentDate,
    a.AppointmentTime,
    a.Status,
    a.ReasonForVisit,
    a.DoctorId,
    d.FirstName + ' ' + d.LastName AS DoctorName
FROM 
    Appointments a
LEFT JOIN 
    AspNetUsers d ON a.DoctorId = d.Id
WHERE 
    a.PatientId = @TestUserId
ORDER BY 
    a.AppointmentDate DESC, a.AppointmentTime; 