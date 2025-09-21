# Data Encryption Implementation for BHCARE

## Overview

This document describes the comprehensive data encryption implementation for the BHCARE application. All sensitive patient and medical data is now encrypted at rest and can only be decrypted by authorized personnel (Admin, Doctor, Nurse, System Administrator).

## Security Features

### 1. Encryption Algorithm
- **Algorithm**: AES-256 (Advanced Encryption Standard)
- **Key Size**: 256-bit encryption key
- **Mode**: CBC (Cipher Block Chaining)
- **IV**: Random Initialization Vector for each encryption operation

### 2. Role-Based Access Control
Only users with the following roles can decrypt sensitive data:
- **Admin**: Full access to all encrypted data
- **Doctor**: Access to patient medical data
- **Nurse**: Access to patient medical data  
- **System Administrator**: Full access to all encrypted data

### 3. Encrypted Data Fields

#### Patient Data
- FullName
- Address
- ContactNumber
- EmergencyContact
- EmergencyContactNumber
- Email
- Diagnosis
- Alert
- Allergies
- MedicalHistory
- CurrentMedications

#### Medical Records
- Diagnosis
- Treatment
- Notes
- ChiefComplaint
- Duration
- Medications
- Prescription
- Instructions

#### Prescriptions
- Diagnosis
- Notes

#### Appointments
- PatientName
- DependentFullName
- ContactNumber
- Address
- EmergencyContact
- EmergencyContactNumber
- Allergies
- MedicalHistory
- CurrentMedications
- Description
- ReasonForVisit
- Prescription
- Instructions

#### Application Users
- Name
- FirstName
- MiddleName
- LastName
- Address
- PhilHealthId

#### Vital Signs
- Notes

#### Lab Results
- TestName
- Result
- Notes

## Implementation Components

### 1. Core Services

#### DataEncryptionService
- Handles encryption/decryption operations
- Manages encryption keys
- Provides role-based access control
- Located: `Services/DataEncryptionService.cs`

#### EncryptedDataService
- Manages entity-level encryption/decryption
- Handles database operations with encryption
- Located: `Services/EncryptedDataService.cs`

#### EncryptExistingDataService
- Migrates existing unencrypted data to encrypted format
- One-time operation for existing data
- Located: `Migrations/EncryptExistingDataMigration.cs`

### 2. Attributes and Extensions

#### EncryptedAttribute
- Marks properties that should be encrypted
- Located: `Attributes/EncryptedAttribute.cs`

#### EncryptionExtensions
- Provides extension methods for encryption/decryption
- Located: `Extensions/EncryptionExtensions.cs`

### 3. Middleware

#### DataEncryptionMiddleware
- Automatically handles encryption/decryption for HTTP requests/responses
- Located: `Middleware/DataEncryptionMiddleware.cs`

### 4. Controllers

#### DataEncryptionController
- Admin interface for managing encryption
- Located: `Controllers/DataEncryptionController.cs`

#### BaseEncryptedController
- Base controller with encryption capabilities
- Located: `Controllers/BaseEncryptedController.cs`

## Configuration

### 1. appsettings.json
```json
{
  "DataEncryption": {
    "Key": "BHCARE_DataEncryption_Key_2024_Secure_32Chars"
  }
}
```

### 2. Program.cs Registration
```csharp
// Register encryption services
builder.Services.AddScoped<IDataEncryptionService, DataEncryptionService>();
builder.Services.AddScoped<IEncryptedDataService, EncryptedDataService>();
builder.Services.AddScoped<EncryptExistingDataService>();

// Add encryption middleware
app.UseMiddleware<DataEncryptionMiddleware>();
```

## Usage Examples

### 1. Encrypting Data
```csharp
// Automatic encryption when saving
var patient = new Patient
{
    FullName = "John Doe",
    Address = "123 Main St",
    ContactNumber = "555-1234"
};

// Data is automatically encrypted when saved
await _context.Patients.AddAsync(patient);
await _context.SaveChangesAsync();
```

### 2. Decrypting Data
```csharp
// Automatic decryption for authorized users
var patient = await _context.Patients.FindAsync(patientId);

// For authorized users (Admin, Doctor, Nurse), data is automatically decrypted
// For unauthorized users, encrypted data is returned
```

### 3. Manual Encryption/Decryption
```csharp
// Encrypt text
var encryptedText = _encryptionService.Encrypt("Sensitive Data");

// Decrypt for authorized user
var decryptedText = _encryptionService.DecryptForUser(encryptedText, user);
```

## Database Schema Changes

### Column Length Updates
All encrypted fields have had their column lengths increased to accommodate encrypted data:

- String fields: Increased by 2-4x original length
- Text fields: No change (already supports large data)

### Migration Process
1. Existing data remains unencrypted until migration
2. Run the encryption migration to encrypt all existing data
3. New data is automatically encrypted

## Security Considerations

### 1. Key Management
- Encryption key is stored in configuration
- Consider using Azure Key Vault or similar for production
- Key rotation should be implemented for enhanced security

### 2. Performance Impact
- Encryption/decryption adds minimal overhead
- Database queries remain efficient
- Caching can be used for frequently accessed data

### 3. Backup and Recovery
- Encrypted data requires the encryption key for recovery
- Ensure encryption key is backed up securely
- Test data recovery procedures

## Migration Instructions

### 1. Backup Database
```sql
-- Create full database backup before encryption
BACKUP DATABASE Barangay TO DISK = 'C:\Backup\Barangay_BeforeEncryption.bak'
```

### 2. Run Encryption Migration
1. Access the Data Encryption Management page as Admin
2. Confirm the encryption process
3. Wait for completion
4. Verify data is encrypted

### 3. Verify Encryption
```sql
-- Check that data is encrypted (should show encrypted strings)
SELECT TOP 5 FullName, Address, ContactNumber FROM Patients
```

## Troubleshooting

### 1. Decryption Issues
- Verify user has proper role (Admin, Doctor, Nurse, System Administrator)
- Check encryption key configuration
- Ensure middleware is properly registered

### 2. Performance Issues
- Monitor database performance after encryption
- Consider indexing strategies for encrypted fields
- Use caching for frequently accessed data

### 3. Data Integrity
- Verify encryption/decryption is working correctly
- Test with sample data before full migration
- Monitor application logs for encryption errors

## Future Enhancements

### 1. Key Rotation
- Implement automatic key rotation
- Support multiple encryption keys
- Gradual migration to new keys

### 2. Field-Level Permissions
- More granular access control
- Different permissions for different data types
- Audit logging for data access

### 3. Advanced Security
- Hardware security modules (HSM)
- Multi-factor authentication for sensitive operations
- Advanced threat detection

## Support and Maintenance

### 1. Regular Security Audits
- Review encryption implementation
- Test decryption capabilities
- Verify role-based access

### 2. Key Management
- Regular key rotation
- Secure key storage
- Backup and recovery procedures

### 3. Monitoring
- Monitor encryption/decryption performance
- Track access patterns
- Alert on security violations

## Conclusion

The data encryption implementation provides comprehensive protection for sensitive patient and medical data while maintaining application functionality. The role-based access control ensures that only authorized personnel can access decrypted data, providing both security and compliance benefits.

For questions or issues related to the encryption implementation, contact the development team or refer to the troubleshooting section above.
