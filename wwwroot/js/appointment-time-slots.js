/**
 * Appointment Time Slots Loading Script
 * This script handles loading available time slots when date and consultation type are selected
 */

document.addEventListener('DOMContentLoaded', function() {
    // Get the necessary elements
    const dateInput = document.getElementById('appointmentDate');
    const consultationTypeSelect = document.getElementById('consultationType');
    const timeSlotSelect = document.getElementById('timeSlot');
    const timeSlotMessage = document.getElementById('timeSlotMessage');
    
    // Function to load available time slots
    function loadTimeSlots() {
        // Check if both date and consultation type are selected
        if (!dateInput || !consultationTypeSelect || !timeSlotSelect) {
            console.error('Required form elements not found');
            return;
        }
        
        const selectedDate = dateInput.value;
        const selectedType = consultationTypeSelect.value;
        
        // Make sure both values are selected
        if (!selectedDate || !selectedType) {
            timeSlotSelect.innerHTML = '<option value="" selected disabled>Select date and consultation type first</option>';
            return;
        }
        
        // Update message to indicate loading
        timeSlotMessage.textContent = 'Loading available time slots...';
        
        // Make an AJAX request to get available time slots
        fetch(`/api/timeslots?date=${selectedDate}&type=${selectedType}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Failed to load time slots');
                }
                return response.json();
            })
            .then(data => {
                // Clear previous options
                timeSlotSelect.innerHTML = '';
                
                // Check if we have any time slots
                if (data && data.length > 0) {
                    // Add a prompt option
                    const promptOption = document.createElement('option');
                    promptOption.value = '';
                    promptOption.textContent = 'Select a time slot';
                    promptOption.disabled = true;
                    promptOption.selected = true;
                    timeSlotSelect.appendChild(promptOption);
                    
                    // Add time slot options
                    data.forEach(slot => {
                        const option = document.createElement('option');
                        option.value = slot.id;
                        
                        // Format the time for display (assuming slot.startTime is in ISO format)
                        const time = new Date(slot.startTime);
                        const formattedTime = time.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
                        
                        option.textContent = formattedTime;
                        timeSlotSelect.appendChild(option);
                    });
                    
                    timeSlotMessage.textContent = `${data.length} time slots available`;
                } else {
                    // No time slots available
                    const noSlotsOption = document.createElement('option');
                    noSlotsOption.value = '';
                    noSlotsOption.textContent = 'No available time slots for selected date/type';
                    noSlotsOption.disabled = true;
                    noSlotsOption.selected = true;
                    timeSlotSelect.appendChild(noSlotsOption);
                    
                    timeSlotMessage.textContent = 'No available time slots. Please select a different date or consultation type.';
                }
            })
            .catch(error => {
                console.error('Error loading time slots:', error);
                timeSlotSelect.innerHTML = '<option value="" selected disabled>Error loading time slots</option>';
                timeSlotMessage.textContent = 'Failed to load time slots. Please try again later.';
            });
    }
    
    // For demo purposes, if the backend API isn't ready yet, use these sample time slots
    function loadSampleTimeSlots() {
        // Check if both date and consultation type are selected
        if (!dateInput || !consultationTypeSelect || !timeSlotSelect) {
            console.error('Required form elements not found');
            return;
        }
        
        const selectedDate = dateInput.value;
        const selectedType = consultationTypeSelect.value;
        
        // Make sure both values are selected
        if (!selectedDate || !selectedType) {
            timeSlotSelect.innerHTML = '<option value="" selected disabled>Select date and consultation type first</option>';
            return;
        }
        
        // Update message to indicate loading
        timeSlotMessage.textContent = 'Loading available time slots...';
        
        // Generate some sample time slots based on selected type
        setTimeout(() => {
            // Clear previous options
            timeSlotSelect.innerHTML = '';
            
            // Generate sample slots based on clinic schedule
            const sampleSlots = [];
            
            // Parse the selected date properly
            const [year, month, day] = selectedDate.split('-').map(Number);
            const selectedDateObj = new Date(year, month - 1, day); // month is 0-indexed in JS Date
            
            const today = new Date();
            // Reset time part for accurate date comparison
            today.setHours(0, 0, 0, 0);
            
            // Don't allow past dates - only compare dates, not times
            if (selectedDateObj.getTime() < today.getTime()) {
                timeSlotSelect.innerHTML = '<option value="" selected disabled>Cannot book appointments for past dates</option>';
                timeSlotMessage.textContent = 'Please select a current or future date.';
                return;
            }
            
            // Get day of week (0 is Sunday, 1 is Monday, etc.)
            const dayOfWeek = selectedDateObj.getDay();
            
            // Clinic is closed on weekends
            if (dayOfWeek === 0 || dayOfWeek === 6) {
                timeSlotSelect.innerHTML = '<option value="" selected disabled>No appointments on weekends</option>';
                timeSlotMessage.textContent = 'The clinic is closed on weekends. Please select a weekday.';
                return;
            }
            
            // Define time slots based on consultation type and day of week
            let availableTimeSlots = [];
            
            switch (selectedType) {
                case 'medical':
                    // Regular clinic hours: Monday-Friday, 8AM-11AM and 1PM-4PM
                    if (dayOfWeek >= 1 && dayOfWeek <= 5) {
                        availableTimeSlots = [
                            {start: 8, end: 11, interval: 20}, // Morning slots
                            {start: 13, end: 16, interval: 20}  // Afternoon slots (1PM - 4PM)
                        ];
                    }
                    break;
                    
                case 'dental':
                    // Dental consultation: 8-11AM (Monday, Wednesday, Friday)
                    if (dayOfWeek === 1 || dayOfWeek === 3 || dayOfWeek === 5) { // Monday, Wednesday, Friday
                        availableTimeSlots = [
                            {start: 8, end: 11, interval: 30} // 8AM - 11AM
                        ];
                    }
                    break;
                    
                case 'immunization':
                    // Immunization: 8AM-12PM (Wednesdays)
                    if (dayOfWeek === 3) { // Wednesday
                        availableTimeSlots = [
                            {start: 8, end: 12, interval: 15} // 8AM - 12PM
                        ];
                    }
                    break;
                    
                case 'checkup':
                    // DOH consult: Monday-Friday 1-4PM
                    if (dayOfWeek >= 1 && dayOfWeek <= 5) {
                        availableTimeSlots = [
                            {start: 13, end: 16, interval: 10} // 1PM - 4PM
                        ];
                    }
                    break;
                    
                case 'family':
                    // Family Planning: 8AM-11AM (Monday-Friday)
                    if (dayOfWeek >= 1 && dayOfWeek <= 5) {
                        availableTimeSlots = [
                            {start: 8, end: 11, interval: 20} // 8AM - 11AM
                        ];
                    }
                    break;
            }
            
            // If no slots available for the selected day
            if (availableTimeSlots.length === 0) {
                timeSlotSelect.innerHTML = '<option value="" selected disabled>No appointments available for this service today</option>';
                timeSlotMessage.textContent = 'This service is not available on the selected date. Please choose another date.';
                return;
            }
            
            // Generate time slots for the available periods
            availableTimeSlots.forEach(period => {
                for (let hour = period.start; hour < period.end; hour++) {
                    for (let minute = 0; minute < 60; minute += period.interval) {
                        // Skip if we would go past the end hour
                        if (hour === period.end - 1 && minute + period.interval > 60) continue;
                        
                        const slotId = `${selectedDate}-${hour}-${minute}`;
                        
                        // Format the time to show in 24-hour format (09:00, 09:20, etc.)
                        const hourFormatted = hour.toString().padStart(2, '0');
                        const minuteFormatted = minute.toString().padStart(2, '0');
                        const timeStr = `${hourFormatted}:${minuteFormatted}`;
                        
                        sampleSlots.push({
                            id: slotId,
                            time: timeStr
                        });
                    }
                }
            });
            
            // If no slots were generated (might happen due to restrictions)
            if (sampleSlots.length === 0) {
                timeSlotSelect.innerHTML = '<option value="" selected disabled>No available time slots</option>';
                timeSlotMessage.textContent = 'No time slots available for this date and service.';
                return;
            }
            
            // Add all time slots to dropdown
            sampleSlots.forEach(slot => {
                const option = document.createElement('option');
                option.value = slot.id;
                option.textContent = slot.time;
                timeSlotSelect.appendChild(option);
            });
            
            timeSlotMessage.textContent = `${sampleSlots.length} time slots available`;
        }, 500); // Simulate network delay
    }
    
    // Event listeners for date and consultation type changes
    if (dateInput) {
        dateInput.addEventListener('change', loadSampleTimeSlots);
    }
    
    if (consultationTypeSelect) {
        consultationTypeSelect.addEventListener('change', loadSampleTimeSlots);
    }
    
    // If both date and consultation type are already selected when page loads, load time slots
    if (dateInput && dateInput.value && consultationTypeSelect && consultationTypeSelect.value) {
        loadSampleTimeSlots();
    }
}); 