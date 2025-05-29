using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Barangay
{
    public class DatabaseFix
    {
        public static async Task FixDatabase(string connectionString)
        {
            try
            {
                // SQL commands to fix the database
                string[] sqlCommands = {
                    // Create AppointmentAttachments table if it doesn't exist
                    @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AppointmentAttachments')
                    BEGIN
                        CREATE TABLE [AppointmentAttachments] (
                            [Id] INT IDENTITY(1,1) NOT NULL,
                            [AppointmentId] INT NOT NULL,
                            [FileName] NVARCHAR(256) NOT NULL,
                            [OriginalFileName] NVARCHAR(256) NOT NULL,
                            [ContentType] NVARCHAR(100) NOT NULL,
                            [FilePath] NVARCHAR(256) NOT NULL,
                            [UploadedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
                            [ApplicationUserId] NVARCHAR(450) NULL,
                            [AttachmentsData] VARBINARY(MAX) NULL,
                            CONSTRAINT [PK_AppointmentAttachments] PRIMARY KEY ([Id])
                        );
                        
                        -- Add foreign key to Appointments table
                        ALTER TABLE [AppointmentAttachments]
                        ADD CONSTRAINT [FK_AppointmentAttachments_Appointments_AppointmentId]
                        FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments]([Id])
                        ON DELETE CASCADE;
                        
                        -- Add foreign key to AspNetUsers table
                        ALTER TABLE [AppointmentAttachments]
                        ADD CONSTRAINT [FK_AppointmentAttachments_AspNetUsers_ApplicationUserId]
                        FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers]([Id]);
                        
                        -- Create index for AppointmentId
                        CREATE INDEX [IX_AppointmentAttachments_AppointmentId] ON [AppointmentAttachments]([AppointmentId]);
                        
                        -- Create index for ApplicationUserId
                        CREATE INDEX [IX_AppointmentAttachments_ApplicationUserId] ON [AppointmentAttachments]([ApplicationUserId]);
                    END",
                    
                    // Add CreatedAt column to StaffMembers table if it doesn't exist
                    @"IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[StaffMembers]') AND name = 'CreatedAt')
                    BEGIN
                        ALTER TABLE [dbo].[StaffMembers]
                        ADD [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE();
                    END",
                    
                    // Fix Password column in Users table - drop and recreate if it exists with wrong type
                    @"IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
                    BEGIN
                        -- Check if Password column exists
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'Password')
                        BEGIN
                            -- Drop the column and recreate it with the correct type
                            ALTER TABLE [dbo].[Users]
                            DROP COLUMN [Password];
                            
                            ALTER TABLE [dbo].[Users]
                            ADD [Password] NVARCHAR(MAX) NULL;
                            
                            PRINT 'Password column recreated in Users table';
                        END
                        ELSE
                        BEGIN
                            -- Add the Password column if it doesn't exist
                            ALTER TABLE [dbo].[Users]
                            ADD [Password] NVARCHAR(MAX) NULL;
                            
                            PRINT 'Password column added to Users table';
                        END
                    END
                    ELSE
                    BEGIN
                        PRINT 'Users table does not exist';
                    END",
                    
                    // Add entry to __EFMigrationsHistory table to record these changes
                    @"IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20240524_AddMissingColumns')
                    BEGIN
                        INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                        VALUES ('20240524_AddMissingColumns', '8.0.0');
                    END",
                    
                    // Add ApplicationUserId column to Appointments table if it doesn't exist
                    @"IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Appointments')
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = 'ApplicationUserId')
                        BEGIN
                            ALTER TABLE [dbo].[Appointments]
                            ADD [ApplicationUserId] NVARCHAR(450) NULL;
                            PRINT 'ApplicationUserId column added to Appointments table';
                        END
                        ELSE
                        BEGIN
                            PRINT 'ApplicationUserId column already exists in Appointments table';
                        END

                        -- Ensure AttachmentsData column exists
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = 'AttachmentsData')
                        BEGIN
                            ALTER TABLE [dbo].[Appointments]
                            ADD [AttachmentsData] NVARCHAR(MAX) NULL;
                            PRINT 'AttachmentsData column added to Appointments table';
                        END
                        ELSE
                        BEGIN
                            PRINT 'AttachmentsData column already exists in Appointments table';
                        END
                    END
                    ELSE
                    BEGIN
                        PRINT 'Appointments table does not exist';
                    END",
                    
                    // Fix the VitalSigns table by recreating it with proper column types
                    @"BEGIN TRY
                        -- Create a backup of the VitalSigns table if it exists
                        IF EXISTS (SELECT * FROM sys.tables WHERE name = 'VitalSigns')
                        BEGIN
                            -- Check if backup table already exists
                            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VitalSigns_Backup')
                            BEGIN
                                -- Create backup table
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

                            -- Drop the existing table
                            DROP TABLE VitalSigns;
                            PRINT 'Dropped existing VitalSigns table';

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

                            -- Add foreign key constraint
                            ALTER TABLE [VitalSigns]
                            ADD CONSTRAINT [FK_VitalSigns_Patients_PatientId]
                            FOREIGN KEY ([PatientId]) REFERENCES [Patients]([UserId])
                            ON DELETE CASCADE;
                            PRINT 'Added foreign key constraint to VitalSigns table';

                            -- Create index for PatientId
                            CREATE INDEX [IX_VitalSigns_PatientId] ON [VitalSigns]([PatientId]);
                            PRINT 'Created index for PatientId on VitalSigns table';

                            -- Copy data from backup table to new table
                            -- This will convert string values to appropriate types
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
                            PRINT 'Data copied from VitalSigns_Backup to VitalSigns';
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

                            -- Add foreign key constraint
                            ALTER TABLE [VitalSigns]
                            ADD CONSTRAINT [FK_VitalSigns_Patients_PatientId]
                            FOREIGN KEY ([PatientId]) REFERENCES [Patients]([UserId])
                            ON DELETE CASCADE;
                            PRINT 'Added foreign key constraint to VitalSigns table';

                            -- Create index for PatientId
                            CREATE INDEX [IX_VitalSigns_PatientId] ON [VitalSigns]([PatientId]);
                            PRINT 'Created index for PatientId on VitalSigns table';
                        END
                    END TRY
                    BEGIN CATCH
                        PRINT 'Error fixing VitalSigns table: ' + ERROR_MESSAGE();
                    END CATCH",

                    // Add missing columns to MedicalRecords table
                    @"IF EXISTS (SELECT * FROM sys.tables WHERE name = 'MedicalRecords')
                    BEGIN
                        -- Add ApplicationUserId column if it doesn't exist
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[MedicalRecords]') AND name = 'ApplicationUserId')
                        BEGIN
                            ALTER TABLE [dbo].[MedicalRecords]
                            ADD [ApplicationUserId] NVARCHAR(450) NULL;
                            PRINT 'ApplicationUserId column added to MedicalRecords table';
                        END

                        -- Ensure Medications column exists
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[MedicalRecords]') AND name = 'Medications')
                        BEGIN
                            ALTER TABLE [dbo].[MedicalRecords]
                            ADD [Medications] NVARCHAR(1000) NULL;
                            PRINT 'Medications column added to MedicalRecords table';
                        END

                        -- Ensure RecordDate column exists
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[MedicalRecords]') AND name = 'RecordDate')
                        BEGIN
                            ALTER TABLE [dbo].[MedicalRecords]
                            ADD [RecordDate] DATETIME2 NOT NULL DEFAULT GETDATE();
                            PRINT 'RecordDate column added to MedicalRecords table';
                        END
                    END
                    ELSE
                    BEGIN
                        PRINT 'MedicalRecords table does not exist';
                    END"
                };

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Connected to database. Executing SQL commands...");

                    foreach (var sql in sqlCommands)
                    {
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            try
                            {
                                await command.ExecuteNonQueryAsync();
                                Console.WriteLine("SQL command executed successfully.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error executing SQL command: {ex.Message}");
                            }
                        }
                    }

                    Console.WriteLine("Database fix completed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fixing database: {ex.Message}");
            }
        }
    }
} 