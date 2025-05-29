@echo off
echo ===================================================
echo Comprehensive TimeSpan Error Fixer for Barangay Project
echo ===================================================
echo.

:: Create a backup directory with timestamp
set timestamp=%date:~-4,4%%date:~-10,2%%date:~-7,2%_%time:~0,2%%time:~3,2%%time:~6,2%
set timestamp=%timestamp: =0%
set backupdir=TimeSpanFixes_Backup_%timestamp%

echo Creating backup directory: %backupdir%
mkdir "%backupdir%"
if errorlevel 1 (
    echo Failed to create backup directory.
    goto :error
)

echo.
echo Backing up and fixing files...
echo.

:: Fix Models\Appointment.cs
if exist "Models\Appointment.cs" (
    echo Fixing Models\Appointment.cs
    copy "Models\Appointment.cs" "%backupdir%\Appointment.cs"
    powershell -Command "$content = Get-Content 'Models\Appointment.cs' -Raw; $content = $content -replace 'public TimeSpan AppointmentTime', 'public TimeSpan? AppointmentTime'; $content = $content -replace 'public string PatientId', 'public string? PatientId'; $content = $content -replace 'public string PatientName', 'public string? PatientName'; $content = $content -replace 'public string ReasonForVisit', 'public string? ReasonForVisit'; $content = $content -replace 'public string DoctorId', 'public string? DoctorId'; $content = $content -replace 'public Doctor Doctor', 'public Doctor? Doctor'; $content = $content -replace 'public Patient Patient', 'public Patient? Patient'; $content = $content -replace 'public string Description', 'public string? Description'; $content = $content -replace 'public string AttachmentPath', 'public string? AttachmentPath'; $content = $content -replace 'public string Prescription', 'public string? Prescription'; $content = $content -replace 'public string Instructions', 'public string? Instructions'; Set-Content 'Models\Appointment.cs' $content"
)

:: Fix Controllers\Api\DoctorsController.cs
:: Fix Controllers\Api\DoctorsController.cs - Enhanced fix
if exist "Controllers\Api\DoctorsController.cs" (
    echo Fixing Controllers\Api\DoctorsController.cs
    copy "Controllers\Api\DoctorsController.cs" "%backupdir%\Api_DoctorsController.cs"
    powershell -Command "$content = Get-Content 'Controllers\Api\DoctorsController.cs' -Raw; $content = $content -replace 's\.MaxDailyPatients\?\.Value \?\? 8', '(s.MaxDailyPatients != null ? s.MaxDailyPatients.Value : 8)'; $content = $content -replace 'a\.AppointmentTime\.HasValue\(\)', 'a.AppointmentTime != null'; $content = $content -replace 'a\.AppointmentTime\.Value', 'a.AppointmentTime.Value'; $content = $content -replace '\.HasValue\(', ' != null'; Set-Content 'Controllers\Api\DoctorsController.cs' $content"
)

:: Add specific fixes for DoctorApiController.cs
if exist "Controllers\DoctorApiController.cs" (
    echo Fixing Controllers\DoctorApiController.cs
    copy "Controllers\DoctorApiController.cs" "%backupdir%\DoctorApiController.cs"
    powershell -Command "$content = Get-Content 'Controllers\DoctorApiController.cs' -Raw; $content = $content -replace '\.HasValue\(', ' != null'; $content = $content -replace '\.AppointmentTime == \"', '.AppointmentTime?.ToString() == \"'; $content = $content -replace 'appointment\.AppointmentTime([^.?])', 'appointment.AppointmentTime?.ToString()$1'; Set-Content 'Controllers\DoctorApiController.cs' $content"
)

