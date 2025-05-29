-- Check the structure of the StaffMembers table
SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'StaffMembers';

-- Create a stored procedure for the nurse dashboard data
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetNurseDashboardData')
    DROP PROCEDURE GetNurseDashboardData
GO

CREATE PROCEDURE GetNurseDashboardData
AS
BEGIN
    -- Get today's appointment counts
    SELECT 
        COUNT(*) AS TotalToday,
        SUM(CASE WHEN Status = 'InProgress' THEN 1 ELSE 0 END) AS InProgress,
        SUM(CASE WHEN Status = 'Pending' THEN 1 ELSE 0 END) AS Waiting,
        SUM(CASE WHEN Status = 'Completed' THEN 1 ELSE 0 END) AS Completed
    FROM dbo.Appointments
    WHERE CAST(AppointmentDate AS DATE) = CAST(GETDATE() AS DATE);
    
    -- Get patient queue
    SELECT TOP 10 a.Id, a.PatientId, a.DoctorId, a.AppointmentDate, a.AppointmentTime, 
           a.Status, p.Name AS PatientName, s.Name AS DoctorName
    FROM dbo.Appointments a
    LEFT JOIN dbo.Patients p ON a.PatientId = p.Id
    LEFT JOIN dbo.StaffMembers s ON a.DoctorId = s.UserId
    WHERE CAST(a.AppointmentDate AS DATE) = CAST(GETDATE() AS DATE)
    ORDER BY a.AppointmentTime;
    
    -- Get patient records
    SELECT TOP 10 p.Id, p.Name AS PatientName, p.Age, p.Gender, 
           MAX(a.AppointmentDate) AS LastVisit
    FROM dbo.Patients p
    LEFT JOIN dbo.Appointments a ON p.Id = a.PatientId
    GROUP BY p.Id, p.Name, p.Age, p.Gender
    ORDER BY LastVisit DESC;
    
    -- Get current appointment
    SELECT TOP 1 a.Id
    FROM dbo.Appointments a
    WHERE CAST(a.AppointmentDate AS DATE) = CAST(GETDATE() AS DATE)
    AND a.Status = 'InProgress'
    ORDER BY a.AppointmentTime;
END
GO 