-- Execute the stored procedure with the correct user ID
EXEC InsertGuardianInformation
    @UserId = '6346e6ca-0f97-432f-b1cf-b1fbec9422ae', -- Corrected ID
    @GuardianFirstName = 'Parent',
    @GuardianLastName = 'Guardian',
    @ResidencyProofPath = '/uploads/residency_proofs/test.jpg',
    @FirstName = 'Parent',
    @LastName = 'Guardian',
    @ProofType = 'GuardianResidencyProof',
    @ConsentStatus = 'Pending'; 