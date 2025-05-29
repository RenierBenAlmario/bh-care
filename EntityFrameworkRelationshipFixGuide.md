# Entity Framework Core Relationship Fixes

This document explains the fixes applied to resolve Entity Framework Core relationship issues in the Barangay Health Center ASP.NET Core MVC project.

## Problems Fixed

### 1. Key Type Mismatch between Patient and FamilyMember

The `Patient` model was using `int Id` as its primary key while the `FamilyMember` model was referencing it with a `string PatientId`, causing an incompatible foreign key relationship.

**Solution**:
- Changed `Patient` model to use `string UserId` as its primary key instead of `int Id`
- Updated `FamilyMember` to use `string PatientId` to maintain a consistent key type
- Configured the relationship in `ApplicationDbContext` with proper navigation properties

### 2. Navigation Property Conflict in Prescription Model

The `Prescription` model had duplicate navigation properties (`PrescriptionMedications` and `Medications`) causing ambiguity.

**Solution**:
- Removed the duplicate property `PrescriptionMedications` 
- Kept only the `Medications` property to represent the relationship

### 3. Duplicate Method in UserApiController

The controller had two methods with route conflicts between `/patient/id/{userId}/medical-history` and `/patient/{patientId}/medical-history`.

**Solution**:
- Renamed the first method to `GetPatientMedicalHistoryById`
- Updated route pattern to avoid conflicts
- Fixed type mismatch in queries to use string for `PatientId`

### 4. Database Migration Issues

Foreign key constraints were preventing proper migration due to existing dependencies.

**Solution**:
- Created several migrations to fix the issues step by step:
  - `20250603000001_UpdateBirthDateToNonNullable`
  - `20250603000002_FixPrescriptionMedicationRelationship` 
  - `20250603000003_FixPatientFamilyMemberRelationship`
  - `20250603000004_FixPatientForeignKeyConstraints`
  - `20250603000005_FixRemainingForeignKeyConstraints`

- The approach involved:
  1. Temporarily making foreign key columns nullable
  2. Dropping existing foreign key constraints
  3. Setting up proper relationships with `OnDelete` behavior defined
  4. Cleaning up any orphaned references
  5. Re-establishing the foreign keys with correct configuration

### 5. Incorrect Navigation Property Types

The `Prescription` and `MedicalRecord` models were incorrectly referencing `ApplicationUser` instead of `Patient` in their navigation properties.

**Solution**:
- Updated the `Patient` navigation property in `Prescription` model to reference the `Patient` class
- Updated the `Patient` navigation property in `MedicalRecord` model to reference the `Patient` class
- Ensured consistent navigation property types across all related models

### 6. Invalid InverseProperty Attributes

The `ApplicationUser` class had `InverseProperty` attributes referencing navigation properties in `MedicalRecord`, `Prescription`, and `Appointment` classes that were changed to use the `Patient` class.

**Solution**:
- Removed the problematic inverse properties from `ApplicationUser`:
  - Removed `PatientMedicalRecords` (inverse to `MedicalRecord.Patient`)
  - Removed `PatientPrescriptions` (inverse to `Prescription.Patient`)
  - Removed `PatientAppointments` (inverse to `Appointment.Patient`)
- Updated `Appointment` class to use `Patient` instead of `ApplicationUser` in its navigation property
- Modified `ApplicationDbContext` to explicitly configure all relationships

### 7. Type Mismatch in AppointmentController

The `AppointmentController` was trying to assign `ApplicationUser` objects directly to the `Patient` navigation property of `Appointment` objects, which now expects `Patient` objects.

**Solution**:
- Updated `BookAppointment` and `Create` methods to query for a matching `Patient` record using the user ID
- Added logic to create a new `Patient` record if one doesn't exist for the current user
- Properly set the navigation properties with the correct object types

## Entity Relationships Fixed

1. **Patient ↔ FamilyMember**: One-to-many relationship properly configured
   - `Patient` can have many `FamilyMember`s
   - `FamilyMember` belongs to one `Patient`

2. **Patient ↔ VitalSign**: One-to-many relationship properly configured
   - `Patient` can have many `VitalSign`s
   - `VitalSign` belongs to one `Patient`

3. **Patient ↔ MedicalRecord**: One-to-many relationship properly configured
   - `Patient` can have many `MedicalRecord`s
   - `MedicalRecord` belongs to one `Patient`

4. **Patient ↔ Prescription**: One-to-many relationship properly configured
   - `Patient` can have many `Prescription`s
   - `Prescription` belongs to one `Patient`

5. **Patient ↔ Appointment**: One-to-many relationship properly configured
   - `Patient` can have many `Appointment`s
   - `Appointment` belongs to one `Patient`

6. **ApplicationUser (Doctor) ↔ MedicalRecord/Prescription/Appointment**: One-to-many relationships properly configured
   - `Doctor` can have many MedicalRecords/Prescriptions/Appointments
   - Each MedicalRecord/Prescription/Appointment belongs to one Doctor

7. **Prescription ↔ PrescriptionMedication**: One-to-many relationship properly configured
   - `Prescription` can have many `PrescriptionMedication`s
   - `PrescriptionMedication` belongs to one `Prescription`

## Applying the Fixes

To apply these fixes, run the following command:

```
dotnet ef database update
```

If you encounter any issues with foreign key constraints, use the provided PowerShell script:

```
.\apply_final_migration.ps1
```

For manual SQL fixes in case of persistent issues, refer to the SQL commands in the `fix-relationships.sql` script.

## Next Steps

After applying these fixes:

1. Verify that all relationships work correctly by testing CRUD operations
2. Check that navigation properties properly load related entities
3. Ensure that foreign key constraints are properly enforced
4. Test that the application works end-to-end without database errors 