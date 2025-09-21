-- Check user with the name Biggie Chunks
SELECT Id, UserName, Email, FirstName, LastName
FROM AspNetUsers
WHERE FirstName = 'Biggie' AND LastName = 'Chunks'; 