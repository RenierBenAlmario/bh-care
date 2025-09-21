-- Script to create the missing Prescriptions and PrescriptionMedicines tables

-- Create Prescriptions table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Prescriptions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Prescriptions] (
        [Id] INT NOT NULL IDENTITY(1,1),
        [PatientId] NVARCHAR(450) NOT NULL,
        [DoctorId] NVARCHAR(450) NOT NULL,
        [Diagnosis] NVARCHAR(1000) NOT NULL,
        [Duration] INT NOT NULL,
        [Notes] NVARCHAR(1000) NOT NULL DEFAULT (''),
        [Status] INT NOT NULL DEFAULT (0),
        [CreatedAt] DATETIME2 NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT (GETUTCDATE()),
        [PrescriptionDate] DATETIME2 NOT NULL DEFAULT (GETUTCDATE()),
        [CompletedAt] DATETIME2 NULL,
        [CancelledAt] DATETIME2 NULL,
        CONSTRAINT [PK_Prescriptions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Prescriptions_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]),
        CONSTRAINT [FK_Prescriptions_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId])
    );

    CREATE INDEX [IX_Prescriptions_DoctorId] ON [Prescriptions] ([DoctorId]);
    CREATE INDEX [IX_Prescriptions_PatientId] ON [Prescriptions] ([PatientId]);
    
    PRINT 'Prescriptions table created successfully.';
END
ELSE
BEGIN
    PRINT 'Prescriptions table already exists.';
END

-- Create PrescriptionMedicines table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PrescriptionMedicines]') AND type in (N'U'))
BEGIN
    CREATE TABLE [PrescriptionMedicines] (
        [Id] INT NOT NULL IDENTITY(1,1),
        [PrescriptionId] INT NOT NULL,
        [MedicationName] NVARCHAR(200) NOT NULL,
        [Dosage] DECIMAL(10,2) NOT NULL,
        [Unit] NVARCHAR(20) NOT NULL,
        [Frequency] NVARCHAR(200) NOT NULL,
        CONSTRAINT [PK_PrescriptionMedicines] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PrescriptionMedicines_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_PrescriptionMedicines_PrescriptionId] ON [PrescriptionMedicines] ([PrescriptionId]);
    
    PRINT 'PrescriptionMedicines table created successfully.';
END
ELSE
BEGIN
    PRINT 'PrescriptionMedicines table already exists.';
END 