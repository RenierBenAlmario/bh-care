# PowerShell script to verify registration process fixes
Write-Host "Barangay Health Center Registration Fix Verification" -ForegroundColor Cyan
Write-Host "------------------------------------------------" -ForegroundColor Cyan

# Configuration
$sqlInstance = "DESKTOP-NU53VS3"  # From appsettings.json
$database = "Barangay"            # From appsettings.json
$appUrl = "https://localhost:5001" # Change this to your application URL

# Function to execute SQL query
function Execute-SqlQuery {
    param (
        [string]$query,
        [string]$description
    )
    
    try {
        Write-Host "Executing query: $description..." -ForegroundColor Yellow
        
        # Execute the SQL query using sqlcmd
        $result = sqlcmd -S $sqlInstance -d $database -Q $query -h -1 -W
        
        Write-Host "✓ Query executed successfully." -ForegroundColor Green
        return $result
    }
    catch {
        Write-Host "✗ Error executing query: $_" -ForegroundColor Red
        return $null
    }
}

# Function to check registration before and after
function Check-Registration {
    # 1. Get count of users and documents before testing
    $beforeQuery = @"
SELECT 
    (SELECT COUNT(*) FROM AspNetUsers WHERE Status = 'Pending') AS PendingUsers,
    (SELECT COUNT(*) FROM UserDocuments WHERE Status = 'Pending') AS PendingDocuments,
    (SELECT COUNT(*) FROM Notifications WHERE ReadAt IS NULL) AS UnreadNotifications
"@

    $before = Execute-SqlQuery -query $beforeQuery -description "Get counts before testing"
    Write-Host "Before testing:" -ForegroundColor White
    Write-Host "  - Pending Users: $($before[0])" -ForegroundColor White
    Write-Host "  - Pending Documents: $($before[2])" -ForegroundColor White
    Write-Host "  - Unread Notifications: $($before[4])" -ForegroundColor White
    
    # 2. Ask user to perform a test registration
    Write-Host "`nPlease perform a test registration in the application:" -ForegroundColor Cyan
    Write-Host "1. Open the application in your browser: $appUrl/Account/SignUp" -ForegroundColor Yellow
    Write-Host "2. Fill out the registration form with test data" -ForegroundColor Yellow
    Write-Host "3. Upload a test PDF or image file for residency proof" -ForegroundColor Yellow
    Write-Host "4. Submit the form" -ForegroundColor Yellow
    Write-Host "5. Return to this script when done" -ForegroundColor Yellow
    
    $userInput = Read-Host "`nDid you complete the test registration? (y/n)"
    
    if ($userInput -ne "y") {
        Write-Host "Test cancelled." -ForegroundColor Red
        return
    }
    
    # 3. Get count of users and documents after testing
    $afterQuery = @"
SELECT 
    (SELECT COUNT(*) FROM AspNetUsers WHERE Status = 'Pending') AS PendingUsers,
    (SELECT COUNT(*) FROM UserDocuments WHERE Status = 'Pending') AS PendingDocuments,
    (SELECT COUNT(*) FROM Notifications WHERE ReadAt IS NULL) AS UnreadNotifications
"@

    $after = Execute-SqlQuery -query $afterQuery -description "Get counts after testing"
    
    # 4. Display results
    Write-Host "`nAfter testing:" -ForegroundColor White
    Write-Host "  - Pending Users: $($after[0]) (Changed by $([int]$after[0] - [int]$before[0]))" -ForegroundColor White
    Write-Host "  - Pending Documents: $($after[2]) (Changed by $([int]$after[2] - [int]$before[2]))" -ForegroundColor White
    Write-Host "  - Unread Notifications: $($after[4]) (Changed by $([int]$after[4] - [int]$before[4]))" -ForegroundColor White
    
    # 5. Check results
    if ([int]$after[0] -gt [int]$before[0] -and 
        [int]$after[2] -gt [int]$before[2] -and 
        [int]$after[4] -gt [int]$before[4]) {
        Write-Host "`n✓ Registration fix VERIFIED! User, document, and notification counts all increased." -ForegroundColor Green
    }
    elseif ([int]$after[0] -gt [int]$before[0] -and 
            [int]$after[2] -gt [int]$before[2]) {
        Write-Host "`n✓ Registration partially fixed. User and document created but notification might be missing." -ForegroundColor Yellow
    }
    else {
        Write-Host "`n✗ Registration fix NOT VERIFIED. Expected all counts to increase." -ForegroundColor Red
    }
    
    # 6. Show the most recent user
    $recentUserQuery = @"
SELECT TOP 1 Id, UserName, Email, Status, IsActive, CreatedAt 
FROM AspNetUsers 
ORDER BY CreatedAt DESC
"@

    $recentUser = Execute-SqlQuery -query $recentUserQuery -description "Get most recent user"
    Write-Host "`nMost recent user registered:" -ForegroundColor Cyan
    Write-Host "  - ID: $($recentUser[0])" -ForegroundColor White
    Write-Host "  - Username: $($recentUser[1])" -ForegroundColor White
    Write-Host "  - Email: $($recentUser[2])" -ForegroundColor White
    Write-Host "  - Status: $($recentUser[3])" -ForegroundColor White
    Write-Host "  - IsActive: $($recentUser[4])" -ForegroundColor White
    Write-Host "  - CreatedAt: $($recentUser[5])" -ForegroundColor White
    
    # 7. Show the associated document
    $userId = $recentUser[0]
    $documentQuery = @"
SELECT Id, FileName, ContentType, Status, FilePath 
FROM UserDocuments 
WHERE UserId = '$userId'
"@

    $document = Execute-SqlQuery -query $documentQuery -description "Get document for recent user"
    if ($document) {
        Write-Host "`nAssociated document:" -ForegroundColor Cyan
        Write-Host "  - ID: $($document[0])" -ForegroundColor White
        Write-Host "  - FileName: $($document[1])" -ForegroundColor White
        Write-Host "  - ContentType: $($document[2])" -ForegroundColor White
        Write-Host "  - Status: $($document[3])" -ForegroundColor White
        Write-Host "  - FilePath: $($document[4])" -ForegroundColor White
    }
    else {
        Write-Host "`n✗ No document found for the most recent user!" -ForegroundColor Red
    }
}

# Main script
$choice = Read-Host "`nDo you want to verify the registration fix (y/n)?"

if ($choice -eq "y") {
    Check-Registration
}
else {
    Write-Host "Verification cancelled." -ForegroundColor Yellow
}

Write-Host "`nScript completed." -ForegroundColor Cyan 