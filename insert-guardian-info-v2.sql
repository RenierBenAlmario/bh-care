-- Try inserting with explicit identity insert
SET IDENTITY_INSERT GuardianInformation ON;

INSERT INTO GuardianInformation (
    GuardianId,
    UserId, 
    GuardianFirstName, 
    GuardianLastName, 
    ResidencyProofPath, 
    FirstName,
    LastName,
    CreatedAt, 
    ProofType, 
    ConsentStatus
)
VALUES (
    1,
    'e346e6ca-0f97-432f-b1cf-b1fbec9422ae', 
    'Test Guardian', 
    'Test Last Name', 
    '/uploads/residency_proofs/test.jpg', 
    'Test Guardian',
    'Test Last Name',
    GETDATE(), 
    'GuardianResidencyProof', 
    'Pending'
);

SET IDENTITY_INSERT GuardianInformation OFF; 