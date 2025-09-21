-- Update GuardianFirstName with FirstName values if null
UPDATE GuardianInformation
SET GuardianFirstName = FirstName
WHERE GuardianFirstName IS NULL;

-- Update GuardianLastName with LastName values if null
UPDATE GuardianInformation
SET GuardianLastName = LastName
WHERE GuardianLastName IS NULL;

-- Set default values for empty/null fields
UPDATE GuardianInformation
SET 
    GuardianFirstName = CASE WHEN GuardianFirstName IS NULL THEN 'Unknown' ELSE GuardianFirstName END,
    GuardianLastName = CASE WHEN GuardianLastName IS NULL THEN 'Unknown' ELSE GuardianLastName END,
    ConsentStatus = CASE WHEN ConsentStatus IS NULL THEN 'Pending' ELSE ConsentStatus END,
    ProofType = CASE WHEN ProofType IS NULL THEN 'GuardianResidencyProof' ELSE ProofType END;

-- Make nullable columns NOT NULL with default values
ALTER TABLE GuardianInformation
ALTER COLUMN GuardianFirstName NVARCHAR(100) NOT NULL;

ALTER TABLE GuardianInformation
ALTER COLUMN GuardianLastName NVARCHAR(100) NOT NULL;

ALTER TABLE GuardianInformation
ALTER COLUMN ConsentStatus NVARCHAR(50) NOT NULL;

ALTER TABLE GuardianInformation
ALTER COLUMN ProofType NVARCHAR(50) NOT NULL; 