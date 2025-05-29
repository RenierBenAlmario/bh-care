# PowerShell script to apply the migration using sqlcmd
# Make sure you have sqlcmd installed and in your PATH

# Database connection parameters - update these with your actual connection details
$server = "(localdb)\MSSQLLocalDB"  # Or your SQL Server instance name
$database = "BarangayHealthCenter"  # Your database name
$sqlFilePath = ".\ApplyAgreedToTermsColumnsMigration.sql"

try {
    # Run the SQL script
    Write-Host "Applying migration 20250511120000_AddAgreedToTermsColumns to database $database on $server" -ForegroundColor Yellow
    Invoke-Sqlcmd -ServerInstance $server -Database $database -InputFile $sqlFilePath -Verbose -ErrorAction Stop
    
    # Verify columns were added successfully
    $checkColumns = @"
    SELECT COLUMN_NAME, DATA_TYPE 
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'AspNetUsers' 
    AND COLUMN_NAME IN ('HasAgreedToTerms', 'AgreedAt');
"@
    
    $columns = Invoke-Sqlcmd -ServerInstance $server -Database $database -Query $checkColumns
    
    if ($columns.Count -eq 2) {
        Write-Host "Migration applied successfully! Columns added:" -ForegroundColor Green
        $columns | Format-Table
    } else {
        Write-Host "Warning: Not all columns were found after migration. Please check manually." -ForegroundColor Yellow
        $columns | Format-Table
    }
} 
catch {
    Write-Host "Error applying migration: $_" -ForegroundColor Red
    
    # Provide alternative SQL commands
    Write-Host "`nIf sqlcmd is not available, run these SQL commands directly in SQL Server Management Studio:" -ForegroundColor Yellow
    Get-Content $sqlFilePath | Write-Host
    
    exit 1
} 