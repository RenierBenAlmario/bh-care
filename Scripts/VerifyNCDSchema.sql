-- Verify NCDRiskAssessment schema matches the model
-- This script checks if all columns defined in the NCDRiskAssessment model exist in the database

USE [Barangay]
GO

PRINT 'Verifying NCDRiskAssessment schema...'
PRINT '====================================='

-- List of expected columns from the NCDRiskAssessment model
DECLARE @ExpectedColumns TABLE (
    ColumnName NVARCHAR(128),
    DataType NVARCHAR(128),
    IsNullable BIT
)

INSERT INTO @ExpectedColumns VALUES
('Id', 'int', 0),
('UserId', 'nvarchar', 1),
('AppointmentId', 'int', 1),
('HealthFacility', 'nvarchar', 1),
('FamilyNo', 'nvarchar', 1),
('Pangalan', 'nvarchar', 1),
('Address', 'nvarchar', 1),
('Barangay', 'nvarchar', 1),
('Birthday', 'datetime', 1),
('Telepono', 'nvarchar', 1),
('Edad', 'int', 1),
('Kasarian', 'nvarchar', 1),
('Relihiyon', 'nvarchar', 1),
('HasDiabetes', 'bit', 0),
('HasHypertension', 'bit', 0),
('HasCancer', 'bit', 0),
('HasCOPD', 'bit', 0),
('HasLungDisease', 'bit', 0),
('HasEyeDisease', 'bit', 0),
('CancerType', 'nvarchar', 1),
('FamilyHasHypertension', 'bit', 0),
('FamilyHasHeartDisease', 'bit', 0),
('FamilyHasStroke', 'bit', 0),
('FamilyHasDiabetes', 'bit', 0),
('FamilyHasCancer', 'bit', 0),
('FamilyHasKidneyDisease', 'bit', 0),
('FamilyHasOtherDisease', 'bit', 0),
('FamilyOtherDiseaseDetails', 'nvarchar', 1),
('SmokingStatus', 'nvarchar', 1),
('HighSaltIntake', 'bit', 0),
('EatsProcessedFood', 'bit', 0),
('AlcoholFrequency', 'nvarchar', 1),
('AlcoholConsumption', 'nvarchar', 1),
('ExerciseDuration', 'nvarchar', 1),
('RiskStatus', 'nvarchar', 1),
('ChestPain', 'nvarchar', 1),
('ChestPainLocation', 'nvarchar', 1),
('ChestPainValue', 'int', 1),
('HasDifficultyBreathing', 'bit', 0),
('HasAsthma', 'bit', 0),
('HasNoRegularExercise', 'bit', 0),
('CreatedAt', 'datetime', 0),
('UpdatedAt', 'datetime', 0),
('AppointmentType', 'nvarchar', 1)

-- Check each expected column
DECLARE @ColumnName NVARCHAR(128)
DECLARE @DataType NVARCHAR(128)
DECLARE @IsNullable BIT
DECLARE @MissingColumns NVARCHAR(MAX) = ''

DECLARE column_cursor CURSOR FOR
SELECT ColumnName, DataType, IsNullable FROM @ExpectedColumns

OPEN column_cursor
FETCH NEXT FROM column_cursor INTO @ColumnName, @DataType, @IsNullable

WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'NCDRiskAssessments' 
        AND COLUMN_NAME = @ColumnName
    )
    BEGIN
        SET @MissingColumns = @MissingColumns + @ColumnName + ', '
        PRINT 'MISSING: ' + @ColumnName
    END
    ELSE
    BEGIN
        PRINT 'FOUND: ' + @ColumnName
    END
    
    FETCH NEXT FROM column_cursor INTO @ColumnName, @DataType, @IsNullable
END

CLOSE column_cursor
DEALLOCATE column_cursor

IF LEN(@MissingColumns) > 0
BEGIN
    PRINT '====================================='
    PRINT 'MISSING COLUMNS: ' + LEFT(@MissingColumns, LEN(@MissingColumns) - 1)
    PRINT 'Schema verification FAILED!'
END
ELSE
BEGIN
    PRINT '====================================='
    PRINT 'All columns found! Schema verification PASSED!'
END

GO 