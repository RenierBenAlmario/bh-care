document.addEventListener('DOMContentLoaded', function() {
    // Get password elements
    const passwordInput = document.getElementById('password');
    const confirmPasswordInput = document.getElementById('confirmPassword');
    
    // Get requirement elements
    const lengthReq = document.getElementById('length');
    const uppercaseReq = document.getElementById('uppercase');
    const lowercaseReq = document.getElementById('lowercase');
    const numberReq = document.getElementById('number');
    const specialReq = document.getElementById('special');
    
    // Get strength indicator
    const strengthText = document.querySelector('.password-strength-text');
    
    // Add event listener to password input
    if (passwordInput) {
        passwordInput.addEventListener('input', validatePassword);
    }
    
    // Add event listener to confirm password input
    if (confirmPasswordInput && passwordInput) {
        confirmPasswordInput.addEventListener('input', function() {
            validatePasswordMatch(passwordInput.value, confirmPasswordInput.value);
        });
    }
    
    // Function to validate password
    function validatePassword() {
        const password = passwordInput.value;
        
        // Check requirements
        const hasLength = password.length >= 8;
        const hasUppercase = /[A-Z]/.test(password);
        const hasLowercase = /[a-z]/.test(password);
        const hasNumber = /[0-9]/.test(password);
        const hasSpecial = /[^A-Za-z0-9]/.test(password);
        
        // Update requirement indicators
        updateRequirement(lengthReq, hasLength, 'At least 8 characters');
        updateRequirement(uppercaseReq, hasUppercase, 'At least 1 uppercase letter');
        updateRequirement(lowercaseReq, hasLowercase, 'At least 1 lowercase letter');
        updateRequirement(numberReq, hasNumber, 'At least 1 number');
        updateRequirement(specialReq, hasSpecial, 'At least 1 special character');
        
        // Calculate password strength
        let score = 0;
        if (hasLength) score++;
        if (hasUppercase) score++;
        if (hasLowercase) score++;
        if (hasNumber) score++;
        if (hasSpecial) score++;
        
        // All requirements met
        const allRequirementsMet = hasLength && hasUppercase && hasLowercase && hasNumber && hasSpecial;
        
        // Update strength indicator
        if (password.length === 0) {
            strengthText.textContent = 'Too weak';
            strengthText.style.color = '#dc3545'; // Red
        } else if (allRequirementsMet) {
            strengthText.textContent = 'Strong';
            strengthText.style.color = '#28a745'; // Green
        } else if (score >= 3) {
            strengthText.textContent = 'Medium';
            strengthText.style.color = '#ffc107'; // Yellow
        } else {
            strengthText.textContent = 'Weak';
            strengthText.style.color = '#dc3545'; // Red
        }
        
        // If confirm password is not empty, validate match
        if (confirmPasswordInput && confirmPasswordInput.value) {
            validatePasswordMatch(password, confirmPasswordInput.value);
        }
        
        return allRequirementsMet;
    }
    
    // Function to validate password match
    function validatePasswordMatch(password, confirmPassword) {
        const match = password === confirmPassword;
        
        // If both are empty, don't show any message
        if (!password && !confirmPassword) {
            return;
        }
        
        // If confirm password is empty, don't show any message
        if (!confirmPassword) {
            return;
        }
        
        // Add visual feedback
        if (match) {
            confirmPasswordInput.style.borderColor = '#28a745';
        } else {
            confirmPasswordInput.style.borderColor = '#dc3545';
        }
    }
    
    // Function to update requirement elements
    function updateRequirement(element, isValid, text) {
        if (!element) return;
        
        if (isValid) {
            element.innerHTML = `<i class="fas fa-check text-success"></i> ${text}`;
            element.style.color = '#28a745'; // Green color for success
        } else {
            element.innerHTML = `<i class="fas fa-times text-danger"></i> ${text}`;
            element.style.color = '#dc3545'; // Red color for error
        }
    }
    
    // Function to override form submission
    if (document.getElementById('registrationForm')) {
        document.getElementById('registrationForm').addEventListener('submit', function(e) {
            // Validate password before submitting
            if (!validatePassword()) {
                e.preventDefault();
                alert('Please ensure password meets all requirements');
            }
        });
    }
}); 