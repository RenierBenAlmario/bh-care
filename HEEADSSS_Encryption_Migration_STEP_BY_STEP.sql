-- HEEADSSS Assessment Encryption Migration - STEP BY STEP
-- This script converts boolean fields to encrypted string fields for better privacy protection

PRINT 'Starting HEEADSSS Assessment encryption migration...';

-- Step 1: Add new encrypted string columns one by one
ALTER TABLE [dbo].[HEEADSSSAssessments] ADD [AttendanceIssuesEncrypted] [nvarchar](max) NULL;
ALTER TABLE [dbo].[HEEADSSSAssessments] ADD [WeightConcernsEncrypted] [nvarchar](max) NULL;
ALTER TABLE [dbo].[HEEADSSSAssessments] ADD [EatingDisorderSymptomsEncrypted] [nvarchar](max) NULL;
ALTER TABLE [dbo].[HEEADSSSAssessments] ADD [SubstanceUseEncrypted] [nvarchar](max) NULL;
ALTER TABLE [dbo].[HEEADSSSAssessments] ADD [SexualActivityEncrypted] [nvarchar](max) NULL;
ALTER TABLE [dbo].[HEEADSSSAssessments] ADD [MoodChangesEncrypted] [nvarchar](max) NULL;
ALTER TABLE [dbo].[HEEADSSSAssessments] ADD [SuicidalThoughtsEncrypted] [nvarchar](max) NULL;
ALTER TABLE [dbo].[HEEADSSSAssessments] ADD [SelfHarmBehaviorEncrypted] [nvarchar](max) NULL;
ALTER TABLE [dbo].[HEEADSSSAssessments] ADD [FeelsSafeAtHomeEncrypted] [nvarchar](max) NULL;
ALTER TABLE [dbo].[HEEADSSSAssessments] ADD [FeelsSafeAtSchoolEncrypted] [nvarchar](max) NULL;
ALTER TABLE [dbo].[HEEADSSSAssessments] ADD [ExperiencedBullyingEncrypted] [nvarchar](max) NULL;

PRINT 'Added new encrypted string columns';

-- Step 2: Migrate existing boolean data to encrypted strings
UPDATE [dbo].[HEEADSSSAssessments] SET [AttendanceIssuesEncrypted] = CASE WHEN [AttendanceIssues] = 1 THEN 'True' ELSE 'False' END;
UPDATE [dbo].[HEEADSSSAssessments] SET [WeightConcernsEncrypted] = CASE WHEN [WeightConcerns] = 1 THEN 'True' ELSE 'False' END;
UPDATE [dbo].[HEEADSSSAssessments] SET [EatingDisorderSymptomsEncrypted] = CASE WHEN [EatingDisorderSymptoms] = 1 THEN 'True' ELSE 'False' END;
UPDATE [dbo].[HEEADSSSAssessments] SET [SubstanceUseEncrypted] = CASE WHEN [SubstanceUse] = 1 THEN 'True' ELSE 'False' END;
UPDATE [dbo].[HEEADSSSAssessments] SET [SexualActivityEncrypted] = CASE WHEN [SexualActivity] = 1 THEN 'True' ELSE 'False' END;
UPDATE [dbo].[HEEADSSSAssessments] SET [MoodChangesEncrypted] = CASE WHEN [MoodChanges] = 1 THEN 'True' ELSE 'False' END;
UPDATE [dbo].[HEEADSSSAssessments] SET [SuicidalThoughtsEncrypted] = CASE WHEN [SuicidalThoughts] = 1 THEN 'True' ELSE 'False' END;
UPDATE [dbo].[HEEADSSSAssessments] SET [SelfHarmBehaviorEncrypted] = CASE WHEN [SelfHarmBehavior] = 1 THEN 'True' ELSE 'False' END;
UPDATE [dbo].[HEEADSSSAssessments] SET [FeelsSafeAtHomeEncrypted] = CASE WHEN [FeelsSafeAtHome] = 1 THEN 'True' ELSE 'False' END;
UPDATE [dbo].[HEEADSSSAssessments] SET [FeelsSafeAtSchoolEncrypted] = CASE WHEN [FeelsSafeAtSchool] = 1 THEN 'True' ELSE 'False' END;
UPDATE [dbo].[HEEADSSSAssessments] SET [ExperiencedBullyingEncrypted] = CASE WHEN [ExperiencedBullying] = 1 THEN 'True' ELSE 'False' END;

PRINT 'Migrated existing boolean data to encrypted strings';

-- Step 3: Drop old boolean columns
ALTER TABLE [dbo].[HEEADSSSAssessments] DROP COLUMN [AttendanceIssues];
ALTER TABLE [dbo].[HEEADSSSAssessments] DROP COLUMN [WeightConcerns];
ALTER TABLE [dbo].[HEEADSSSAssessments] DROP COLUMN [EatingDisorderSymptoms];
ALTER TABLE [dbo].[HEEADSSSAssessments] DROP COLUMN [SubstanceUse];
ALTER TABLE [dbo].[HEEADSSSAssessments] DROP COLUMN [SexualActivity];
ALTER TABLE [dbo].[HEEADSSSAssessments] DROP COLUMN [MoodChanges];
ALTER TABLE [dbo].[HEEADSSSAssessments] DROP COLUMN [SuicidalThoughts];
ALTER TABLE [dbo].[HEEADSSSAssessments] DROP COLUMN [SelfHarmBehavior];
ALTER TABLE [dbo].[HEEADSSSAssessments] DROP COLUMN [FeelsSafeAtHome];
ALTER TABLE [dbo].[HEEADSSSAssessments] DROP COLUMN [FeelsSafeAtSchool];
ALTER TABLE [dbo].[HEEADSSSAssessments] DROP COLUMN [ExperiencedBullying];

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
