-- Update ImmunizationShortcutForms table column sizes to accommodate encrypted data
-- This script increases the column sizes to handle encrypted data which is much larger than plain text

USE Barangay;
GO

-- Update column sizes for encrypted fields
ALTER TABLE [dbo].[ImmunizationShortcutForms]
ALTER COLUMN [ChildName] NVARCHAR(4000) NOT NULL;

ALTER TABLE [dbo].[ImmunizationShortcutForms]
ALTER COLUMN [MotherName] NVARCHAR(4000) NOT NULL;

ALTER TABLE [dbo].[ImmunizationShortcutForms]
ALTER COLUMN [FatherName] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationShortcutForms]
ALTER COLUMN [Address] NVARCHAR(4000) NOT NULL;

ALTER TABLE [dbo].[ImmunizationShortcutForms]
ALTER COLUMN [Barangay] NVARCHAR(4000) NOT NULL;

ALTER TABLE [dbo].[ImmunizationShortcutForms]
ALTER COLUMN [Email] NVARCHAR(4000) NOT NULL;

ALTER TABLE [dbo].[ImmunizationShortcutForms]
ALTER COLUMN [ContactNumber] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationShortcutForms]
ALTER COLUMN [PreferredDate] NVARCHAR(4000) NOT NULL;

ALTER TABLE [dbo].[ImmunizationShortcutForms]
ALTER COLUMN [PreferredTime] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationShortcutForms]
ALTER COLUMN [Notes] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationShortcutForms]
ALTER COLUMN [CreatedAt] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationShortcutForms]
ALTER COLUMN [UpdatedAt] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationShortcutForms]
ALTER COLUMN [CreatedBy] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationShortcutForms]
ALTER COLUMN [Status] NVARCHAR(4000);

PRINT 'Successfully updated ImmunizationShortcutForms table column sizes for encrypted data.';
GO
