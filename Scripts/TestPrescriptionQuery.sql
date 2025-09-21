USE [Barangay]
GO

-- Test the exact query logic used by the prescriptions page
DECLARE @CurrentUserId NVARCHAR(450) = 'eee7f324-6daa-4b50-ad64-b847c6015acc';
DECLARE @CurrentDate DATETIME = GETDATE();

PRINT 'Testing prescription query logic...';
PRINT 'Current User ID: ' + @CurrentUserId;
PRINT 'Current Date: ' + CAST(@CurrentDate AS VARCHAR(50));

-- Test 1: Check if prescriptions exist for this user
SELECT 
    'Prescriptions for User' as Test,
    COUNT(*) as Count
FROM Prescriptions 
WHERE PatientId = @CurrentUserId;

-- Test 2: Check active prescriptions (not cancelled and not expired)
SELECT 
    'Active Prescriptions' as Test,
    COUNT(*) as Count
FROM Prescriptions 
WHERE PatientId = @CurrentUserId 
    AND Status != 5  -- Not Cancelled
    AND DATEADD(day, Duration, PrescriptionDate) > @CurrentDate;

-- Test 3: Check past prescriptions (expired or cancelled)
SELECT 
    'Past Prescriptions' as Test,
    COUNT(*) as Count
FROM Prescriptions 
WHERE PatientId = @CurrentUserId 
    AND (DATEADD(day, Duration, PrescriptionDate) <= @CurrentDate OR Status = 5);

-- Test 4: Show all prescriptions with details
SELECT 
    Id,
    PatientId,
    Status,
    PrescriptionDate,
    Duration,
    DATEADD(day, Duration, PrescriptionDate) as ValidUntil,
    CASE 
        WHEN DATEADD(day, Duration, PrescriptionDate) > @CurrentDate AND Status != 5 THEN 'ACTIVE'
        ELSE 'PAST'
    END as Status
FROM Prescriptions 
WHERE PatientId = @CurrentUserId
ORDER BY PrescriptionDate DESC;

PRINT 'Test completed.'; 