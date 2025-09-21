-- Test script to verify the consultation flow
-- This script simulates the exact data loading process used by the consultation page

USE [Barangay]
GO

PRINT '=== CONSULTATION FLOW TEST ==='
PRINT ''

-- Test 1: Verify appointment exists and is accessible
PRINT '1. APPOINTMENT VERIFICATION:'
PRINT '============================'
DECLARE @AppointmentId INT = 2039
DECLARE @DoctorId NVARCHAR(450) = 'ea85984a-127e-4ab3-bbe0-e59bacada348'
DECLARE @PatientId NVARCHAR(450) = 'eee7f324-6daa-4b50-ad64-b847c6015acc'

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
        ELSE 'Unknown'
    END as StatusName
FROM Appointments 
WHERE Id = @AppointmentId;

PRINT ''

-- Test 2: Verify patient exists
PRINT '2. PATIENT VERIFICATION:'
PRINT '======================='
SELECT 
    UserId,
    FullName,
    Gender,
    BirthDate,
    ContactNumber
FROM Patients 
WHERE UserId = @PatientId;

PRINT ''

-- Test 3: Verify doctor exists and has correct role
PRINT '3. DOCTOR VERIFICATION:'
PRINT '======================'
SELECT 
    u.Id,
    u.Email,
    u.FullName,
    r.Name as Role
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Id = @DoctorId AND r.Name = 'Doctor';

PRINT ''

-- Test 4: Check for existing medical records (should be 0 for pending appointment)
PRINT '4. MEDICAL RECORDS CHECK:'
PRINT '========================'
SELECT 
    COUNT(*) as MedicalRecordCount
FROM MedicalRecords 
WHERE PatientId = @PatientId;

PRINT ''

-- Test 5: Check for vital signs (optional)
PRINT '5. VITAL SIGNS CHECK:'
PRINT '===================='
SELECT 
    COUNT(*) as VitalSignsCount
FROM VitalSigns 
WHERE PatientId = @PatientId;

PRINT ''

-- Test 6: Simulate the exact consultation query
PRINT '6. CONSULTATION QUERY SIMULATION:'
PRINT '================================'
DECLARE @Today DATE = GETDATE()

SELECT 
    'Appointment' as DataType,
    a.Id,
    a.DoctorId,
    a.PatientId,
    a.Status,
    a.AppointmentDate,
    a.AppointmentTime
FROM Appointments a
WHERE a.Id = @AppointmentId

UNION ALL

SELECT 
    'Patient' as DataType,
    NULL as Id,
    p.UserId as DoctorId,
    p.UserId as PatientId,
    NULL as Status,
    NULL as AppointmentDate,
    NULL as AppointmentTime
FROM Patients p
WHERE p.UserId = @PatientId;

PRINT ''
PRINT '=== TEST COMPLETE ==='
PRINT ''
PRINT 'Expected Results:'
PRINT '- Appointment should exist with Status = 0 (Pending)'
PRINT '- Patient should exist with name "Renier Perez Almario"'
PRINT '- Doctor should exist with role "Doctor"'
PRINT '- Medical records should be 0 (normal for pending appointment)'
PRINT '- Vital signs may be 0 or more (optional)'
GO 