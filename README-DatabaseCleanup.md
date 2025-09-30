# Database Cleanup Instructions

This guide explains how to clean your BHCARE database using terminal commands while preserving the system admin account.

## ⚠️ WARNING

**This operation will permanently delete ALL data from your database except the system admin account (admin@example.com).**

The following data will be removed:
- All user accounts (except admin@example.com)
- All patient records and medical data
- All appointments and consultations
- All forms and assessments
- All staff and doctor records
- All notifications and messages
- All immunization records
- All health reports

**This action cannot be undone!**

## Prerequisites

1. **SQL Server Command Line Tools (sqlcmd)** must be installed
2. **PowerShell** (for PowerShell script)
3. **Database access** with the credentials from your appsettings.json

## Method 1: Using Batch File (Windows)

1. Open Command Prompt or PowerShell as Administrator
2. Navigate to your project directory
3. Run the batch file:
   ```cmd
   cleanup-database.bat
   ```
4. Type `yes` when prompted to confirm
5. Wait for the cleanup to complete
6. Check `cleanup-log.txt` for detailed output

## Method 2: Using PowerShell Script

1. Open PowerShell as Administrator
2. Navigate to your project directory
3. Run the PowerShell script:
   ```powershell
   .\cleanup-database.ps1
   ```
4. Type `yes` when prompted to confirm
5. Wait for the cleanup to complete
6. Check `cleanup-log.txt` for detailed output

## Method 3: Direct SQL Command

If you prefer to run the SQL script directly:

```cmd
sqlcmd -S "tcp:bhcare.database.windows.net,1433" -d "bhcareDB" -U "bhcare" -P "Thebenzzz10" -i "SQL\force-cleanup.sql"
```

## Method 4: Using SQL Server Management Studio

1. Open SQL Server Management Studio
2. Connect to your database server
3. Open the file `SQL\force-cleanup.sql`
4. Execute the script (F5)

## What Gets Preserved

- **System Admin Account**: admin@example.com
- **Admin Password**: Admin@123
- **Admin Roles**: Admin and User roles
- **Database Schema**: All table structures remain intact

## After Cleanup

1. **Login with admin account**:
   - Email: admin@example.com
   - Password: Admin@123

2. **Verify cleanup**:
   - Check that no other users exist
   - Verify all patient data is gone
   - Confirm appointments are cleared

3. **Start fresh**:
   - Add new staff members
   - Register new patients
   - Create new appointments

## Troubleshooting

### Common Issues

1. **"sqlcmd is not recognized"**
   - Install SQL Server Command Line Tools
   - Add sqlcmd to your system PATH

2. **Connection failed**
   - Verify database credentials in appsettings.json
   - Check network connectivity
   - Ensure firewall allows SQL Server connections

3. **Permission denied**
   - Run terminal as Administrator
   - Verify database user has sufficient permissions

### Log Files

- **cleanup-log.txt**: Contains detailed output from the SQL script
- Check this file if cleanup fails for error details

## Safety Features

The cleanup script includes several safety features:

1. **Admin Account Verification**: Checks that admin account exists before starting
2. **Transaction Rollback**: Automatically rolls back if any error occurs
3. **Foreign Key Handling**: Temporarily disables constraints during cleanup
4. **Verification Step**: Confirms admin account integrity after cleanup

## Recovery

If something goes wrong:

1. **Check the log file** for specific error messages
2. **Restore from backup** if you have one
3. **Re-run the admin account creation script** if needed

## Support

If you encounter issues:

1. Check the `cleanup-log.txt` file for error details
2. Verify your database connection settings
3. Ensure you have the necessary permissions
4. Contact your system administrator if needed

---

**Remember**: Always backup your database before running any cleanup operations!
