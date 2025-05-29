using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Barangay
{
    public class CheckDatabase
    {
        public static async Task CheckTables(string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Connected to database. Checking tables...");

                    // Check if AppointmentAttachments table exists
                    string checkAppointmentAttachmentsTable = @"
                        IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AppointmentAttachments')
                            SELECT 'AppointmentAttachments table exists' AS Result;
                        ELSE
                            SELECT 'AppointmentAttachments table does not exist' AS Result;
                    ";

                    using (SqlCommand command = new SqlCommand(checkAppointmentAttachmentsTable, connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        Console.WriteLine(result);
                    }

                    // Check if StaffMembers table has CreatedAt column
                    string checkStaffMembersCreatedAtColumn = @"
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[StaffMembers]') AND name = 'CreatedAt')
                            SELECT 'StaffMembers.CreatedAt column exists' AS Result;
                        ELSE
                            SELECT 'StaffMembers.CreatedAt column does not exist' AS Result;
                    ";

                    using (SqlCommand command = new SqlCommand(checkStaffMembersCreatedAtColumn, connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        Console.WriteLine(result);
                    }

                    // Check if Users table has Password column
                    string checkUsersPasswordColumn = @"
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'Password')
                            SELECT 'Users.Password column exists' AS Result;
                        ELSE
                            SELECT 'Users.Password column does not exist' AS Result;
                    ";

                    using (SqlCommand command = new SqlCommand(checkUsersPasswordColumn, connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        Console.WriteLine(result);
                    }

                    // Check if Appointments table has ApplicationUserId column
                    string checkAppointmentsApplicationUserIdColumn = @"
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = 'ApplicationUserId')
                            SELECT 'Appointments.ApplicationUserId column exists' AS Result;
                        ELSE
                            SELECT 'Appointments.ApplicationUserId column does not exist' AS Result;
                    ";

                    using (SqlCommand command = new SqlCommand(checkAppointmentsApplicationUserIdColumn, connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        Console.WriteLine(result);
                    }

                    // Check if Appointments table has AttachmentsData column
                    string checkAppointmentsAttachmentsDataColumn = @"
                        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = 'AttachmentsData')
                            SELECT 'Appointments.AttachmentsData column exists' AS Result;
                        ELSE
                            SELECT 'Appointments.AttachmentsData column does not exist' AS Result;
                    ";

                    using (SqlCommand command = new SqlCommand(checkAppointmentsAttachmentsDataColumn, connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        Console.WriteLine(result);
                    }

                    Console.WriteLine("Database check completed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking database: {ex.Message}");
            }
        }
    }
} 