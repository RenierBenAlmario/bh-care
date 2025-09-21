-- Script to update ConsultationTimeSlots table with new columns for tracking bookings

-- First transaction: Add columns if they don't exist
BEGIN TRANSACTION;

BEGIN TRY
    -- Check if the table exists
    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConsultationTimeSlots]') AND type in (N'U'))
    BEGIN
        PRINT 'ConsultationTimeSlots table found, checking for missing columns...';
        
        -- Add BookedById column if it doesn't exist
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ConsultationTimeSlots]') AND name = 'BookedById')
        BEGIN
            PRINT 'Adding BookedById column...';
            ALTER TABLE [dbo].[ConsultationTimeSlots] 
            ADD [BookedById] NVARCHAR(450) NULL;
            PRINT 'BookedById column added successfully.';
        END
        ELSE
        BEGIN
            PRINT 'BookedById column already exists.';
        END
        
        -- Add BookedAt column if it doesn't exist
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ConsultationTimeSlots]') AND name = 'BookedAt')
        BEGIN
            PRINT 'Adding BookedAt column...';
            ALTER TABLE [dbo].[ConsultationTimeSlots] 
            ADD [BookedAt] DATETIME2 NULL;
            PRINT 'BookedAt column added successfully.';
        END
        ELSE
        BEGIN
            PRINT 'BookedAt column already exists.';
        END
    END
    ELSE
    BEGIN
        PRINT 'ConsultationTimeSlots table not found. No updates performed.';
    END
    
    COMMIT TRANSACTION;
    PRINT 'Successfully added columns to ConsultationTimeSlots table.';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Error adding columns to ConsultationTimeSlots table: ' + ERROR_MESSAGE();
END CATCH

-- Second transaction: Update the data if columns exist
BEGIN TRANSACTION;

BEGIN TRY
    -- Check if the table and both columns exist
    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConsultationTimeSlots]') AND type in (N'U'))
       AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ConsultationTimeSlots]') AND name = 'BookedById')
       AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ConsultationTimeSlots]') AND name = 'BookedAt')
    BEGIN
        PRINT 'Cleaning up potentially conflicting time slots...';
        
        -- First, get a list of all booked appointment times
        DECLARE @BookedAppointments TABLE (
            AppointmentDate DATE,
            AppointmentTime TIME,
            PatientId NVARCHAR(450)
        );
        
        INSERT INTO @BookedAppointments
        SELECT DISTINCT 
            CAST(AppointmentDate AS DATE), 
            CAST(AppointmentTime AS TIME),
            PatientId
        FROM Appointments
        WHERE Status <> 4; -- Not cancelled
        
        -- Mark any consultation time slots as booked if they match existing appointments
        UPDATE cts
        SET 
            cts.IsBooked = 1,
            cts.BookedAt = GETDATE(),
            cts.BookedById = ba.PatientId
        FROM ConsultationTimeSlots cts
        INNER JOIN @BookedAppointments ba 
            ON CAST(cts.StartTime AS DATE) = ba.AppointmentDate
            AND ABS(DATEDIFF(MINUTE, CAST(cts.StartTime AS TIME), ba.AppointmentTime)) < 30
        WHERE cts.IsBooked = 0;
        
        PRINT 'Time slots updated based on existing appointments.';
    END
    ELSE
    BEGIN
        PRINT 'Update skipped because one or more required columns do not exist yet.';
    END
    
    COMMIT TRANSACTION;
    PRINT 'ConsultationTimeSlots data update completed successfully.';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Error updating ConsultationTimeSlots data: ' + ERROR_MESSAGE();
END CATCH 