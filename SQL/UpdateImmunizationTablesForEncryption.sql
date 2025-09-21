-- Update ImmunizationRecord and ImmunizationShortcutForm table column sizes to accommodate encrypted data
-- This script increases the column sizes to handle encrypted data which is much larger than plain text

USE Barangay;
GO

PRINT 'Starting database schema update for encrypted data...';

-- Update ImmunizationRecords table column sizes for encrypted fields
PRINT 'Updating ImmunizationRecords table...';

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [ChildName] NVARCHAR(4000) NOT NULL;

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [DateOfBirth] NVARCHAR(4000) NOT NULL;

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [PlaceOfBirth] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [Address] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [MotherName] NVARCHAR(4000) NOT NULL;

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [FatherName] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [Sex] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [BirthHeight] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [BirthWeight] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [HealthCenter] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [Barangay] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [FamilyNumber] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [Email] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [ContactNumber] NVARCHAR(4000);

-- Update vaccine-related fields
ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [BCGVaccineDate] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [BCGVaccineRemarks] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [HepatitisBVaccineDate] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [HepatitisBVaccineRemarks] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [Pentavalent1Date] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [Pentavalent1Remarks] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [Pentavalent2Date] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [Pentavalent2Remarks] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [Pentavalent3Date] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [Pentavalent3Remarks] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [OPV1Date] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [OPV1Remarks] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [OPV2Date] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [OPV2Remarks] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [OPV3Date] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [OPV3Remarks] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [IPV1Date] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [IPV1Remarks] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [IPV2Date] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [IPV2Remarks] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [PCV1Date] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [PCV1Remarks] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [PCV2Date] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [PCV2Remarks] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [PCV3Date] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [PCV3Remarks] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [MMR1Date] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [MMR1Remarks] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [MMR2Date] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [MMR2Remarks] NVARCHAR(4000);

-- Update record management fields
ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [CreatedAt] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [UpdatedAt] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [CreatedBy] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [UpdatedBy] NVARCHAR(4000);

ALTER TABLE [dbo].[ImmunizationRecords]
ALTER COLUMN [Status] NVARCHAR(4000);

PRINT 'ImmunizationRecords table updated successfully.';

-- Update ImmunizationShortcutForms table column sizes for encrypted fields
PRINT 'Updating ImmunizationShortcutForms table...';

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

PRINT 'ImmunizationShortcutForms table updated successfully.';

PRINT 'Database schema update completed successfully!';
PRINT 'All encrypted fields now have sufficient column sizes (4000 characters) to store encrypted data.';
GO
