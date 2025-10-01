-- Complete Database Cleanup Script
-- This script removes all data except the system admin account
-- Admin account: admin@example.com (preserved)

USE [bhcareDB]
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

PRINT 'Starting complete database cleanup...';
PRINT 'Preserving system admin account: admin@example.com';

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- Get admin user ID for reference
    DECLARE @AdminUserId NVARCHAR(450);
    SELECT @AdminUserId = Id FROM AspNetUsers WHERE NormalizedEmail = 'ADMIN@EXAMPLE.COM';
    
    IF @AdminUserId IS NULL
    BEGIN
        PRINT 'ERROR: Admin account not found! Aborting cleanup for safety.';
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    PRINT 'Admin account found with ID: ' + @AdminUserId;
    
    -- Disable foreign key checks temporarily (Azure SQL compatible)
    DECLARE @sql NVARCHAR(MAX) = '';
    SELECT @sql = @sql + 'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(name) + ' NOCHECK CONSTRAINT ALL; '
    FROM sys.tables;
    EXEC sp_executesql @sql;
    
    -- Clear all user-related data (except admin)
    PRINT 'Clearing user-related data...';
    
    -- Clear user documents
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'UserDocuments')
    AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('UserDocuments') AND name = 'UserId')
    BEGIN
        DELETE FROM UserDocuments WHERE UserId != @AdminUserId;
        PRINT 'Cleared UserDocuments (preserved admin data)';
    END
    
    -- Clear user permissions
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'UserPermissions')
    AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('UserPermissions') AND name = 'UserId')
    BEGIN
        DELETE FROM UserPermissions WHERE UserId != @AdminUserId;
        PRINT 'Cleared UserPermissions (preserved admin data)';
    END
    
    -- Clear feedback ratings
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'FeedbackRatings')
    AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FeedbackRatings') AND name = 'UserId')
    BEGIN
        DELETE FROM FeedbackRatings WHERE UserId != @AdminUserId;
        PRINT 'Cleared FeedbackRatings (preserved admin data)';
    END
    
    -- Clear feedback
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Feedbacks')
    AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Feedbacks') AND name = 'UserId')
    BEGIN
        DELETE FROM Feedbacks WHERE UserId != @AdminUserId;
        PRINT 'Cleared Feedbacks (preserved admin data)';
    END
    
    -- Clear notifications
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Notifications')
    AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Notifications') AND name = 'UserId')
    BEGIN
        DELETE FROM Notifications WHERE UserId != @AdminUserId;
        PRINT 'Cleared Notifications (preserved admin data)';
    END
    
    -- Clear messages
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Messages')
    AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Messages') AND name = 'SenderId')
    AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Messages') AND name = 'ReceiverId')
    BEGIN
        DELETE FROM Messages WHERE SenderId != @AdminUserId AND ReceiverId != @AdminUserId;
        PRINT 'Cleared Messages (preserved admin data)';
    END
    
    -- Clear email verifications
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EmailVerifications')
    AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmailVerifications') AND name = 'Email')
    BEGIN
        DELETE FROM EmailVerifications WHERE Email != 'admin@example.com';
        PRINT 'Cleared EmailVerifications (preserved admin data)';
    END
    
    -- Clear password reset OTPs
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'PasswordResetOTPs')
    AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PasswordResetOTPs') AND name = 'UserId')
    BEGIN
        DELETE FROM PasswordResetOTPs WHERE UserId != @AdminUserId;
        PRINT 'Cleared PasswordResetOTPs (preserved admin data)';
    END
    
    -- Clear URL tokens
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'UrlTokens')
    BEGIN
        DELETE FROM UrlTokens;
        PRINT 'Cleared UrlTokens';
    END
    
    -- Clear all patient-related data
    PRINT 'Clearing patient-related data...';
    
    -- Clear patient histories
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'PatientHistories')
    BEGIN
        DELETE FROM PatientHistories;
        PRINT 'Cleared PatientHistories';
    END
    
    -- Clear lab results
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LabResults')
    BEGIN
        DELETE FROM LabResults;
        PRINT 'Cleared LabResults';
    END
    
    -- Clear medical histories
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'MedicalHistories')
    BEGIN
        DELETE FROM MedicalHistories;
        PRINT 'Cleared MedicalHistories';
    END
    
    -- Clear vital signs
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'VitalSigns')
    BEGIN
        DELETE FROM VitalSigns;
        PRINT 'Cleared VitalSigns';
    END
    
    -- Clear prescription medications
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'PrescriptionMedications')
    BEGIN
        DELETE FROM PrescriptionMedications;
        PRINT 'Cleared PrescriptionMedications';
    END
    
    -- Clear prescriptions
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Prescriptions')
    BEGIN
        DELETE FROM Prescriptions;
        PRINT 'Cleared Prescriptions';
    END
    
    -- Clear medical records
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'MedicalRecords')
    BEGIN
        DELETE FROM MedicalRecords;
        PRINT 'Cleared MedicalRecords';
    END
    
    -- Clear family members
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'FamilyMembers')
    BEGIN
        DELETE FROM FamilyMembers;
        PRINT 'Cleared FamilyMembers';
    END
    
    -- Clear family records
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'FamilyRecords')
    BEGIN
        DELETE FROM FamilyRecords;
        PRINT 'Cleared FamilyRecords';
    END
    
    -- Clear guardian information
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GuardianInformation')
    BEGIN
        DELETE FROM GuardianInformation;
        PRINT 'Cleared GuardianInformation';
    END
    
    -- Clear patients
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Patients')
    BEGIN
        DELETE FROM Patients;
        PRINT 'Cleared Patients';
    END
    
    -- Clear appointment-related data
    PRINT 'Clearing appointment-related data...';
    
    -- Clear appointment files
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AppointmentFiles')
    BEGIN
        DELETE FROM AppointmentFiles;
        PRINT 'Cleared AppointmentFiles';
    END
    
    -- Clear appointment attachments
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AppointmentAttachments')
    BEGIN
        DELETE FROM AppointmentAttachments;
        PRINT 'Cleared AppointmentAttachments';
    END
    
    -- Clear appointments
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Appointments')
    BEGIN
        DELETE FROM Appointments;
        PRINT 'Cleared Appointments';
    END
    
    -- Clear consultation time slots
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ConsultationTimeSlots')
    BEGIN
        DELETE FROM ConsultationTimeSlots;
        PRINT 'Cleared ConsultationTimeSlots';
    END
    
    -- Clear doctor availabilities
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DoctorAvailabilities')
    BEGIN
        DELETE FROM DoctorAvailabilities;
        PRINT 'Cleared DoctorAvailabilities';
    END
    
    -- Clear assessment data
    PRINT 'Clearing assessment data...';
    
    -- Clear integrated assessments
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'IntegratedAssessments')
    BEGIN
        DELETE FROM IntegratedAssessments;
        PRINT 'Cleared IntegratedAssessments';
    END
    
    -- Clear adolescent health info
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AdolescentHealthInfo')
    BEGIN
        DELETE FROM AdolescentHealthInfo;
        PRINT 'Cleared AdolescentHealthInfo';
    END
    
    -- Clear NCD risk assessments
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'NCDRiskAssessments')
    BEGIN
        DELETE FROM NCDRiskAssessments;
        PRINT 'Cleared NCDRiskAssessments';
    END
    
    -- Clear HEEADSSS assessments
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'HEEADSSSAssessments')
    BEGIN
        DELETE FROM HEEADSSSAssessments;
        PRINT 'Cleared HEEADSSSAssessments';
    END
    
    -- Clear assessments
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Assessments')
    BEGIN
        DELETE FROM Assessments;
        PRINT 'Cleared Assessments';
    END
    
    -- Clear immunization data
    PRINT 'Clearing immunization data...';
    
    -- Clear immunization shortcut forms
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ImmunizationShortcutForms')
    BEGIN
        DELETE FROM ImmunizationShortcutForms;
        PRINT 'Cleared ImmunizationShortcutForms';
    END
    
    -- Clear immunization records
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ImmunizationRecords')
    BEGIN
        DELETE FROM ImmunizationRecords;
        PRINT 'Cleared ImmunizationRecords';
    END
    
    -- Clear staff and doctor data
    PRINT 'Clearing staff and doctor data...';
    
    -- Clear staff permissions
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'StaffPermissions')
    BEGIN
        DELETE FROM StaffPermissions;
        PRINT 'Cleared StaffPermissions';
    END
    
    -- Clear staff members
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'StaffMembers')
    BEGIN
        DELETE FROM StaffMembers;
        PRINT 'Cleared StaffMembers';
    END
    
    -- Clear doctors
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Doctors')
    BEGIN
        DELETE FROM Doctors;
        PRINT 'Cleared Doctors';
    END
    
    -- Clear health reports
    PRINT 'Clearing health reports...';
    
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'HealthReports')
    BEGIN
        DELETE FROM HealthReports;
        PRINT 'Cleared HealthReports';
    END
    
    -- Clear all users except admin
    PRINT 'Clearing all users except admin...';
    
    -- Remove user roles for non-admin users
    DELETE FROM AspNetUserRoles WHERE UserId != @AdminUserId;
    PRINT 'Cleared user roles (preserved admin roles)';
    
    -- Remove all users except admin (using ID comparison)
    DELETE FROM AspNetUsers WHERE Id != @AdminUserId;
    PRINT 'Cleared all users (preserved admin account)';
    
    -- Reset identity columns for clean numbering
    PRINT 'Resetting identity columns...';
    
    -- Reset user number sequence (if exists)
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'UserNumber')
    BEGIN
        -- Update admin user number to 1
        UPDATE AspNetUsers SET UserNumber = 1 WHERE Id = @AdminUserId;
        PRINT 'Reset admin user number to 1';
    END
    
    -- Re-enable foreign key checks (Azure SQL compatible)
    SET @sql = '';
    SELECT @sql = @sql + 'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(name) + ' WITH CHECK CHECK CONSTRAINT ALL; '
    FROM sys.tables;
    EXEC sp_executesql @sql;
    
    -- Verify admin account is still intact
    PRINT 'Verifying admin account integrity...';
    
    DECLARE @AdminEmail NVARCHAR(256);
    DECLARE @AdminStatus NVARCHAR(50);
    DECLARE @AdminRoleCount INT;
    DECLARE @TotalUsers INT;
    
    SELECT @AdminEmail = Email, @AdminStatus = Status 
    FROM AspNetUsers 
    WHERE Id = @AdminUserId;
    
    SELECT @AdminRoleCount = COUNT(*) 
    FROM AspNetUserRoles 
    WHERE UserId = @AdminUserId;
    
    SELECT @TotalUsers = COUNT(*) FROM AspNetUsers;
    
    IF @AdminEmail IS NOT NULL AND @AdminRoleCount > 0 AND @TotalUsers = 1
    BEGIN
        PRINT 'Admin account verified: ' + @AdminEmail + ' (Status: ' + ISNULL(@AdminStatus, 'Unknown') + ', Roles: ' + CAST(@AdminRoleCount AS NVARCHAR) + ')';
        PRINT 'Total users remaining: ' + CAST(@TotalUsers AS NVARCHAR);
    END
    ELSE
    BEGIN
        PRINT 'ERROR: Admin account verification failed!';
        PRINT 'Admin Email: ' + ISNULL(@AdminEmail, 'NULL');
        PRINT 'Admin Roles: ' + CAST(@AdminRoleCount AS NVARCHAR);
        PRINT 'Total Users: ' + CAST(@TotalUsers AS NVARCHAR);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    -- Final cleanup statistics
    PRINT 'Cleanup completed successfully!';
    PRINT 'Database has been reset to initial state with only the system admin account preserved.';
    PRINT 'Admin account: ' + @AdminEmail;
    PRINT 'All other user accounts, patient data, appointments, and forms have been cleared.';
    
    COMMIT TRANSACTION;
    
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
        
    PRINT 'ERROR: Database cleanup failed!';
    PRINT 'Error Message: ' + ERROR_MESSAGE();
    PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS NVARCHAR);
    PRINT 'Error Severity: ' + CAST(ERROR_SEVERITY() AS NVARCHAR);
    PRINT 'Error State: ' + CAST(ERROR_STATE() AS NVARCHAR);
    PRINT 'Error Line: ' + CAST(ERROR_LINE() AS NVARCHAR);
    
    -- Re-enable foreign key checks in case of error (Azure SQL compatible)
    SET @sql = '';
    SELECT @sql = @sql + 'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(name) + ' WITH CHECK CHECK CONSTRAINT ALL; '
    FROM sys.tables;
    EXEC sp_executesql @sql;
END CATCH

PRINT 'Database cleanup script execution completed.';

