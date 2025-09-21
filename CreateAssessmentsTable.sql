-- Script to create Assessments table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Assessments')
BEGIN
    CREATE TABLE [dbo].[Assessments](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [FamilyNumber] [nvarchar](100) NULL,
        [ReasonForVisit] [nvarchar](max) NULL,
        [Symptoms] [nvarchar](max) NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_Assessments] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
    PRINT 'Assessments table created successfully.'
END
ELSE
BEGIN
    PRINT 'Assessments table already exists.'
END 