:: Add specific fixes for NurseApiController.cs
if exist "Controllers\NurseApiController.cs" (
    echo Fixing Controllers\NurseApiController.cs
    copy "Controllers\NurseApiController.cs" "%backupdir%\NurseApiController.cs"
    powershell -Command "$content = Get-Content 'Controllers\NurseApiController.cs' -Raw; $content = $content -replace '\.HasValue\(', ' != null'; $content = $content -replace '\.AppointmentTime == \"', '.AppointmentTime?.ToString() == \"'; $content = $content -replace 'appointment\.AppointmentTime([^.?])', 'appointment.AppointmentTime?.ToString()$1'; Set-Content 'Controllers\NurseApiController.cs' $content"
)

:: Add specific fixes for AppointmentsController.cs
if exist "Controllers\AppointmentsController.cs" (
    echo Fixing Controllers\AppointmentsController.cs
    copy "Controllers\AppointmentsController.cs" "%backupdir%\AppointmentsController.cs"
    powershell -Command "$content = Get-Content 'Controllers\AppointmentsController.cs' -Raw; $content = $content -replace '\.HasValue\(', ' != null'; $content = $content -replace '(string timeString = )appointment\.AppointmentTime;', '$1appointment.AppointmentTime?.ToString() ?? string.Empty;'; $content = $content -replace 'appointment\.AppointmentTime == \"', 'appointment.AppointmentTime?.ToString() == \"'; Set-Content 'Controllers\AppointmentsController.cs' $content"
)

:: Add specific fixes for DoctorsController.cs
if exist "Controllers\DoctorsController.cs" (
    echo Fixing Controllers\DoctorsController.cs
    copy "Controllers\DoctorsController.cs" "%backupdir%\DoctorsController.cs"
    powershell -Command "$content = Get-Content 'Controllers\DoctorsController.cs' -Raw; $content = $content -replace '\.HasValue\(', ' != null'; $content = $content -replace 'appointment\.AppointmentTime == \"', 'appointment.AppointmentTime?.ToString() == \"'; $content = $content -replace 'TimeSpan\.Parse\(model\.AppointmentTime\)', 'TimeSpan.Parse(model.AppointmentTime ?? \"00:00\")'; Set-Content 'Controllers\DoctorsController.cs' $content"
)

:: Add specific fixes for Book.cshtml.cs
if exist "Pages\Appointment\Book.cshtml.cs" (
    echo Fixing Pages\Appointment\Book.cshtml.cs
    copy "Pages\Appointment\Book.cshtml.cs" "%backupdir%\Book.cshtml.cs"
    powershell -Command "$content = Get-Content 'Pages\Appointment\Book.cshtml.cs' -Raw; $content = $content -replace 'appointment\.AppointmentTime = Input\.AppointmentTime', 'appointment.AppointmentTime = TimeSpan.Parse(Input.AppointmentTime ?? \"00:00\")'; $content = $content -replace '(Input\.AppointmentTime = )appointment\.AppointmentTime;', '$1appointment.AppointmentTime?.ToString() ?? string.Empty;'; $content = $content -replace '\.HasValue\(', ' != null'; if (-not ($content -match 'private async Task LoadFormData')) { $content = $content -replace '(public async Task<IActionResult> OnPostAsync\(\)[^}]*})', '$1\r\n\r\n        private async Task LoadFormData()\r\n        {\r\n            Doctors = await _context.StaffMembers.Where(s => s.Role == \"Doctor\").ToListAsync();\r\n            TimeSlots = new List<string> { \"9:00 AM\", \"10:00 AM\", \"11:00 AM\", \"1:00 PM\", \"2:00 PM\", \"3:00 PM\", \"4:00 PM\" };\r\n        }'; } Set-Content 'Pages\Appointment\Book.cshtml.cs' $content"
)

:: Add specific fixes for UserDashboard.cshtml
if exist "Pages\User\UserDashboard.cshtml" (
    echo Fixing Pages\User\UserDashboard.cshtml
    copy "Pages\User\UserDashboard.cshtml" "%backupdir%\UserDashboard.cshtml"
    powershell -Command "$content = Get-Content 'Pages\User\UserDashboard.cshtml' -Raw; $content = $content -replace '\.HasValue\(', ' != null'; $content = $content -replace 'a\.AppointmentTime\.ToString\(\"h:mm tt\"\)', 'a.AppointmentTime?.ToString(\"h:mm tt\")'; Set-Content 'Pages\User\UserDashboard.cshtml' $content"
)

