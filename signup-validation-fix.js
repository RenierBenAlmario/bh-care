// Apply validation to the signup form
document.addEventListener('DOMContentLoaded', function() {
    // Select all input elements and error containers
    const emailInput = document.querySelector('input[type="email"]');
    const contactNumberInput = document.querySelector('input[name$="ContactNumber"]');
    const firstNameInput = document.querySelector('input[name$="FirstName"]');
    const middleNameInput = document.querySelector('input[name$="MiddleName"]');
    const lastNameInput = document.querySelector('input[name$="LastName"]');
    const suffixInput = document.querySelector('input[name$="Suffix"]');
    const birthDateInput = document.querySelector('input[type="date"]');
    const passwordInput = document.querySelector('input[type="password"][name$="Password"]');
    const confirmPasswordInput = document.querySelector('input[type="password"][name$="ConfirmPassword"]');
    const form = document.querySelector('form');

    // Regex patterns for validation
    const patterns = {
        email: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
        contactNumber: /^(09\d{9}|\+639\d{9})$/,
        name: /^[A-Za-z\s\-']+$/
    };

    // Error messages
    const errorMessages = {
        email: 'Invalid email address format',
        contactNumber: 'Contact number must be in format 09XXXXXXXXX or +639XXXXXXXXX',
        name: 'Only letters, spaces, hyphens, and apostrophes are allowed',
        birthDate: 'Birth date cannot be in the future or too far in the past',
        password: 'Password is required',
        confirmPassword: 'The Confirm Password field is required'
    };

    // Function to validate email
    function validateEmail(email) {
        return patterns.email.test(email);
    }

    // Function to validate contact number
    function validateContactNumber(number) {
        // Clean the input as user types
        if (number.startsWith('09')) {
            // Remove non-digits for 09XXXXXXXXX format
            return number.replace(/[^\d]/g, '');
        } else if (number.startsWith('+')) {
            // Keep + at beginning and remove non-digits afterward
            return '+' + number.substring(1).replace(/[^\d]/g, '');
        } else if (number && !number.startsWith('09') && !number.startsWith('+')) {
            // Add 09 prefix if missing
            return '09' + number.replace(/[^\d]/g, '');
        }
        return number;
    }

    // Function to validate name fields
    function validateName(name) {
        return !name || patterns.name.test(name);
    }

    // Function to validate birth date
    function validateBirthDate(dateString) {
        const selectedDate = new Date(dateString);
        const today = new Date();
        const minDate = new Date();
        minDate.setFullYear(today.getFullYear() - 120); // Max age of 120 years
        
        // Check if date is valid
        if (isNaN(selectedDate.getTime())) {
            return false;
        }
        
        // Check if date is in the future
        if (selectedDate > today) {
            return false;
        }
        
        // Check if date is too far in the past
        if (selectedDate < minDate) {
            return false;
        }
        
        return true;
    }

    // Function to display error message on the form
    function showError(input, message) {
        // First check if there's already an error span below the input
        let errorSpan = input.parentElement.querySelector('.text-danger');
        
        // If no existing error span found, create one
        if (!errorSpan) {
            errorSpan = document.createElement('span');
            errorSpan.className = 'text-danger';
            input.parentElement.appendChild(errorSpan);
        }
        
        // Show the error message
        errorSpan.textContent = message;
        
        // Add invalid class to the input
        input.classList.add('is-invalid');
        input.classList.remove('is-valid');
    }

    // Function to clear error message
    function clearError(input) {
        const errorSpan = input.parentElement.querySelector('.text-danger');
        if (errorSpan) {
            errorSpan.textContent = '';
        }
        input.classList.remove('is-invalid');
        input.classList.add('is-valid');
    }

    // Apply validation to email input
    if (emailInput) {
        emailInput.addEventListener('input', function() {
            const email = this.value.trim();
            if (email && !validateEmail(email)) {
                showError(this, errorMessages.email);
            } else {
                clearError(this);
            }
        });
    }

    // Apply validation to contact number input
    if (contactNumberInput) {
        contactNumberInput.addEventListener('input', function() {
            // Format the number as user types
            this.value = validateContactNumber(this.value);
            
            // Validate the formatted number
            if (this.value && !patterns.contactNumber.test(this.value)) {
                showError(this, errorMessages.contactNumber);
            } else {
                clearError(this);
            }
        });
    }

    // Apply validation to name fields
    [firstNameInput, middleNameInput, lastNameInput, suffixInput].forEach(function(input) {
        if (input) {
            input.addEventListener('input', function() {
                const name = this.value.trim();
                if (name && !validateName(name)) {
                    showError(this, errorMessages.name);
                } else {
                    clearError(this);
                }
            });
        }
    });

    // Apply validation to birth date input
    if (birthDateInput) {
        birthDateInput.addEventListener('change', function() {
            const date = this.value;
            if (date && !validateBirthDate(date)) {
                showError(this, errorMessages.birthDate);
            } else {
                clearError(this);
            }
        });
    }

    // Form submission validation
    if (form) {
        form.addEventListener('submit', function(event) {
            let isValid = true;
            
            // Validate email
            if (emailInput && emailInput.value.trim() && !validateEmail(emailInput.value.trim())) {
                showError(emailInput, errorMessages.email);
                isValid = false;
            }
            
            // Validate contact number
            if (contactNumberInput && contactNumberInput.value.trim() && !patterns.contactNumber.test(contactNumberInput.value.trim())) {
                showError(contactNumberInput, errorMessages.contactNumber);
                isValid = false;
            }
            
            // Validate name fields
            [firstNameInput, middleNameInput, lastNameInput, suffixInput].forEach(function(input) {
                if (input && input.value.trim() && !validateName(input.value.trim())) {
                    showError(input, errorMessages.name);
                    isValid = false;
                }
            });
            
            // Validate birth date
            if (birthDateInput && birthDateInput.value && !validateBirthDate(birthDateInput.value)) {
                showError(birthDateInput, errorMessages.birthDate);
                isValid = false;
            }
            
            // Prevent form submission if validation fails
            if (!isValid) {
                event.preventDefault();
            }
        });
    }

    // Initial validation on page load for any prefilled fields
    if (emailInput && emailInput.value) {
        emailInput.dispatchEvent(new Event('input'));
    }
    
    if (contactNumberInput && contactNumberInput.value) {
        contactNumberInput.dispatchEvent(new Event('input'));
    }
    
    [firstNameInput, middleNameInput, lastNameInput, suffixInput].forEach(function(input) {
        if (input && input.value) {
            input.dispatchEvent(new Event('input'));
        }
    });
    
    if (birthDateInput && birthDateInput.value) {
        birthDateInput.dispatchEvent(new Event('change'));
    }
}); 