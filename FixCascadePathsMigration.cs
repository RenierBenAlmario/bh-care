using Microsoft.EntityFrameworkCore.Migrations;

namespace Barangay.Migrations
{
    public partial class FixCascadePathsMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop existing foreign key constraints if they exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Messages_AspNetUsers_SenderId')
                BEGIN
                    ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_SenderId];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Messages_AspNetUsers_ReceiverId')
                BEGIN
                    ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId];
                END
            ");

            // Re-create the foreign key constraints with NO ACTION instead of CASCADE
            migrationBuilder.Sql(@"
                ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_SenderId] 
                    FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId] 
                    FOREIGN KEY ([ReceiverId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
            ");
            
            // Also fix other cascade paths that might be causing issues
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_MedicalRecords_Patients_PatientId' AND delete_referential_action = 1) -- 1 = CASCADE
                BEGIN
                    ALTER TABLE [MedicalRecords] DROP CONSTRAINT [FK_MedicalRecords_Patients_PatientId];
                    ALTER TABLE [MedicalRecords] ADD CONSTRAINT [FK_MedicalRecords_Patients_PatientId] 
                        FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION;
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Prescriptions_AspNetUsers_PatientId' AND delete_referential_action = 1) -- 1 = CASCADE
                BEGIN
                    ALTER TABLE [Prescriptions] DROP CONSTRAINT [FK_Prescriptions_AspNetUsers_PatientId];
                    ALTER TABLE [Prescriptions] ADD CONSTRAINT [FK_Prescriptions_AspNetUsers_PatientId] 
                        FOREIGN KEY ([PatientId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Prescriptions_AspNetUsers_DoctorId' AND delete_referential_action = 1) -- 1 = CASCADE
                BEGIN
                    ALTER TABLE [Prescriptions] DROP CONSTRAINT [FK_Prescriptions_AspNetUsers_DoctorId];
                    ALTER TABLE [Prescriptions] ADD CONSTRAINT [FK_Prescriptions_AspNetUsers_DoctorId] 
                        FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the NO ACTION constraints
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Messages_AspNetUsers_SenderId')
                BEGIN
                    ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_SenderId];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Messages_AspNetUsers_ReceiverId')
                BEGIN
                    ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId];
                END
            ");

            // Note: We don't restore the CASCADE constraints in the Down method to avoid potential issues
        }
    }
} 