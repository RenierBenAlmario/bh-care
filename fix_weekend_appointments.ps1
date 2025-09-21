# PowerShell script to fix weekend appointments
Write-Host "Fixing weekend appointments for doctors..." -ForegroundColor Green

# SQL Server connection string (adjust as needed)
$connectionString = "Server=(localdb)\\mssqllocaldb;Database=BarangayDb;Trusted_Connection=true;MultipleActiveResultSets=true"

try {
    # Create SQL connection
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    Write-Host "Connected to database successfully!" -ForegroundColor Green
    
    # Update existing DoctorAvailability records
    $updateQuery = @"
    UPDATE DoctorAvailabilities 
    SET Saturday = 1, 
        Sunday = 1, 
        StartTime = '08:00:00', 
        EndTime = '17:00:00',
        IsAvailable = 1
    WHERE DoctorId IN (
        SELECT u.Id 
        FROM AspNetUsers u 
        JOIN AspNetUserRoles ur ON u.Id = ur.UserId 
        JOIN AspNetRoles r ON ur.RoleId = r.Id 
        WHERE r.Name = 'Doctor'
    );
    " @
    
    $command = New-Object System.Data.SqlClient.SqlCommand($updateQuery, $connection)
    $rowsAffected = $command.ExecuteNonQuery()
    Write-Host "Updated $rowsAffected existing doctor availability records" -ForegroundColor Yellow
    
    # Create new DoctorAvailability records for doctors who don't have them
    $insertQuery = @"
    INSERT INTO DoctorAvailabilities (DoctorId, IsAvailable, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday, StartTime, EndTime, LastUpdated)
    SELECT u.Id, 1, 1, 1, 1, 1, 1, 1, 1, '08:00:00', '17:00:00', GETDATE()
    FROM AspNetUsers u 
    JOIN AspNetUserRoles ur ON u.Id = ur.UserId 
    JOIN AspNetRoles r ON ur.RoleId = r.Id 
    WHERE r.Name = 'Doctor'
    AND NOT EXISTS (
        SELECT 1 FROM DoctorAvailabilities da WHERE da.DoctorId = u.Id
    );
    " @
    
    $command = New-Object System.Data.SqlClient.SqlCommand($insertQuery, $connection)
    $rowsAffected = $command.ExecuteNonQuery()
    Write-Host "Created $rowsAffected new doctor availability records" -ForegroundColor Yellow
    
    # Show current status
    $statusQuery = @"
    SELECT u.UserName, da.Saturday, da.Sunday, da.StartTime, da.EndTime, da.IsAvailable
    FROM AspNetUsers u
    JOIN DoctorAvailabilities da ON u.Id = da.DoctorId
    WHERE da.Saturday = 1 AND da.Sunday = 1;
    " @
    
    $command = New-Object System.Data.SqlClient.SqlCommand($statusQuery, $connection)
    $reader = $command.ExecuteReader()
    
    Write-Host "`nDoctors with weekend availability:" -ForegroundColor Cyan
    while ($reader.Read()) {
        $username = $reader["UserName"]
        $saturday = $reader["Saturday"]
        $sunday = $reader["Sunday"]
        $startTime = $reader["StartTime"]
        $endTime = $reader["EndTime"]
        $isAvailable = $reader["IsAvailable"]
        
        Write-Host "  $username - Sat: $saturday, Sun: $sunday, Hours: $startTime-$endTime, Active: $isAvailable" -ForegroundColor White
    }
    $reader.Close()
    
    Write-Host "`nWeekend appointments are now enabled! 🎉" -ForegroundColor Green
    Write-Host "Try booking a Saturday or Sunday appointment now." -ForegroundColor Green
    
} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
} finally {
    if ($connection.State -eq 'Open') {
        $connection.Close()
    }
}

Write-Host "`nPress any key to continue..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
