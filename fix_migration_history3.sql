-- Mark AddUnitPropertyToPrescriptionMedications as applied since schema is already correct
USE [bhcareDB]
GO

IF NOT EXISTS (
    SELECT 1
    FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250803133403_AddUnitPropertyToPrescriptionMedications'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250803133403_AddUnitPropertyToPrescriptionMedications', N'9.0.5');
    PRINT 'AddUnitPropertyToPrescriptionMedications migration marked as applied.';
END
ELSE
BEGIN
    PRINT 'Migration already in history.';
END
GO
