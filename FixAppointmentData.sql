-- Fix database constraints for Appointments table
IF EXISTS (SELECT 1 FROM sys.columns 
          WHERE Name = 'DoctorId'
          AND Object_ID = Object_ID('Appointments')
          AND is_nullable = 0)
BEGIN
    ALTER TABLE Appointments ALTER COLUMN DoctorId NVARCHAR(450) NULL;
    PRINT 'Modified DoctorId column to allow NULL values';
END

-- Fix Gender column size in Appointments table
IF EXISTS (SELECT 1 FROM sys.columns 
          WHERE Name = 'Gender'
          AND Object_ID = Object_ID('Appointments')
          AND max_length < 20)
BEGIN
    ALTER TABLE Appointments ALTER COLUMN Gender NVARCHAR(50) NULL;
    PRINT 'Modified Gender column to allow longer values';
END

-- Also check and fix NCDRiskAssessments table to ensure nullable fields
IF OBJECT_ID('NCDRiskAssessments', 'U') IS NOT NULL
BEGIN
    -- Make sure columns that could be empty are nullable
    IF EXISTS (SELECT 1 FROM sys.columns 
              WHERE Name = 'CancerType'
              AND Object_ID = Object_ID('NCDRiskAssessments')
              AND is_nullable = 0)
    BEGIN
        ALTER TABLE NCDRiskAssessments ALTER COLUMN CancerType NVARCHAR(255) NULL;
        PRINT 'Modified CancerType column to allow NULL values';
    END
    
    IF EXISTS (SELECT 1 FROM sys.columns 
              WHERE Name = 'FamilyOtherDiseaseDetails'
              AND Object_ID = Object_ID('NCDRiskAssessments')
              AND is_nullable = 0)
    BEGIN
        ALTER TABLE NCDRiskAssessments ALTER COLUMN FamilyOtherDiseaseDetails NVARCHAR(MAX) NULL;
        PRINT 'Modified FamilyOtherDiseaseDetails column to allow NULL values';
    END

    IF EXISTS (SELECT 1 FROM sys.columns 
              WHERE Name = 'Relihiyon'
              AND Object_ID = Object_ID('NCDRiskAssessments')
              AND is_nullable = 0)
    BEGIN
        ALTER TABLE NCDRiskAssessments ALTER COLUMN Relihiyon NVARCHAR(255) NULL;
        PRINT 'Modified Relihiyon column to allow NULL values';
    END
END

-- Check and fix HEEADSSSAssessments table
IF OBJECT_ID('HEEADSSSAssessments', 'U') IS NOT NULL
BEGIN
    -- Make common fields nullable that could cause errors
    DECLARE @HEEADSSSColumns TABLE (ColumnName NVARCHAR(255));
    INSERT INTO @HEEADSSSColumns VALUES 
        ('DrugsTobaccoUse'),
        ('DrugsAlcoholUse'),
        ('DrugsIllicitDrugUse'),
        ('SafetyPhysicalAbuse'),
        ('SafetyRelationshipViolence'),
        ('SafetyProtectiveGear'),
        ('SafetyGunsAtHome'),
        ('SuicideDepressionFeelings'),
        ('SuicideSelfHarmThoughts'),
        ('SuicideFamilyHistory'),
        ('Notes'),
        ('RecommendedActions');
    
    DECLARE @ColName NVARCHAR(255);
    DECLARE @SQL NVARCHAR(MAX);
    
    DECLARE HEEADSSSCursor CURSOR FOR
    SELECT ColumnName FROM @HEEADSSSColumns;
    
    OPEN HEEADSSSCursor;
    FETCH NEXT FROM HEEADSSSCursor INTO @ColName;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF EXISTS (SELECT 1 FROM sys.columns 
                  WHERE Name = @ColName
                  AND Object_ID = Object_ID('HEEADSSSAssessments')
                  AND is_nullable = 0)
        BEGIN
            SET @SQL = 'ALTER TABLE HEEADSSSAssessments ALTER COLUMN ' + @ColName + ' NVARCHAR(MAX) NULL';
            EXEC sp_executesql @SQL;
            PRINT 'Modified ' + @ColName + ' column to allow NULL values';
        END
        
        FETCH NEXT FROM HEEADSSSCursor INTO @ColName;
    END
    
    CLOSE HEEADSSSCursor;
    DEALLOCATE HEEADSSSCursor;
