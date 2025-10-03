-- Fix migration history: Mark AddAppointmentIdToMedicalRecords as applied
USE [bhcareDB]
GO

-- Mark the migration as applied since AppointmentId column already exists
IF NOT EXISTS (
    SELECT 1
    FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250730144649_AddAppointmentIdToMedicalRecords'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250730144649_AddAppointmentIdToMedicalRecords', N'9.0.5');
    PRINT 'AddAppointmentIdToMedicalRecords migration marked as applied.';
END
ELSE
BEGIN
    PRINT 'AddAppointmentIdToMedicalRecords migration already in history.';
END
GO
