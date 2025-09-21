# PowerShell script to fix merge conflicts by choosing HEAD version
$ErrorActionPreference = "Stop"

function Fix-MergeConflicts {
    param (
        [string]$FilePath
    )

    try {
        # Read file content
        $content = Get-Content -Path $FilePath -Raw -Encoding UTF8

        # Check if there are merge conflicts
        if ($content -match '<<<<<<< HEAD') {
            Write-Host "Fixing conflicts in $FilePath"

            # Pattern 1: Standard merge conflict blocks
            $pattern1 = '<<<<<<< HEAD(.*?)=======.*?>>>>>>> ada8bb23e45c309a5b25bf64f9efebd33211447f'
            $content = [regex]::Replace($content, $pattern1, '$1', [System.Text.RegularExpressions.RegexOptions]::Singleline)

            # Pattern 2: Just in case there are variations in the markers
            $pattern2 = '<<<<<<< .*?HEAD(.*?)=======.*?>>>>>>> .*?'
            $content = [regex]::Replace($content, $pattern2, '$1', [System.Text.RegularExpressions.RegexOptions]::Singleline)

            # Write the new content back to the file
            Set-Content -Path $FilePath -Value $content -Encoding UTF8
            Write-Host "  - Fixed successfully"
            return $true
        }
        return $false
    }
    catch {
        Write-Host "Error processing $FilePath`: $_" -ForegroundColor Red
        return $false
    }
}

# Find all .cs files
Write-Host "Finding files with merge conflicts..."
$files = Get-ChildItem -Path . -Filter *.cs -Recurse

# Count of files with conflicts
$conflictCount = 0
$fixedCount = 0

# Fix conflicts in each file
foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName -Raw -ErrorAction SilentlyContinue
    if ($content -and $content -match '<<<<<<< HEAD') {
        $conflictCount++
        $isFixed = Fix-MergeConflicts -FilePath $file.FullName
        if ($isFixed) {
            $fixedCount++
        }
    }
}

Write-Host "Found $conflictCount files with merge conflicts"
Write-Host "Successfully fixed $fixedCount files"

# Check if any migrations file has conflicts - these are particularly problematic
$migrationsFiles = Get-ChildItem -Path .\Migrations -Filter *.cs -Recurse
foreach ($file in $migrationsFiles) {
    $content = Get-Content -Path $file.FullName -Raw -ErrorAction SilentlyContinue
    if ($content -and $content -match '<<<<<<< HEAD') {
        Write-Host "Warning: Migration file still has conflicts: $($file.FullName)" -ForegroundColor Yellow
    }
}

Write-Host "Done!" 