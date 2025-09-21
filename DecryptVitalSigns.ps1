# Vital Signs Decryptor PowerShell Script
# This script decrypts the encrypted vital signs values from the consultation interface

Add-Type -AssemblyName System.Security

function Decrypt-VitalSign {
    param(
        [string]$CipherText,
        [string]$Key = "BHCARE_DataEncryption_Key_2024_Secure_32Chars"
    )
    
    try {
        # Ensure key is exactly 32 characters
        if ($Key.Length -lt 32) {
            $Key = $Key.PadRight(32, '0')
        } elseif ($Key.Length -gt 32) {
            $Key = $Key.Substring(0, 32)
        }
        
        $encryptedBytes = [Convert]::FromBase64String($CipherText)
        
        # Create AES object
        $aes = [System.Security.Cryptography.Aes]::Create()
        $aes.Key = [System.Text.Encoding]::UTF8.GetBytes($Key)
        
        # Extract IV from the beginning of the encrypted data
        $iv = New-Object byte[] 16
        [Array]::Copy($encryptedBytes, 0, $iv, 0, 16)
        $aes.IV = $iv
        
        # Extract encrypted data
        $encryptedData = New-Object byte[] ($encryptedBytes.Length - 16)
        [Array]::Copy($encryptedBytes, 16, $encryptedData, 0, $encryptedData.Length)
        
        # Decrypt
        $decryptor = $aes.CreateDecryptor()
        $decryptedBytes = $decryptor.TransformFinalBlock($encryptedData, 0, $encryptedData.Length)
        
        $aes.Dispose()
        
        return [System.Text.Encoding]::UTF8.GetString($decryptedBytes)
    }
    catch {
        return "DECRYPTION ERROR: $($_.Exception.Message)"
    }
}

Write-Host "=== Vital Signs Decryptor ===" -ForegroundColor Green
Write-Host ""

# The encrypted vital signs from the consultation interface
$encryptedVitalSigns = @{
    "Blood Pressure" = "eJ40XPbpYtmMxkwQpbPb31OwKHLxGKx2I+JrQpVWUdk="
    "Temperature" = "LfUSUfoAmvwtsCkee2AGwzRo7oAhJCBJErlajqldgsw="
    "Heart Rate" = "uyayL1QUmdbeMHnTWPGCVxEc0AoDzAdYpbMFUrZetko="
    "SpO2" = "ufoulqxU5fEp9sUtx4Hva9o0iMWQZffGxlvWEquDkKk="
    "Weight" = "5K9Tcuitrd49KRvZ4UU1nUTHH2wDk1c2LwGWd0+l1iU="
    "Height" = "UmpUfl7p/gxcwmhq0agujzc2llcNqpXaHVxgAbhETM4="
}

foreach ($vitalSign in $encryptedVitalSigns.GetEnumerator()) {
    $decryptedValue = Decrypt-VitalSign -CipherText $vitalSign.Value
    Write-Host "$($vitalSign.Key): $decryptedValue" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Decryption completed!" -ForegroundColor Green
