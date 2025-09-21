-- Script to create the Prescriptions and PrescriptionMedicines tables with proper migration handling

-- First, check if the tables already exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Prescriptions')
BEGIN
    PRINT 'Creating Prescriptions table...';
    
    -- Create the Prescriptions table
    CREATE TABLE [dbo].[Prescriptions](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [PatientId] [nvarchar](450) NOT NULL,
        [DoctorId] [nvarchar](450) NOT NULL,
        [Diagnosis] [nvarchar](1000) NOT NULL,
        [Duration] [int] NOT NULL,
        [Notes] [nvarchar](1000) NOT NULL,
        [Status] [int] NOT NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [UpdatedAt] [datetime2](7) NOT NULL,
        [PrescriptionDate] [datetime2](7) NOT NULL,
        [CompletedAt] [datetime2](7) NULL,
        [CancelledAt] [datetime2](7) NULL,
        CONSTRAINT [PK_Prescriptions] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    -- Only create foreign keys if the referenced tables exist
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers')
    BEGIN
        ALTER TABLE [dbo].[Prescriptions] WITH CHECK ADD CONSTRAINT [FK_Prescriptions_AspNetUsers_DoctorId] 
        FOREIGN KEY([DoctorId]) REFERENCES [dbo].[AspNetUsers] ([Id]);
    END
    ELSE
    BEGIN
        PRINT 'Warning: AspNetUsers table not found. Foreign key for DoctorId not created.';
    END
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Patients')
    BEGIN
        ALTER TABLE [dbo].[Prescriptions] WITH CHECK ADD CONSTRAINT [FK_Prescriptions_Patients_PatientId] 
        FOREIGN KEY([PatientId]) REFERENCES [dbo].[Patients] ([UserId]);
    END
    ELSE
    BEGIN
        PRINT 'Warning: Patients table not found. Foreign key for PatientId not created.';
    END
    
    PRINT 'Prescriptions table created successfully.';
END
ELSE
BEGIN
    PRINT 'Prescriptions table already exists.';
END

-- Now create the PrescriptionMedicines table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PrescriptionMedicines')
BEGIN
    PRINT 'Creating PrescriptionMedicines table...';
    
    CREATE TABLE [dbo].[PrescriptionMedicines](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [PrescriptionId] [int] NOT NULL,
        [MedicationName] [nvarchar](200) NOT NULL,
        [Dosage] [decimal](10, 2) NOT NULL,
        [Unit] [nvarchar](20) NOT NULL,
        [Frequency] [nvarchar](200) NOT NULL,
        CONSTRAINT [PK_PrescriptionMedicines] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    -- Only create foreign key if Prescriptions table exists
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Prescriptions')
    BEGIN
        ALTER TABLE [dbo].[PrescriptionMedicines] WITH CHECK ADD CONSTRAINT [FK_PrescriptionMedicines_Prescriptions_PrescriptionId] 
        FOREIGN KEY([PrescriptionId]) REFERENCES [dbo].[Prescriptions] ([Id])
        ON DELETE CASCADE;
        
        CREATE INDEX [IX_PrescriptionMedicines_PrescriptionId] ON [dbo].[PrescriptionMedicines]([PrescriptionId]);
    END
    ELSE
    BEGIN
        PRINT 'Warning: Prescriptions table not found. Foreign key for PrescriptionId not created.';
    END
    
    PRINT 'PrescriptionMedicines table created successfully.';
END
ELSE
BEGIN
    PRINT 'PrescriptionMedicines table already exists.';
END

-- Update the database context model snapshot version if needed
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    PRINT 'Warning: EF Migrations history table not found. Unable to update migration history.';
END
ELSE
BEGIN
    -- Check if our migration is already recorded
    IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250528192216_AddPrescriptionMedicinesTable')
    BEGIN
        -- Insert our custom migration record so EF knows these tables exist
        INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES ('20250528192216_AddPrescriptionMedicinesTable', '7.0.5');
        
        PRINT 'Migration history updated.';
    END
    ELSE
    BEGIN
        PRINT 'Migration history already contains this migration.';
    END
END

PRINT 'Database update completed successfully.'; 