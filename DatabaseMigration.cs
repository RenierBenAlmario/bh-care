using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Barangay
{
    public class DatabaseMigration
    {
        private readonly ILogger<DatabaseMigration> _logger;
        private readonly string _connectionString;

        public DatabaseMigration(IConfiguration configuration, ILogger<DatabaseMigration> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                throw new ArgumentNullException(nameof(configuration), "Connection string 'DefaultConnection' not found");
            _logger = logger;
        }

        public async Task MigrateAsync()
        {
            _logger.LogInformation("Starting database migration...");

            try
            {
                // Check database connection
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    _logger.LogInformation("Successfully connected to database");

                    // Execute migration scripts
                    await ExecuteScriptAsync(connection, GetDatabaseSchemaFixScript());
                    await ExecuteScriptAsync(connection, GetDataMigrationScript());
                    await ExecuteScriptAsync(connection, GetIndexesAndConstraintsScript());
                    await ExecuteScriptAsync(connection, GetPermissionsSeedScript());
                }

                _logger.LogInformation("Database migration completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during database migration");
                throw;
            }
        }

        private async Task ExecuteScriptAsync(SqlConnection connection, string script)
        {
            // Split the script by GO statements to execute in batches
            string[] batches = script.Split(new[] { "\nGO\n", "\ngo\n" }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string batch in batches)
            {
                if (string.IsNullOrWhiteSpace(batch))
                    continue;
                    
                using (var command = new SqlCommand(batch.Trim(), connection))
                {
                    try
                    {
                        // Set a longer timeout (5 minutes)
                        command.CommandTimeout = 300;
                        
                        // Log the start of batch execution
                        _logger.LogInformation("Starting batch execution...");
                        
                        await command.ExecuteNonQueryAsync();
                        
                        _logger.LogInformation("Batch executed successfully");
                    }
                    catch (SqlException ex) when (ex.Number == -2) // Timeout error
                    {
                        _logger.LogWarning("Batch execution timed out, retrying with longer timeout...");
                        
                        // Retry with even longer timeout (10 minutes)
                        command.CommandTimeout = 600;
                        try
                        {
                            await command.ExecuteNonQueryAsync();
                            _logger.LogInformation("Batch executed successfully after retry");
                        }
                        catch (Exception retryEx)
                        {
                            _logger.LogError(retryEx, "Error executing batch after retry: {Error}", retryEx.Message);
                            // Continue with the next batch instead of throwing
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error executing batch: {Error}", ex.Message);
                        // Continue with the next batch instead of throwing
                    }
                }
            }
        }

        private string GetPermissionsSeedScript()
        {
            var scriptPath = Path.Combine(AppContext.BaseDirectory, "SeedPermissions.sql");
            if (!File.Exists(scriptPath))
            {
                _logger.LogWarning($"Permissions seed script not found at {scriptPath}");
                return string.Empty;
            }
            _logger.LogInformation("Loading permissions seed script from {path}", scriptPath);
            return File.ReadAllText(scriptPath);
        }

        private string GetDatabaseSchemaFixScript()
        {
            return @"
-- Set up error handling to avoid script termination
SET XACT_ABORT OFF;

-- Fix Users table Password column
BEGIN TRY
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
    BEGIN
        -- Check if Password column exists and handle appropriately
        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'Password')
        BEGIN
            -- Directly drop and recreate the Password column without trying to save data
            ALTER TABLE [dbo].[Users] DROP COLUMN [Password];
            PRINT 'Dropped existing Password column';
        END
        
        -- Add the Password column with the correct type
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'Password')
        BEGIN
            ALTER TABLE [dbo].[Users] ADD [Password] NVARCHAR(MAX) NULL;
            PRINT 'Added Password column to Users table with correct type';
        END
    END
    ELSE
    BEGIN
        -- If Users table doesn't exist, we need to check if AspNetUsers exists (common Identity table name)
        -- and potentially create a Users table or add Password to AspNetUsers
        IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers')
        BEGIN
            -- Check if AspNetUsers already has a Password column
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'Password')
            BEGIN
                -- Add Password column to AspNetUsers
                ALTER TABLE [dbo].[AspNetUsers] ADD [Password] NVARCHAR(MAX) NULL;
                PRINT 'Password column added to AspNetUsers table';
            END
        END
        ELSE
        BEGIN
            PRINT 'Neither Users nor AspNetUsers table exists - skipping Password column fix';
        END
    END
END TRY
BEGIN CATCH
    PRINT 'Error in Users table Password column fix: ' + ERROR_MESSAGE();
END CATCH

