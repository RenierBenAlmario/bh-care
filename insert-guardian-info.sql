-- Insert a test guardian record for the user with ID from the screenshot
INSERT INTO GuardianInformation (
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
    'e346e6ca-0f97-432f-b1cf-b1fbec9422ae', -- User ID from screenshot
    'Test Guardian', 
    'Test Last Name', 
    '/uploads/residency_proofs/test.jpg', 
    'Test Guardian',
    'Test Last Name',
    GETDATE(), 
    'GuardianResidencyProof', 
    'Pending'
); 