:: Add specific fixes for AdminDashboard.cshtml.cs
if exist "Pages\Admin\AdminDashboard.cshtml.cs" (
    echo Fixing Pages\Admin\AdminDashboard.cshtml.cs
    copy "Pages\Admin\AdminDashboard.cshtml.cs" "%backupdir%\AdminDashboard.cshtml.cs"
    powershell -Command "$content = Get-Content 'Pages\Admin\AdminDashboard.cshtml.cs' -Raw; $content = $content -replace '\.HasValue\(', ' != null'; $content = $content -replace 'a\.AppointmentTime \?\? TimeSpan\.Zero', 'a.AppointmentTime != null ? a.AppointmentTime.Value : TimeSpan.Zero'; $content = $content -replace 'Operator \"\\?\\?\" cannot be applied to operands of type', '// Fixed nullable TimeSpan issue'; Set-Content 'Pages\Admin\AdminDashboard.cshtml.cs' $content"
)

:: Fix Controllers\AppointmentController.cs
if exist "Controllers\AppointmentController.cs" (
    echo Fixing Controllers\AppointmentController.cs
    copy "Controllers\AppointmentController.cs" "%backupdir%\AppointmentController.cs"
    powershell -Command "$content = Get-Content 'Controllers\AppointmentController.cs' -Raw; $content = $content -replace 'appointment\.AppointmentTime\.HasValue\(\)', 'appointment.AppointmentTime != null'; $content = $content -replace 'appointment\.AppointmentTime\.Value', 'appointment.AppointmentTime.Value'; $content = $content -replace 'appointment\.AppointmentTime == \"', 'appointment.AppointmentTime.ToString() == \"'; $content = $content -replace '(model\.AppointmentTime = )TimeSpan\.Parse\(([^)]+)\)', '$1TimeSpan.Parse($2)'; Set-Content 'Controllers\AppointmentController.cs' $content"
)

:: Fix Controllers\AppointmentsController.cs
if exist "Controllers\AppointmentsController.cs" (
    echo Fixing Controllers\AppointmentsController.cs
    copy "Controllers\AppointmentsController.cs" "%backupdir%\AppointmentsController.cs"
    powershell -Command "$content = Get-Content 'Controllers\AppointmentsController.cs' -Raw; $content = $content -replace 'appointment\.AppointmentTime\.HasValue\(\)', 'appointment.AppointmentTime != null'; $content = $content -replace 'appointment\.AppointmentTime\.Value', 'appointment.AppointmentTime.Value'; $content = $content -replace 'appointment\.AppointmentTime\.ToString\(\"', 'appointment.AppointmentTime?.ToString(\"'; $content = $content -replace '(model\.AppointmentTime = )TimeSpan\.Parse\(([^)]+)\)', '$1TimeSpan.Parse($2)'; $content = $content -replace 'TimeSlots = appointment\.AppointmentTime', 'TimeSlots = new List<string> { appointment.AppointmentTime?.ToString() ?? \"\" }'; Set-Content 'Controllers\AppointmentsController.cs' $content"
)

:: Fix Controllers\DoctorApiController.cs
if exist "Controllers\DoctorApiController.cs" (
    echo Fixing Controllers\DoctorApiController.cs
    copy "Controllers\DoctorApiController.cs" "%backupdir%\DoctorApiController.cs"
    powershell -Command "$content = Get-Content 'Controllers\DoctorApiController.cs' -Raw; $content = $content -replace 'appointment\.AppointmentTime\.HasValue\(\)', 'appointment.AppointmentTime != null'; $content = $content -replace 'appointment\.AppointmentTime\.Value', 'appointment.AppointmentTime.Value'; $content = $content -replace 'appointment\.AppointmentTime', 'appointment.AppointmentTime.Value'; Set-Content 'Controllers\DoctorApiController.cs' $content"
)

