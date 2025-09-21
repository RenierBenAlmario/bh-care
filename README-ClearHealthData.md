# Clearing Sample Health Data

This document provides instructions for clearing sample data from the Vital Signs History, Medical History, and Laboratory Results sections of the Barangay Health Care application.

## SQL Script

A SQL script has been created to remove data from the health records tables. The script file is located at:

```
SQL/CleanupSampleData.sql
```

## How to Execute the Script

### Using SQL Server Management Studio

1. Open SQL Server Management Studio
2. Connect to your database server
3. Select the Barangay database
4. Open the SQL/CleanupSampleData.sql file
5. Click the Execute button or press F5

### Using the Command Line

If you have sqlcmd installed, you can run:

```
sqlcmd -S YourServerName -d Barangay -i "SQL/CleanupSampleData.sql"
```

Replace `YourServerName` with your SQL Server instance name.

### Manual Execution

If you prefer to run the commands manually, you can execute the following SQL statements:

```sql
-- Clear Vital Signs data
DELETE FROM VitalSigns;

-- Clear Medical Records data
DELETE FROM MedicalRecords;
```

## Laboratory Results

The Laboratory Results shown in the application are generated dynamically in code and are not stored in a dedicated database table. They will be refreshed automatically when the application restarts.

## Verification

After running the script, log in to the application and navigate to the Medical Records page. The Vital Signs History and Medical History sections should be empty, showing "No vital signs found" and "No medical history records found" respectively.

The Laboratory Results section may still show sample data as these are generated dynamically in the code. 