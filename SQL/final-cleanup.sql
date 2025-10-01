-- Final Database Cleanup Script
-- This script removes all data except the system admin account
-- Admin account: admin@example.com (preserved)

USE [bhcareDB]
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

PRINT 'Starting final database cleanup...';
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
    
    -- Clear all related data first to avoid foreign key constraints
    PRINT 'Clearing all related data...';
    
    -- Clear Identity-related tables first
    DELETE FROM AspNetUserClaims WHERE UserId != @AdminUserId;
    DELETE FROM AspNetUserLogins WHERE UserId != @AdminUserId;
    DELETE FROM AspNetUserTokens WHERE UserId != @AdminUserId;
    DELETE FROM AspNetUserRoles WHERE UserId != @AdminUserId;
    
    -- Clear user-related data
    DELETE FROM UserDocuments WHERE UserId != @AdminUserId;
    DELETE FROM UserPermissions WHERE UserId != @AdminUserId;
    DELETE FROM FeedbackRatings WHERE UserId != @AdminUserId;
    DELETE FROM Feedbacks WHERE UserId != @AdminUserId;
    DELETE FROM Notifications WHERE UserId != @AdminUserId;
    DELETE FROM Messages WHERE SenderId != @AdminUserId AND ReceiverId != @AdminUserId;
    DELETE FROM EmailVerifications WHERE Email != 'admin@example.com';
    DELETE FROM PasswordResetOTPs WHERE UserId != @AdminUserId;
    DELETE FROM UrlTokens;
    
    -- Clear all patient-related data
    DELETE FROM PatientHistories;
    DELETE FROM LabResults;
    DELETE FROM MedicalHistories;
    DELETE FROM VitalSigns;
    DELETE FROM PrescriptionMedications;
    DELETE FROM Prescriptions;
    DELETE FROM MedicalRecords;
    DELETE FROM FamilyMembers;
    DELETE FROM FamilyRecords;
    DELETE FROM GuardianInformation;
    DELETE FROM Patients;
    
    -- Clear appointment-related data
    DELETE FROM AppointmentFiles;
    DELETE FROM AppointmentAttachments;
    DELETE FROM Appointments;
    DELETE FROM ConsultationTimeSlots;
    DELETE FROM DoctorAvailabilities;
    
    -- Clear assessment data
    DELETE FROM IntegratedAssessments;
    DELETE FROM AdolescentHealthInfo;
    DELETE FROM NCDRiskAssessments;
    DELETE FROM HEEADSSSAssessments;
    DELETE FROM Assessments;
    
    -- Clear immunization data
    DELETE FROM ImmunizationShortcutForms;
    DELETE FROM ImmunizationRecords;
    
    -- Clear staff and doctor data
    DELETE FROM StaffPermissions;
    DELETE FROM StaffMembers;
    DELETE FROM Doctors;
    
    -- Clear health reports
    DELETE FROM HealthReports;
    
    -- Clear role permissions
    DELETE FROM RolePermissions;
    DELETE FROM StaffPositionPermission;
    
    -- Clear medications
    DELETE FROM Medications;
    
    -- Clear User table (if exists)
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'User')
    BEGIN
        DELETE FROM [User];
    END
    
    -- Now clear all users except admin
    PRINT 'Clearing all users except admin...';
    DELETE FROM AspNetUsers WHERE Id != @AdminUserId;
    PRINT 'Cleared all users (preserved admin account)';
    
    -- Reset admin user number to 1
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'UserNumber')
    BEGIN
        UPDATE AspNetUsers SET UserNumber = 1 WHERE Id = @AdminUserId;
        PRINT 'Reset admin user number to 1';
    END
    
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
    
    IF @AdminEmail IS NOT NULL AND @TotalUsers = 1
    BEGIN
        PRINT 'Admin account verified: ' + @AdminEmail + ' (Status: ' + ISNULL(@AdminStatus, 'Unknown') + ')';
        PRINT 'Total users remaining: ' + CAST(@TotalUsers AS NVARCHAR);
    END
    ELSE
    BEGIN
        PRINT 'ERROR: Admin account verification failed!';
        PRINT 'Admin Email: ' + ISNULL(@AdminEmail, 'NULL');
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
END CATCH

PRINT 'Database cleanup script execution completed.';

