# Run this script to apply the SQL script for adding UserNumber column

# Navigate to project directory (this assumes running from project root already)
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptPath

# Navigate to the project directory
Set-Location -Path $projectRoot

# Get connection string from appsettings.json
$appsettingsPath = Join-Path -Path $projectRoot -ChildPath "appsettings.json"
$appsettings = Get-Content -Path $appsettingsPath | ConvertFrom-Json
$connectionString = $appsettings.ConnectionStrings.DefaultConnection

# SQL script path
$sqlFilePath = Join-Path -Path $scriptPath -ChildPath "AddUserNumberColumn.sql"

Write-Host "Applying UserNumber SQL script..." -ForegroundColor Green
Write-Host "Using SQL file: $sqlFilePath" -ForegroundColor Gray

if (Get-Command "sqlcmd" -ErrorAction SilentlyContinue) {
    # Use sqlcmd if available
    Write-Host "Using sqlcmd to execute SQL script..." -ForegroundColor Yellow
    sqlcmd -S "(localdb)\mssqllocaldb" -d "Barangay" -i $sqlFilePath
} 
elseif (Get-Command "Invoke-Sqlcmd" -ErrorAction SilentlyContinue) {
    # Use PowerShell's Invoke-Sqlcmd if available
    Write-Host "Using Invoke-Sqlcmd to execute SQL script..." -ForegroundColor Yellow
    Invoke-Sqlcmd -InputFile $sqlFilePath -ConnectionString $connectionString
}
else {
    # Use dotnet ef dbcontext as fallback
    Write-Host "SqlCmd not found. Using dotnet ef dbcontext execute..." -ForegroundColor Yellow
    $sql = Get-Content -Path $sqlFilePath -Raw
    dotnet ef dbcontext execute "$sql"
}

Write-Host "SQL script executed successfully." -ForegroundColor Green 