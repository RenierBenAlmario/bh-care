# Database Migration and Update Guide

This guide explains how to update your database after making changes to your models or when pulling updates from the repository.

## Quick Start

For a simple update, just run the batch file:

```
run-database-update.bat
```

This will:
1. Run the PowerShell script to apply migrations
2. Verify the database structure
3. Show the verification results

## Manual Process

If you prefer to run the steps manually or need more control:

### Step 1: Apply Migrations

Run the PowerShell script to apply migrations:

```powershell
.\update-database.ps1
```

This script will:
- Check if EF Core tools are installed and install them if needed
- Verify existing migrations
- Optionally create a database backup
- Apply all pending migrations
- Verify the database integrity

### Step 2: Verify Database Structure

Run the SQL verification script:

```sql
sqlcmd -S YOUR_SERVER -d Barangay -E -i verify-database-structure.sql -o results.txt
```

Replace `YOUR_SERVER` with your SQL Server instance name (default is DESKTOP-NU53VS3).

## Troubleshooting

If you encounter any issues:

1. **Connection String**: Verify your connection string in `appsettings.json` is correct
   - Current connection string: `Server=DESKTOP-NU53VS3;Database=Barangay;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True`
   - Update the server name if needed

2. **SQL Server**: Ensure SQL Server is running
   ```powershell
   # Check if SQL Server is running
   Get-Service MSSQLSERVER
   
   # Start SQL Server if needed
   Start-Service MSSQLSERVER
   ```

3. **EF Core Tools**: Make sure the tools are installed
   ```
   dotnet tool install --global dotnet-ef
   ```

4. **Specific Migration Issues**: If a particular migration is causing problems, you can target a specific migration:
   ```
   dotnet ef database update MIGRATION_NAME
   ```

## Manual Database Fixes

If you need to manually fix database issues, you can run custom SQL scripts:

1. Fix NULL values in UserDocuments table:
   ```sql
   UPDATE [UserDocuments] SET [FileName] = '' WHERE [FileName] IS NULL;
   UPDATE [UserDocuments] SET [FilePath] = '' WHERE [FilePath] IS NULL;
   UPDATE [UserDocuments] SET [Status] = 'Pending' WHERE [Status] IS NULL;
   ```

2. Fix cascade delete behavior:
   ```sql
   -- Example: Change a relationship to NO ACTION
   ALTER TABLE [TableName] 
   DROP CONSTRAINT [FK_Constraint_Name];
   
   ALTER TABLE [TableName]
   ADD CONSTRAINT [FK_Constraint_Name] 
   FOREIGN KEY ([ColumnName]) REFERENCES [ReferencedTable]([ReferencedColumn])
   ON DELETE NO ACTION;
   ```

## Contact

If you encounter any issues with the database update process or need assistance, please contact the system administrator. 