END

-- Check if we need to add a default doctor for existing appointments
IF EXISTS (SELECT 1 FROM Appointments WHERE DoctorId IS NULL)
BEGIN
    DECLARE @DefaultDoctorId NVARCHAR(450);
    
    -- Try to find a doctor from AspNetUsers or Staff table
    SELECT TOP 1 @DefaultDoctorId = Id 
    FROM AspNetUsers 
    WHERE UserType = 'Doctor' OR Specialization IS NOT NULL;
    
    -- If no doctor found, use the first user
    IF @DefaultDoctorId IS NULL
    BEGIN
        SELECT TOP 1 @DefaultDoctorId = Id FROM AspNetUsers;
    END
    
    -- Update existing appointments with NULL DoctorId
    IF @DefaultDoctorId IS NOT NULL
    BEGIN
        UPDATE Appointments 
        SET DoctorId = @DefaultDoctorId
        WHERE DoctorId IS NULL;
        
        PRINT 'Updated ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' appointments with default DoctorId: ' + @DefaultDoctorId;
    END
END

-- Print the database table structure for Appointments
SELECT 
    c.name AS 'ColumnName',
    t.name AS 'DataType',
    c.max_length AS 'MaxLength',
    c.is_nullable AS 'IsNullable'
FROM 
    sys.columns c
JOIN 
    sys.types t ON c.user_type_id = t.user_type_id
WHERE 
    c.object_id = OBJECT_ID('Appointments')
ORDER BY 
    c.column_id;

-- Print basic info about database tables
SELECT 
    t.name AS 'TableName',
    COUNT(c.name) AS 'ColumnCount'
FROM 
    sys.tables t
JOIN 
    sys.columns c ON t.object_id = c.object_id
GROUP BY 
    t.name
ORDER BY 
    t.name; 

-- Insert a test appointment record if none exist
IF NOT EXISTS (SELECT TOP 1 1 FROM Appointments)
BEGIN
    DECLARE @UserId NVARCHAR(450);
    DECLARE @DoctorId NVARCHAR(450);
    
    -- Get the first user from AspNetUsers
    SELECT TOP 1 @UserId = Id FROM AspNetUsers WHERE UserType != 1; -- Not a doctor
    
    -- Get the first doctor from AspNetUsers
    SELECT TOP 1 @DoctorId = Id FROM AspNetUsers WHERE UserType = 1; -- Doctor
    
    -- If no doctor found, use the same user
    IF @DoctorId IS NULL
        SET @DoctorId = @UserId;
    
    -- Insert a test appointment record
    IF @UserId IS NOT NULL
    BEGIN
        INSERT INTO Appointments (
            PatientId,
            PatientName,
            Gender,
            ContactNumber,
            AppointmentDate,
            AppointmentTime,
            DoctorId,
            ReasonForVisit,
            Description,
            Status,
            AgeValue,
            CreatedAt,
            UpdatedAt,
            AppointmentTimeInput
        )
        VALUES (
            @UserId,
            'Test Patient',
            'Male',
            '123-456-7890',
            DATEADD(DAY, 1, GETDATE()), -- Tomorrow
            '09:00:00', -- 9 AM
            @DoctorId,
            'General Checkup',
            'Test appointment created by FixAppointmentData.sql',
            0, -- Pending
            30, -- Age
            GETDATE(),
            GETDATE(),
            '09:00 AM'
        );
        
        PRINT 'Test appointment record created successfully with ID: ' + CAST(SCOPE_IDENTITY() AS NVARCHAR);
    END
    ELSE
    BEGIN
        PRINT 'No users found in the AspNetUsers table. Cannot create test appointment.';
    END
END
ELSE
BEGIN
    PRINT 'Appointments table already has records. No test appointment created.';
END 