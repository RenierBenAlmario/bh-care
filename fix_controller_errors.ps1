# PowerShell script to fix specific date-related errors in Controllers

Write-Host "Starting controller-specific fixes..." -ForegroundColor Green

# Files to process
$files = @(
    "Controllers/AppointmentController.cs",
    "Controllers/NewAppointmentController.cs",
    "Controllers/ReportsApiController.cs",
    "Controllers/DoctorApiController.cs",
    "Controllers/NurseApiController.cs",
    "Controllers/UserApiController.cs"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "Processing: $file" -ForegroundColor Cyan
        $content = Get-Content -Path $file -Raw
        $modified = $false

        # Fix specific patterns in controller files
        
        # 1. Fix IsGreaterThanOrEqual and IsLessThanOrEqual calls
        if ($content -match "Argument 1: cannot convert from 'System\.DateTime' to 'string\?'") {
            $content = $content -replace '(IsGreaterThanOrEqual|IsLessThanOrEqual|IsLessThan|IsEqual)\(([^,]+),\s*([^)]+)\)', '$1($3.ToDateString(), $2)'
            $modified = $true
        }
        
        # 2. Fix DateTime to string conversions for AppointmentDate
        if ($content -match "Cannot implicitly convert type 'System\.DateTime' to 'string'") {
            $content = $content -replace '(\w+)\.AppointmentDate\s*=\s*([^;]+);', '$1.AppointmentDate = $2.ToDateString();'
            $modified = $true
        }
        
        # 3. Fix string date to DateTime comparisons
        if ($content -match "Operator '(>=|<=|==|!=|>|<)' cannot be applied to operands of type 'string' and 'DateTime") {
            $content = $content -replace '(\w+\.(?:AppointmentDate|RecordDate))\s*==\s*(DateTime\.[^;]+)', '$1.IsEqual($2)'
            $content = $content -replace '(\w+\.(?:AppointmentDate|RecordDate))\s*>=\s*(DateTime\.[^;]+)', '$1.IsGreaterThanOrEqual($2)'
            $content = $content -replace '(\w+\.(?:AppointmentDate|RecordDate))\s*<=\s*(DateTime\.[^;]+)', '$1.IsLessThanOrEqual($2)'
            $content = $content -replace '(\w+\.(?:AppointmentDate|RecordDate))\s*>\s*(DateTime\.[^;]+)', '$1.ToDateTime() > $2'
            $content = $content -replace '(\w+\.(?:AppointmentDate|RecordDate))\s*<\s*(DateTime\.[^;]+)', '$1.IsLessThan($2)'
            $modified = $true
        }
        
        # 4. Fix method group errors
        if ($content -match "is a method, which is not valid in the given context" -or $content -match "Cannot convert method group") {
            $content = $content -replace '(\w+)\.Date(?!\()', '$1.Date()'
            $modified = $true
        }
        
        # 5. Fix format provider errors
        if ($content -match "cannot convert from 'string' to 'System\.IFormatProvider") {
            $content = $content -replace '\.ToString\("([^"]+)",\s*"([^"]+)"\)', '.ToString("$1", CultureInfo.GetCultureInfo("$2"))'
            $modified = $true
        }
        
        # 6. Fix specific errors in ReportsApiController for date range filtering
        if ($file -eq "Controllers/ReportsApiController.cs") {
            # Fix date range filtering with extension methods
            $dateRangePattern = '(\w+)\.AppointmentDate\s*>=\s*([^&]+)\s*&&\s*\1\.AppointmentDate\s*<=\s*([^)]+)'
            $dateRangeReplacement = '$1.AppointmentDate.IsGreaterThanOrEqual($2) && $1.AppointmentDate.IsLessThanOrEqual($3)'
            $content = $content -replace $dateRangePattern, $dateRangeReplacement
            
            # Fix MedicalRecord.Date method group errors
            $content = $content -replace '(MedicalRecord\.\w+)\.Date(?!\()', '$1.Date()'
            $modified = $true
        }
        
        if ($modified) {
            Set-Content -Path $file -Value $content -NoNewline
            Write-Host "Updated file: $file" -ForegroundColor Green
        } else {
            Write-Host "No changes needed for: $file" -ForegroundColor Yellow
        }
    } else {
        Write-Host "File not found: $file" -ForegroundColor Red
    }
}

Write-Host "Script completed." -ForegroundColor Green 