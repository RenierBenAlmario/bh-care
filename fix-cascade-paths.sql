-- Fix Cascade Paths for Messages Table
-- This script fixes the foreign key constraint issues with SenderId and ReceiverId in Messages table

PRINT 'Starting to fix cascade paths issues...';

-- First, drop existing foreign key constraints if they exist
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Messages_AspNetUsers_SenderId')
BEGIN
    ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_SenderId];
    PRINT 'Dropped FK_Messages_AspNetUsers_SenderId constraint';
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Messages_AspNetUsers_ReceiverId')
BEGIN
    ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId];
    PRINT 'Dropped FK_Messages_AspNetUsers_ReceiverId constraint';
END

-- Re-create the foreign key constraints with NO ACTION instead of CASCADE
ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_SenderId] 
    FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
PRINT 'Added FK_Messages_AspNetUsers_SenderId constraint with NO ACTION';

ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId] 
    FOREIGN KEY ([ReceiverId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
PRINT 'Added FK_Messages_AspNetUsers_ReceiverId constraint with NO ACTION';

-- Also check and fix other cascade paths that might be causing issues
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_MedicalRecords_Patients_PatientId' AND delete_referential_action = 1) -- 1 = CASCADE
BEGIN
    ALTER TABLE [MedicalRecords] DROP CONSTRAINT [FK_MedicalRecords_Patients_PatientId];
    ALTER TABLE [MedicalRecords] ADD CONSTRAINT [FK_MedicalRecords_Patients_PatientId] 
        FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION;
    PRINT 'Fixed FK_MedicalRecords_Patients_PatientId cascade path';
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Prescriptions_AspNetUsers_PatientId' AND delete_referential_action = 1) -- 1 = CASCADE
BEGIN
    ALTER TABLE [Prescriptions] DROP CONSTRAINT [FK_Prescriptions_AspNetUsers_PatientId];
    ALTER TABLE [Prescriptions] ADD CONSTRAINT [FK_Prescriptions_AspNetUsers_PatientId] 
        FOREIGN KEY ([PatientId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
    PRINT 'Fixed FK_Prescriptions_AspNetUsers_PatientId cascade path';
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Prescriptions_AspNetUsers_DoctorId' AND delete_referential_action = 1) -- 1 = CASCADE
BEGIN
    ALTER TABLE [Prescriptions] DROP CONSTRAINT [FK_Prescriptions_AspNetUsers_DoctorId];
    ALTER TABLE [Prescriptions] ADD CONSTRAINT [FK_Prescriptions_AspNetUsers_DoctorId] 
        FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
    PRINT 'Fixed FK_Prescriptions_AspNetUsers_DoctorId cascade path';
END

PRINT 'Cascade paths fix completed successfully!';

-- Verify the constraints were created successfully
SELECT 
    fk.name AS 'Foreign Key', 
    OBJECT_NAME(fk.parent_object_id) AS 'Table',
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS 'Column',
    OBJECT_NAME(fk.referenced_object_id) AS 'Referenced Table',
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS 'Referenced Column',
    fk.delete_referential_action_desc AS 'Delete Action',
    fk.update_referential_action_desc AS 'Update Action'
FROM 
    sys.foreign_keys fk
INNER JOIN 
    sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
WHERE 
    OBJECT_NAME(fk.parent_object_id) = 'Messages'
ORDER BY 
    fk.name 