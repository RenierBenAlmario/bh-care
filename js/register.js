document.addEventListener('DOMContentLoaded', function() {
    const registrationForm = document.getElementById('registrationForm');
    const successModal = document.getElementById('successModal');
    const closeModalBtn = document.querySelector('.close-modal');
    
    // Form validation and submission
    registrationForm.addEventListener('submit', function(e) {
        e.preventDefault();
        
        // Get form data
        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;
        const confirmPassword = document.getElementById('confirmPassword').value;
        const fullName = document.getElementById('fullName').value;
        const email = document.getElementById('email').value;
        const contactNumber = document.getElementById('contactNumber').value;
        const address = document.getElementById('address').value;
        const residencyProofInput = document.getElementById('residencyProof');
        const termsAgreement = document.getElementById('termsAgreement').checked;
        const privacyAgreement = document.getElementById('privacyAgreement').checked;
        
        // Validate form
        if (password !== confirmPassword) {
            alert('Passwords do not match!');
            return;
        }
        
        if (!termsAgreement || !privacyAgreement) {
            alert('You must agree to the terms and privacy policy!');
            return;
        }
        
        // Get file name (simulating file upload)
        let residencyProof = '';
        if (residencyProofInput.files.length > 0) {
            residencyProof = residencyProofInput.files[0].name;
        } else {
            alert('Please upload proof of residency!');
            return;
        }
        
        // Create user object
        const user = {
            username,
            password, // In a real app, this would be hashed
            fullName,
            email,
            contactNumber,
            address,
            residencyProof
        };
        
        // Add user to database
        DB.addUser(user);
        
        // Show success modal
        successModal.style.display = 'flex';
        
        // Reset form
        registrationForm.reset();
    });
    
    // Close modal event
    closeModalBtn.addEventListener('click', function() {
        successModal.style.display = 'none';
        // Redirect to home page after closing modal
        window.location.href = 'index.html';
    });
    
    // Close modal if clicked outside
    window.addEventListener('click', function(event) {
        if (event.target === successModal) {
            successModal.style.display = 'none';
            // Redirect to home page after closing modal
            window.location.href = 'index.html';
        }
    });
}); 