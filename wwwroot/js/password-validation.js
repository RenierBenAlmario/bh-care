/**
 * Password validation for Barangay Health Center system
 * This script adds real-time password strength validation and feedback
 */

document.addEventListener('DOMContentLoaded', function() {
    // Get password-related elements
    const passwordInput = document.getElementById('password');
    const confirmPasswordInput = document.getElementById('confirmPassword');
    const togglePassword = document.getElementById('togglePassword');
    const toggleConfirmPassword = document.getElementById('toggleConfirmPassword');
    
    // Password strength elements
    const passwordStrengthBar = document.getElementById('passwordStrength');
    const passwordStrengthText = document.querySelector('.password-strength-text');
    const passwordFeedback = document.getElementById('passwordFeedback');
    
    // Password requirement elements
    const lengthRequirement = document.getElementById('length');
    const uppercaseRequirement = document.getElementById('uppercase');
    const lowercaseRequirement = document.getElementById('lowercase');
    const numberRequirement = document.getElementById('number');
    const specialRequirement = document.getElementById('special');
    
    // Password match indicators
    const passwordMatch = document.getElementById('passwordMatch');
    const passwordMismatch = document.getElementById('passwordMismatch');
    
    // Initialize console log for debugging
    console.log('Password validation script loaded');
    
    // Add event listener for password input
    if (passwordInput) {
        console.log('Password input found, attaching event listener');
        passwordInput.addEventListener('input', function() {
            const password = this.value;
            console.log('Password changed: length =', password.length);
            
            // Check requirements
            validatePasswordRequirements(password);
            
            // Calculate strength
            const strengthResult = calculatePasswordStrength(password);
            updatePasswordStrengthIndicator(strengthResult);
            
            // Check match if confirm password has value
            if (confirmPasswordInput && confirmPasswordInput.value) {
                validatePasswordMatch();
            }
        });
    } else {
        console.error('Password input not found (#password)');
    }
    
    // Add event listener for confirm password
    if (confirmPasswordInput) {
        confirmPasswordInput.addEventListener('input', validatePasswordMatch);
    }
    
    // Add event listeners for password visibility toggles
    if (togglePassword && passwordInput) {
        togglePassword.addEventListener('click', function() {
            togglePasswordVisibility(passwordInput, this);
        });
    }
    
    if (toggleConfirmPassword && confirmPasswordInput) {
        toggleConfirmPassword.addEventListener('click', function() {
            togglePasswordVisibility(confirmPasswordInput, this);
        });
    }
    
    // Function to validate password requirements
    function validatePasswordRequirements(password) {
        // Check length requirement (8+ characters)
        if (lengthRequirement) {
            const hasLength = password.length >= 8;
            updateRequirement(lengthRequirement, hasLength, 'At least 8 characters');
        }
        
        // Check uppercase requirement
        if (uppercaseRequirement) {
            const hasUppercase = /[A-Z]/.test(password);
            updateRequirement(uppercaseRequirement, hasUppercase, 'At least 1 uppercase letter');
        }
        
        // Check lowercase requirement
        if (lowercaseRequirement) {
            const hasLowercase = /[a-z]/.test(password);
            updateRequirement(lowercaseRequirement, hasLowercase, 'At least 1 lowercase letter');
        }
        
        // Check number requirement
        if (numberRequirement) {
            const hasNumber = /[0-9]/.test(password);
            updateRequirement(numberRequirement, hasNumber, 'At least 1 number');
        }
        
        // Check special character requirement
        if (specialRequirement) {
            const hasSpecial = /[^A-Za-z0-9]/.test(password);
            updateRequirement(specialRequirement, hasSpecial, 'At least 1 special character');
        }
    }
    
    // Function to validate password match
    function validatePasswordMatch() {
        if (!passwordInput || !confirmPasswordInput) return;
        
        const password = passwordInput.value;
        const confirmPassword = confirmPasswordInput.value;
        
        console.log('Validating password match');
        
        if (!confirmPassword) {
            // If empty, hide both messages
            if (passwordMatch) passwordMatch.classList.add('d-none');
            if (passwordMismatch) passwordMismatch.classList.add('d-none');
            confirmPasswordInput.classList.remove('is-valid');
            confirmPasswordInput.classList.remove('is-invalid');
        } else if (password === confirmPassword) {
            if (passwordMatch) passwordMatch.classList.remove('d-none');
            if (passwordMismatch) passwordMismatch.classList.add('d-none');
            confirmPasswordInput.classList.remove('is-invalid');
            confirmPasswordInput.classList.add('is-valid');
        } else {
            if (passwordMatch) passwordMatch.classList.add('d-none');
            if (passwordMismatch) passwordMismatch.classList.remove('d-none');
            confirmPasswordInput.classList.add('is-invalid');
            confirmPasswordInput.classList.remove('is-valid');
        }
    }
    
    // Function to calculate password strength
    function calculatePasswordStrength(password) {
        if (!password) {
            return { 
                score: 0, 
                feedback: { 
                    warning: "Password is required", 
                    suggestions: ["Enter a password"] 
                } 
            };
        }
        
        // Use zxcvbn library if available (preferred method)
        if (typeof zxcvbn === 'function') {
            console.log('Using zxcvbn for password strength calculation');
            try {
                const result = zxcvbn(password);
                return {
                    score: result.score,
                    feedback: {
                        warning: result.feedback.warning || "",
                        suggestions: result.feedback.suggestions || []
                    }
                };
            } catch (error) {
                console.error('Error using zxcvbn:', error);
                // Fall back to basic implementation
            }
        }
        
        // Basic implementation if zxcvbn is not available
        console.log('Using basic password strength calculation (zxcvbn not available)');
        let score = 0;
        const feedback = { warning: "", suggestions: [] };
        
        // Length check (0-2 points)
        if (password.length >= 8) score += 1;
        if (password.length >= 12) score += 1;
        
        // Character variety checks
        const hasUppercase = /[A-Z]/.test(password);
        const hasLowercase = /[a-z]/.test(password);
        const hasNumber = /[0-9]/.test(password);
        const hasSpecial = /[^A-Za-z0-9]/.test(password);
        
        // Award points for character variety
        if (hasUppercase) score += 1;
        if (hasLowercase) score += 1;
        if (hasNumber) score += 1;
        if (hasSpecial) score += 1;
        
        // Check for common patterns
        const commonPatterns = [
            /^123456/, /password/i, /qwerty/i, /admin/i, 
            /welcome/i, /letmein/i, /abc123/i, /monkey/i,
            /^12345/, /football/i, /baseball/i, /dragon/i
        ];
        
        const hasCommonPattern = commonPatterns.some(pattern => pattern.test(password));
        if (hasCommonPattern) {
            score = Math.max(0, score - 2);
            feedback.warning = "Common password pattern detected";
            feedback.suggestions.push("Avoid using common words or patterns");
        }
        
        // Generate feedback based on score
        if (score <= 2) {
            feedback.warning = "This password is too weak";
            if (!feedback.suggestions.length) {
                feedback.suggestions.push("Add more variety with uppercase, numbers, and special characters");
                feedback.suggestions.push("Make your password longer");
            }
        } else if (score <= 4) {
            feedback.warning = "This password could be stronger";
            if (!feedback.suggestions.length) {
                feedback.suggestions.push("Consider adding more variety or length");
            }
        }
        
        // Normalize score to 0-4 range
        score = Math.min(4, score);
        
        return { score, feedback };
    }
    
    // Function to update the password strength indicator
    function updatePasswordStrengthIndicator(strengthResult) {
        if (!passwordStrengthBar || !passwordStrengthText) {
            console.error('Password strength elements not found');
            return;
        }
        
        const score = strengthResult.score;
        const feedback = strengthResult.feedback;
        const strengthPercentage = (score / 4) * 100;
        
        console.log('Updating password strength indicator: score =', score);
        
        // Update progress bar
        passwordStrengthBar.style.width = `${strengthPercentage}%`;
        passwordStrengthBar.setAttribute('aria-valuenow', strengthPercentage);
        
        // Update color and text based on score
        if (score <= 1) {
            passwordStrengthBar.className = 'progress-bar bg-danger';
            passwordStrengthText.textContent = 'Very Weak';
            passwordStrengthText.className = 'password-strength-text text-danger';
        } else if (score === 2) {
            passwordStrengthBar.className = 'progress-bar bg-warning';
            passwordStrengthText.textContent = 'Weak';
            passwordStrengthText.className = 'password-strength-text text-warning';
        } else if (score === 3) {
            passwordStrengthBar.className = 'progress-bar bg-info';
            passwordStrengthText.textContent = 'Good';
            passwordStrengthText.className = 'password-strength-text text-info';
        } else {
            passwordStrengthBar.className = 'progress-bar bg-success';
            passwordStrengthText.textContent = 'Strong';
            passwordStrengthText.className = 'password-strength-text text-success';
        }
        
        // Display feedback if available
        if (passwordFeedback) {
            if (feedback.warning || (feedback.suggestions && feedback.suggestions.length > 0)) {
                let feedbackHtml = '';
                
                if (feedback.warning) {
                    feedbackHtml += `<div class="alert alert-warning py-1 mb-2">${feedback.warning}</div>`;
                }
                
                if (feedback.suggestions && feedback.suggestions.length > 0) {
                    feedbackHtml += '<ul class="mb-0 ps-3">';
                    feedback.suggestions.forEach(suggestion => {
                        feedbackHtml += `<li>${suggestion}</li>`;
                    });
                    feedbackHtml += '</ul>';
                }
                
                passwordFeedback.innerHTML = feedbackHtml;
                passwordFeedback.classList.remove('d-none');
            } else {
                passwordFeedback.classList.add('d-none');
            }
        }
    }
    
    // Update requirement item with check or cross icon
    function updateRequirement(element, isValid, text) {
        if (!element) return;
        
        if (isValid) {
            element.innerHTML = `<i class="fas fa-check text-success me-1"></i> ${text}`;
            element.classList.add('valid');
            element.classList.remove('invalid');
        } else {
            element.innerHTML = `<i class="fas fa-times text-danger me-1"></i> ${text}`;
            element.classList.add('invalid');
            element.classList.remove('valid');
        }
    }
    
    // Toggle password visibility
    function togglePasswordVisibility(inputElement, buttonElement) {
        if (!inputElement || !buttonElement) return;
        
        const type = inputElement.getAttribute('type') === 'password' ? 'text' : 'password';
        inputElement.setAttribute('type', type);
        
        const icon = buttonElement.querySelector('i');
        if (icon) {
            icon.classList.toggle('fa-eye');
            icon.classList.toggle('fa-eye-slash');
        }
        
        // Toggle active class for styling
        buttonElement.classList.toggle('active');
    }
    
    // Run initial validation if password field has value (e.g., on page reload)
    if (passwordInput && passwordInput.value) {
        console.log('Password field already has value, running initial validation');
        passwordInput.dispatchEvent(new Event('input'));
    }
    
    if (confirmPasswordInput && confirmPasswordInput.value && passwordInput && passwordInput.value) {
        console.log('Confirm password field already has value, validating match');
        validatePasswordMatch();
    }
}); 