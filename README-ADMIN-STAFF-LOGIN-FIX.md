# Admin Staff Login Fix

**Date:** May 22, 2025, 01:26 AM PST

## Issue Description

Admin staff users (e.g., 123@example.com) are unable to log in despite having the correct password, receiving an error message: "the account has not have user role." The database shows they have the admin_staff role assigned, but the login system doesn't recognize it.

## Changes Made

1. **Updated Login Role Redirection (Pages/Account/Login.cshtml.cs):**
   - Added specific handling for the "Admin Staff" role
   - Redirected admin staff users to /AdminStaff/Dashboard

2. **Updated Index Page Login Logic (Pages/Index.cshtml.cs):**
   - Added the same role-based redirection for "Admin Staff" role
   - Ensured consistent behavior between direct login and homepage login

3. **SQL Fix Script (scripts/fix-admin-staff-role.sql):**
   - Created a script to check if the user exists
   - Verifies the "Admin Staff" role exists and creates it if missing
   - Assigns the "Admin Staff" role to the user if needed
   - Updates user account to be active and verified

## How to Apply the Fix

1. **Update Code Files:**
   - Deploy the updated Login.cshtml.cs and Index.cshtml.cs files to your server

2. **Run the SQL Fix Script:**
   - Option 1: Execute the batch file by double-clicking `run-fix-admin-staff-role.bat`
   - Option 2: Run the SQL script directly in SQL Server Management Studio:
     ```
     sqlcmd -S localhost -d BarangayHealthCare -E -i scripts/fix-admin-staff-role.sql
     ```

3. **Restart the Application:**
   - Restart the ASP.NET Core application to apply all changes

## Testing

1. **Attempt to log in as an admin staff user:**
   - Username: 123@example.com
   - Verify successful login and redirection to /AdminStaff/Dashboard

2. **Check the logs:**
   - Verify log entries showing "Redirecting Admin Staff user to dashboard"

## Troubleshooting

If issues persist:

1. **Verify database connection:**
   - Check connection string in appsettings.json

2. **Check logs for errors:**
   - Look for specific error messages related to role validation

3. **Manual database check:**
   - Run this SQL query to verify role assignment:
     ```sql
     SELECT u.email, r.name 
     FROM AspNetUsers u 
     JOIN AspNetUserRoles ur ON u.Id = ur.UserId 
     JOIN AspNetRoles r ON r.Id = ur.RoleId 
     WHERE u.Email = '123@example.com';
     ``` 