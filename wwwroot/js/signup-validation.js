/**
 * Custom validation for the Barangay signup form
 * This script adds enhanced client-side validation for specific fields
 */
document.addEventListener('DOMContentLoaded', function() {
    // Get the form and input elements
    const form = document.getElementById('signupForm');
    if (!form) return;
    
    // Input fields
    const emailInput = document.querySelector('input[name="Input.Email"]');
    const contactNumberInput = document.querySelector('input[name="Input.ContactNumber"]');
    const firstNameInput = document.querySelector('input[name="Input.FirstName"]');
    const middleNameInput = document.querySelector('input[name="Input.MiddleName"]');
    const lastNameInput = document.querySelector('input[name="Input.LastName"]');
    const suffixInput = document.querySelector('input[name="Input.Suffix"]');
    const birthDateInput = document.querySelector('input[name="Input.BirthDate"]');
    const guardianFullNameInput = document.querySelector('input[name="Input.GuardianFullName"]');
    const guardianContactNumberInput = document.querySelector('input[name="Input.GuardianContactNumber"]');
    
    // Validation patterns
    const patterns = {
        email: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
        contactNumber: /^(09\d{9}|\+639\d{9})$/,
        name: /^[A-Za-z\s\-']+$/
    };
    
    // Custom error messages
    const errorMessages = {
        email: 'Invalid email address format',
        contactNumber: 'Contact number must be in format 09XXXXXXXXX or +639XXXXXXXXX',
        name: 'Only letters, spaces, hyphens, and apostrophes are allowed',
        birthDate: 'Birth date cannot be in the future or too far in the past',
        guardianRequired: 'Guardian information is required for users under 18',
    };
    
    // Validate email format
    if (emailInput) {
        emailInput.addEventListener('input', function() {
            validateEmailFormat(this);
        });
        
        emailInput.addEventListener('blur', function() {
            validateEmailFormat(this);
        });
    }
    
    // Format and validate contact number
    if (contactNumberInput) {
        contactNumberInput.addEventListener('input', function() {
            formatAndValidateContactNumber(this);
        });
        
        contactNumberInput.addEventListener('blur', function() {
            formatAndValidateContactNumber(this);
        });
    }
    
    // Validate name fields (only letters, hyphens, apostrophes, and spaces)
    [firstNameInput, middleNameInput, lastNameInput, suffixInput, guardianFullNameInput].forEach(input => {
        if (input) {
            input.addEventListener('input', function() {
                validateNameField(this);
            });
            
            input.addEventListener('blur', function() {
                validateNameField(this);
            });
        }
    });
    
    // Validate birth date (prevent future dates and very old dates)
    if (birthDateInput) {
        birthDateInput.addEventListener('change', function() {
            validateBirthDate(this);
        });
        
        birthDateInput.addEventListener('blur', function() {
            validateBirthDate(this);
        });
    }
    
    // Format and validate guardian contact number
    if (guardianContactNumberInput) {
        guardianContactNumberInput.addEventListener('input', function() {
            formatAndValidateContactNumber(this);
        });
        
        guardianContactNumberInput.addEventListener('blur', function() {
            formatAndValidateContactNumber(this);
        });
    }
    
    // Form submission validation
    if (form) {
        form.addEventListener('submit', function(event) {
            let isValid = true;
            
            // Validate all fields before submission
            if (emailInput && !validateEmailFormat(emailInput, true)) {
                isValid = false;
            }
            
            if (contactNumberInput && !formatAndValidateContactNumber(contactNumberInput, true)) {
                isValid = false;
            }
            
            [firstNameInput, middleNameInput, lastNameInput, suffixInput, guardianFullNameInput].forEach(input => {
                if (input && input.value && !validateNameField(input, true)) {
                    isValid = false;
                }
            });
            
            if (birthDateInput && !validateBirthDate(birthDateInput, true)) {
                isValid = false;
            }
            
            // Check if guardian info is required based on age
            if (birthDateInput && birthDateInput.value) {
                const isMinor = getAge(birthDateInput.value) < 18;
                if (isMinor) {
                    if (guardianFullNameInput && !guardianFullNameInput.value.trim()) {
                        showError(guardianFullNameInput, errorMessages.guardianRequired);
                        isValid = false;
                    }
                    
                    if (guardianContactNumberInput && !guardianContactNumberInput.value.trim()) {
                        showError(guardianContactNumberInput, errorMessages.guardianRequired);
                        isValid = false;
                    } else if (guardianContactNumberInput.value && !patterns.contactNumber.test(guardianContactNumberInput.value)) {
                        showError(guardianContactNumberInput, errorMessages.contactNumber);
                        isValid = false;
                    }
                }
            }
            
            if (!isValid) {
                event.preventDefault();
            }
        });
    }
    
    // Function to validate email format
    function validateEmailFormat(input, isSubmit = false) {
        const email = input.value.trim();
        
        // Skip validation if empty and not submitting
        if (!email && !isSubmit) {
            clearError(input);
            return true;
        }
        
        // Basic validation - check if contains @ and .
        if (!email.includes('@') || !email.includes('.')) {
            showError(input, errorMessages.email);
            return false;
        }
        
        // Advanced validation with regex
        if (!patterns.email.test(email)) {
            showError(input, errorMessages.email);
            return false;
        }
        
        clearError(input);
        return true;
    }
    
    // Function to format and validate contact number
    function formatAndValidateContactNumber(input, isSubmit = false) {
        let number = input.value.trim();
        
        // Skip validation if empty and not submitting
        if (!number && !isSubmit) {
            clearError(input);
            return true;
        }
        
        // Format the phone number as the user types
        if (number.startsWith('09')) {
            // Allow only digits for 09XXXXXXXXX format
            number = number.replace(/[^\d]/g, '');
            input.value = number;
        } else if (number.startsWith('+')) {
            // Keep + sign at beginning and only digits afterward
            const plusSign = number.charAt(0);
            const digitsOnly = number.substring(1).replace(/[^\d]/g, '');
            number = plusSign + digitsOnly;
            input.value = number;
        } else if (number && !number.startsWith('09') && !number.startsWith('+')) {
            // Automatically add 09 prefix for convenience
            number = '09' + number.replace(/[^\d]/g, '');
            input.value = number;
        }
        
        // Validate against the required pattern
        if (!patterns.contactNumber.test(number)) {
            showError(input, errorMessages.contactNumber);
            return false;
        }
        
        clearError(input);
        return true;
    }
    
    // Function to validate name fields
    function validateNameField(input, isSubmit = false) {
        const name = input.value.trim();
        
        // Skip validation if empty and not submitting
        if (!name && !isSubmit) {
            clearError(input);
            return true;
        }
        
        // Validate name field - only letters, spaces, hyphens, and apostrophes
        if (!patterns.name.test(name)) {
            showError(input, errorMessages.name);
            return false;
        }
        
        clearError(input);
        return true;
    }
    
    // Function to validate birth date
    function validateBirthDate(input, isSubmit = false) {
        const dateValue = input.value;
        
        // Skip validation if empty and not submitting
        if (!dateValue && !isSubmit) {
            clearError(input);
            return true;
        }
        
        const selectedDate = new Date(dateValue);
        const today = new Date();
        const minDate = new Date();
        minDate.setFullYear(today.getFullYear() - 120); // Max age 120 years
        
        // Check if date is valid
        if (isNaN(selectedDate.getTime())) {
            showError(input, 'Please enter a valid date');
            return false;
        }
        
        // Check if date is in the future
        if (selectedDate > today) {
            showError(input, 'Birth date cannot be in the future');
            return false;
        }
        
        // Check if date is too far in the past
        if (selectedDate < minDate) {
            showError(input, 'Birth date is too far in the past');
            return false;
        }
        
        // Toggle guardian section based on age
        const age = getAge(dateValue);
        const guardianSection = document.getElementById('guardianInfoSection');
        const guardianRequired = document.querySelectorAll('.guardian-required');
        
        if (guardianSection) {
            if (age < 18) {
                guardianSection.classList.remove('d-none');
                guardianRequired.forEach(elem => elem.classList.remove('d-none'));
                
                // Make guardian fields required for minors
                if (guardianFullNameInput) guardianFullNameInput.setAttribute('required', 'required');
                if (guardianContactNumberInput) guardianContactNumberInput.setAttribute('required', 'required');
            } else {
                guardianSection.classList.add('d-none');
                guardianRequired.forEach(elem => elem.classList.add('d-none'));
                
                // Remove required for adults
                if (guardianFullNameInput) guardianFullNameInput.removeAttribute('required');
                if (guardianContactNumberInput) guardianContactNumberInput.removeAttribute('required');
            }
        }
        
        clearError(input);
        return true;
    }
    
    // Function to calculate age from birthdate
    function getAge(birthDateString) {
        const today = new Date();
        const birthDate = new Date(birthDateString);
        let age = today.getFullYear() - birthDate.getFullYear();
        const monthDiff = today.getMonth() - birthDate.getMonth();
        
        if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
            age--;
        }
        
        return age;
    }
    
    // Function to show error message
    function showError(input, message) {
        // Find or create error container
        let errorSpan = input.parentElement.querySelector('.text-danger');
        if (!errorSpan) {
            errorSpan = document.createElement('span');
            errorSpan.className = 'text-danger';
            input.insertAdjacentElement('afterend', errorSpan);
        }
        
        // Show error message
        errorSpan.textContent = message;
        errorSpan.style.display = 'block';
        
        // Add invalid class to input
        input.classList.add('is-invalid');
        input.classList.remove('is-valid');
    }
    
    // Function to clear error message
    function clearError(input) {
        // Find error span
        const errorSpan = input.parentElement.querySelector('.text-danger');
        if (errorSpan) {
            errorSpan.textContent = '';
            errorSpan.style.display = 'none';
        }
        
        // Remove invalid class, add valid class
        input.classList.remove('is-invalid');
        if (input.value) {
            input.classList.add('is-valid');
        }
    }
    
    // Initialize validation on page load
    if (emailInput && emailInput.value) validateEmailFormat(emailInput);
    if (contactNumberInput && contactNumberInput.value) formatAndValidateContactNumber(contactNumberInput);
    if (birthDateInput && birthDateInput.value) validateBirthDate(birthDateInput);
    
    [firstNameInput, middleNameInput, lastNameInput, suffixInput, guardianFullNameInput].forEach(input => {
        if (input && input.value) validateNameField(input);
    });
}); 