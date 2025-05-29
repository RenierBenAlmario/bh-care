document.addEventListener('DOMContentLoaded', function() {
  const form = document.getElementById('signupForm');
  if (!form) return;

  // Initialize variables for validation elements
  const usernameInput = document.getElementById('username');
  const emailInput = document.getElementById('email');
  const firstNameInput = document.getElementById('firstName');
  const middleNameInput = document.getElementById('middleName');
  const lastNameInput = document.getElementById('lastName');
  const suffixInput = document.getElementById('suffix');
  const contactNumberInput = document.getElementById('contactNumber');
  const birthDateInput = document.getElementById('birthDate');
  const passwordInput = document.getElementById('password');
  const confirmPasswordInput = document.getElementById('confirmPassword');
  const guardianSection = document.getElementById('guardianSection');

  // Improved regex patterns for validation
  const patterns = {
    username: /^[a-zA-Z0-9_]{3,}$/,
    // More strict email validation
    email: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
    // Strict name validation - only letters, spaces, hyphens, and apostrophes
    name: /^[A-Za-z\s\-']+$/,
    // Strict contact number validation for Philippines format
    contactNumber: /^(09\d{9}|\+639\d{9})$/,
    password: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+{}\[\]:;"'<>,.?/~\\|-]).{8,}$/
  };

  // Error message templates
  const errorMessages = {
    username: 'Username must be at least 3 characters and can only contain letters, numbers, and underscores.',
    email: 'Please enter a valid email address (e.g., example@domain.com).',
    name: 'Only letters, spaces, hyphens, and apostrophes are allowed. No numbers or special characters.',
    contactNumber: 'Contact number must be in format 09XXXXXXXXX or +639XXXXXXXXX. Only digits allowed.',
    birthDate: 'Please enter a valid birth date. The date must not be in the future or too far in the past.',
    password: 'Password must be at least 8 characters and include uppercase, lowercase, number, and special character.',
    confirmPassword: 'Passwords do not match.',
    required: 'This field is required.'
  };

  // Function to show error message
  function showError(input, message) {
    const formGroup = input.closest('.form-group');
    const errorElement = formGroup.querySelector('.error-message') || 
                         createErrorElement(formGroup);
    
    input.classList.add('is-invalid');
    errorElement.textContent = message;
    errorElement.style.display = 'block';
  }

  // Function to create error element if it doesn't exist
  function createErrorElement(formGroup) {
    const errorElement = document.createElement('div');
    errorElement.className = 'error-message text-danger mt-1';
    formGroup.appendChild(errorElement);
    return errorElement;
  }

  // Function to clear error message
  function clearError(input) {
    const formGroup = input.closest('.form-group');
    const errorElement = formGroup.querySelector('.error-message');
    
    input.classList.remove('is-invalid');
    if (errorElement) {
      errorElement.textContent = '';
      errorElement.style.display = 'none';
    }
  }

  // Function to check age from birthdate
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

  // Function to validate birth date
  function validateBirthDate(dateString) {
    const selectedDate = new Date(dateString);
    const today = new Date();
    const minDate = new Date();
    minDate.setFullYear(today.getFullYear() - 120); // Assuming max age is 120 years
    
    // Check if date is valid (not NaN)
    if (isNaN(selectedDate.getTime())) {
      return { valid: false, message: 'Please enter a valid date.' };
    }
    
    // Check if date is in the future
    if (selectedDate > today) {
      return { valid: false, message: 'Birth date cannot be in the future.' };
    }
    
    // Check if date is too far in the past
    if (selectedDate < minDate) {
      return { valid: false, message: 'Birth date is too far in the past. Please enter a valid date.' };
    }
    
    return { valid: true };
  }

  // Function to toggle guardian info section
  function toggleGuardianSection(birthDateString) {
    if (!guardianSection) return;

    if (birthDateString) {
      const age = getAge(birthDateString);
      if (age < 18) {
        guardianSection.style.display = 'block';
        // Make guardian fields required
        const guardianInputs = guardianSection.querySelectorAll('input');
        guardianInputs.forEach(input => {
          input.setAttribute('required', 'required');
        });
      } else {
        guardianSection.style.display = 'none';
        // Make guardian fields not required
        const guardianInputs = guardianSection.querySelectorAll('input');
        guardianInputs.forEach(input => {
          input.removeAttribute('required');
        });
      }
    }
  }

  // Validate username with real-time feedback
  if (usernameInput) {
    usernameInput.addEventListener('input', function() {
      clearError(this);
      if (this.value && !patterns.username.test(this.value)) {
        showError(this, errorMessages.username);
      }
    });
  }

  // Validate email with real-time feedback
  if (emailInput) {
    emailInput.addEventListener('input', function() {
      clearError(this);
      if (this.value) {
        // First, check if it's a valid format using basic validation
        if (!this.value.includes('@') || !this.value.includes('.')) {
          showError(this, errorMessages.email);
        } 
        // Then apply the strict regex pattern
        else if (!patterns.email.test(this.value)) {
          showError(this, errorMessages.email);
        }
      }
    });
  }

  // Validate name fields with real-time feedback
  [firstNameInput, middleNameInput, lastNameInput, suffixInput].forEach(input => {
    if (input) {
      input.addEventListener('input', function() {
        clearError(this);
        if (this.value && !patterns.name.test(this.value)) {
          showError(this, errorMessages.name);
        }
      });
    }
  });

  // Validate contact number with real-time feedback
  if (contactNumberInput) {
    contactNumberInput.addEventListener('input', function() {
      clearError(this);
      
      // First, enforce numeric input for 09XXXXXXXXX format
      if (this.value.startsWith('09')) {
        // Keep only digits for the 09XXXXXXXXX format
        this.value = this.value.replace(/[^\d]/g, '');
      } 
      // Allow + only at the beginning for +639XXXXXXXXX format
      else if (this.value.startsWith('+')) {
        // Keep the + and only digits after it
        const plusSign = this.value.charAt(0);
        const digitsOnly = this.value.substring(1).replace(/[^\d]/g, '');
        this.value = plusSign + digitsOnly;
      } 
      // If it doesn't start with 09 or +, enforce 09 prefix
      else if (this.value && !this.value.startsWith('09') && !this.value.startsWith('+')) {
        // Convert to 09 format for convenience
        this.value = '09' + this.value.replace(/[^\d]/g, '');
      }
      
      // Validate the final pattern
      if (this.value && !patterns.contactNumber.test(this.value)) {
        showError(this, errorMessages.contactNumber);
      }
    });
  }

  // Validate birth date with real-time feedback
  if (birthDateInput) {
    birthDateInput.addEventListener('change', function() {
      clearError(this);
      
      if (this.value) {
        const validation = validateBirthDate(this.value);
        if (!validation.valid) {
          showError(this, validation.message);
          return;
        }
        
        // Toggle guardian section based on age
        toggleGuardianSection(this.value);
      }
    });
  }

  // Validate password with real-time feedback
  if (passwordInput) {
    passwordInput.addEventListener('input', function() {
      clearError(this);
      if (this.value && !patterns.password.test(this.value)) {
        showError(this, errorMessages.password);
      }

      // Check confirm password match if it has a value
      if (confirmPasswordInput && confirmPasswordInput.value) {
        if (this.value !== confirmPasswordInput.value) {
          showError(confirmPasswordInput, errorMessages.confirmPassword);
        } else {
          clearError(confirmPasswordInput);
        }
      }
    });
  }

  // Validate confirm password with real-time feedback
  if (confirmPasswordInput) {
    confirmPasswordInput.addEventListener('input', function() {
      clearError(this);
      if (passwordInput && this.value !== passwordInput.value) {
        showError(this, errorMessages.confirmPassword);
      }
    });
  }

  // Form submission validation
  if (form) {
    form.addEventListener('submit', function(event) {
      let isValid = true;
      
      // Validate required fields
      form.querySelectorAll('[required]').forEach(input => {
        if (!input.value.trim()) {
          showError(input, errorMessages.required);
          isValid = false;
        }
      });

      // Validate username
      if (usernameInput && usernameInput.value && !patterns.username.test(usernameInput.value)) {
        showError(usernameInput, errorMessages.username);
        isValid = false;
      }

      // Validate email
      if (emailInput && emailInput.value) {
        if (!emailInput.value.includes('@') || !emailInput.value.includes('.') || !patterns.email.test(emailInput.value)) {
          showError(emailInput, errorMessages.email);
          isValid = false;
        }
      }

      // Validate name fields
      [firstNameInput, middleNameInput, lastNameInput, suffixInput].forEach(input => {
        if (input && input.value && !patterns.name.test(input.value)) {
          showError(input, errorMessages.name);
          isValid = false;
        }
      });

      // Validate contact number
      if (contactNumberInput && contactNumberInput.value && !patterns.contactNumber.test(contactNumberInput.value)) {
        showError(contactNumberInput, errorMessages.contactNumber);
        isValid = false;
      }

      // Validate birth date
      if (birthDateInput && birthDateInput.value) {
        const validation = validateBirthDate(birthDateInput.value);
        if (!validation.valid) {
          showError(birthDateInput, validation.message);
          isValid = false;
        }
      }

      // Validate password
      if (passwordInput && passwordInput.value && !patterns.password.test(passwordInput.value)) {
        showError(passwordInput, errorMessages.password);
        isValid = false;
      }

      // Validate confirm password
      if (confirmPasswordInput && passwordInput && confirmPasswordInput.value !== passwordInput.value) {
        showError(confirmPasswordInput, errorMessages.confirmPassword);
        isValid = false;
      }

      // Validate guardian information if user is under 18
      if (birthDateInput && birthDateInput.value) {
        const age = getAge(birthDateInput.value);
        if (age < 18) {
          const guardianNameInput = document.getElementById('guardianName');
          const guardianContactInput = document.getElementById('guardianContactNumber');
          const guardianRelationInput = document.getElementById('guardianRelationship');
          
          if (guardianNameInput && !guardianNameInput.value.trim()) {
            showError(guardianNameInput, errorMessages.required);
            isValid = false;
          } else if (guardianNameInput && !patterns.name.test(guardianNameInput.value)) {
            showError(guardianNameInput, errorMessages.name);
            isValid = false;
          }
          
          if (guardianContactInput && !guardianContactInput.value.trim()) {
            showError(guardianContactInput, errorMessages.required);
            isValid = false;
          } else if (guardianContactInput && !patterns.contactNumber.test(guardianContactInput.value)) {
            showError(guardianContactInput, errorMessages.contactNumber);
            isValid = false;
          }
          
          if (guardianRelationInput && !guardianRelationInput.value.trim()) {
            showError(guardianRelationInput, errorMessages.required);
            isValid = false;
          }
        }
      }

      if (!isValid) {
        event.preventDefault();
      }
    });
  }

  // Initialize guardian section based on birthdate if it exists
  if (birthDateInput && birthDateInput.value) {
    toggleGuardianSection(birthDateInput.value);
  }

  // Initialize all fields on page load to show any validation issues
  if (form) {
    form.querySelectorAll('input').forEach(input => {
      if (input.value) {
        // Trigger validation for pre-filled fields
        const event = new Event('input', { bubbles: true });
        input.dispatchEvent(event);
        
        if (input.type === 'date' && input.value) {
          const changeEvent = new Event('change', { bubbles: true });
          input.dispatchEvent(changeEvent);
        }
      }
    });
  }
}); 