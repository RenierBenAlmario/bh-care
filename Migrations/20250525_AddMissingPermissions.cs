using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Barangay.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add missing permissions for different roles
            
            // Nurse permissions
            AddPermissionIfNotExists(migrationBuilder, "ManageAppointments", "Ability to manage appointments", "Appointment Management");
            AddPermissionIfNotExists(migrationBuilder, "Access Nurse Dashboard", "Access to the nurse dashboard", "Dashboard Access");
            AddPermissionIfNotExists(migrationBuilder, "Record Vital Signs", "Ability to record patient vital signs", "Medical Records");
            AddPermissionIfNotExists(migrationBuilder, "View Patient History", "Ability to view patient medical history", "Medical Records");
            AddPermissionIfNotExists(migrationBuilder, "Manage Medical Records", "Ability to manage patient medical records", "Medical Records");
            
            // Doctor permissions
            AddPermissionIfNotExists(migrationBuilder, "Access Doctor Dashboard", "Access to the doctor dashboard", "Dashboard Access");
            AddPermissionIfNotExists(migrationBuilder, "Create Prescriptions", "Ability to create prescriptions for patients", "Medical Records");
            AddPermissionIfNotExists(migrationBuilder, "View Reports", "Ability to view system reports", "Reporting");
            
            // Admin permissions
            AddPermissionIfNotExists(migrationBuilder, "Manage Users", "Ability to manage system users", "User Management");
            AddPermissionIfNotExists(migrationBuilder, "Access Admin Dashboard", "Access to the admin dashboard", "Dashboard Access");
            AddPermissionIfNotExists(migrationBuilder, "Approve Users", "Ability to approve new user registrations", "User Management");
            AddPermissionIfNotExists(migrationBuilder, "Manage Permissions", "Ability to manage user permissions", "User Management");
            
            // Common permissions
            AddPermissionIfNotExists(migrationBuilder, "Access Dashboard", "Basic access to the system dashboard", "Dashboard Access");
            AddPermissionIfNotExists(migrationBuilder, "View Dashboard", "View the system dashboard", "Dashboard Access");
        }

        private void AddPermissionIfNotExists(MigrationBuilder migrationBuilder, string name, string description, string category)
        {
            migrationBuilder.Sql($@"
                IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = '{name}')
                BEGIN
                    INSERT INTO [Permissions] ([Name], [Description], [Category])
                    VALUES ('{name}', '{description}', '{category}');
                    PRINT 'Added permission: {name}';
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No need to remove permissions in the Down method
        }
    }
} 