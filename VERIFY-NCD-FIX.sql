-- SQL Queries to Verify NCD Risk Assessment Form Fix
-- Run these queries to test and verify the form submission is working

-- 1. Check current NCD Risk Assessment records
SELECT COUNT(*) as TotalRecords FROM NCDRiskAssessments;

-- 2. View recent NCD Risk Assessment submissions
SELECT TOP 5 
    Id, 
    UserId, 
    AppointmentId, 
    FirstName, 
    LastName, 
    Birthday, 
    Kasarian, 
    HasDiabetes, 
    HasHypertension, 
    CreatedAt 
FROM NCDRiskAssessments 
ORDER BY CreatedAt DESC;

-- 3. Check if specific appointment has NCD assessment
SELECT 
    a.Id as AppointmentId,
    a.UserId,
    n.Id as NCDAssessmentId,
    n.FirstName,
    n.LastName,
    n.CreatedAt as AssessmentDate
FROM Appointments a
LEFT JOIN NCDRiskAssessments n ON a.Id = n.AppointmentId
WHERE a.Id = 1; -- Replace with actual appointment ID

-- 4. Verify foreign key relationships
SELECT 
    n.Id,
    n.UserId,
    n.AppointmentId,
    u.UserName,
    a.AppointmentDate
FROM NCDRiskAssessments n
LEFT JOIN AspNetUsers u ON n.UserId = u.Id
LEFT JOIN Appointments a ON n.AppointmentId = a.Id
ORDER BY n.CreatedAt DESC;

-- 5. Check for any validation issues with required fields
SELECT 
    Id,
    UserId,
    AppointmentId,
    FirstName,
    LastName,
    Birthday,
    Kasarian,
    CASE 
        WHEN UserId IS NULL THEN 'Missing UserId'
        WHEN AppointmentId IS NULL THEN 'Missing AppointmentId'
        WHEN FirstName IS NULL OR FirstName = '' THEN 'Missing FirstName'
        WHEN LastName IS NULL OR LastName = '' THEN 'Missing LastName'
        ELSE 'OK'
    END as ValidationStatus
FROM NCDRiskAssessments
ORDER BY CreatedAt DESC;

-- 6. Test data cleanup (if needed)
-- DELETE FROM NCDRiskAssessments WHERE FirstName = 'Test' AND LastName = 'User';