:: Fix Controllers\DoctorsController.cs
if exist "Controllers\DoctorsController.cs" (
    echo Fixing Controllers\DoctorsController.cs
    copy "Controllers\DoctorsController.cs" "%backupdir%\DoctorsController.cs"
    powershell -Command "$content = Get-Content 'Controllers\DoctorsController.cs' -Raw; $content = $content -replace 'appointment\.AppointmentTime == \"', 'appointment.AppointmentTime.ToString() == \"'; $content = $content -replace '(model\.AppointmentTime = )TimeSpan\.Parse\(([^)]+)\)', '$1TimeSpan.Parse($2)'; $content = $content -replace 'TimeSpan\.Parse\(model\.AppointmentTime\)', 'model.AppointmentTime.ToString()'; Set-Content 'Controllers\DoctorsController.cs' $content"
)

:: Fix Controllers\NurseApiController.cs
if exist "Controllers\NurseApiController.cs" (
    echo Fixing Controllers\NurseApiController.cs
    copy "Controllers\NurseApiController.cs" "%backupdir%\NurseApiController.cs"
    powershell -Command "$content = Get-Content 'Controllers\NurseApiController.cs' -Raw; $content = $content -replace 'appointment\.AppointmentTime\.HasValue\(\)', 'appointment.AppointmentTime != null'; $content = $content -replace 'appointment\.AppointmentTime\.Value', 'appointment.AppointmentTime.Value'; Set-Content 'Controllers\NurseApiController.cs' $content"
)

:: Fix Pages\User\UserDashboard.cshtml
if exist "Pages\User\UserDashboard.cshtml" (
    echo Fixing Pages\User\UserDashboard.cshtml
    copy "Pages\User\UserDashboard.cshtml" "%backupdir%\UserDashboard.cshtml"
    powershell -Command "$content = Get-Content 'Pages\User\UserDashboard.cshtml' -Raw; $content = $content -replace 'a\.AppointmentTime\.HasValue\(\)', 'a.AppointmentTime != null'; $content = $content -replace 'a\.AppointmentTime\.Value\.ToString\(\"h:mm tt\"\)', 'a.AppointmentTime?.ToString(\"h:mm tt\")'; $content = $content -replace '(a\.AppointmentTime\.ToString\()\"([^\"]+)\"\)', '$1\"$2\")'; Set-Content 'Pages\User\UserDashboard.cshtml' $content"
)

:: Fix Pages\User\UserDashboard.cshtml.cs
if exist "Pages\User\UserDashboard.cshtml.cs" (
    echo Fixing Pages\User\UserDashboard.cshtml.cs
    copy "Pages\User\UserDashboard.cshtml.cs" "%backupdir%\UserDashboard.cshtml.cs"
    powershell -Command "$content = Get-Content 'Pages\User\UserDashboard.cshtml.cs' -Raw; $content = $content -replace 'AppointmentTime = appointment\.AppointmentTime', 'AppointmentTime = appointment.AppointmentTime?.ToString() ?? string.Empty'; Set-Content 'Pages\User\UserDashboard.cshtml.cs' $content"
)

:: Fix Pages\User\Appointment.cshtml.cs
if exist "Pages\User\Appointment.cshtml.cs" (
    echo Fixing Pages\User\Appointment.cshtml.cs
    copy "Pages\User\Appointment.cshtml.cs" "%backupdir%\Appointment.cshtml.cs"
    powershell -Command "$content = Get-Content 'Pages\User\Appointment.cshtml.cs' -Raw; $content = $content -replace 'appointment\.AppointmentTime\.HasValue\(\)', 'appointment.AppointmentTime != null'; $content = $content -replace 'appointment\.AppointmentTime\.Value', 'appointment.AppointmentTime.Value'; $content = $content -replace 'doctor\.WorkingHours', 'doctor.StaffMember?.WorkingHours ?? \"9:00 AM - 5:00 PM\"'; Set-Content 'Pages\User\Appointment.cshtml.cs' $content"
)

