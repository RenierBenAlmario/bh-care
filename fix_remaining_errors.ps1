# Script to fix remaining date-related errors

Write-Host "Starting fixes for remaining date errors..." -ForegroundColor Green

# 1. Fix format provider errors in CSHTML files
Write-Host "Fixing format provider errors in .cshtml files..." -ForegroundColor Cyan
$cshtml_files = Get-ChildItem -Path . -Filter "*.cshtml" -Recurse | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }

foreach ($file in $cshtml_files) {
    $content = Get-Content -Path $file.FullName -Raw
    $modified = $false

    # Fix string culture name to IFormatProvider
    if ($content -match '\.ToString\("([^"]+)",\s*"([^"]+)"\)') {
        $content = $content -replace '\.ToString\("([^"]+)",\s*"([^"]+)"\)', '.ToString("$1", FormatProviderExtensions.GetProvider("$2"))'
        $modified = $true
    }

    if ($modified) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  Updated file: $($file.FullName)" -ForegroundColor Green
    }
}

# 2. Fix AppointmentController issues 
Write-Host "Fixing AppointmentController and NewAppointmentController issues..." -ForegroundColor Cyan
$appointment_controllers = @(
    "Controllers/AppointmentController.cs",
    "Controllers/NewAppointmentController.cs"
)

foreach ($controller in $appointment_controllers) {
    if (Test-Path $controller) {
        $content = Get-Content -Path $controller -Raw
        
        # Fix DateTime to string parameter conversion
        $content = $content -replace 'model\.Date\.IsGreaterThanOrEqual\((DateTime\.[^)]+)\)', '$1.IsLessThanOrEqual(model.Date)'
        $content = $content -replace 'model\.Date\.IsLessThanOrEqual\((DateTime\.[^)]+)\)', '$1.IsGreaterThanOrEqual(model.Date)'
        
        # Fix DateTime to string conversion for AppointmentDate assignment
        $content = $content -replace '(\w+)\.AppointmentDate\s*=\s*(model\.[^;]+);', '$1.AppointmentDate = $2;'
        
        Set-Content -Path $controller -Value $content -NoNewline
        Write-Host "  Updated file: $controller" -ForegroundColor Green
    }
}

# 3. Fix string to DateTime conversions in page models
Write-Host "Fixing string to DateTime conversions in page models..." -ForegroundColor Cyan
$page_models = @(
    "Pages/Admin/AdminDashboard.cshtml.cs",
    "Pages/Nurse/PatientQueue.cshtml.cs",
    "Pages/Doctor/NewPrescription.cshtml.cs"
)

foreach ($model in $page_models) {
    if (Test-Path $model) {
        $content = Get-Content -Path $model -Raw
        
        # Fix string to DateTime conversion
        $content = $content -replace '(DateTime\s+\w+\s*=\s*)([^.]+);', '$1$2.ToDateTime();'
        $content = $content -replace '(DateTime\?\s+\w+\s*=\s*)([^.]+);', '$1$2.ToNullableDateTime();'
        
        Set-Content -Path $model -Value $content -NoNewline
        Write-Host "  Updated file: $model" -ForegroundColor Green
    }
}

# 4. Fix DateTime to string conversions
Write-Host "Fixing DateTime to string conversions..." -ForegroundColor Cyan
$datetime_to_string_files = @(
    "Pages/Nurse/DiagnoseDB.cshtml.cs",
    "Pages/Nurse/NewAppointment.cshtml.cs",
    "Pages/User/Appointment.cshtml.cs"
)

foreach ($file in $datetime_to_string_files) {
    if (Test-Path $file) {
        $content = Get-Content -Path $file -Raw
        
        # Fix DateTime to string conversion
        $content = $content -replace '(\w+\.[A-Za-z]+Date)\s*=\s*([^.]+);', '$1 = $2.ToDateString();'
        
        Set-Content -Path $file -Value $content -NoNewline
        Write-Host "  Updated file: $file" -ForegroundColor Green
    }
}

# 5. Fix method group errors in comparison expressions
Write-Host "Fixing method group errors in comparisons..." -ForegroundColor Cyan
$method_group_files = @(
    "Pages/Appointments/Create.cshtml.cs",
    "Pages/Appointment/Book.cshtml.cs",
    "Pages/User/Appointment.cshtml.cs",
    "Pages/User/UserDashboard.cshtml.cs"
)

foreach ($file in $method_group_files) {
    if (Test-Path $file) {
        $content = Get-Content -Path $file -Raw
        
        # Fix method group errors in comparisons
        $content = $content -replace '(DateTime\.[^)]+)\s*==\s*(\w+)\.Date(?!\()', '$1.IsEqual($2)'
        $content = $content -replace '(\w+)\.Date(?!\()\s*==\s*(DateTime\.[^)]+)', '$1.Date() == $2.Date'
        $content = $content -replace '([a-zA-Z_][a-zA-Z0-9_]*?)\.Date\s', '$1.Date() '
        
        # Fix method group in string assignment
        $content = $content -replace 'string\s+\w+\s*=\s*(\w+)\.Date(?!\()', 'string $1Date = $1.Date().ToString("yyyy-MM-dd")'
        
        Set-Content -Path $file -Value $content -NoNewline
        Write-Host "  Updated file: $file" -ForegroundColor Green
    }
}

