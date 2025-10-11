# PowerShell script to fix migration history
Write-Host "Fixing Entity Framework migration history..." -ForegroundColor Green

# Get connection string from appsettings.json
$config = Get-Content "appsettings.json" | ConvertFrom-Json
$connectionString = $config.ConnectionStrings.DefaultConnection

Write-Host "Connection String: $($connectionString.Substring(0, 50))..." -ForegroundColor Yellow

# SQL to mark migrations as applied
$sql = @"
-- Mark all existing migrations as applied since the database schema already exists
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__EFMigrationsHistory')
BEGIN
    PRINT 'Marking existing migrations as applied...';
    
    -- List of migrations to mark as applied
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    SELECT MigrationId, ProductVersion FROM (VALUES
        ('20250715101311_InitialCreate', '8.0.0'),
        ('20250730144649_AddAppointmentIdToMedicalRecords', '8.0.0'),
        ('20250803133403_AddUnitPropertyToPrescriptionMedications', '8.0.0'),
        ('20250809100607_AddNCDRiskAssessmentColumns', '8.0.0'),
        ('20250810045502_AddMissingNCDRiskAssessmentColumns', '8.0.0'),
        ('20250909160657_AddUserBarangay', '8.0.0'),
        ('20250912233707_AddNotificationSettings', '8.0.0'),
        ('20250913161343_FixMissingColumns', '8.0.0'),
        ('20250915141549_UpdateVitalSignsToStringColumns', '8.0.0'),
        ('20250915143035_AddEncryptedColumnsToVitalSigns', '8.0.0'),
        ('20250927202147_AddAllMissingNCDRiskAssessmentColumns', '8.0.0'),
        ('20251001110232_AddUserSuspensionSystem', '8.0.0'),
        ('20251001115429_FixDatabaseSchema', '8.0.0'),
        ('20251003101955_ConfigureHEEADSSSColumnTypes_Clean', '8.0.0'),
        ('20251004030323_AddRemainingNCDRiskAssessmentColumns', '8.0.0'),
        ('20251004112318_AddHasStrokeSymptomsColumn', '8.0.0'),
        ('20251004125509_AddMissingHEEADSSSColumnsSafely', '8.0.0'),
        ('20251004130115_AddReferredByColumn', '8.0.0'),
        ('20251004132422_AddEatingHabitsColumns', '8.0.0'),
        ('20251005040757_AddCOPDColumns', '8.0.0')
    ) AS V(MigrationId, ProductVersion)
    WHERE NOT EXISTS (
        SELECT 1 FROM __EFMigrationsHistory h 
        WHERE h.MigrationId = V.MigrationId
    );
    
    PRINT 'Migration history updated successfully.';
END
ELSE
BEGIN
    PRINT 'Migration history table not found.';
END
"@

# Write SQL to temporary file
$sql | Out-File -FilePath "temp_migration_fix.sql" -Encoding UTF8

Write-Host "SQL script created. Attempting to execute..." -ForegroundColor Yellow

# Try to execute using sqlcmd
try {
    # Parse connection string to get server and database
    if ($connectionString -match "Server=([^;]+);.*Initial Catalog=([^;]+);.*User ID=([^;]+);.*Password=([^;]+);") {
        $server = $matches[1]
        $database = $matches[2] 
        $userId = $matches[3]
        $password = $matches[4]
        
        Write-Host "Executing SQL against: $server / $database" -ForegroundColor Yellow
        
        # Execute the SQL
        sqlcmd -S $server -d $database -U $userId -P $password -i "temp_migration_fix.sql"
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Migration history fixed successfully!" -ForegroundColor Green
        } else {
            Write-Host "SQL execution failed with exit code: $LASTEXITCODE" -ForegroundColor Red
        }
    } else {
        Write-Host "Could not parse connection string for sqlcmd" -ForegroundColor Red
    }
} catch {
    Write-Host "Error executing SQL: $($_.Exception.Message)" -ForegroundColor Red
}

# Clean up
Remove-Item "temp_migration_fix.sql" -ErrorAction SilentlyContinue

Write-Host "Script completed." -ForegroundColor Green