:: Fix Pages\Appointment\Book.cshtml.cs
if exist "Pages\Appointment\Book.cshtml.cs" (
    echo Fixing Pages\Appointment\Book.cshtml.cs
    copy "Pages\Appointment\Book.cshtml.cs" "%backupdir%\Book.cshtml.cs"
    powershell -Command "$content = Get-Content 'Pages\Appointment\Book.cshtml.cs' -Raw; $content = $content -replace 'appointment\.AppointmentTime = Input\.AppointmentTime', 'appointment.AppointmentTime = TimeSpan.Parse(Input.AppointmentTime)'; if (-not ($content -match 'private async Task LoadFormData')) { $content = $content -replace '(public async Task<IActionResult> OnPostAsync\(\)[^}]*})', '$1\r\n\r\n        private async Task LoadFormData()\r\n        {\r\n            Doctors = await _context.StaffMembers.Where(s => s.Role == \"Doctor\").ToListAsync();\r\n            TimeSlots = new List<string> { \"9:00 AM\", \"10:00 AM\", \"11:00 AM\", \"1:00 PM\", \"2:00 PM\", \"3:00 PM\", \"4:00 PM\" };\r\n        }'; } Set-Content 'Pages\Appointment\Book.cshtml.cs' $content"
)

:: Fix Pages\Doctor\DoctorDashboard.cshtml.cs
if exist "Pages\Doctor\DoctorDashboard.cshtml.cs" (
    echo Fixing Pages\Doctor\DoctorDashboard.cshtml.cs
    copy "Pages\Doctor\DoctorDashboard.cshtml.cs" "%backupdir%\DoctorDashboard.cshtml.cs"
    powershell -Command "$content = Get-Content 'Pages\Doctor\DoctorDashboard.cshtml.cs' -Raw; $content = $content -replace 'MaxDailyPatients = doctor\.MaxDailyPatients \?\? 20', 'MaxDailyPatients = doctor.MaxDailyPatients != null ? doctor.MaxDailyPatients.Value : 20'; $content = $content -replace 'MaxDailyPatients = staffMember\.MaxDailyPatients \?\? 20', 'MaxDailyPatients = staffMember.MaxDailyPatients != null ? staffMember.MaxDailyPatients.Value : 20'; Set-Content 'Pages\Doctor\DoctorDashboard.cshtml.cs' $content"
)

:: Fix Pages\Admin\AdminDashboard.cshtml.cs
if exist "Pages\Admin\AdminDashboard.cshtml.cs" (
    echo Fixing Pages\Admin\AdminDashboard.cshtml.cs
    copy "Pages\Admin\AdminDashboard.cshtml.cs" "%backupdir%\AdminDashboard.cshtml.cs"
    powershell -Command "$content = Get-Content 'Pages\Admin\AdminDashboard.cshtml.cs' -Raw; $content = $content -replace 'AppointmentTime = a\.AppointmentTime \?\? TimeSpan\.Zero', 'AppointmentTime = a.AppointmentTime != null ? a.AppointmentTime.Value : TimeSpan.Zero'; Set-Content 'Pages\Admin\AdminDashboard.cshtml.cs' $content"
)

:: Fix Pages\Doctor\Consultation.cshtml.cs
if exist "Pages\Doctor\Consultation.cshtml.cs" (
    echo Fixing Pages\Doctor\Consultation.cshtml.cs
    copy "Pages\Doctor\Consultation.cshtml.cs" "%backupdir%\Consultation.cshtml.cs"
    powershell -Command "$content = Get-Content 'Pages\Doctor\Consultation.cshtml.cs' -Raw; $content = $content -replace 'public Patient Patient', 'public Patient? Patient = null'; $content = $content -replace 'public List<MedicalRecord> MedicalHistory', 'public List<MedicalRecord> MedicalHistory = new()'; $content = $content -replace 'public string Diagnosis', 'public string? Diagnosis'; $content = $content -replace 'public string Treatment', 'public string? Treatment'; $content = $content -replace 'public string Doctor', 'public string? Doctor'; $content = $content -replace 'public string Id', 'public string? Id'; $content = $content -replace 'public string Name', 'public string? Name'; $content = $content -replace 'public string Gender', 'public string? Gender'; $content = $content -replace 'public string BloodType', 'public string? BloodType'; $content = $content -replace 'public string Allergies', 'public string? Allergies'; Set-Content 'Pages\Doctor\Consultation.cshtml.cs' $content"
)

