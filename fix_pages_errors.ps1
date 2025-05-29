# PowerShell script to fix specific date-related errors in Pages and Models

Write-Host "Starting page-specific fixes..." -ForegroundColor Green

# Function to apply fixes to a file
function Apply-Fixes {
    param(
        [string]$FilePath
    )
    
    if (Test-Path $FilePath) {
        Write-Host "Processing: $FilePath" -ForegroundColor Cyan
        $content = Get-Content -Path $FilePath -Raw
        $modified = $false
        
        # 1. Fix DateTime.Date errors on strings
        if ($content -match "does not contain a definition for 'Date'") {
            # Handle DateTime usage on string dates
            $content = $content -replace '(DateTime\.[^.]+)\.Date(?!\()', '$1.Date'
            # Handle method group errors for Date on strings
            $content = $content -replace '(\w+(?:\.AppointmentDate|\.RecordDate|\.Date))(?!\(\))(?=\s+(==|!=|>=|<=|>|<))', '$1()'
            $modified = $true
        }
        
        # 2. Fix format provider errors
        if ($content -match "cannot convert from 'string' to 'System\.IFormatProvider") {
            $content = $content -replace '\.ToString\("([^"]+)",\s*"([^"]+)"\)', '.ToString("$1", CultureInfo.GetCultureInfo("$2"))'
            $modified = $true
        }
        
        # 3. Fix DateTime to string conversion errors
        if ($content -match "Cannot implicitly convert type 'System\.DateTime' to 'string'") {
            $content = $content -replace '(\w+)\.AppointmentDate\s*=\s*([^;]+);', '$1.AppointmentDate = $2.ToDateString();'
            $modified = $true
        }
        
        # 4. Fix string to DateTime conversion errors
        if ($content -match "Cannot implicitly convert type 'string' to 'System\.DateTime'") {
            $content = $content -replace '(DateTime\s+\w+\s*=\s*)([^.]+);', '$1$2.ToDateTime();'
            $modified = $true
        }
        
        # 5. Fix operator errors between string and DateTime
        if ($content -match "Operator '(==|!=|>=|<=|>|<)' cannot be applied to operands of type '(string|DateTime)'") {
            $content = $content -replace '(\w+\.(?:AppointmentDate|RecordDate))\s*==\s*(DateTime\.[^;]+)', '$1.IsEqual($2)'
            $content = $content -replace '(\w+\.(?:AppointmentDate|RecordDate))\s*>=\s*(DateTime\.[^;]+)', '$1.IsGreaterThanOrEqual($2)'
            $content = $content -replace '(\w+\.(?:AppointmentDate|RecordDate))\s*<=\s*(DateTime\.[^;]+)', '$1.IsLessThanOrEqual($2)'
            $content = $content -replace '(\w+\.(?:AppointmentDate|RecordDate))\s*>\s*(DateTime\.[^;]+)', '$1.ToDateTime() > $2'
            $content = $content -replace '(\w+\.(?:AppointmentDate|RecordDate))\s*<\s*(DateTime\.[^;]+)', '$1.IsLessThan($2)'
            
            # Also fix DateTime on left side
            $content = $content -replace '(DateTime\.[^;]+)\s*==\s*(\w+\.(?:AppointmentDate|RecordDate))', '$2.IsEqual($1)'
            $modified = $true
        }
        
        # 6. Fix method group assignment errors
        if ($content -match "Cannot assign 'method group' to anonymous type property") {
            $content = $content -replace 'Date\s*=\s*(\w+)\.Date(?!\()', 'Date = $1.Date()'
            $modified = $true
        }
        
        # 7. Fix cannot convert method group to non-delegate type
        if ($content -match "Cannot convert method group '(Date|ToString)' to non-delegate type") {
            $content = $content -replace '(\w+)\.(Date|ToString)(?!\()', '$1.$2()'
            $modified = $true
        }
        
        if ($modified) {
            Set-Content -Path $FilePath -Value $content -NoNewline
            Write-Host "Updated file: $FilePath" -ForegroundColor Green
            return $true
        } else {
            Write-Host "No changes needed for: $FilePath" -ForegroundColor Yellow
            return $false
        }
    } else {
        Write-Host "File not found: $FilePath" -ForegroundColor Red
        return $false
    }
}

# Get problematic files from the build errors
$problemFiles = @(
    "Models/Patient.cs",
    "Models/AppointmentCreateModel.cs",
    "Models/Appointment.cs",
    "Nurse/NurseDashboard.cshtml.cs",
    "Pages/Appointments/Create.cshtml.cs",
    "Pages/Appointments/Details.cshtml",
    "Pages/BookAppointment.cshtml.cs",
    "Pages/Appointment/Book.cshtml.cs",
    "Pages/Admin/AdminDashboard.cshtml.cs",
    "Pages/User/UserDashboard.cshtml",
    "Pages/User/Appointment.cshtml.cs",
    "Pages/Doctor/Dashboard.cshtml.cs",
    "Pages/Doctor/Appointment/Details.cshtml",
    "Pages/Doctor/DoctorDashboard.cshtml.cs",
    "Pages/Nurse/MedicalHistory.cshtml.cs",
    "Pages/Nurse/DiagnoseDB.cshtml.cs",
    "Pages/Doctor/PatientRecords.cshtml",
    "Pages/Doctor/Reports.cshtml.cs",
    "Pages/Nurse/PatientQueue.cshtml.cs",
    "Pages/Nurse/NewAppointment.cshtml.cs",
    "Pages/Doctor/NewPrescription.cshtml.cs",
    "Pages/Doctor/Consultation.cshtml",
    "Pages/Nurse/MedicalHistory.cshtml",
    "Pages/Doctor/Appointments.cshtml"
)

$fixedCount = 0

foreach ($file in $problemFiles) {
    if (Apply-Fixes -FilePath $file) {
        $fixedCount++
    }
}

# Also search all CSHTML files for date format provider errors
$cshtml = Get-ChildItem -Path . -Filter *.cshtml -Recurse | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }
foreach ($file in $cshtml) {
    $content = Get-Content -Path $file.FullName -Raw
    if ($content -match "string' to 'System\.IFormatProvider" -or $content -match "\.ToString\([^)]+,[^)]+\)") {
        if (Apply-Fixes -FilePath $file.FullName) {
            $fixedCount++
        }
    }
}

Write-Host "Fixed $fixedCount files." -ForegroundColor Green
Write-Host "Script completed." -ForegroundColor Green 