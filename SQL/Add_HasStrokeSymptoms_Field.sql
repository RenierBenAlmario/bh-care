-- Add HasStrokeSymptoms field to NCDRiskAssessments table
-- This field stores the answer to Q2.8: "Nakakaramdam ka ba hirap sa pagsasalita, panghihina ng braso at/o binti o pamamanhid sa kalahating bahagi ng katawan?"

ALTER TABLE [dbo].[NCDRiskAssessments]
ADD [HasStrokeSymptoms] NVARCHAR(4000) NULL;

-- Add a comment to document the field
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Q2.8: Nakakaramdam ka ba hirap sa pagsasalita, panghihina ng braso at/o binti o pamamanhid sa kalahating bahagi ng katawan? (Do you feel difficulty speaking, weakness in the arm and/or leg, or numbness in half of the body?)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'NCDRiskAssessments', 
    @level2type = N'COLUMN', @level2name = N'HasStrokeSymptoms';

-- Set default value for existing records
UPDATE [dbo].[NCDRiskAssessments] 
SET [HasStrokeSymptoms] = 'Hindi' 
WHERE [HasStrokeSymptoms] IS NULL;
