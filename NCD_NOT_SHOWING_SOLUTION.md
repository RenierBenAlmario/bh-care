# NCD Risk Assessment Not Showing - Complete Diagnosis & Solution

## 🔍 **PROBLEM DIAGNOSIS**

The NCD Risk Assessment is not showing up in the appointment details page because:

1. **No Database Record**: There's no NCD Risk Assessment record in the database for appointment ID 61
2. **Missing Database Field**: The `HasStrokeSymptoms` field needs to be added to the database
3. **Data Creation Issue**: The assessment might not have been properly created or linked to the appointment

## 🔧 **COMPLETE SOLUTION STEPS**

### **Step 1: Run Database Migration**
First, add the missing `HasStrokeSymptoms` field to the database:

```sql
-- Execute this SQL script in your database
ALTER TABLE [dbo].[NCDRiskAssessments]
ADD [HasStrokeSymptoms] NVARCHAR(4000) NULL;

-- Set default value for existing records
UPDATE [dbo].[NCDRiskAssessments] 
SET [HasStrokeSymptoms] = 'Hindi' 
WHERE [HasStrokeSymptoms] IS NULL;
```

### **Step 2: Check Existing Data**
Run this query to check if there are any NCD Risk Assessment records:

```sql
-- Check for NCD Risk Assessment records
SELECT 
    Id, 
    UserId, 
    AppointmentId, 
    FirstName, 
    LastName, 
    CreatedAt,
    UpdatedAt
FROM [dbo].[NCDRiskAssessments] 
WHERE AppointmentId = 61;

-- Check all NCD Risk Assessment records
SELECT 
    Id, 
    UserId, 
    AppointmentId, 
    FirstName, 
    LastName, 
    CreatedAt
FROM [dbo].[NCDRiskAssessments] 
ORDER BY CreatedAt DESC;
```

### **Step 3: Create NCD Risk Assessment (If None Exists)**
If no record exists, you need to create one. There are two ways:

#### **Option A: Use the User Form**
1. Go to the user-facing NCD Risk Assessment form
2. Fill out the form with appointment ID 61
3. Submit the form to create the database record

#### **Option B: Use the Nurse Create Form**
1. Go to the appointment details page
2. Click the "**+ Create New Assessment**" button
3. Fill out the nurse form to create the assessment

### **Step 4: Verify Data Creation**
After creating the assessment, check the database again:

```sql
-- Verify the record was created
SELECT 
    Id, 
    UserId, 
    AppointmentId, 
    FirstName, 
    LastName, 
    HasChestPain,
    HasStrokeSymptoms,
    CreatedAt
FROM [dbo].[NCDRiskAssessments] 
WHERE AppointmentId = 61;
```

## 🎯 **EXPECTED RESULTS**

### **Before Fix:**
- Appointment details shows "No NCD Risk Assessment data available"
- EditNCDAssessment page shows empty form
- No database record exists

### **After Fix:**
- Appointment details shows "NCD Risk Assessment" as available/completed
- EditNCDAssessment page shows existing data
- Database record exists with proper data

## 🚨 **TROUBLESHOOTING**

### **If Still Not Showing After Database Migration:**
1. **Check Database Connection**: Ensure the application can connect to the database
2. **Check Appointment ID**: Verify appointment ID 61 exists in the Appointments table
3. **Check User ID**: Ensure the user has proper permissions
4. **Check Logs**: Look at server logs for any errors during data loading

### **If Data Exists But Not Loading:**
1. **Check Encryption**: Ensure data is properly encrypted/decrypted
2. **Check Field Mapping**: Verify all fields are properly mapped
3. **Check Normalization**: Ensure boolean values are properly normalized

## 📋 **DEBUGGING QUERIES**

### **Check Appointment Exists:**
```sql
SELECT Id, PatientId, Status, CreatedAt 
FROM [dbo].[Appointments] 
WHERE Id = 61;
```

### **Check Patient Exists:**
```sql
SELECT p.Id, u.FirstName, u.LastName 
FROM [dbo].[Patients] p
INNER JOIN [dbo].[Users] u ON p.UserId = u.Id
INNER JOIN [dbo].[Appointments] a ON a.PatientId = p.Id
WHERE a.Id = 61;
```

### **Check All NCD Records:**
```sql
SELECT 
    Id, 
    UserId, 
    AppointmentId, 
    FirstName, 
    LastName, 
    CreatedAt,
    UpdatedAt
FROM [dbo].[NCDRiskAssessments] 
ORDER BY CreatedAt DESC;
```

## 🔄 **NEXT STEPS**

1. **Run the database migration** to add the `HasStrokeSymptoms` field
2. **Check if NCD record exists** for appointment ID 61
3. **Create NCD record** if none exists (using user form or nurse create form)
4. **Test the edit form** to ensure data loads properly
5. **Verify all fields** are working correctly

The root cause is likely that no NCD Risk Assessment record exists in the database for this appointment. Once you create the record (either through the user form or nurse create form), the edit functionality should work properly.
