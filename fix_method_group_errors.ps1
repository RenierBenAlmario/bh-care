# PowerShell script to fix method group errors

# Get all .cs files in the project (excluding bin and obj folders)
$files = Get-ChildItem -Path . -Filter *.cs -Recurse | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }

Write-Host "Found $($files.Count) .cs files in the project."

# Patterns to find and replace
$patterns = @(
    # Fix method group errors by adding parentheses to method calls
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
    # Fix DateTime to string conversion
    @{
        Find = 'AppointmentDate\s+=\s+DateTime\.'
        Replace = 'AppointmentDate = DateTime.'
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
    
    if ($modified) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        $fileCount++
        Write-Host "Updated file: $($file.FullName)"
    }
}

Write-Host "Completed processing files."
Write-Host "Total files modified: $fileCount"
Write-Host "Total replacements made: $replacementCount"

# Now fix specific DateTime to string conversion issues
$dateTimeToStringFiles = @(
    "Controllers/AppointmentController.cs",
    "Controllers/NewAppointmentController.cs"
)

foreach ($file in $dateTimeToStringFiles) {
    if (Test-Path $file) {
        Write-Host "Processing special file: $file"
        $content = Get-Content -Path $file -Raw
        
        # Fix DateTime.Today to string conversion
        $content = $content -replace 'DateTime\.Today\.ToString\("yyyy-MM-dd"\)', 'DateTime.Today.ToDateString()'
        
        # Fix DateTime to string in AppointmentDate assignments
        $content = $content -replace 'AppointmentDate\s+=\s+appointmentDate;', 'AppointmentDate = appointmentDate.ToDateString();'
        
        Set-Content -Path $file -Value $content -NoNewline
        Write-Host "Updated special file: $file"
    } else {
        Write-Host "Special file not found: $file"
    }
}

Write-Host "Script completed." 