# 6. Fix MedicalRecord.Date issues in Reports
Write-Host "Fixing MedicalRecord.Date issues in Reports..." -ForegroundColor Cyan
$report_files = @(
    "Controllers/ReportsApiController.cs",
    "Pages/Doctor/Reports.cshtml.cs"
)

foreach ($file in $report_files) {
    if (Test-Path $file) {
        $content = Get-Content -Path $file -Raw
        
        # Fix MedicalRecord.Date errors
        $content = $content -replace '(\w+)\.RecordDate\.Date(?!\()', '$1.RecordDate.Date()'
        
        # Fix string date and DateTime? comparisons
        $content = $content -replace '(\w+\.RecordDate)\s*>=\s*(startDate)', '$1.IsGreaterThanOrEqual($2 ?? DateTime.MinValue)'
        $content = $content -replace '(\w+\.RecordDate)\s*<=\s*(endDate)', '$1.IsLessThanOrEqual($2 ?? DateTime.MaxValue)'
        
        Set-Content -Path $file -Value $content -NoNewline
        Write-Host "  Updated file: $file" -ForegroundColor Green
    }
}

# 7. Fix NurseApiController issues
Write-Host "Fixing NurseApiController issues..." -ForegroundColor Cyan
if (Test-Path "Controllers/NurseApiController.cs") {
    $content = Get-Content -Path "Controllers/NurseApiController.cs" -Raw
    
    # Fix string and DateTime comparison
    $content = $content -replace '(\w+\.AppointmentDate)\s*==\s*(DateTime\.[^;]+)', '$1.IsEqual($2)'
    
    # Fix method group assignment
    $content = $content -replace 'Date\s*=\s*([a-zA-Z_][a-zA-Z0-9_]*?)\.Date(?!\()', 'Date = $1.Date()'
    
    Set-Content -Path "Controllers/NurseApiController.cs" -Value $content -NoNewline
    Write-Host "  Updated file: Controllers/NurseApiController.cs" -ForegroundColor Green
}

# 8. Fix DoctorApiController issues
Write-Host "Fixing DoctorApiController issues..." -ForegroundColor Cyan
if (Test-Path "Controllers/DoctorApiController.cs") {
    $content = Get-Content -Path "Controllers/DoctorApiController.cs" -Raw
    
    # Fix string format provider
    $content = $content -replace '\.ToString\("([^"]+)",\s*"([^"]+)"\)', '.ToString("$1", FormatProviderExtensions.GetProvider("$2"))'
    
    # Fix string and DateTime comparison
    $content = $content -replace '(\w+\.AppointmentDate)\s*>=\s*(DateTime\.[^;]+)', '$1.IsGreaterThanOrEqual($2)'
    $content = $content -replace '(\w+\.AppointmentDate)\s*<=\s*(DateTime\.[^;]+)', '$1.IsLessThanOrEqual($2)'
    
    Set-Content -Path "Controllers/DoctorApiController.cs" -Value $content -NoNewline
    Write-Host "  Updated file: Controllers/DoctorApiController.cs" -ForegroundColor Green
}

# 9. Fix Dashboard issues
Write-Host "Fixing Dashboard date comparison issues..." -ForegroundColor Cyan
$dashboard_files = @(
    "Pages/Doctor/Dashboard.cshtml.cs",
    "Pages/User/UserDashboard.cshtml.cs",
    "Pages/Doctor/DoctorDashboard.cshtml.cs"
)

foreach ($file in $dashboard_files) {
    if (Test-Path $file) {
        $content = Get-Content -Path $file -Raw
        
        # Fix string and DateTime comparison
        $content = $content -replace '(\w+\.AppointmentDate)\s*>=\s*(DateTime\.[^;]+)', '$1.IsGreaterThanOrEqual($2)'
        $content = $content -replace '(\w+\.AppointmentDate)\s*<=\s*(DateTime\.[^;]+)', '$1.IsLessThanOrEqual($2)'
        $content = $content -replace '(\w+\.AppointmentDate)\s*<\s*(DateTime\.[^;]+)', '$1.IsLessThan($2)'
        $content = $content -replace '(\w+\.AppointmentDate)\s*>\s*(DateTime\.[^;]+)', '$1.ToDateTime() > $2'
        
        # Fix operator issues
        $content = $content -replace '(DateTime\.[^)]+)\s*&&\s*(true|false)', '($1 != DateTime.MinValue && $2)'
        $content = $content -replace '(DateTime\.[^)]+)\s*\|\|\s*(true|false)', '($1 == DateTime.MinValue || $2)'
        
        Set-Content -Path $file -Value $content -NoNewline
        Write-Host "  Updated file: $file" -ForegroundColor Green
    }
}

Write-Host "Completed fixing remaining date errors." -ForegroundColor Green
Write-Host "Run 'dotnet build' to verify that all errors have been fixed." -ForegroundColor Yellow 