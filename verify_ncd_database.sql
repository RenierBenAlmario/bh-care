-- Database verification script for NCD Risk Assessment
-- Run this in SQL Server Management Studio after submitting the form

-- Check if any NCD Risk Assessments exist
SELECT COUNT(*) as TotalNCDAssessments FROM [Barangay].[dbo].[NCDRiskAssessments];

-- Show the latest NCD Risk Assessments
SELECT TOP 5 
    Id,
    UserId,
    AppointmentId,
    HealthFacility,
    FamilyNo,
    Address,
    Barangay,
    Birthday,
    Telepono,
    Edad,
    Kasarian,
    Relihiyon,
    HasDiabetes,
    HasHypertension,
    HasCancer,
    SmokingStatus,
    RiskStatus,
    CreatedAt,
    UpdatedAt
FROM [Barangay].[dbo].[NCDRiskAssessments]
ORDER BY CreatedAt DESC;

-- Check if the specific appointment exists
SELECT Id, PatientId, AppointmentDate, Status 
FROM [Barangay].[dbo].[Appointments] 
WHERE Id = 1416;

-- Check for any recent NCD assessments with AppointmentId 1416
SELECT Id, UserId, AppointmentId, CreatedAt
FROM [Barangay].[dbo].[NCDRiskAssessments]
WHERE AppointmentId = 1416;