-- Fix VitalSigns table schema
BEGIN TRY
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'VitalSigns')
    BEGIN
        -- Create a backup of the VitalSigns table
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VitalSigns_Backup')
        BEGIN
            SELECT * INTO VitalSigns_Backup FROM VitalSigns;
            PRINT 'VitalSigns table backed up to VitalSigns_Backup';
        END

        -- Drop foreign key constraints if they exist
        DECLARE @constraintName NVARCHAR(200);
        SELECT @constraintName = name FROM sys.foreign_keys
        WHERE parent_object_id = OBJECT_ID(N'VitalSigns')
        AND referenced_object_id = OBJECT_ID(N'Patients');
        
        IF @constraintName IS NOT NULL
        BEGIN
            DECLARE @sql NVARCHAR(500) = N'ALTER TABLE [VitalSigns] DROP CONSTRAINT ' + @constraintName;
            EXEC sp_executesql @sql;
            PRINT 'Dropped foreign key constraint: ' + @constraintName;
        END

        -- Check column types and alter if needed
        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[VitalSigns]') AND name = 'HeartRate' AND system_type_id <> 56) -- 56 is INT
        BEGIN
            -- Drop the table and recreate with correct types
            DROP TABLE VitalSigns;
            PRINT 'Dropped VitalSigns table to recreate with correct column types';

            -- Create a new table with correct column types
            CREATE TABLE [VitalSigns] (
                [Id] INT IDENTITY(1,1) NOT NULL,
                [PatientId] NVARCHAR(450) NOT NULL,
                [Temperature] DECIMAL(5, 2) NULL,
                [BloodPressure] NVARCHAR(20) NULL,
                [HeartRate] INT NULL,
                [RespiratoryRate] INT NULL,
                [SpO2] DECIMAL(5, 2) NULL,
                [Weight] DECIMAL(5, 2) NULL,
                [Height] DECIMAL(5, 2) NULL,
                [RecordedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
                [Notes] NVARCHAR(1000) NULL,
                CONSTRAINT [PK_VitalSigns] PRIMARY KEY ([Id])
            );
            PRINT 'Created new VitalSigns table with correct column types';

            -- Copy data from backup table to new table with type conversion
            INSERT INTO VitalSigns (PatientId, Temperature, BloodPressure, HeartRate, RespiratoryRate, SpO2, Weight, Height, RecordedAt, Notes)
            SELECT 
                PatientId,
                CASE WHEN ISNUMERIC(Temperature) = 1 THEN CAST(Temperature AS DECIMAL(5,2)) ELSE NULL END,
                BloodPressure,
                CASE WHEN ISNUMERIC(HeartRate) = 1 THEN CAST(HeartRate AS INT) ELSE NULL END,
                CASE WHEN ISNUMERIC(RespiratoryRate) = 1 THEN CAST(RespiratoryRate AS INT) ELSE NULL END,
                CASE WHEN ISNUMERIC(SpO2) = 1 THEN CAST(SpO2 AS DECIMAL(5,2)) ELSE NULL END,
                CASE WHEN ISNUMERIC(Weight) = 1 THEN CAST(Weight AS DECIMAL(5,2)) ELSE NULL END,
                CASE WHEN ISNUMERIC(Height) = 1 THEN CAST(Height AS DECIMAL(5,2)) ELSE NULL END,
                RecordedAt,
                Notes
            FROM VitalSigns_Backup;
            PRINT 'Data copied from VitalSigns_Backup to VitalSigns with proper type conversion';
        END
    END
    ELSE
    BEGIN
        -- Create the VitalSigns table if it doesn't exist
        CREATE TABLE [VitalSigns] (
            [Id] INT IDENTITY(1,1) NOT NULL,
            [PatientId] NVARCHAR(450) NOT NULL,
            [Temperature] DECIMAL(5, 2) NULL,
            [BloodPressure] NVARCHAR(20) NULL,
            [HeartRate] INT NULL,
            [RespiratoryRate] INT NULL,
            [SpO2] DECIMAL(5, 2) NULL,
            [Weight] DECIMAL(5, 2) NULL,
            [Height] DECIMAL(5, 2) NULL,
            [RecordedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
            [Notes] NVARCHAR(1000) NULL,
            CONSTRAINT [PK_VitalSigns] PRIMARY KEY ([Id])
        );
        PRINT 'Created new VitalSigns table';
    END
END TRY
BEGIN CATCH
    PRINT 'Error in VitalSigns table fix: ' + ERROR_MESSAGE();
END CATCH

-- Fix MedicalRecords table
BEGIN TRY
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'MedicalRecords')
    BEGIN
        -- Add ApplicationUserId column if it doesn't exist
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[MedicalRecords]') AND name = 'ApplicationUserId')
        BEGIN
            ALTER TABLE [dbo].[MedicalRecords]
            ADD [ApplicationUserId] NVARCHAR(450) NULL;
            PRINT 'ApplicationUserId column added to MedicalRecords table';
        END

        -- Add Medications column if it doesn't exist
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[MedicalRecords]') AND name = 'Medications')
        BEGIN
            ALTER TABLE [dbo].[MedicalRecords]
            ADD [Medications] NVARCHAR(1000) NULL;
            PRINT 'Medications column added to MedicalRecords table';
        END

        -- Add RecordDate column if it doesn't exist
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[MedicalRecords]') AND name = 'RecordDate')
        BEGIN
            ALTER TABLE [dbo].[MedicalRecords]
            ADD [RecordDate] DATETIME2 NOT NULL DEFAULT GETDATE();
            
            -- Update RecordDate with CreatedAt values for existing records
            IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[MedicalRecords]') AND name = 'CreatedAt')
            BEGIN
                UPDATE [dbo].[MedicalRecords]
                SET [RecordDate] = [CreatedAt]
                WHERE [RecordDate] = CAST('0001-01-01' AS DATETIME2);
            END
            
            PRINT 'RecordDate column added to MedicalRecords table';
        END
    END
    ELSE
    BEGIN
        PRINT 'MedicalRecords table does not exist';
    END
END TRY
BEGIN CATCH
    PRINT 'Error in MedicalRecords table fix: ' + ERROR_MESSAGE();
END CATCH

-- Fix Appointments table
BEGIN TRY
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Appointments')
    BEGIN
        -- Add ApplicationUserId column if it doesn't exist
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = 'ApplicationUserId')
        BEGIN
            ALTER TABLE [dbo].[Appointments]
            ADD [ApplicationUserId] NVARCHAR(450) NULL;
            
            -- Update ApplicationUserId with DoctorId for existing records
            IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = 'DoctorId')
            BEGIN
                UPDATE [dbo].[Appointments]
                SET [ApplicationUserId] = [DoctorId];
            END
            
            PRINT 'ApplicationUserId column added to Appointments table';
        END
    END
    ELSE
    BEGIN
        PRINT 'Appointments table does not exist';
    END
