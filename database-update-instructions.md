# Database Update Instructions

This guide provides instructions on how to update your database for the Barangay project.

## Option 1: Using Comprehensive SQL Script (Recommended)

We've created a comprehensive SQL script that applies all necessary database fixes in one go:

```bash
# Connect to your SQL Server and run the comprehensive script
sqlcmd -S DESKTOP-NU53VS3 -d Barangay -i complete-database-update.sql -E
```

This script will:
- Add missing columns to AspNetUsers table (MiddleName, Status, Suffix)
- Fix cascade paths issue in Messages table
- Create necessary tables (AppointmentAttachments, AppointmentFiles)
- Add required columns to existing tables
- Create indexes for foreign keys
- Mark all migrations as applied
- Verify the database schema

## Option 2: Using Individual SQL Scripts

We've also created several individual SQL scripts to help you manage your database:

1. **add-missing-columns.sql** - Adds missing columns to AspNetUsers table
2. **fix-cascade-paths.sql** - Fixes foreign key constraints to avoid cascade path issues
3. **mark-migrations-as-applied.sql** - Marks all migrations as applied in the EF Core migrations history table
4. **update-database-schema.sql** - Updates the database schema with necessary changes

To apply these scripts:

```bash
# Connect to your SQL Server and run the scripts
sqlcmd -S DESKTOP-NU53VS3 -d Barangay -i add-missing-columns.sql -E
sqlcmd -S DESKTOP-NU53VS3 -d Barangay -i fix-cascade-paths.sql -E
sqlcmd -S DESKTOP-NU53VS3 -d Barangay -i mark-migrations-as-applied.sql -E
sqlcmd -S DESKTOP-NU53VS3 -d Barangay -i update-database-schema.sql -E
```

## Option 3: Using PowerShell Script (For Development)

For development environments, you can use the PowerShell script:

```powershell
# Run the PowerShell script
.\update-database.ps1
```

This script will:
- Check if Entity Framework Core tools are installed
- Create a backup of your database (if possible)
- Add a new migration with a timestamp
- Update the database
- Verify the migration was applied

## Option 4: Using Batch File (For Development)

Alternatively, you can use the batch file:

```cmd
# Run the batch file
update-database.bat
```

## Option 5: Manual Commands (For Development)

If you prefer to run the commands manually:

```bash
# Install EF Core tools (if not already installed)
dotnet tool install --global dotnet-ef

# Add a new migration
dotnet ef migrations add YourMigrationName

# Update the database
dotnet ef database update
```

## Troubleshooting

If you encounter issues with foreign key constraints or cascade paths:

1. Run the `fix-cascade-paths.sql` script to fix foreign key constraints
2. If you continue to have issues, try the `mark-migrations-as-applied.sql` script to mark all migrations as applied
3. Use the `update-database-schema.sql` script to update the database schema directly
4. For missing columns like 'MiddleName', 'Status', or 'Suffix', run the `add-missing-columns.sql` script

## Current Database Connection

Your current connection string is:
```
Server=DESKTOP-NU53VS3;Database=Barangay;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

Make sure the SQL Server instance is running and accessible. 