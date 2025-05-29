// Booking Calendar Functionality
document.addEventListener('DOMContentLoaded', function() {
    const consultationTypeSelect = document.getElementById('consultationType');
    const timeSlotSelect = document.getElementById('timeSlot');
    const appointmentDateInput = document.getElementById('appointmentDate');
    
    if (consultationTypeSelect && timeSlotSelect && appointmentDateInput) {
        // Set min date to today
        const today = new Date();
        const formattedDate = today.toISOString().split('T')[0];
        appointmentDateInput.min = formattedDate;
        
        // Handle consultation type change
        consultationTypeSelect.addEventListener('change', function() {
            updateAvailableTimeSlots();
        });
        
        // Handle date change
        appointmentDateInput.addEventListener('change', function() {
            updateAvailableTimeSlots();
        });
        
        // Function to update available time slots based on consultation type and selected date
        function updateAvailableTimeSlots() {
            const consultationType = consultationTypeSelect.value;
            const selectedDate = new Date(appointmentDateInput.value);
            const dayOfWeek = selectedDate.getDay(); // 0 = Sunday, 1 = Monday, etc.
            
            // Clear existing options except the first one
            while (timeSlotSelect.options.length > 1) {
                timeSlotSelect.remove(1);
            }
            
            // If no consultation type selected or no date selected, return
            if (!consultationType || isNaN(selectedDate.getTime())) {
                return;
            }
            
            // Add time slots based on consultation type and day of week
            switch (consultationType) {
                case 'medical':
                    // Medical Consultation: Mon-Fri (1-5)
                    if (dayOfWeek >= 1 && dayOfWeek <= 5) {
                        // Morning slots
                        addOption('8:00 AM', '8:00 AM');
                        addOption('8:30 AM', '8:30 AM');
                        addOption('9:00 AM', '9:00 AM');
                        addOption('9:30 AM', '9:30 AM');
                        addOption('10:00 AM', '10:00 AM');
                        addOption('10:30 AM', '10:30 AM');
                        // Afternoon slots
                        addOption('1:00 PM', '1:00 PM');
                        addOption('1:30 PM', '1:30 PM');
                        addOption('2:00 PM', '2:00 PM');
                        addOption('2:30 PM', '2:30 PM');
                        addOption('3:00 PM', '3:00 PM');
                        addOption('3:30 PM', '3:30 PM');
                    } else {
                        addOption('No slots available on weekends', '', true);
                    }
                    break;
                    
                case 'dental':
                    // Dental Consultation: Mon, Wed, Fri (1, 3, 5)
                    if (dayOfWeek === 1 || dayOfWeek === 3 || dayOfWeek === 5) {
                        addOption('8:00 AM', '8:00 AM');
                        addOption('8:30 AM', '8:30 AM');
                        addOption('9:00 AM', '9:00 AM');
                        addOption('9:30 AM', '9:30 AM');
                        addOption('10:00 AM', '10:00 AM');
                        addOption('10:30 AM', '10:30 AM');
                    } else {
                        addOption('Available only on Mon, Wed, Fri', '', true);
                    }
                    break;
                    
                case 'immunization':
                    // Immunization: Wed only (3)
                    if (dayOfWeek === 3) {
                        addOption('8:00 AM', '8:00 AM');
                        addOption('8:30 AM', '8:30 AM');
                        addOption('9:00 AM', '9:00 AM');
                        addOption('9:30 AM', '9:30 AM');
                        addOption('10:00 AM', '10:00 AM');
                        addOption('10:30 AM', '10:30 AM');
                        addOption('11:00 AM', '11:00 AM');
                        addOption('11:30 AM', '11:30 AM');
                    } else {
                        addOption('Available only on Wednesdays', '', true);
                    }
                    break;
                    
                case 'checkup':
                    // BP/Sugar/Weight Check-up: Mon-Thu (1-4)
                    if (dayOfWeek >= 1 && dayOfWeek <= 4) {
                        // Morning slots
                        addOption('8:00 AM', '8:00 AM');
                        addOption('8:30 AM', '8:30 AM');
                        addOption('9:00 AM', '9:00 AM');
                        addOption('9:30 AM', '9:30 AM');
                        addOption('10:00 AM', '10:00 AM');
                        addOption('10:30 AM', '10:30 AM');
                        // Afternoon slots
                        addOption('1:00 PM', '1:00 PM');
                        addOption('1:30 PM', '1:30 PM');
                        addOption('2:00 PM', '2:00 PM');
                        addOption('2:30 PM', '2:30 PM');
                        addOption('3:00 PM', '3:00 PM');
                        addOption('3:30 PM', '3:30 PM');
                    } else {
                        addOption('Available only Mon-Thu', '', true);
                    }
                    break;
                    
                case 'family':
                    // Prenatal & Family Planning: Mon, Wed, Fri (1, 3, 5)
                    if (dayOfWeek === 1 || dayOfWeek === 3 || dayOfWeek === 5) {
                        // Morning slots
                        addOption('8:00 AM', '8:00 AM');
                        addOption('8:30 AM', '8:30 AM');
                        addOption('9:00 AM', '9:00 AM');
                        addOption('9:30 AM', '9:30 AM');
                        addOption('10:00 AM', '10:00 AM');
                        addOption('10:30 AM', '10:30 AM');
                        // Afternoon slots
                        addOption('1:00 PM', '1:00 PM');
                        addOption('1:30 PM', '1:30 PM');
                        addOption('2:00 PM', '2:00 PM');
                        addOption('2:30 PM', '2:30 PM');
                        addOption('3:00 PM', '3:00 PM');
                        addOption('3:30 PM', '3:30 PM');
                    } else {
                        addOption('Available only on Mon, Wed, Fri', '', true);
                    }
                    break;
                    
                case 'dots':
                    // DOTS: Mon-Fri (1-5)
                    if (dayOfWeek >= 1 && dayOfWeek <= 5) {
                        addOption('1:00 PM', '1:00 PM');
                        addOption('1:30 PM', '1:30 PM');
                        addOption('2:00 PM', '2:00 PM');
                        addOption('2:30 PM', '2:30 PM');
                        addOption('3:00 PM', '3:00 PM');
                        addOption('3:30 PM', '3:30 PM');
                    } else {
                        addOption('No slots available on weekends', '', true);
                    }
                    break;
            }
        }
        
        // Helper function to add an option to the select
        function addOption(text, value, disabled = false) {
            const option = document.createElement('option');
            option.text = text;
            option.value = value;
            if (disabled) {
                option.disabled = true;
            }
            timeSlotSelect.appendChild(option);
        }
    }
    
    // Handle booking for someone else checkbox
    const bookingForOtherCheckbox = document.getElementById('bookingForOther');
    const relationshipField = document.getElementById('relationshipField');
    
    if (bookingForOtherCheckbox && relationshipField) {
        bookingForOtherCheckbox.addEventListener('change', function() {
            relationshipField.style.display = this.checked ? 'block' : 'none';
        });
    }
    
    // Handle step navigation
    const toStep2Button = document.getElementById('toStep2Button');
    const step1 = document.getElementById('step1');
    const step2a = document.getElementById('step2a');
    const step2b = document.getElementById('step2b');
    const step3 = document.getElementById('step3');
    
    // Handle Next button from Step 1
    if (toStep2Button && step1) {
        toStep2Button.addEventListener('click', function() {
            // Validate step 1 fields
            const requiredFields = step1.querySelectorAll('[required]');
            let isValid = true;
            
            requiredFields.forEach(field => {
                if (!field.value) {
                    isValid = false;
                    field.classList.add('is-invalid');
                } else {
                    field.classList.remove('is-invalid');
                }
            });
            
            if (isValid) {
                // Get the age to determine which form to show
                const age = parseInt(document.getElementById('age').value);
                
                step1.style.display = 'none';
                
                // Show appropriate assessment form based on age
                if (age >= 20) {
                    // For adults (20+), show NCD Risk Assessment
                    if (step2a) step2a.style.display = 'block';
                } else if (age >= 10 && age <= 19) {
                    // For adolescents (10-19), show HEEADSSS Assessment
                    if (step2b) step2b.style.display = 'block';
                } else {
                    // For children under 10, skip to confirmation
                    if (step3) step3.style.display = 'block';
                    populateSummary();
                }
                
                // Populate form fields with patient information
                populateFormFields();
            }
        });
    }
    
    // Handle back button from Step 2a (NCD Risk Assessment)
    const backToStep1Button = document.getElementById('backToStep1Button');
    if (backToStep1Button && step1 && step2a) {
        backToStep1Button.addEventListener('click', function() {
            step2a.style.display = 'none';
            step1.style.display = 'block';
        });
    }
    
    // Handle next button from Step 2a to Step 3
    const toStep3Button = document.getElementById('toStep3Button');
    if (toStep3Button && step2a && step3) {
        toStep3Button.addEventListener('click', function() {
            step2a.style.display = 'none';
            step3.style.display = 'block';
            populateSummary();
        });
    }
    
    // Handle back button from Step 2b (HEEADSSS Assessment)
    const backToStep1ButtonHeeadsss = document.getElementById('backToStep1ButtonHeeadsss');
    if (backToStep1ButtonHeeadsss && step1 && step2b) {
        backToStep1ButtonHeeadsss.addEventListener('click', function() {
            step2b.style.display = 'none';
            step1.style.display = 'block';
        });
    }
    
    // Handle next button from Step 2b to Step 3
    const toStep3ButtonHeeadsss = document.getElementById('toStep3ButtonHeeadsss');
    if (toStep3ButtonHeeadsss && step2b && step3) {
        toStep3ButtonHeeadsss.addEventListener('click', function() {
            step2b.style.display = 'none';
            step3.style.display = 'block';
            populateSummary();
        });
    }
    
    // Handle back button from Step 3 to appropriate Step 2
    const backToStep2Button = document.getElementById('backToStep2Button');
    if (backToStep2Button && step3) {
        backToStep2Button.addEventListener('click', function() {
            step3.style.display = 'none';
            
            // Determine which step 2 to go back to based on age
            const age = parseInt(document.getElementById('age').value);
            if (age >= 20 && step2a) {
                step2a.style.display = 'block';
            } else if (age >= 10 && age <= 19 && step2b) {
                step2b.style.display = 'block';
            } else {
                // If no assessment form was shown, go back to step 1
                step1.style.display = 'block';
            }
        });
    }
    
    // Handle form submission
    const appointmentForm = document.getElementById('appointmentForm');
    if (appointmentForm) {
        appointmentForm.addEventListener('submit', function(event) {
            // Final validation before submission
            const allRequiredFields = document.querySelectorAll('[required]');
            let isValid = true;
            
            allRequiredFields.forEach(field => {
                if (!field.value) {
                    isValid = false;
                    field.classList.add('is-invalid');
                } else {
                    field.classList.remove('is-invalid');
                }
            });
            
            if (!isValid) {
                event.preventDefault();
                alert('Please fill in all required fields.');
            }
        });
    }
    
    // Function to populate assessment form fields with patient information
    function populateFormFields() {
        const fullName = document.getElementById('fullName').value;
        const age = document.getElementById('age').value;
        const phoneNumber = document.getElementById('phoneNumber').value;
        
        // Populate NCD Risk Assessment form
        const ncdNameField = document.getElementById('ncdPatientName');
        const ncdAgeField = document.getElementById('ncdAge');
        const ncdPhoneField = document.getElementById('ncdPhone');
        
        if (ncdNameField) ncdNameField.value = fullName;
        if (ncdAgeField) ncdAgeField.value = age;
        if (ncdPhoneField) ncdPhoneField.value = phoneNumber;
        
        // Populate HEEADSSS Assessment form
        const heeadsssNameField = document.getElementById('heeadsssPatientName');
        const heeadsssAgeField = document.getElementById('heeadsssAge');
        const heeadsssPhoneField = document.getElementById('heeadsssPhone');
        
        if (heeadsssNameField) heeadsssNameField.value = fullName;
        if (heeadsssAgeField) heeadsssAgeField.value = age;
        if (heeadsssPhoneField) heeadsssPhoneField.value = phoneNumber;
    }
    
    // Function to populate booking summary
    function populateSummary() {
        const summaryName = document.getElementById('summaryName');
        const summaryAge = document.getElementById('summaryAge');
        const summaryPhone = document.getElementById('summaryPhone');
        const summaryDate = document.getElementById('summaryDate');
        const summaryTime = document.getElementById('summaryTime');
        const summaryType = document.getElementById('summaryType');
        const summaryReason = document.getElementById('summaryReason');
        
        if (summaryName) summaryName.textContent = document.getElementById('fullName').value;
        if (summaryAge) summaryAge.textContent = document.getElementById('age').value;
        if (summaryPhone) summaryPhone.textContent = document.getElementById('phoneNumber').value;
        
        if (summaryDate) {
            const dateValue = document.getElementById('appointmentDate').value;
            summaryDate.textContent = formatDate(dateValue);
        }
        
        if (summaryTime) {
            const timeSelect = document.getElementById('timeSlot');
            summaryTime.textContent = timeSelect.options[timeSelect.selectedIndex].text;
        }
        
        if (summaryType) {
            const typeSelect = document.getElementById('consultationType');
            summaryType.textContent = typeSelect.options[typeSelect.selectedIndex].text;
        }
        
        if (summaryReason) summaryReason.textContent = document.getElementById('reasonForVisit').value;
    }
    
    // Helper function to format date
    function formatDate(dateString) {
        const date = new Date(dateString);
        const options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
        return date.toLocaleDateString('en-US', options);
    }
}); 