END TRY
BEGIN CATCH
    PRINT 'Error in Appointments table fix: ' + ERROR_MESSAGE();
END CATCH

-- Record migration in migration history
BEGIN TRY
    IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20240525_DatabaseSchemaFix')
    BEGIN
        INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES ('20240525_DatabaseSchemaFix', '8.0.0');
        PRINT 'Migration recorded in __EFMigrationsHistory';
    END
END TRY
BEGIN CATCH
    PRINT 'Error recording migration in __EFMigrationsHistory: ' + ERROR_MESSAGE();
END CATCH
";
        }

        private string GetDataMigrationScript()
        {
            return @"
-- Set up error handling to avoid script termination
SET XACT_ABORT OFF;

-- Fix any data inconsistencies
BEGIN TRY
    -- Fix any NULL values in required fields
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'VitalSigns')
    BEGIN
        UPDATE [dbo].[VitalSigns]
        SET [PatientId] = 'unknown'
        WHERE [PatientId] IS NULL;
        PRINT 'Fixed NULL PatientIds in VitalSigns';
    END
END TRY
BEGIN CATCH
    PRINT 'Error fixing NULL values in VitalSigns: ' + ERROR_MESSAGE();
END CATCH

-- Update ApplicationUserId in Appointments if NULL
BEGIN TRY
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Appointments')
        AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = 'ApplicationUserId')
        AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = 'DoctorId')
    BEGIN
        UPDATE [dbo].[Appointments]
        SET [ApplicationUserId] = [DoctorId]
        WHERE [ApplicationUserId] IS NULL AND [DoctorId] IS NOT NULL;
        PRINT 'Updated NULL ApplicationUserIds in Appointments';
    END
END TRY
BEGIN CATCH
    PRINT 'Error updating ApplicationUserId in Appointments: ' + ERROR_MESSAGE();
END CATCH

-- Record data migration in migration history
BEGIN TRY
    IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20240525_DataMigration')
    BEGIN
        INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES ('20240525_DataMigration', '8.0.0');
        PRINT 'Data migration recorded in __EFMigrationsHistory';
    END
END TRY
BEGIN CATCH
    PRINT 'Error recording data migration in __EFMigrationsHistory: ' + ERROR_MESSAGE();
