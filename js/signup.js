// Remove any code that automatically checks the checkboxes
// Add validation for the checkboxes on form submission

document.addEventListener('DOMContentLoaded', function() {
    // Get form and checkbox elements
    const form = document.getElementById('signupForm');
    const privacyTerms = document.getElementById('privacyTerms');
    const residencyConfirm = document.getElementById('residencyConfirm');
    const checkboxErrorContainer = document.getElementById('checkboxErrorContainer');
    const checkboxErrorMessage = document.getElementById('checkboxErrorMessage');
    
    // Add form submission validation
    if (form) {
        form.addEventListener('submit', function(e) {
            // Check if checkboxes are checked
            let isValid = true;
            
            if (privacyTerms && !privacyTerms.checked) {
                isValid = false;
                if (checkboxErrorMessage) {
                    checkboxErrorMessage.textContent = 'You must agree to the data privacy terms';
                }
            }
            
            if (residencyConfirm && !residencyConfirm.checked) {
                isValid = false;
                if (checkboxErrorMessage) {
                    checkboxErrorMessage.textContent = 'You must confirm your residency in Barangay 161';
                }
            }
            
            if (!isValid && checkboxErrorContainer) {
                e.preventDefault();
                checkboxErrorContainer.classList.remove('d-none');
                
                // If both checkboxes are unchecked, show comprehensive message
                if (privacyTerms && residencyConfirm && !privacyTerms.checked && !residencyConfirm.checked) {
                    checkboxErrorMessage.textContent = 'You must agree to the data privacy terms and confirm your residency';
                }
                
                // Scroll to error
                checkboxErrorContainer.scrollIntoView({ behavior: 'smooth', block: 'center' });
            } else if (checkboxErrorContainer) {
                checkboxErrorContainer.classList.add('d-none');
            }
        });
    }
    
    // Accept terms button functionality
    const acceptTermsButton = document.getElementById('acceptTerms');
    if (acceptTermsButton && privacyTerms) {
        acceptTermsButton.addEventListener('click', function() {
            privacyTerms.checked = true;
        });
    }
}); 