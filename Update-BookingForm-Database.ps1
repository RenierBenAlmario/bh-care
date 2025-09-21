# Update-BookingForm-Database.ps1
# Script to update the database with booking form changes

# Get the connection string from appsettings.json
$appSettingsPath = Join-Path $PSScriptRoot "appsettings.json"
$connectionString = (Get-Content $appSettingsPath -Raw | ConvertFrom-Json).ConnectionStrings.DefaultConnection

# Check if the connection string is valid
if (-not $connectionString) {
    Write-Error "Connection string not found in appsettings.json"
    exit 1
}

Write-Host "Using connection string from appsettings.json" -ForegroundColor Cyan

# Path to the SQL script
$sqlScriptPath = Join-Path $PSScriptRoot "update-database-booking-form.sql"

if (-not (Test-Path $sqlScriptPath)) {
    Write-Error "SQL script not found at: $sqlScriptPath"
    exit 1
}

Write-Host "Found SQL script at: $sqlScriptPath" -ForegroundColor Green

# Execute the SQL script using Invoke-Sqlcmd
try {
    Write-Host "Executing SQL script..." -ForegroundColor Yellow
    
    # Extract server and database name from connection string
    $pattern = "Server=(.*?);Database=(.*?)(?:;|$)"
    $match = [regex]::Match($connectionString, $pattern)
    
    if ($match.Success) {
        $server = $match.Groups[1].Value
        $database = $match.Groups[2].Value
        
        Write-Host "Server: $server" -ForegroundColor Cyan
        Write-Host "Database: $database" -ForegroundColor Cyan
        
        # Check if sqlcmd is available
        $sqlcmd = Get-Command sqlcmd -ErrorAction SilentlyContinue
        
        if ($sqlcmd) {
            # Execute using sqlcmd
            sqlcmd -S $server -d $database -i $sqlScriptPath -E
        }
        else {
            # Alternative: Use Invoke-Sqlcmd if available (requires SqlServer module)
            $sqlServerModule = Get-Module -ListAvailable -Name SqlServer
            
            if ($sqlServerModule) {
                Import-Module SqlServer
                Invoke-Sqlcmd -ServerInstance $server -Database $database -InputFile $sqlScriptPath -QueryTimeout 120
            }
            else {
                # Try using System.Data.SqlClient as a last resort
                Write-Host "Using .NET SqlClient to execute the script..." -ForegroundColor Yellow
                
                $sqlConnection = New-Object System.Data.SqlClient.SqlConnection
                $sqlConnection.ConnectionString = $connectionString
                
                $sqlCommand = New-Object System.Data.SqlClient.SqlCommand
                $sqlCommand.Connection = $sqlConnection
                $sqlCommand.CommandTimeout = 120
                
                $sqlCommand.CommandText = Get-Content $sqlScriptPath -Raw
                
                try {
                    $sqlConnection.Open()
                    $sqlCommand.ExecuteNonQuery()
                }
                finally {
                    $sqlConnection.Close()
                }
            }
        }
        
        Write-Host "SQL script executed successfully!" -ForegroundColor Green
    }
    else {
        Write-Error "Could not parse server and database from connection string"
        exit 1
    }
}
catch {
    Write-Error "Error executing SQL script: $_"
    exit 1
}

# Create an EF Core migration for the booking form changes
Write-Host "Creating EF Core migration for booking form changes..." -ForegroundColor Yellow

try {
    # Check if dotnet ef is installed
    $dotnetEf = dotnet ef --version 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        # Run dotnet ef migrations add
        $timestamp = Get-Date -Format "yyyyMMddHHmmss"
        $migrationName = "BookingFormUpdates_$timestamp"
        
        dotnet ef migrations add $migrationName
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Migration created successfully: $migrationName" -ForegroundColor Green
            
            # Apply the migration
            Write-Host "Applying migration to database..." -ForegroundColor Yellow
            dotnet ef database update
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "Migration applied successfully!" -ForegroundColor Green
            }
            else {
                Write-Error "Failed to apply migration"
            }
        }
        else {
            Write-Error "Failed to create migration"
        }
    }
    else {
        Write-Warning "dotnet ef tools not found. Installing..."
        dotnet tool install --global dotnet-ef
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "dotnet ef tools installed successfully. Please run this script again." -ForegroundColor Green
            exit 0
        }
        else {
            Write-Error "Failed to install dotnet ef tools"
            exit 1
        }
    }
}
catch {
    Write-Error "Error creating or applying migration: $_"
    exit 1
}

Write-Host "Database update completed successfully!" -ForegroundColor Green 