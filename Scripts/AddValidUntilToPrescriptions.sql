USE [Barangay]
GO

/* Add ValidUntil column to Prescriptions and backfill values */
PRINT 'Adding ValidUntil column to Prescriptions table and backfilling...';

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Prescriptions' AND COLUMN_NAME = 'ValidUntil'
)
BEGIN
    ALTER TABLE Prescriptions ADD ValidUntil DATETIME2 NULL;
    PRINT 'Added ValidUntil column (NULLable) to Prescriptions.';
END
ELSE
BEGIN
    PRINT 'ValidUntil column already exists in Prescriptions.';
END

/* Backfill: ValidUntil = PrescriptionDate + Duration (days) when null */
UPDATE P
SET ValidUntil = DATEADD(DAY, ISNULL(Duration, 0), ISNULL(PrescriptionDate, SYSUTCDATETIME()))
FROM Prescriptions P
WHERE P.ValidUntil IS NULL;

/* Show sample of updated rows */
SELECT TOP 25 Id, PrescriptionDate, Duration, ValidUntil
FROM Prescriptions
ORDER BY Id DESC;

PRINT 'ValidUntil column migration completed.';
