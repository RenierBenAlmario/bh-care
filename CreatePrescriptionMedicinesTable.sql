-- Script to create the missing PrescriptionMedicines table
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