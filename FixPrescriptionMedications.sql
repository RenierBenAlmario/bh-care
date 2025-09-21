-- Script to ensure proper relationship between PrescriptionMedications and Prescriptions tables

-- Check if PrescriptionMedications table exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'PrescriptionMedications')
BEGIN
    PRINT 'Fixing PrescriptionMedications relationships...';
    
    -- Check if the foreign key constraint already exists
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PrescriptionMedications_Prescriptions_PrescriptionId')
    BEGIN
        -- Check if Prescriptions table exists before adding foreign key
        IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Prescriptions')
        BEGIN
            -- Add foreign key constraint if it doesn't exist
            ALTER TABLE [dbo].[PrescriptionMedications] WITH CHECK ADD CONSTRAINT [FK_PrescriptionMedications_Prescriptions_PrescriptionId] 
            FOREIGN KEY([PrescriptionId]) REFERENCES [dbo].[Prescriptions] ([Id]);
            
            PRINT 'Foreign key added between PrescriptionMedications and Prescriptions tables.';
        END
        ELSE
        BEGIN
            PRINT 'Warning: Prescriptions table not found. Foreign key not created.';
        END
    END
    ELSE
    BEGIN
        PRINT 'Foreign key relationship already exists.';
    END
    
    -- Create index if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PrescriptionMedications_PrescriptionId' AND object_id = OBJECT_ID('PrescriptionMedications'))
    BEGIN
        CREATE INDEX [IX_PrescriptionMedications_PrescriptionId] ON [dbo].[PrescriptionMedications]([PrescriptionId]);
        PRINT 'Index created on PrescriptionId column.';
    END
    ELSE
    BEGIN
        PRINT 'Index already exists.';
    END
END
ELSE
BEGIN
    PRINT 'PrescriptionMedications table does not exist.';
END

PRINT 'Script completed.'; 