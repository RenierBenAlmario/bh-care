USE [Barangay]
GO

-- Create LabResults table
PRINT 'Creating LabResults table...'

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'LabResults')
BEGIN
    CREATE TABLE [dbo].[LabResults] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [PatientId] NVARCHAR(450) NOT NULL,
        [Date] DATETIME2 NOT NULL,
        [TestName] NVARCHAR(100) NOT NULL,
        [Result] NVARCHAR(500) NOT NULL,
        [ReferenceRange] NVARCHAR(100) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL,
        [Notes] NVARCHAR(1000) NULL,
        CONSTRAINT [PK_LabResults] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_LabResults_AspNetUsers_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );
    
    PRINT 'LabResults table created successfully';
END
ELSE
BEGIN
    PRINT 'LabResults table already exists';
END

-- Create index for better performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LabResults_PatientId')
BEGIN
    CREATE INDEX [IX_LabResults_PatientId] ON [dbo].[LabResults]([PatientId]);
    PRINT 'Index IX_LabResults_PatientId created';
END

-- Add some sample lab results for testing
PRINT 'Adding sample lab results...'

IF NOT EXISTS (SELECT 1 FROM LabResults WHERE PatientId = 'eee7f324-6daa-4b50-ad64-b847c6015acc')
BEGIN
    INSERT INTO LabResults (PatientId, Date, TestName, Result, ReferenceRange, Status, Notes) VALUES
    ('eee7f324-6daa-4b50-ad64-b847c6015acc', '2025-08-06', 'Complete Blood Count', 'Normal', 'Normal Range', 'Normal', 'All values within normal limits'),
    ('eee7f324-6daa-4b50-ad64-b847c6015acc', '2025-08-06', 'Blood Glucose', '95 mg/dL', '70-100 mg/dL', 'Normal', 'Fasting glucose level'),
    ('eee7f324-6daa-4b50-ad64-b847c6015acc', '2025-08-06', 'Cholesterol Panel', '180 mg/dL', '<200 mg/dL', 'Normal', 'Total cholesterol level');
    
    PRINT 'Sample lab results added successfully';
END
ELSE
BEGIN
    PRINT 'Sample lab results already exist';
END

-- Verify the table was created
SELECT 
    'LabResults Table' as TableName,
    COUNT(*) as RecordCount
FROM LabResults;

PRINT 'LabResults table setup completed successfully!'; 