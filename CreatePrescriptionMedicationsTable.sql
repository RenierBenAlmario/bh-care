-- Script to create the PrescriptionMedications table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PrescriptionMedications')
BEGIN
    PRINT 'Creating PrescriptionMedications table...';
    
    CREATE TABLE [dbo].[PrescriptionMedications](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [PrescriptionId] [int] NOT NULL,
        [MedicationId] [int] NOT NULL,
        [Dosage] [nvarchar](100) NOT NULL,
        [Frequency] [nvarchar](100) NOT NULL,
        [Instructions] [nvarchar](500) NOT NULL DEFAULT (''),
        [MedicalRecordId] [int] NOT NULL,
        [MedicationName] [nvarchar](max) NOT NULL,
        [Duration] [nvarchar](max) NOT NULL,
        [CreatedAt] [datetime2](7) NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] [datetime2](7) NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_PrescriptionMedications] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    -- Only create foreign key if Prescriptions table exists
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Prescriptions')
    BEGIN
        ALTER TABLE [dbo].[PrescriptionMedications] WITH CHECK ADD CONSTRAINT [FK_PrescriptionMedications_Prescriptions_PrescriptionId] 
        FOREIGN KEY([PrescriptionId]) REFERENCES [dbo].[Prescriptions] ([Id]);
        
        CREATE INDEX [IX_PrescriptionMedications_PrescriptionId] ON [dbo].[PrescriptionMedications]([PrescriptionId]);
        PRINT 'Foreign key and index created.';
    END
    ELSE
    BEGIN
        PRINT 'Warning: Prescriptions table not found. Foreign key not created.';
    END
    
    -- Create foreign keys for MedicationId if Medications table exists
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Medications')
    BEGIN
        ALTER TABLE [dbo].[PrescriptionMedications] WITH CHECK ADD CONSTRAINT [FK_PrescriptionMedications_Medications_MedicationId] 
        FOREIGN KEY([MedicationId]) REFERENCES [dbo].[Medications] ([Id]);
        
        CREATE INDEX [IX_PrescriptionMedications_MedicationId] ON [dbo].[PrescriptionMedications]([MedicationId]);
        PRINT 'Medication foreign key and index created.';
    END
    ELSE
    BEGIN
        PRINT 'Warning: Medications table not found. Foreign key for MedicationId not created.';
    END
    
    -- Create foreign keys for MedicalRecordId if MedicalRecords table exists
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'MedicalRecords')
    BEGIN
        ALTER TABLE [dbo].[PrescriptionMedications] WITH CHECK ADD CONSTRAINT [FK_PrescriptionMedications_MedicalRecords_MedicalRecordId] 
        FOREIGN KEY([MedicalRecordId]) REFERENCES [dbo].[MedicalRecords] ([Id]);
        
        CREATE INDEX [IX_PrescriptionMedications_MedicalRecordId] ON [dbo].[PrescriptionMedications]([MedicalRecordId]);
        PRINT 'MedicalRecord foreign key and index created.';
    END
    ELSE
    BEGIN
        PRINT 'Warning: MedicalRecords table not found. Foreign key for MedicalRecordId not created.';
    END
    
    PRINT 'PrescriptionMedications table created successfully.';
END
ELSE
BEGIN
    PRINT 'PrescriptionMedications table already exists.';
END 