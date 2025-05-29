# PowerShell script to fix all date-related errors in the codebase

Write-Host "Starting date error fixes..." -ForegroundColor Green

# Get all .cs files in the project (excluding bin and obj folders)
$files = Get-ChildItem -Path . -Filter *.cs -Recurse | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }

Write-Host "Found $($files.Count) .cs files in the project."

# Patterns to find and replace
$patterns = @(
    # 1. Fix method group errors by adding parentheses to method calls
    @{
        Find = '\.Date\s+=='
        Replace = '.Date() =='
    },
    @{
        Find = '\.Date\s+!='
        Replace = '.Date() !='
    },
    @{
        Find = '\.Date\s+>='
        Replace = '.Date() >='
    },
    @{
        Find = '\.Date\s+<='
        Replace = '.Date() <='
    },
    @{
        Find = '\.Date\s+>'
        Replace = '.Date() >'
    },
    @{
        Find = '\.Date\s+<'
        Replace = '.Date() <'
    },
    
    # 2. Fix assignment of method groups
    @{
        Find = '=\s+(\w+)\.Date([^(])'
        Replace = '= $1.Date()$2'
    },
    
    # 3. Fix DateTime.Date on non-DateTime objects
    @{
        Find = '(DateTime\.[^.]+)\.Date\s'
        Replace = '$1.Date '
    },
    
    # 4. Fix string to DateTime conversion
    @{
        Find = '(\w+)\.AppointmentDate\s+=\s+DateTime\.Today'
        Replace = '$1.AppointmentDate = DateTime.Today.ToDateString()'
    },
    @{
        Find = '(\w+)\.AppointmentDate\s+=\s+appointmentDate'
        Replace = '$1.AppointmentDate = appointmentDate.ToDateString()'
    },
    
    # 5. Fix DateTime to string comparisons
    @{
        Find = '(string\.Compare\()([^,]+),\s*DateTime\.([^)]+)\)'
        Replace = '$1$2, DateTime.$3.ToDateString())'
    },
    
    # 6. Fix direct comparisons between string and DateTime
    @{
        Find = '(\w+\.AppointmentDate)\s+==\s+(DateTime\.[^;]+)'
        Replace = '$1.IsEqual($2)'
    },
    @{
        Find = '(\w+\.AppointmentDate)\s+>=\s+(DateTime\.[^;]+)'
        Replace = '$1.IsGreaterThanOrEqual($2)'
    },
    @{
        Find = '(\w+\.AppointmentDate)\s+<=\s+(DateTime\.[^;]+)'
        Replace = '$1.IsLessThanOrEqual($2)'
    },
    @{
        Find = '(\w+\.AppointmentDate)\s+>\s+(DateTime\.[^;]+)'
        Replace = 'string.Compare($1, $2.ToDateString()) > 0'
    },
    @{
        Find = '(\w+\.AppointmentDate)\s+<\s+(DateTime\.[^;]+)'
        Replace = '$1.IsLessThan($2)'
    },
    
    # 7. Fix format provider issues
    @{
        Find = '\.ToString\("([^"]+)",\s*"([^"]+)"\)'
        Replace = '.ToString("$1", CultureInfo.GetCultureInfo("$2"))'
    }
)

$fileCount = 0
$replacementCount = 0

# Process each file
foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName -Raw
    $original = $content
    $modified = $false
    
    foreach ($pattern in $patterns) {
        if ($content -match $pattern.Find) {
            $content = $content -replace $pattern.Find, $pattern.Replace
            $modified = $true
            $replacementCount++
        }
    }
    
    # Special fixes for DateTime to string assignments
    if ($content -match "Cannot implicitly convert type 'System\.DateTime' to 'string'") {
        $content = $content -replace '(\w+)\.AppointmentDate\s+=\s+(\w+);', '$1.AppointmentDate = $2.ToDateString();'
        $modified = $true
        $replacementCount++
    }

    # Special fixes for MedicalRecord.Date method group errors
    if ($content -match "MedicalRecord.*Date") {
        $content = $content -replace '(\w+\.RecordDate)\.Date', '$1.Date()'
        $modified = $true
        $replacementCount++
    }
    
    if ($modified) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        $fileCount++
        Write-Host "Updated file: $($file.FullName)"
    }
}

Write-Host "Completed processing files." -ForegroundColor Green
Write-Host "Total files modified: $fileCount" -ForegroundColor Cyan
Write-Host "Total replacements made: $replacementCount" -ForegroundColor Cyan

Write-Host "Applying specialized fixes for remaining critical errors..." -ForegroundColor Yellow

# Special targeted fixes for specific files
$specialFixes = @(
    @{
        File = "Pages/Doctor/NewPrescription.cshtml.cs"
        Finds = @(
            @{
                Find = 'DateTime\?.*=\s+(\w+)\.Date;'
                Replace = 'DateTime? = $1.ToDateTime();'
            }
        )
    },
    @{
        File = "Pages/Admin/AdminDashboard.cshtml.cs"
        Finds = @(
            @{
                Find = 'DateTime\s+\w+\s+=\s+(\w+);'
                Replace = 'DateTime $1Date = $1.ToDateTime();'
            }
        )
    },
    @{
        File = "Pages/User/UserDashboard.cshtml"
        Finds = @(
            @{
                Find = '\.ToString\("[^"]+",\s+"[^"]+"\)'
                Replace = '.FormatDate("MM/dd/yyyy", CultureInfo.GetCultureInfo("en-US"))'
            }
        )
    }
)

foreach ($fix in $specialFixes) {
    if (Test-Path $fix.File) {
        $content = Get-Content -Path $fix.File -Raw
        $modified = $false
        
        foreach ($pattern in $fix.Finds) {
            if ($content -match $pattern.Find) {
                $content = $content -replace $pattern.Find, $pattern.Replace
                $modified = $true
            }
        }
        
        if ($modified) {
            Set-Content -Path $fix.File -Value $content -NoNewline
            Write-Host "Applied special fix to: $($fix.File)" -ForegroundColor Magenta
        }
    }
}

Write-Host "Script completed." -ForegroundColor Green 