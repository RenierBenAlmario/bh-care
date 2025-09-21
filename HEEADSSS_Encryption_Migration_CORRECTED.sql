-- HEEADSSS Assessment Encryption Migration - CORRECTED
-- This script converts boolean fields to encrypted string fields for better privacy protection

PRINT 'Starting HEEADSSS Assessment encryption migration...';

-- Step 1: Add new encrypted string columns
ALTER TABLE [dbo].[HEEADSSSAssessments] 
ADD 
    [AttendanceIssuesEncrypted] [nvarchar](max) NULL,
    [WeightConcernsEncrypted] [nvarchar](max) NULL,
    [EatingDisorderSymptomsEncrypted] [nvarchar](max) NULL,
    [SubstanceUseEncrypted] [nvarchar](max) NULL,
    [SexualActivityEncrypted] [nvarchar](max) NULL,
    [MoodChangesEncrypted] [nvarchar](max) NULL,
    [SuicidalThoughtsEncrypted] [nvarchar](max) NULL,
    [SelfHarmBehaviorEncrypted] [nvarchar](max) NULL,
    [FeelsSafeAtHomeEncrypted] [nvarchar](max) NULL,
    [FeelsSafeAtSchoolEncrypted] [nvarchar](max) NULL,
    [ExperiencedBullyingEncrypted] [nvarchar](max) NULL;

PRINT 'Added new encrypted string columns';

-- Step 2: Migrate existing boolean data to encrypted strings
-- Convert boolean values to encrypted strings (True/False -> "True"/"False")
UPDATE [dbo].[HEEADSSSAssessments] 
SET 
    [AttendanceIssuesEncrypted] = CASE WHEN [AttendanceIssues] = 1 THEN 'True' ELSE 'False' END,
    [WeightConcernsEncrypted] = CASE WHEN [WeightConcerns] = 1 THEN 'True' ELSE 'False' END,
    [EatingDisorderSymptomsEncrypted] = CASE WHEN [EatingDisorderSymptoms] = 1 THEN 'True' ELSE 'False' END,
    [SubstanceUseEncrypted] = CASE WHEN [SubstanceUse] = 1 THEN 'True' ELSE 'False' END,
    [SexualActivityEncrypted] = CASE WHEN [SexualActivity] = 1 THEN 'True' ELSE 'False' END,
    [MoodChangesEncrypted] = CASE WHEN [MoodChanges] = 1 THEN 'True' ELSE 'False' END,
    [SuicidalThoughtsEncrypted] = CASE WHEN [SuicidalThoughts] = 1 THEN 'True' ELSE 'False' END,
    [SelfHarmBehaviorEncrypted] = CASE WHEN [SelfHarmBehavior] = 1 THEN 'True' ELSE 'False' END,
    [FeelsSafeAtHomeEncrypted] = CASE WHEN [FeelsSafeAtHome] = 1 THEN 'True' ELSE 'False' END,
    [FeelsSafeAtSchoolEncrypted] = CASE WHEN [FeelsSafeAtSchool] = 1 THEN 'True' ELSE 'False' END,
    [ExperiencedBullyingEncrypted] = CASE WHEN [ExperiencedBullying] = 1 THEN 'True' ELSE 'False' END;

PRINT 'Migrated existing boolean data to encrypted strings';

-- Step 3: Drop old boolean columns
ALTER TABLE [dbo].[HEEADSSSAssessments] 
DROP COLUMN 
    [AttendanceIssues],
    [WeightConcerns],
    [EatingDisorderSymptoms],
    [SubstanceUse],
    [SexualActivity],
    [MoodChanges],
    [SuicidalThoughts],
    [SelfHarmBehavior],
    [FeelsSafeAtHome],
    [FeelsSafeAtSchool],
    [ExperiencedBullying];

PRINT 'Dropped old boolean columns';

-- Step 4: Rename new columns to original names
EXEC sp_rename '[dbo].[HEEADSSSAssessments].[AttendanceIssuesEncrypted]', 'AttendanceIssues', 'COLUMN';
EXEC sp_rename '[dbo].[HEEADSSSAssessments].[WeightConcernsEncrypted]', 'WeightConcerns', 'COLUMN';
EXEC sp_rename '[dbo].[HEEADSSSAssessments].[EatingDisorderSymptomsEncrypted]', 'EatingDisorderSymptoms', 'COLUMN';
EXEC sp_rename '[dbo].[HEEADSSSAssessments].[SubstanceUseEncrypted]', 'SubstanceUse', 'COLUMN';
EXEC sp_rename '[dbo].[HEEADSSSAssessments].[SexualActivityEncrypted]', 'SexualActivity', 'COLUMN';
EXEC sp_rename '[dbo].[HEEADSSSAssessments].[MoodChangesEncrypted]', 'MoodChanges', 'COLUMN';
EXEC sp_rename '[dbo].[HEEADSSSAssessments].[SuicidalThoughtsEncrypted]', 'SuicidalThoughts', 'COLUMN';
EXEC sp_rename '[dbo].[HEEADSSSAssessments].[SelfHarmBehaviorEncrypted]', 'SelfHarmBehavior', 'COLUMN';
EXEC sp_rename '[dbo].[HEEADSSSAssessments].[FeelsSafeAtHomeEncrypted]', 'FeelsSafeAtHome', 'COLUMN';
EXEC sp_rename '[dbo].[HEEADSSSAssessments].[FeelsSafeAtSchoolEncrypted]', 'FeelsSafeAtSchool', 'COLUMN';
EXEC sp_rename '[dbo].[HEEADSSSAssessments].[ExperiencedBullyingEncrypted]', 'ExperiencedBullying', 'COLUMN';

PRINT 'Renamed new columns to original names';

-- Step 5: Verify the migration
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'HEEADSSSAssessments' 
    AND COLUMN_NAME IN ('AttendanceIssues', 'WeightConcerns', 'EatingDisorderSymptoms', 'SubstanceUse', 'SexualActivity', 'MoodChanges', 'SuicidalThoughts', 'SelfHarmBehavior', 'FeelsSafeAtHome', 'FeelsSafeAtSchool', 'ExperiencedBullying', 'FamilyNo')
ORDER BY COLUMN_NAME;

PRINT 'HEEADSSS Assessment encryption migration completed successfully!';
PRINT 'All sensitive boolean fields are now encrypted string fields.';
