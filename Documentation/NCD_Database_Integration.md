# NCD Database Integration Documentation

## Overview
The NCD Form Management system now integrates with the `NCDRiskAssessments` database table, allowing form data to be saved directly to the database instead of just logging it as JSON.

## Database Schema Integration

### NCDRiskAssessments Table Mapping
The form data is now mapped to the following database columns:

#### **Health Facility Information**
- `HealthFacility` ← `healthFacility`
- `FamilyNo` ← `familyNo`
- `Address` ← `address`
- `Barangay` ← `barangay`

#### **Personal Information**
- `FirstName` ← `firstName`
- `MiddleName` ← `middleName`
- `LastName` ← `lastName`
- `Telepono` ← `telepono`
- `Birthday` ← `birthday` (parsed as DateTime)
- `Edad` ← `edad` (parsed as int)
- `Kasarian` ← `kasarian`
- `Relihiyon` ← `relihiyon`
- `CivilStatus` ← `civilStatus`
- `Occupation` ← `occupation`

#### **Medical History**
- `HasDiabetes` ← `hasDiabetes` (checkbox)
- `HasHypertension` ← `hasHypertension` (checkbox)
- `HasCancer` ← `hasCancer` (checkbox)
- `HasCOPD` ← `hasCOPD` (checkbox)
- `HasLungDisease` ← `hasLungDisease` (checkbox)
- `HasEyeDisease` ← `hasEyeDisease` (checkbox)
- `CancerType` ← `cancerSite`
- `DiabetesYear` ← `yearDiagnosed` (parsed as int)
- `HypertensionYear` ← `yearDiagnosed` (parsed as int)
- `CancerYear` ← `yearDiagnosed` (parsed as int)
- `LungDiseaseYear` ← `yearDiagnosed` (parsed as int)
- `DiabetesMedication` ← `medication`
- `HypertensionMedication` ← `medication`
- `CancerMedication` ← `medication`
- `LungDiseaseMedication` ← `medication`

#### **Family History**
- `FamilyHasHypertension` ← `familyHasHypertension` (checkbox)
- `FamilyHasHeartDisease` ← `familyHasHeartDisease` (checkbox)
- `FamilyHasStroke` ← `familyHasStroke` (checkbox)
- `FamilyHasDiabetes` ← `familyHasDiabetes` (checkbox)
- `FamilyHasCancer` ← `familyHasCancer` (checkbox)
- `FamilyHasKidneyDisease` ← `familyHasKidneyDisease` (checkbox)
- `FamilyHasOtherDisease` ← `familyHasOtherDisease` (checkbox)
- `FamilyOtherDiseaseDetails` ← `familyOtherDiseaseDetails`

#### **Lifestyle Factors**
- `HighSaltIntake` ← `highSaltIntake` (checkbox)
- `AlcoholConsumption` ← `drinksAlcohol` (radio button)
- `AlcoholFrequency` ← `alcoholFrequency` (radio button)
- `HasNoRegularExercise` ← `hasRegularExercise` (inverted logic)

#### **Smoking Status**
- `SmokingStatus` ← Derived from `isSmoker` and `smokingQuitDuration`:
  - "Current Smoker" if `isSmoker` = "Yes"
  - "Quit Less Than 1 Year" if `smokingQuitDuration` = "LessThan1Year"
  - "Quit More Than 1 Year" if `smokingQuitDuration` = "MoreThan1Year"
  - "Non-smoker" otherwise

#### **System Fields**
- `UserId` ← Current user identity
- `AppointmentId` ← Default value (0) - should be set based on actual appointment
- `AppointmentType` ← "NCD Assessment"
- `RiskStatus` ← "Assessment Completed"
- `CreatedAt` ← Current UTC timestamp
- `UpdatedAt` ← Current UTC timestamp

## Technical Implementation

### Database Context Integration
```csharp
private readonly ApplicationDbContext _context;

public NCDFormManagementModel(
    IWebHostEnvironment environment,
    ILogger<NCDFormManagementModel> logger,
    ApplicationDbContext context)
{
    _environment = environment;
    _logger = logger;
    _context = context;
}
```

