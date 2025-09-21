# VitalSigns Table String Conversion - Implementation Guide

## Overview
This implementation converts the VitalSigns table from using integer and decimal data types to nvarchar (string) types to support proper encryption of sensitive medical data.

## Changes Made

### 1. Model Updates (`Models/VitalSign.cs`)
- **Temperature**: Changed from `decimal(5,2)` to `string` with `[StringLength(50)]`
- **HeartRate**: Changed from `int` to `string` with `[StringLength(50)]`
- **RespiratoryRate**: Changed from `int` to `string` with `[StringLength(50)]`
- **SpO2**: Changed from `decimal(5,2)` to `string` with `[StringLength(50)]`
- **Weight**: Changed from `decimal(5,2)` to `string` with `[StringLength(50)]`
- **Height**: Changed from `decimal(5,2)` to `string` with `[StringLength(50)]`

### 2. Database Migration Options

#### Option A: Direct SQL Script (`VitalSigns_StringConversion_Migration.sql`)
- Creates backup table before changes
- Adds new nvarchar columns
- Converts existing data from numeric to string format
- Drops old columns and renames new ones
- Adds performance indexes
- Safe rollback capability

#### Option B: Entity Framework Migration (`Migrations/ConvertVitalSignsToStringTypes.cs`)
- Uses EF Core migration system
- Handles data conversion automatically
- Provides rollback functionality
- Integrates with existing migration history

### 3. Encrypted Fields Configuration
All encrypted fields are properly configured with:
- `[StringLength(1000)]` attribute for database column size
- `[Encrypted]` attribute for automatic encryption/decryption
- Proper nullable string types

## Implementation Steps

### Step 1: Backup Database
```sql
-- Always backup your database before running migrations
BACKUP DATABASE [Barangay] TO DISK = 'C:\Backup\Barangay_Before_VitalSigns_Conversion.bak'
```

### Step 2: Choose Migration Method

#### Method A: Direct SQL Execution
1. Run the SQL script: `VitalSigns_StringConversion_Migration.sql`
2. Verify table structure after execution
3. Test data integrity

#### Method B: Entity Framework Migration
1. Add migration: `dotnet ef migrations add ConvertVitalSignsToStringTypes`
2. Update database: `dotnet ef database update`
3. Verify changes

### Step 3: Verify Changes
```sql
-- Check table structure
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'VitalSigns' 
ORDER BY ORDINAL_POSITION;

-- Verify data conversion
SELECT TOP 10 
    Id, PatientId, Temperature, HeartRate, RespiratoryRate, 
    SpO2, Weight, Height, RecordedAt
FROM VitalSigns;
```

## Benefits of String Conversion

1. **Encryption Support**: String fields can be properly encrypted using the `[Encrypted]` attribute
2. **Data Flexibility**: Can store formatted values (e.g., "120/80" for blood pressure)
3. **Consistency**: All vital sign fields use the same data type
4. **Future-Proof**: Easier to extend with additional formatting or validation

## Data Validation Considerations

After conversion, implement validation in your application layer:

```csharp
// Example validation for numeric string fields
public bool IsValidTemperature(string temperature)
{
    if (string.IsNullOrEmpty(temperature)) return true; // Allow null/empty
    
    return decimal.TryParse(temperature, out decimal temp) && 
           temp >= 30.0m && temp <= 45.0m; // Reasonable temperature range
}

public bool IsValidHeartRate(string heartRate)
{
    if (string.IsNullOrEmpty(heartRate)) return true;
    
    return int.TryParse(heartRate, out int rate) && 
           rate >= 30 && rate <= 200; // Reasonable heart rate range
}
```

## Rollback Procedure

If you need to rollback the changes:

1. **Using SQL Script**: Restore from backup table
2. **Using EF Migration**: Run `dotnet ef database update <previous-migration>`

## Testing Checklist

- [ ] Data conversion completed successfully
- [ ] All existing records preserved
- [ ] New records can be inserted with string values
- [ ] Encryption/decryption works properly
- [ ] Application validation handles string inputs
- [ ] Performance indexes are working
- [ ] Backup tables can be safely removed after verification

## Security Notes

- Encrypted fields (`EncryptedTemperature`, `EncryptedHeartRate`, etc.) are automatically handled by the encryption framework
- Regular fields (`Temperature`, `HeartRate`, etc.) should be used for display and non-sensitive operations
- Consider implementing field-level encryption for all sensitive data
- Regular security audits should include verification of encryption implementation

## Next Steps

1. Run the migration script
2. Update application code to handle string inputs
3. Implement proper validation
4. Test thoroughly in development environment
5. Deploy to production with proper monitoring
6. Remove backup tables after successful verification

