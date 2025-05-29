# Barangay Management System - Database Update Package

This package contains all the scripts needed to update your database to the latest version and fix any database-related issues.

## Quick Start

1. Open Command Prompt as Administrator
2. Navigate to the project directory
3. Run the following command:

```
run-database-update.bat
```

This will:
- Apply all pending database migrations
- Create the missing UserDocuments table
- Add the required columns to AspNetUsers
- Add missing roles to AspNetRoles
- Verify the database structure
- Show detailed results

## What's Included

- **update-database.ps1** - PowerShell script to apply database migrations
- **verify-database-structure.sql** - SQL script to verify database structure
- **create-userdocuments-table.sql** - SQL script to create the missing UserDocuments table
- **fix-missing-columns.sql** - SQL script to add missing columns to AspNetUsers table
- **fix-missing-roles.sql** - SQL script to add required roles to AspNetRoles table
- **run-database-update.bat** - Batch file to run all scripts in the correct order
- **database-migration-summary.md** - Detailed documentation of the update process

## Requirements

- SQL Server (running)
- .NET SDK 8.0 or later
- PowerShell 7.0 or later
- SqlCmd (installed with SQL Server)

## Manual Update Steps

If you prefer to run the steps manually:

1. Apply Entity Framework migrations:
   ```powershell
   dotnet ef database update
   ```

2. Create the UserDocuments table:
   ```
   sqlcmd -S DESKTOP-NU53VS3 -d Barangay -E -i create-userdocuments-table.sql
   ```

3. Add missing columns to AspNetUsers:
   ```
   sqlcmd -S DESKTOP-NU53VS3 -d Barangay -E -i fix-missing-columns.sql
   ```

4. Add missing roles:
   ```
   sqlcmd -S DESKTOP-NU53VS3 -d Barangay -E -i fix-missing-roles.sql
   ```

5. Verify the database structure:
   ```
   sqlcmd -S DESKTOP-NU53VS3 -d Barangay -E -i verify-database-structure.sql
   ```

## Troubleshooting

If you encounter any issues:

1. Make sure SQL Server is running
2. Check that your connection string in appsettings.json is correct
3. Verify you have the necessary permissions to modify the database
4. Run each script individually to identify which step is failing

For detailed information about the database update process, please refer to the [database-migration-summary.md](database-migration-summary.md) file. 