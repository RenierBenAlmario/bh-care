# Fix CSS conflict in BHCARE project
Write-Host "Fixing CSS conflicts in BHCARE project..." -ForegroundColor Green

# Step 1: Clean the project's temporary directories
Write-Host "Cleaning temporary directories..." -ForegroundColor Yellow
if (Test-Path "obj") {
    Remove-Item "obj" -Recurse -Force
    Write-Host "Removed 'obj' directory" -ForegroundColor Cyan
}

if (Test-Path "bin") {
    Remove-Item "bin" -Recurse -Force
    Write-Host "Removed 'bin' directory" -ForegroundColor Cyan
}

# Step 2: Modify the .csproj file to remove any duplicate references
$csprojFile = "Barangay.csproj"
Write-Host "Checking project file for conflicting references..." -ForegroundColor Yellow

if (Test-Path $csprojFile) {
    $content = Get-Content $csprojFile -Raw
    
    # Look for and remove any ItemGroup that references external CSS paths
    $pattern = '<ItemGroup>\s*<Content Include="D:\\Imbakan\\Fixer\\BHCARE-main123\\BHCARE-main\\wwwroot\\css\\.*?</ItemGroup>'
    $newContent = $content -replace $pattern, ''
    
    # If changes were made, save the file
    if ($content -ne $newContent) {
        Set-Content -Path $csprojFile -Value $newContent
        Write-Host "Removed external CSS references from project file" -ForegroundColor Green
    } else {
        Write-Host "No external CSS references found in project file" -ForegroundColor Cyan
    }
} else {
    Write-Host "Project file not found: $csprojFile" -ForegroundColor Red
}

# Step 3: Rename duplicate CSS files if needed
$cssFiles = @{
    "admin-dashboard-bhcare.css" = "admin-dashboard-bhcare-new.css";
    "admin-dashboard.css" = "admin-dashboard-new.css"
}

foreach ($file in $cssFiles.Keys) {
    $source = "wwwroot\css\$file"
    $target = "wwwroot\css\$($cssFiles[$file])"
    
    if (Test-Path $source) {
        Write-Host "Renaming $source to $target to avoid conflicts" -ForegroundColor Yellow
        if (Test-Path $target) {
            Remove-Item $target -Force
        }
        Rename-Item -Path $source -NewName $cssFiles[$file]
    }
}

Write-Host "CSS conflict fix completed. Run 'dotnet build' to verify the fix." -ForegroundColor Green 