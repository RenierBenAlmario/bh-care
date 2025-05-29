-- Add missing columns to NCDRiskAssessments table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[NCDRiskAssessments]') AND name = 'AlcoholConsumption')
BEGIN
    ALTER TABLE [dbo].[NCDRiskAssessments] ADD [AlcoholConsumption] nvarchar(max) NULL DEFAULT ''
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[NCDRiskAssessments]') AND name = 'SmokingStatus')
BEGIN
    ALTER TABLE [dbo].[NCDRiskAssessments] ADD [SmokingStatus] nvarchar(max) NULL DEFAULT ''
END 