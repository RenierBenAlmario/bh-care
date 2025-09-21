-- Ensure ResidencyProofPath has values
UPDATE GuardianInformation
SET ResidencyProofPath = ''
WHERE ResidencyProofPath IS NULL; 