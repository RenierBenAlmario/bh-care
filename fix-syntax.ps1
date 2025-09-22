# PowerShell script to fix corrupted syntax in ImmunizationArchive.cshtml.cs
$content = Get-Content 'Pages/Admin/ImmunizationArchive.cshtml.cs' -Raw

# Fix all "FieldName = ," patterns
$content = $content -replace '(\w+)\s*=\s*,', '$1 = record.$1,'

# Fix any remaining "= ," patterns
$content = $content -replace '=\s*,', '= record.FieldName,'

# Fix any "??" operator issues
$content = $content -replace '=\s*\?\?\s*', '= record.FieldName ?? '

Set-Content 'Pages/Admin/ImmunizationArchive.cshtml.cs' -Value $content
Write-Host "Syntax fixes applied"
