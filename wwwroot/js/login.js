// Login Page Functionality

document.addEventListener('DOMContentLoaded', function() {
    // Form elements
    const loginForm = document.getElementById('loginForm');
    const emailOrUsername = document.getElementById('EmailOrUsername');
    const password = document.getElementById('password');
    const togglePassword = document.getElementById('togglePassword');
    const loginButton = document.getElementById('loginButton');
    const loginButtonContent = document.querySelector('.login-button-content');
    const loginButtonSpinner = document.querySelector('.login-button-spinner');
    
    // Validation elements
    const emailUsernameError = document.getElementById('emailUsernameError');
    const passwordError = document.getElementById('passwordError');
    
    // Toggle password visibility
    if (togglePassword && password) {
        togglePassword.addEventListener('click', function() {
            // Toggle password visibility
            const type = password.getAttribute('type') === 'password' ? 'text' : 'password';
            password.setAttribute('type', type);
            
            // Toggle icon
            const icon = togglePassword.querySelector('i');
            if (icon) {
                icon.classList.toggle('fa-eye');
                icon.classList.toggle('fa-eye-slash');
            }
            
            // Add active class for styling
            togglePassword.classList.toggle('active');
            
            // Focus back on password for better UX
            password.focus();
        });
    }
    
    // Form validation
    if (loginForm) {
        // Real-time validation for email/username field
        if (emailOrUsername) {
            emailOrUsername.addEventListener('blur', function() {
                validateEmailOrUsername();
            });
            
            emailOrUsername.addEventListener('input', function() {
                if (emailOrUsername.classList.contains('is-invalid')) {
                    validateEmailOrUsername();
                }
            });
        }
        
        // Real-time validation for password field
        if (password) {
            password.addEventListener('blur', function() {
                validatePassword();
            });
            
            password.addEventListener('input', function() {
                if (password.classList.contains('is-invalid')) {
                    validatePassword();
                }
            });
        }
        
        // Form submission
        loginForm.addEventListener('submit', function(e) {
            // Validate form before submission
            if (!validateForm()) {
                e.preventDefault();
                return;
            }
            
            // Show loading state
            if (loginButton && loginButtonContent && loginButtonSpinner) {
                loginButtonContent.classList.add('d-none');
                loginButtonSpinner.classList.remove('d-none');
                loginButton.disabled = true;
                
                // Add timeout to return to normal state if server takes too long
                setTimeout(function() {
                    loginButtonContent.classList.remove('d-none');
                    loginButtonSpinner.classList.add('d-none');
                    loginButton.disabled = false;
                }, 10000); // 10-second timeout
            }
        });
    }
    
    // Validate email or username field
    function validateEmailOrUsername() {
        if (!emailOrUsername || !emailUsernameError) return true;
        
        if (!emailOrUsername.value.trim()) {
            emailOrUsername.classList.add('is-invalid');
            emailUsernameError.style.display = 'block';
            return false;
        } else {
            emailOrUsername.classList.remove('is-invalid');
            emailOrUsername.classList.add('is-valid');
            emailUsernameError.style.display = 'none';
            return true;
        }
    }
    
    // Validate password field
    function validatePassword() {
        if (!password || !passwordError) return true;
        
        if (!password.value) {
            password.classList.add('is-invalid');
            passwordError.style.display = 'block';
            return false;
        } else {
            password.classList.remove('is-invalid');
            password.classList.add('is-valid');
            passwordError.style.display = 'none';
            return true;
        }
    }
    
    // Validate the entire form
    function validateForm() {
        const isEmailUsernameValid = validateEmailOrUsername();
        const isPasswordValid = validatePassword();
        
        return isEmailUsernameValid && isPasswordValid;
    }
    
    // Add animation to the form container
    const formContainer = document.querySelector('.login-form-container');
    if (formContainer) {
        // Add fade-in animation
        formContainer.style.opacity = '0';
        formContainer.style.transform = 'translateY(20px)';
        
        setTimeout(() => {
            formContainer.style.transition = 'opacity 0.5s ease-in, transform 0.5s ease-in';
            formContainer.style.opacity = '1';
            formContainer.style.transform = 'translateY(0)';
        }, 100);
    }
    
    // Focus animation for input fields
    const inputFields = document.querySelectorAll('.form-control');
    inputFields.forEach(input => {
        input.addEventListener('focus', function() {
            this.parentElement.classList.add('input-focused');
            const label = document.querySelector(`label[for="${this.id}"]`);
            if (label) {
                label.classList.add('label-focused');
            }
        });
        
        input.addEventListener('blur', function() {
            if (!this.value) {
                this.parentElement.classList.remove('input-focused');
                const label = document.querySelector(`label[for="${this.id}"]`);
                if (label) {
                    label.classList.remove('label-focused');
                }
            }
        });
    });
    
    // Apply focus style to input on label click for better UX
    const labels = document.querySelectorAll('.form-label');
    labels.forEach(label => {
        label.addEventListener('click', function() {
            const forAttribute = this.getAttribute('for');
            if (forAttribute) {
                const input = document.getElementById(forAttribute);
                if (input) {
                    input.focus();
                }
            }
        });
    });
    
    // Initialize the form with validation state
    validateForm();
    
    // Add micro-interaction for login button
    if (loginButton) {
        loginButton.addEventListener('mouseover', function() {
            this.style.transform = 'scale(1.05)';
            this.style.boxShadow = '0 4px 8px rgba(0, 0, 0, 0.2)';
        });
        
        loginButton.addEventListener('mouseout', function() {
            this.style.transform = 'scale(1)';
            this.style.boxShadow = 'none';
        });
    }
}); 