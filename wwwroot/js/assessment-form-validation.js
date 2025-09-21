/**
 * Assessment Form Validation Script
 * This script handles conditional validation for assessment form fields
 * so that hidden or conditionally visible fields are not required when not visible.
 */

document.addEventListener('DOMContentLoaded', function() {
    // Get all the elements we need to conditionally validate
    const cancerCheckbox = document.querySelector('input[name="NCDModel.HasCancer"]');
    const cancerTypeField = document.querySelector('input[name="NCDModel.CancerType"]');
    
    const familyHasOtherCheckbox = document.querySelector('input[name="NCDModel.FamilyHasOtherDisease"]');
    const familyOtherDetailsField = document.querySelector('input[name="NCDModel.FamilyOtherDiseaseDetails"]');
    
    // Initialize visibility based on initial state
    if (cancerCheckbox && cancerTypeField) {
        toggleFieldValidation(cancerTypeField, cancerCheckbox.checked);
        cancerCheckbox.addEventListener('change', function() {
            toggleFieldValidation(cancerTypeField, this.checked);
        });
    }

    if (familyHasOtherCheckbox && familyOtherDetailsField) {
        toggleFieldValidation(familyOtherDetailsField, familyHasOtherCheckbox.checked);
        familyHasOtherCheckbox.addEventListener('change', function() {
            toggleFieldValidation(familyOtherDetailsField, this.checked);
        });
    }
    
    // Handle Filipino fields that appear in validation errors
    const alcoholFieldsMap = {
        'DrugsTobaccoUse': 'drugsTobaccoUseResponse',
        'DrugsAlcoholUse': 'drugsAlcoholUseResponse',
        'DrugsIllicitDrugUse': 'drugsIllicitDrugUseResponse',
        'SafetyPhysicalAbuse': 'safetyPhysicalAbuseResponse',
        'SafetyRelationshipViolence': 'safetyRelationshipViolenceResponse',
        'SafetyProtectiveGear': 'safetyProtectiveGearResponse',
        'SafetyGunsAtHome': 'safetyGunsAtHomeResponse'
    };
    
    // For each Filipino field in our map, check if we have a response dropdown
    Object.entries(alcoholFieldsMap).forEach(([fieldName, responseDropdownId]) => {
        const textField = document.querySelector(`textarea[name="HEEADSSSModel.${fieldName}"]`);
        const responseDropdown = document.getElementById(responseDropdownId);
        
        if (textField && responseDropdown) {
            // Set the text field as not required initially
            textField.required = false;
            responseDropdown.addEventListener('change', function() {
                // If they selected "Yes", make the text field required
                toggleFieldValidation(textField, this.value === "yes");
            });
        }
    });
    
    // Add event listeners to all form steps navigation buttons to handle validation
    const nextButtons = document.querySelectorAll('.btn-next');
    nextButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            // Get the current step
            const currentStepElem = this.closest('.booking-step');
            if (!currentStepElem) return;
            
            // Find all required fields in this step
            const requiredFields = currentStepElem.querySelectorAll('[required]');
            
            // Check if all required fields are valid
            let isValid = true;
            requiredFields.forEach(field => {
                if (!field.validity.valid) {
                    isValid = false;
                    field.reportValidity();
                }
            });
            
            // If not valid, prevent going to next step
            if (!isValid) {
                e.preventDefault();
            }
        });
    });
});

/**
 * Toggles required attribute and validation for a field
 * @param {HTMLElement} field - The field element to toggle validation for
 * @param {boolean} isRequired - Whether the field should be required
 */
function toggleFieldValidation(field, isRequired) {
    if (!field) return;
    
    if (isRequired) {
        field.setAttribute('required', 'required');
    } else {
        field.removeAttribute('required');
        // Clear any validation error styling
        field.classList.remove('is-invalid');
        // Clear the field value if it's hidden
        field.value = '';
    }
} 