-- Add missing columns and modify existing ones in Appointments table
SET QUOTED_IDENTIFIER ON;

-- Drop unnecessary columns
ALTER TABLE [dbo].[Appointments] DROP COLUMN [PatientName];
ALTER TABLE [dbo].[Appointments] DROP COLUMN [EndTime];
ALTER TABLE [dbo].[Appointments] DROP COLUMN [Fee];
ALTER TABLE [dbo].[Appointments] DROP COLUMN [Notes];
ALTER TABLE [dbo].[Appointments] DROP COLUMN [PatientId1];

-- Add new columns
ALTER TABLE [dbo].[Appointments]
ADD [Age] INT NOT NULL DEFAULT 0,
    [AttachmentName] NVARCHAR(MAX) NULL,
    [AttachmentPath] NVARCHAR(MAX) NULL,
    [AttachmentType] NVARCHAR(MAX) NULL,
    [Diagnosis] NVARCHAR(MAX) NULL,
    [FamilyMemberId] INT NULL,
    [Instructions] NVARCHAR(MAX) NULL,
    [Prescription] NVARCHAR(MAX) NULL,
    [UpdatedAt] DATETIME2 NULL; 