### Save Method
```csharp
public async Task<IActionResult> OnPostSaveFormDataAsync(string fileName, string pageNumber, IFormCollection formData)
{
    // Process the form data and save to NCDRiskAssessments table
    var result = await SaveToNCDDatabase(formData, pageNumber);
    
    if (result.Success)
    {
        return new JsonResult(new 
        { 
            success = true, 
            message = $"NCD form data saved successfully to database for Page {pageNumber}",
            ncdId = result.NCDId
        });
    }
    else
    {
        return new JsonResult(new { success = false, message = result.ErrorMessage });
    }
}
```

### Database Save Method
```csharp
private async Task<(bool Success, int? NCDId, string ErrorMessage)> SaveToNCDDatabase(IFormCollection formData, string pageNumber)
{
    try
    {
        // Create new NCD Risk Assessment record
        var ncdAssessment = new NCDRiskAssessment
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AppointmentType = "NCD Assessment"
        };

        // Map form data to NCDRiskAssessment properties
        MapFormDataToNCD(formData, ncdAssessment);

        // Add to database
        _context.NCDRiskAssessments.Add(ncdAssessment);
        await _context.SaveChangesAsync();

        return (true, ncdAssessment.Id, null);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error saving NCD data to database: {ex.Message}");
        return (false, null, $"Database error: {ex.Message}");
    }
}
```

## Data Mapping Logic

### Checkbox Handling
```csharp
ncdAssessment.HasDiabetes = formData.ContainsKey("hasDiabetes");
```

### Radio Button Handling
```csharp
ncdAssessment.AlcoholConsumption = formData["drinksAlcohol"].ToString();
```

### Date Parsing
```csharp
if (DateTime.TryParse(formData["birthday"].ToString(), out DateTime birthday))
{
    ncdAssessment.Birthday = birthday;
}
```

### Number Parsing
```csharp
if (int.TryParse(formData["edad"].ToString(), out int age))
{
    ncdAssessment.Edad = age;
}
```

### Complex Logic (Smoking Status)
```csharp
if (formData["isSmoker"].ToString() == "Yes")
{
    ncdAssessment.SmokingStatus = "Current Smoker";
}
else if (formData["smokingQuitDuration"].ToString() == "LessThan1Year")
{
    ncdAssessment.SmokingStatus = "Quit Less Than 1 Year";
}
// ... more logic
```

## Error Handling

### Database Errors
- Connection issues
- Constraint violations
- Data type mismatches
- Transaction failures

### Form Data Errors
- Missing required fields
- Invalid data formats
- Parsing errors

### Logging
All database operations are logged with:
- User identity
- Form data details
- Success/failure status
- Error messages
- NCD record ID

## Response Format

### Success Response
```json
{
    "success": true,
    "message": "NCD form data saved successfully to database for Page 1",
    "ncdId": 123
}
```

### Error Response
```json
{
    "success": false,
    "message": "Database error: [specific error message]"
}
```

## Database Schema Requirements

### Required Fields
- `Id` (auto-generated)
- `CreatedAt` (auto-set)
- `UpdatedAt` (auto-set)
- `UserId` (from current user)
- `AppointmentId` (default: 0)

### Optional Fields
All other fields are optional and will be set to null/empty if not provided in the form.

## Future Enhancements

### Appointment Integration
- Link to actual appointment records
- Set proper `AppointmentId` based on context
- Associate with specific patient records

### Data Validation
- Add comprehensive validation rules
- Check for required field combinations
- Validate data ranges and formats

### Audit Trail
- Track form modifications
- Log all changes with timestamps
- Maintain version history

### Reporting Integration
- Generate NCD reports from saved data
- Export to various formats
- Statistical analysis capabilities

## Testing

### Unit Tests
- Test data mapping functions
- Validate form data parsing
- Check error handling scenarios

### Integration Tests
- Test database connectivity
- Verify data persistence
- Check transaction handling

### User Acceptance Tests
- Test complete form submission workflow
- Verify data accuracy in database
- Check error message display

## Security Considerations

### Data Validation
- Sanitize all input data
- Prevent SQL injection attacks
- Validate data types and ranges

### Access Control
- Admin-only access to form management
- Audit logging for all operations
- Secure database connections

### Data Privacy
- Encrypt sensitive health information
- Implement data retention policies
- Comply with health data regulations

---

*Last Updated: December 2024*
*Version: 1.0*
*Author: Barangay Health Care System Development Team*
