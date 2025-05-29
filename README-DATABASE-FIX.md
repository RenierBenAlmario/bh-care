# Database Schema Fix for Barangay Health Center System

This document provides instructions for fixing the database schema issue where the AspNetUsers table is missing the `HasAgreedToTerms` and `AgreedAt` columns, causing a SqlException with error number 207.

## Option 1: Using PowerShell (Recommended)

1. Open PowerShell as Administrator
2. Navigate to the project directory where the scripts are located
3. Run the PowerShell script:

```powershell
.\ApplyAgreedToTermsMigration.ps1
```

This script will:
- Apply the SQL migration to add the missing columns
- Verify the columns were added successfully
- Display the results

## Option 2: Using SQL Server Management Studio (SSMS)

1. Open SQL Server Management Studio
2. Connect to your database server
3. Open the `ApplyAgreedToTermsColumnsMigration.sql` script
4. Execute the script against your database
5. Run the `VerifyDatabaseColumns.sql` script to confirm the changes were applied

## Option 3: Using Entity Framework Migrations

If you prefer to use the standard Entity Framework approach:

1. Open Package Manager Console in Visual Studio
2. Run the following commands:

```powershell
Add-Migration AddAgreedToTermsColumns
Update-Database
```

## Verification

To verify that the fix worked:

1. Run the `VerifyDatabaseColumns.sql` script
2. Restart the application and sign up a new user
3. Check the Admin User Management page to see if the new user appears with "Pending" status
4. Verify that the notification system works correctly

## Troubleshooting

If you still encounter issues:

1. Check the error logs for the exact error message
2. Verify that the database connection string in `appsettings.json` is correct
3. Restart the application to ensure it picks up the schema changes
4. If necessary, run the SQL commands manually:

```sql
ALTER TABLE [AspNetUsers]
ADD [HasAgreedToTerms] bit NOT NULL DEFAULT 0,
    [AgreedAt] datetime2 NULL;
```

## What This Fix Addresses

- Adds missing columns to AspNetUsers table:
  - HasAgreedToTerms (bit, default: 0)
  - AgreedAt (datetime2, nullable)
- Registers the migration in the __EFMigrationsHistory table
- Ensures new user sign-ups are saved with "Pending" status
- Allows admins to view and approve pending users
- Fixes the notification system for pending user approvals 