// booking-form-handler.js - Manages the appointment booking process

document.addEventListener('DOMContentLoaded', function() {
    // Get references to common booking elements
    const bookButtons = document.querySelectorAll('.book-appointment-btn');
    const bookingModal = document.getElementById('bookingModal');
    const bookingForm = document.getElementById('appointmentForm');
    
    // Initialize booking modal if using Bootstrap
    if (typeof bootstrap !== 'undefined' && bookingModal) {
        const modal = new bootstrap.Modal(bookingModal);
        
        // Show modal when book buttons are clicked
        bookButtons.forEach(button => {
            button.addEventListener('click', function(e) {
                e.preventDefault();
                
                // If the button has a data-doctor-id attribute, use it
                const doctorId = this.getAttribute('data-doctor-id');
                if (doctorId && document.getElementById('DoctorId')) {
                    document.getElementById('DoctorId').value = doctorId;
                }
                
                // Show the modal
                modal.show();
                
                // Log for debugging
                console.log('Booking modal opened');
            });
        });
    } else {
        // For non-Bootstrap implementation, handle direct form display
        bookButtons.forEach(button => {
            button.addEventListener('click', function(e) {
                e.preventDefault();
                
                // Handle direct navigation to booking page
                const doctorId = this.getAttribute('data-doctor-id') || '';
                window.location.href = `/api/Appointment/book?doctorId=${doctorId}`;
                
                // Log for debugging
                console.log('Navigating to booking page');
            });
        });
    }
    
    // Initialize direct form if it exists on the page
    if (bookingForm) {
        console.log('Booking form found on page, initializing...');
        
        // Handle doctor and date selection to get available time slots
        const doctorSelect = document.getElementById('DoctorId');
        const dateInput = document.getElementById('Date');
        const timeSelect = document.getElementById('Time');
        
        if (doctorSelect && dateInput && timeSelect) {
            const updateTimeSlots = function() {
                const doctorId = doctorSelect.value;
                const date = dateInput.value;
                
                if (!doctorId || !date) {
                    timeSelect.innerHTML = '<option value="">Select date and doctor first</option>';
                    timeSelect.disabled = true;
                    return;
                }
                
                // Disable the select while loading
                timeSelect.disabled = true;
                timeSelect.innerHTML = '<option value="">Loading available times...</option>';
                
                // Get available time slots from API
                fetch(`/api/Appointment/GetAvailableTimeSlots?doctorId=${doctorId}&date=${date}`)
                    .then(response => {
                        if (!response.ok) {
                            throw new Error('Failed to load time slots');
                        }
                        return response.json();
                    })
                    .then(slots => {
                        // Clear the select
                        timeSelect.innerHTML = '';
                        
                        // Add default option
                        const defaultOption = document.createElement('option');
                        defaultOption.value = '';
                        defaultOption.textContent = slots.length ? 'Select a time' : 'No times available';
                        timeSelect.appendChild(defaultOption);
                        
                        // Add available slots
                        slots.forEach(slot => {
                            const option = document.createElement('option');
                            option.value = slot;
                            option.textContent = slot;
                            timeSelect.appendChild(option);
                        });
                        
                        // Enable the select if there are slots
                        timeSelect.disabled = slots.length === 0;
                    })
                    .catch(error => {
                        console.error('Error loading time slots:', error);
                        timeSelect.innerHTML = '<option value="">Error loading time slots</option>';
                        timeSelect.disabled = true;
                    });
            };
            
            // Update time slots when doctor or date changes
            doctorSelect.addEventListener('change', updateTimeSlots);
            dateInput.addEventListener('change', updateTimeSlots);
        }
        
        // Form validation
        bookingForm.addEventListener('submit', function(event) {
            if (!this.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            
            // Add validation styles
            this.classList.add('was-validated');
            
            // Log form submission attempt
            console.log('Form submission attempted, valid:', this.checkValidity());
        });
    } else {
        console.log('No booking form found on page');
    }
    
    // Debug function to check if the booking elements are present
    function logBookingElements() {
        console.log('Book buttons:', bookButtons.length);
        console.log('Booking modal:', bookingModal ? 'Found' : 'Not found');
        console.log('Booking form:', bookingForm ? 'Found' : 'Not found');
    }
    
    // Run debug check
    logBookingElements();
}); 