-- Diagnostic script for Consultation data loading issue
-- This script helps identify why the consultation page is failing to load data

USE [Barangay]
GO

PRINT '=== CONSULTATION DIAGNOSTIC REPORT ==='
PRINT ''

-- 1. Check today's appointments
PRINT '1. TODAY''S APPOINTMENTS:'
PRINT '========================'
SELECT 
    Id,
    DoctorId,
    PatientId,
    AppointmentDate,
    AppointmentTime,
    Status,
    CASE Status
        WHEN 0 THEN 'Pending'
        WHEN 1 THEN 'Confirmed'
        WHEN 2 THEN 'InProgress'
        WHEN 3 THEN 'Completed'
        WHEN 4 THEN 'Cancelled'
        WHEN 5 THEN 'Urgent'
        WHEN 6 THEN 'NoShow'
        ELSE 'Unknown'
    END as StatusName
FROM Appointments 
WHERE CONVERT(date, AppointmentDate) = CONVERT(date, GETDATE())
ORDER BY AppointmentTime;

PRINT ''

-- 2. Check appointments for the last 7 days
PRINT '2. APPOINTMENTS LAST 7 DAYS:'
PRINT '============================'
SELECT 
    Id,
    DoctorId,
    PatientId,
    AppointmentDate,
    AppointmentTime,
    Status,
    CASE Status
        WHEN 0 THEN 'Pending'
        WHEN 1 THEN 'Confirmed'
        WHEN 2 THEN 'InProgress'
        WHEN 3 THEN 'Completed'
        WHEN 4 THEN 'Cancelled'
        WHEN 5 THEN 'Urgent'
        WHEN 6 THEN 'NoShow'
        ELSE 'Unknown'
    END as StatusName
FROM Appointments 
WHERE AppointmentDate >= DATEADD(day, -7, GETDATE())
ORDER BY AppointmentDate DESC, AppointmentTime;

PRINT ''

-- 3. Check if there are any doctors in the system
PRINT '3. DOCTORS IN SYSTEM:'
PRINT '===================='
SELECT 
    u.Id as UserId,
    u.Email,
    u.UserName,
    u.FullName
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name = 'Doctor';

PRINT ''

-- 4. Check if there are any patients in the system
PRINT '4. PATIENTS IN SYSTEM:'
PRINT '====================='
SELECT TOP 10
    p.UserId,
    p.FullName,
    p.Gender,
    p.BirthDate
FROM Patients p;

PRINT ''

-- 5. Check appointment status distribution
PRINT '5. APPOINTMENT STATUS DISTRIBUTION:'
PRINT '=================================='
SELECT 
    Status,
    CASE Status
        WHEN 0 THEN 'Pending'
        WHEN 1 THEN 'Confirmed'
        WHEN 2 THEN 'InProgress'
        WHEN 3 THEN 'Completed'
        WHEN 4 THEN 'Cancelled'
        WHEN 5 THEN 'Urgent'
        WHEN 6 THEN 'NoShow'
        ELSE 'Unknown'
    END as StatusName,
    COUNT(*) as Count
FROM Appointments 
GROUP BY Status
ORDER BY Status;

PRINT ''

-- 6. Check for appointments with missing doctor or patient references
PRINT '6. APPOINTMENTS WITH MISSING REFERENCES:'
PRINT '========================================'
SELECT 
    'Missing Doctor' as Issue,
    Id,
    DoctorId,
    PatientId,
    AppointmentDate
FROM Appointments a
WHERE NOT EXISTS (SELECT 1 FROM AspNetUsers u WHERE u.Id = a.DoctorId)

UNION ALL

SELECT 
    'Missing Patient' as Issue,
    Id,
    DoctorId,
    PatientId,
    AppointmentDate
FROM Appointments a
WHERE NOT EXISTS (SELECT 1 FROM Patients p WHERE p.UserId = a.PatientId);

PRINT ''
PRINT '=== DIAGNOSTIC COMPLETE ==='
GO 