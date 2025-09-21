-- Create a stored procedure to insert a guardian record
CREATE OR ALTER PROCEDURE InsertGuardianInformation
    @UserId NVARCHAR(450),
    @GuardianFirstName NVARCHAR(100),
    @GuardianLastName NVARCHAR(100),
    @ResidencyProofPath NVARCHAR(MAX),
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @ProofType NVARCHAR(50),
    @ConsentStatus NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO GuardianInformation (
        UserId,
        FirstName,
        LastName,
        ResidencyProofPath,
        CreatedAt,
        GuardianFirstName,
        GuardianLastName,
        ProofType,
        ConsentStatus
    )
    VALUES (
        @UserId,
        @FirstName,
        @LastName,
        @ResidencyProofPath,
        GETDATE(),
        @GuardianFirstName,
        @GuardianLastName,
        @ProofType,
        @ConsentStatus
    );
    
    SELECT SCOPE_IDENTITY() AS GuardianId;
END;
GO

-- Execute the stored procedure
EXEC InsertGuardianInformation
    @UserId = 'e346e6ca-0f97-432f-b1cf-b1fbec9422ae',
    @GuardianFirstName = 'Parent',
    @GuardianLastName = 'Guardian',
    @ResidencyProofPath = '/uploads/residency_proofs/test.jpg',
    @FirstName = 'Parent',
    @LastName = 'Guardian',
    @ProofType = 'GuardianResidencyProof',
    @ConsentStatus = 'Pending'; 