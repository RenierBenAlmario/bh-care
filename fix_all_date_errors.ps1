# Master script to fix all date-related errors in the codebase

Write-Host "==== STARTING COMPREHENSIVE DATE ERROR FIXES ====" -ForegroundColor Cyan

# 1. Fix the DateExtensions.cs file first
Write-Host "===== Step 1: Fixing DateExtensions.cs =====" -ForegroundColor Magenta
$content = Get-Content -Path "Extensions/DateExtensions.cs" -Raw
$content = $content -replace '\.Date\(\) ==', '.Date =='
Set-Content -Path "Extensions/DateExtensions.cs" -Value $content -NoNewline
Write-Host "Fixed DateExtensions.cs" -ForegroundColor Green

# 2. Run general date error fixes
Write-Host "===== Step 2: Running general date error fixes =====" -ForegroundColor Magenta
& .\fix_date_errors.ps1

# 3. Run controller-specific fixes
Write-Host "===== Step 3: Running controller-specific fixes =====" -ForegroundColor Magenta
& .\fix_controller_errors.ps1

# 4. Run page and model fixes
Write-Host "===== Step 4: Running page and model fixes =====" -ForegroundColor Magenta
& .\fix_pages_errors.ps1

# 5. Apply additional specialized fixes
Write-Host "===== Step 5: Applying additional specialized fixes =====" -ForegroundColor Magenta

# Fix specific errors not caught by previous scripts
$specialFixList = @(
    @{
        File = "Models/Appointment.cs"
        Find = "DateTime\.ToString"
        Replace = "DateTime.Now.ToString"
    },
    @{
        File = "Pages/Doctor/NewPrescription.cshtml.cs"
        Find = "DateTime\?\s+=\s+(\w+)"
        Replace = "DateTime? = $1.ToDateTime()"
    }
)

foreach ($fix in $specialFixList) {
    if (Test-Path $fix.File) {
        $content = Get-Content -Path $fix.File -Raw
        $content = $content -replace $fix.Find, $fix.Replace
        Set-Content -Path $fix.File -Value $content -NoNewline
        Write-Host "Applied special fix to: $($fix.File)" -ForegroundColor Green
    }
}

Write-Host "===== Completed all date error fixes =====" -ForegroundColor Cyan
Write-Host "Run 'dotnet build' to verify that all errors have been fixed." -ForegroundColor Yellow 