:: Fix Pages\Doctor\NewPrescription.cshtml.cs
if exist "Pages\Doctor\NewPrescription.cshtml.cs" (
    echo Fixing Pages\Doctor\NewPrescription.cshtml.cs
    copy "Pages\Doctor\NewPrescription.cshtml.cs" "%backupdir%\NewPrescription.cshtml.cs"
    powershell -Command "$content = Get-Content 'Pages\Doctor\NewPrescription.cshtml.cs' -Raw; $content = $content -replace 'public Patient Patient', 'public Patient? Patient = null'; $content = $content -replace 'public List<Medication> Medications', 'public List<Medication> Medications = new()'; $content = $content -replace 'public string Id', 'public string? Id'; $content = $content -replace 'public string Name', 'public string? Name'; $content = $content -replace 'public string Allergies', 'public string? Allergies'; Set-Content 'Pages\Doctor\NewPrescription.cshtml.cs' $content"
)

:: Fix Pages\Doctor\PatientRecords.cshtml.cs
if exist "Pages\Doctor\PatientRecords.cshtml.cs" (
    echo Fixing Pages\Doctor\PatientRecords.cshtml.cs
    copy "Pages\Doctor\PatientRecords.cshtml.cs" "%backupdir%\PatientRecords.cshtml.cs"
    powershell -Command "$content = Get-Content 'Pages\Doctor\PatientRecords.cshtml.cs' -Raw; $content = $content -replace 'var patient = await _context.Patients.FindAsync\(id\);', 'var patient = await _context.Patients.FindAsync(id);\r\n            if (patient == null) return NotFound();'; $content = $content -replace '(Patient = )patient;', '$1patient;'; $content = $content -replace '(MedicalRecords = )await', '$1await'; $content = $content -replace '(Prescriptions = )await', '$1await'; $content = $content -replace '(Appointments = )await', '$1await'; $content = $content -replace 'Patient\.Id', 'Patient?.Id ?? string.Empty'; $content = $content -replace 'Patient\.FullName', 'Patient?.FullName ?? string.Empty'; Set-Content 'Pages\Doctor\PatientRecords.cshtml.cs' $content"
)

:: Fix Pages\Doctor\Reports.cshtml.cs
if exist "Pages\Doctor\Reports.cshtml.cs" (
    echo Fixing Pages\Doctor\Reports.cshtml.cs
    copy "Pages\Doctor\Reports.cshtml.cs" "%backupdir%\Reports.cshtml.cs"
    powershell -Command "$content = Get-Content 'Pages\Doctor\Reports.cshtml.cs' -Raw; $content = $content -replace 'var doctor = await _context.StaffMembers.FirstOrDefaultAsync\(s => s.UserId == userId\);', 'var doctor = await _context.StaffMembers.FirstOrDefaultAsync(s => s.UserId == userId);\r\n            if (doctor == null) return Page();'; Set-Content 'Pages\Doctor\Reports.cshtml.cs' $content"
)

echo.
echo ===================================================
echo All fixes applied successfully!
echo.
echo Verifying build before proceeding with database operations...
echo.

:: Attempt to build the project first to verify fixes
echo Building project to verify fixes...
dotnet build > build_errors.txt 2>&1

