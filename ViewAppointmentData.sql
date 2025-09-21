-- View Appointments Table Structure
EXEC sp_columns 'Appointments';

-- View NCDRiskAssessments Table Structure
EXEC sp_columns 'NCDRiskAssessments';

-- View all data in Appointments table
SELECT * FROM Appointments;

-- View all data in NCDRiskAssessments table
SELECT * FROM NCDRiskAssessments;

-- Check appointment data for the test user
SELECT * FROM Appointments 
WHERE PatientName LIKE '%Renier Ben Perez Almario%'; 