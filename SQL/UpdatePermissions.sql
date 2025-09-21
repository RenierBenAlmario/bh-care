-- Script to update permissions in the database with missing permissions for Doctor, Nurse, Prescriptions, and Records

-- Doctor permissions
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Access Doctor Dashboard')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Access Doctor Dashboard', 'Can access the doctor dashboard', 'Dashboard');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Write Prescriptions')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Write Prescriptions', 'Can write and update prescriptions', 'Medical Records');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Manage Consultations')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Manage Consultations', 'Can create and manage patient consultations', 'Medical Records');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'View Patient Details')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('View Patient Details', 'Can view detailed patient information', 'Medical Records');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Print Medical Records')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Print Medical Records', 'Can print patient medical records', 'Medical Records');
END

-- Nurse permissions
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Access Nurse Dashboard')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Access Nurse Dashboard', 'Can access the nurse dashboard', 'Dashboard');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Record Vital Signs')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Record Vital Signs', 'Can record patient vital signs', 'Medical Records');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Manage Patient Queue')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Manage Patient Queue', 'Can manage the patient queue', 'Patient Management');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'View Patient History')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('View Patient History', 'Can view patient medical history', 'Medical Records');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Manage Diagnoses')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Manage Diagnoses', 'Can create and manage diagnoses', 'Medical Records');
END

-- VitalSigns permissions
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Access Vital Signs')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Access Vital Signs', 'Can access the vital signs page', 'Vital Signs');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Record Vital Signs Data')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Record Vital Signs Data', 'Can record patient vital signs data', 'Vital Signs');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'View Vital Signs Data')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('View Vital Signs Data', 'Can view patient vital signs data', 'Vital Signs');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Delete Vital Signs Data')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Delete Vital Signs Data', 'Can delete patient vital signs records', 'Vital Signs');
END

-- Prescription permissions
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Create Prescriptions')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Create Prescriptions', 'Can create new prescriptions', 'Prescriptions');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'View Prescriptions')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('View Prescriptions', 'Can view patient prescriptions', 'Prescriptions');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Edit Prescriptions')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Edit Prescriptions', 'Can edit existing prescriptions', 'Prescriptions');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Delete Prescriptions')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Delete Prescriptions', 'Can delete prescriptions', 'Prescriptions');
END

-- Records permissions
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Edit Medical Records')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Edit Medical Records', 'Can edit existing medical records', 'Medical Records');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Delete Medical Records')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Delete Medical Records', 'Can delete medical records', 'Medical Records');
END

-- User Management permissions
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Approve Users')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Approve Users', 'Can approve user registrations', 'User Management');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Delete Users')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Delete Users', 'Can delete users from the system', 'User Management');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Manage Medical Records')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Manage Medical Records', 'Can create, edit, and delete medical records', 'Medical Records');
END

PRINT 'Permissions updated successfully';