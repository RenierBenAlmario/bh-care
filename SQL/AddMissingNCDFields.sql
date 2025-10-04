-- Add missing columns to NCDRiskAssessments table for complete form mapping
-- This migration adds all the missing form fields that were identified

-- Chest Pain Questions (Q2.1-2.8) - Missing mappings
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [Pananakit21] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [Pananakit22] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [Pananakit23] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [Pananakit24] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [Pananakit25] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [Pananakit26] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [Pananakit27] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [Pananakit28] NVARCHAR(4000) DEFAULT 'false';

-- Nutrition - Missing detailed mappings
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [NutrisyonMadalasGulay] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [NutrisyonMadalasPratas] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [NutrisyonMadalasIsda] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [NutrisyonMadalasKarne] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [NutrisyonKumakainMatatamis] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [NutrisyonKumakainMamantika] NVARCHAR(4000) DEFAULT 'false';

-- Alcohol - Missing detailed mappings
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [AlcoholInom] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [AlchoholTypeBeer] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [AlchoholTypeWine] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [AlchoholTypeWhisky] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [BeerConsumption1] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [BeerConsumption2] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [BeerConsumption3] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [WineConsumption1] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [WineConsumption2] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [WhiskyConsumption1] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [WhiskyConsumption2] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [AlcoholOkasyon] NVARCHAR(4000) DEFAULT 'false';

-- Exercise - Missing detailed mappings
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [EhersisyoRegular] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [EhersisyoDuration] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [EhersisyoType] NVARCHAR(4000) DEFAULT 'false';

-- Smoking - Missing detailed mappings
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [SigarilyoKadami] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [SigarilyoTumigil] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [SigarilyoUsok] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [SigarilyoSticks] NVARCHAR(4000) DEFAULT 'false';

-- Stress - Missing detailed mappings
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [StressMadalas] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [StressSino] NVARCHAR(4000) DEFAULT 'false';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [StressEpekto] NVARCHAR(4000) DEFAULT 'false';

-- Additional missing fields for complete form mapping
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [HealthFacilityName] NVARCHAR(4000) DEFAULT 'Baesa Health Center';
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [DateAssessment] NVARCHAR(4000) DEFAULT '';

-- Lung Disease - Missing proper mapping
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [HasLungDiseaseNonInfectious] NVARCHAR(4000) DEFAULT 'false';

-- Eye Disease - Missing proper mapping  
ALTER TABLE [dbo].[NCDRiskAssessments] ADD [HasEyeDiseaseCondition] NVARCHAR(4000) DEFAULT 'false';

-- Add comments for documentation
EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Chest Pain Questions Q2.1-2.8 from NCD Risk Assessment Form', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'NCDRiskAssessments', 
    @level2type = N'COLUMN', @level2name = N'Pananakit21';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Nutrition detailed mappings from NCD Risk Assessment Form', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'NCDRiskAssessments', 
    @level2type = N'COLUMN', @level2name = N'NutrisyonMadalasGulay';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Alcohol detailed mappings from NCD Risk Assessment Form', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'NCDRiskAssessments', 
    @level2type = N'COLUMN', @level2name = N'AlcoholInom';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Exercise detailed mappings from NCD Risk Assessment Form', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'NCDRiskAssessments', 
    @level2type = N'COLUMN', @level2name = N'EhersisyoRegular';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Smoking detailed mappings from NCD Risk Assessment Form', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'NCDRiskAssessments', 
    @level2type = N'COLUMN', @level2name = N'SigarilyoKadami';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Stress detailed mappings from NCD Risk Assessment Form', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'NCDRiskAssessments', 
    @level2type = N'COLUMN', @level2name = N'StressMadalas';

PRINT 'Successfully added all missing NCD Risk Assessment form fields to database';
