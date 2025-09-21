-- Test script to verify consultation access
-- This script simulates the exact query that the consultation page uses

USE [Barangay]
GO

PRINT '=== CONSULTATION ACCESS TEST ==='
PRINT ''

-- Get today's date
DECLARE @Today DATE = GETDATE()
PRINT 'Today''s date: ' + CONVERT(VARCHAR, @Today, 120)
PRINT ''

-- Test the exact query from the consultation page
-- DoctorId: ea85984a-127e-4ab3-bbe0-e59bacada348 (from diagnostic)
-- Status: 0, 1, 2 (Pending, Confirmed, InProgress)
-- Date: Today

PRINT 'Testing consultation query for doctor: ea85984a-127e-4ab3-bbe0-e59bacada348'
PRINT '======================================================================'

SELECT 
    a.Id,
    a.DoctorId,
    a.PatientId,
    a.AppointmentDate,
    a.AppointmentTime,
    a.Status,
    CASE a.Status
        WHEN 0 THEN 'Pending'
        WHEN 1 THEN 'Confirmed'
        WHEN 2 THEN 'InProgress'
        ELSE 'Other'
    END as StatusName,
    p.FullName as PatientName
FROM Appointments a
LEFT JOIN Patients p ON a.PatientId = p.UserId
WHERE a.DoctorId = 'ea85984a-127e-4ab3-bbe0-e59bacada348' 
  AND CONVERT(date, a.AppointmentDate) = @Today
  AND a.Status IN (0, 1, 2)
ORDER BY a.AppointmentTime;

PRINT ''
PRINT 'Query completed successfully!'
PRINT ''

-- Test with different doctor ID to see if that's the issue
PRINT 'Testing with a different doctor ID (should return no results):'
PRINT '============================================================='
SELECT COUNT(*) as AppointmentCount
FROM Appointments a
WHERE a.DoctorId = 'different-doctor-id' 
  AND CONVERT(date, a.AppointmentDate) = @Today
  AND a.Status IN (0, 1, 2);

PRINT ''
PRINT '=== TEST COMPLETE ==='
GO 