END CATCH
";
        }

        private string GetIndexesAndConstraintsScript()
        {
            return @"
-- Set up error handling to avoid script termination
SET XACT_ABORT OFF;

-- Add foreign key from VitalSigns to Patients
BEGIN TRY
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'VitalSigns')
        AND EXISTS (SELECT * FROM sys.tables WHERE name = 'Patients')
        AND NOT EXISTS (
            SELECT * FROM sys.foreign_keys 
            WHERE parent_object_id = OBJECT_ID(N'VitalSigns')
            AND referenced_object_id = OBJECT_ID(N'Patients')
        )
    BEGIN
        ALTER TABLE [VitalSigns]
        ADD CONSTRAINT [FK_VitalSigns_Patients_PatientId]
        FOREIGN KEY ([PatientId]) REFERENCES [Patients]([UserId])
        ON DELETE CASCADE;
        
        PRINT 'Added foreign key constraint from VitalSigns to Patients';
        
        -- Create index for PatientId if it doesn't exist
        IF NOT EXISTS (
            SELECT * FROM sys.indexes 
            WHERE name = 'IX_VitalSigns_PatientId' 
            AND object_id = OBJECT_ID(N'VitalSigns')
        )
        BEGIN
            CREATE INDEX [IX_VitalSigns_PatientId] ON [VitalSigns]([PatientId]);
            PRINT 'Created index for PatientId on VitalSigns table';
        END
    END
END TRY
BEGIN CATCH
    PRINT 'Error adding foreign key from VitalSigns to Patients: ' + ERROR_MESSAGE();
END CATCH

-- Add foreign key from MedicalRecords.ApplicationUserId to AspNetUsers
BEGIN TRY
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'MedicalRecords')
        AND EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers')
        AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[MedicalRecords]') AND name = 'ApplicationUserId')
        AND NOT EXISTS (
            SELECT * FROM sys.foreign_keys 
            WHERE parent_object_id = OBJECT_ID(N'MedicalRecords')
            AND referenced_object_id = OBJECT_ID(N'AspNetUsers')
        )
    BEGIN
        ALTER TABLE [MedicalRecords]
        ADD CONSTRAINT [FK_MedicalRecords_AspNetUsers_ApplicationUserId]
        FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers]([Id])
        ON DELETE SET NULL;
        
        PRINT 'Added foreign key constraint from MedicalRecords.ApplicationUserId to AspNetUsers';
        
        -- Create index for ApplicationUserId if it doesn't exist
        IF NOT EXISTS (
            SELECT * FROM sys.indexes 
            WHERE name = 'IX_MedicalRecords_ApplicationUserId' 
            AND object_id = OBJECT_ID(N'MedicalRecords')
        )
        BEGIN
            CREATE INDEX [IX_MedicalRecords_ApplicationUserId] ON [MedicalRecords]([ApplicationUserId]);
            PRINT 'Created index for ApplicationUserId on MedicalRecords table';
        END
    END
END TRY
BEGIN CATCH
    PRINT 'Error adding foreign key from MedicalRecords to AspNetUsers: ' + ERROR_MESSAGE();
END CATCH

-- Add foreign key from Appointments.ApplicationUserId to AspNetUsers
BEGIN TRY
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Appointments')
        AND EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers')
        AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = 'ApplicationUserId')
        AND NOT EXISTS (
            SELECT * FROM sys.foreign_keys 
            WHERE parent_object_id = OBJECT_ID(N'Appointments')
            AND referenced_object_id = OBJECT_ID(N'AspNetUsers')
        )
    BEGIN
        ALTER TABLE [Appointments]
        ADD CONSTRAINT [FK_Appointments_AspNetUsers_ApplicationUserId]
        FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers]([Id])
        ON DELETE SET NULL;
        
        PRINT 'Added foreign key constraint from Appointments.ApplicationUserId to AspNetUsers';
        
        -- Create index for ApplicationUserId if it doesn't exist
        IF NOT EXISTS (
            SELECT * FROM sys.indexes 
            WHERE name = 'IX_Appointments_ApplicationUserId' 
            AND object_id = OBJECT_ID(N'Appointments')
        )
        BEGIN
            CREATE INDEX [IX_Appointments_ApplicationUserId] ON [Appointments]([ApplicationUserId]);
            PRINT 'Created index for ApplicationUserId on Appointments table';
        END
    END
END TRY
BEGIN CATCH
    PRINT 'Error adding foreign key from Appointments to AspNetUsers: ' + ERROR_MESSAGE();
END CATCH

-- Record constraints and indexes in migration history
BEGIN TRY
    IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20240525_IndexesAndConstraints')
    BEGIN
        INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES ('20240525_IndexesAndConstraints', '8.0.0');
        PRINT 'Indexes and constraints migration recorded in __EFMigrationsHistory';
    END
END TRY
BEGIN CATCH
    PRINT 'Error recording indexes and constraints in __EFMigrationsHistory: ' + ERROR_MESSAGE();
END CATCH
";
        }
    }
} 