document.addEventListener('DOMContentLoaded', function() {
    // Find the error message about Middle Name field being required
    function removeMiddleNameRequiredError() {
        // Direct access to the error message shown in the image
        const errorMessageElements = document.querySelectorAll('*');
        for (let element of errorMessageElements) {
            if (element.textContent === 'The Middle Name field is required.') {
                // Remove the error message by hiding its parent container
                element.style.display = 'none';
                if (element.parentElement) {
                    element.parentElement.style.display = 'none';
                }
                console.log('Middle Name required message hidden');
                return true;
            }
        }
        return false;
    }
    
    // Check immediately on load
    if (!removeMiddleNameRequiredError()) {
        // If not found immediately, set an interval to keep trying
        const checkInterval = setInterval(function() {
            if (removeMiddleNameRequiredError()) {
                clearInterval(checkInterval);
            }
        }, 500); // Check every 500ms
        
        // Stop checking after 10 seconds to prevent infinite loop
        setTimeout(function() {
            clearInterval(checkInterval);
        }, 10000);
    }
    
    // Also modify any form to prevent middle name from being required
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            // Find middle name input
            const middleNameInput = form.querySelector('input[name="Middle"]') || 
                                   form.querySelector('input[name="MiddleName"]');
            
            if (middleNameInput) {
                // Ensure it's not marked as required
                middleNameInput.removeAttribute('required');
                
                // If there's a validation error for middle name, remove it
                removeMiddleNameRequiredError();
            }
        });
    });
}); 