if errorlevel 1 (
    echo.
    echo Build failed! Analyzing errors and attempting additional fixes...
    
    :: Check for common TimeSpan-related errors and fix them
    powershell -Command "$errors = Get-Content 'build_errors.txt'; $needsRebuild = $false; foreach ($error in $errors) { if ($error -match '(.*\.cs)\((\d+),(\d+)\).*') { $file = $matches[1]; $line = [int]$matches[2]; if ($error -match 'Cannot implicitly convert type') { echo \"Fixing conversion error in $file at line $line\"; $content = Get-Content $file; if ($content[$line-1] -match 'AppointmentTime\s*=\s*([^;]+)') { $content[$line-1] = $content[$line-1] -replace 'AppointmentTime\s*=\s*([^;]+)', 'AppointmentTime = $1?.ToString() ?? string.Empty'; Set-Content $file $content; $needsRebuild = $true; } } elseif ($error -match 'Operator.*cannot be applied to operand of type') { echo \"Fixing operator error in $file at line $line\"; $content = Get-Content $file; if ($content[$line-1] -match '\.AppointmentTime\s*([=!<>]+)\s*') { $content[$line-1] = $content[$line-1] -replace '\.AppointmentTime\s*([=!<>]+)\s*', '.AppointmentTime?.ToString() $1 '; Set-Content $file $content; $needsRebuild = $true; } } elseif ($error -match 'Cannot convert method group') { echo \"Fixing method group error in $file at line $line\"; $content = Get-Content $file; if ($content[$line-1] -match '\.HasValue\(\)') { $content[$line-1] = $content[$line-1] -replace '\.HasValue\(\)', ' != null'; Set-Content $file $content; $needsRebuild = $true; } } } } if ($needsRebuild) { echo \"Additional fixes applied. Attempting rebuild...\"; } else { echo \"Could not automatically fix all errors. Manual intervention required.\"; }"
    
    :: Try rebuilding after additional fixes
    echo.
    echo Attempting rebuild after additional fixes...
    dotnet build
    
    if errorlevel 1 (
        echo.
        echo Build still failing. You may need to manually fix some issues.
        echo Check the backup files in the %backupdir% directory if you need to restore any files.
        echo.
        echo Common manual fixes to try:
        echo 1. Check for any remaining TimeSpan comparisons that need to be fixed
        echo 2. Look for null checks on TimeSpan properties that need to be updated
        echo 3. Verify that all TimeSpan properties are properly marked as nullable
        echo 4. Check for any ToString() calls on potentially null TimeSpan values
        echo.
        echo You can view detailed build errors in the build_errors.txt file.
        goto :error
    ) else (
        echo.
        echo Build successful after additional fixes! You can now proceed with database operations.
    )
) else (
    echo.
    echo Build successful! You can now proceed with database operations.
)

echo.
echo Next steps:
echo 1. Drop the database: dotnet ef database drop --force
echo 2. Remove the last migration: dotnet ef migrations remove
echo 3. Create a new migration: dotnet ef migrations add FixTimeSpanHandling
echo 4. Update the database: dotnet ef database update
echo.
echo Would you like to automatically run these commands now? (Y/N)
set /p runCommands=

if /i "%runCommands%"=="Y" (
    echo.
    echo Running database commands...
    
    echo.
    echo Dropping database...
    dotnet ef database drop --force
    if errorlevel 1 goto :db_error
    
    echo.
    echo Removing last migration...
    dotnet ef migrations remove
    if errorlevel 1 goto :db_error
    
    echo.
    echo Creating new migration...
    dotnet ef migrations add FixTimeSpanHandling
    if errorlevel 1 goto :db_error
    
    echo.
    echo Updating database...
    dotnet ef database update
    if errorlevel 1 goto :db_error
    
    echo.
    echo All database operations completed successfully!
) else (
    echo.
    echo You chose to run the database commands manually.
)

goto :end

:db_error
echo.
echo Error occurred during database operations.
echo Please check the error messages above.
goto :end

:error
echo.
echo Error occurred during the fix process.
echo Please check the error messages above.

:end
pause