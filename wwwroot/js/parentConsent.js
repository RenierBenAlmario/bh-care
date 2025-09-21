// Parent Consent Modal Handling for Barangay Health Care System
// Current date reference: June 16, 2025

document.addEventListener('DOMContentLoaded', function() {
    console.log('Parent consent script loaded');
    
    // Find the birth date input field
    const birthDateInput = document.getElementById('Input_BirthDate');
    const signupForm = document.querySelector('form#signupForm');
    const loadingIndicator = document.getElementById('loadingIndicator');
    const guardianFirstNameInput = document.getElementById('guardianFirstName');
    const guardianLastNameInput = document.getElementById('guardianLastName');
    const guardianResidencyProofInput = document.getElementById('guardianResidencyProof');
    
    // Hidden fields
    const guardianFirstNameHidden = document.getElementById('Input_GuardianFirstName');
    const guardianLastNameHidden = document.getElementById('Input_GuardianLastName');
    
    // Reference date (June 16, 2025)
    const referenceDate = new Date(2025, 5, 16); // Month is 0-indexed, so 5 = June
    
    console.log('Birth date input element:', birthDateInput);
    console.log('Form element:', signupForm);
    
    // Function to calculate age based on birthdate and reference date
    function calculateAge(birthDate) {
        console.log('Calculating age for birthdate:', birthDate);
        const birthDateObj = new Date(birthDate);
        
        // Check for valid date
        if (isNaN(birthDateObj.getTime())) {
            console.log('Invalid date');
            return null;
        }
        
        let age = referenceDate.getFullYear() - birthDateObj.getFullYear();
        
        // Adjust age if birthday hasn't occurred yet this year
        const birthMonth = birthDateObj.getMonth();
        const referenceMonth = referenceDate.getMonth();
        
        if (referenceMonth < birthMonth || 
            (referenceMonth === birthMonth && referenceDate.getDate() < birthDateObj.getDate())) {
            age--;
        }
        
        console.log('Calculated age:', age);
        return age;
    }
    
    // Function to show guardian section in the page directly
    function showGuardianSection(show) {
        console.log('Showing guardian section:', show);
        const guardianInfoSection = document.getElementById('guardianInfoSection');
        
        if (guardianInfoSection) {
            guardianInfoSection.style.display = show ? 'block' : 'none';
            
            // Make fields required when displayed
            if (show) {
                const requiredInputs = guardianInfoSection.querySelectorAll('input[data-required="true"]');
                requiredInputs.forEach(input => {
                    input.setAttribute('required', 'required');
                });
            } else {
                const requiredInputs = guardianInfoSection.querySelectorAll('input[data-required="true"]');
                requiredInputs.forEach(input => {
                    input.removeAttribute('required');
                });
            }
        } else {
            console.error('Guardian info section not found in the page');
        }
    }
    
    // Function to validate guardian information
    function validateGuardianInfo() {
        // For direct form input
        if (guardianFirstNameInput && guardianLastNameInput) {
            if (!guardianFirstNameInput.value.trim()) {
                return { valid: false, message: 'Guardian first name is required' };
            }
            
            if (!guardianLastNameInput.value.trim()) {
                return { valid: false, message: 'Guardian last name is required' };
            }
        }
        
        // Validate file input
        if (guardianResidencyProofInput && guardianResidencyProofInput.files && guardianResidencyProofInput.files.length > 0) {
            const file = guardianResidencyProofInput.files[0];
            const fileSize = file.size / 1024 / 1024; // in MB
            const fileType = file.type;
            
            // Check file size (max 5MB)
            if (fileSize > 5) {
                return { valid: false, message: 'Guardian residency proof file size must be less than 5MB' };
            }
            
            // Check file type
            if (!['image/jpeg', 'image/jpg', 'image/png', 'application/pdf'].includes(fileType)) {
                return { valid: false, message: 'Guardian residency proof must be JPG, JPEG, PNG, or PDF' };
            }
        }
        
        return { valid: true };
    }
    
    // Store filled guardian information in hidden fields when submitting
    if (guardianFirstNameInput && guardianLastNameInput) {
        guardianFirstNameInput.addEventListener('change', function() {
            if (guardianFirstNameHidden) {
                guardianFirstNameHidden.value = this.value;
                console.log('Updated hidden guardian first name:', this.value);
            }
        });
        
        guardianLastNameInput.addEventListener('change', function() {
            if (guardianLastNameHidden) {
                guardianLastNameHidden.value = this.value;
                console.log('Updated hidden guardian last name:', this.value);
            }
        });
    }
    
    // Event handlers for birthdate input
    if (birthDateInput) {
        console.log('Adding event listener to birth date input');
        
        // Check age on page load
        if (birthDateInput.value) {
            const age = calculateAge(birthDateInput.value);
            console.log('Initial age check:', age);
            
            if (age !== null && age < 18) {
                showGuardianSection(true);
            } else {
                showGuardianSection(false);
            }
        }
        
        // Handle birth date changes
        birthDateInput.addEventListener('change', function() {
            const birthDateValue = this.value;
            console.log('Birth date changed to:', birthDateValue);
            const age = calculateAge(birthDateValue);
            
            if (age !== null && age < 18) {
                showGuardianSection(true);
            } else {
                showGuardianSection(false);
                
                // Clear guardian fields when not needed
                if (guardianFirstNameInput) guardianFirstNameInput.value = '';
                if (guardianLastNameInput) guardianLastNameInput.value = '';
                if (guardianFirstNameHidden) guardianFirstNameHidden.value = '';
                if (guardianLastNameHidden) guardianLastNameHidden.value = '';
                if (guardianResidencyProofInput) guardianResidencyProofInput.value = '';
            }
        });
    } else {
        console.error('Birth date input not found');
    }
    
    // Handle form submission
    if (signupForm) {
        console.log('Adding event listener to form');
        
        signupForm.addEventListener('submit', function(e) {
            console.log('Form submitted');
            
            // Check if user needs guardian consent
            const birthDateValue = birthDateInput ? birthDateInput.value : null;
            const age = calculateAge(birthDateValue);
            
            if (age !== null && age < 18) {
                console.log('User is under 18, checking guardian information');
                // Validate guardian information
                const validation = validateGuardianInfo();
                if (!validation.valid) {
                    e.preventDefault(); // Stop form submission
                    console.error('Guardian validation failed:', validation.message);
                    alert('Guardian information required: ' + validation.message);
                    return false;
                }
            }
            
            // Check if loadingIndicator exists before showing it
            if (loadingIndicator) {
                loadingIndicator.classList.remove('d-none');
            }
            
            return true; // Allow form submission
        });
    } else {
        console.error('Signup form not found');
    }
}); 