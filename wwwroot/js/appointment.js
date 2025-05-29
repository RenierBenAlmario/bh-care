// Appointment booking functionality
document.addEventListener('DOMContentLoaded', function() {
    const bookingForm = document.getElementById('appointmentForm');
    
    if (bookingForm) {
        bookingForm.addEventListener('submit', function(e) {
            e.preventDefault();
            
            // Show loading state
            const bookingButton = document.querySelector('button[type="submit"]');
            const originalButtonText = bookingButton.innerHTML;
            bookingButton.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Booking...';
            bookingButton.disabled = true;
            
            // Get form data
            const doctorId = document.getElementById('doctorId').value;
            const date = document.getElementById('appointmentDate').value;
            const time = document.getElementById('appointmentTime').value;
            const patientName = document.getElementById('patientName').value;
            const patientAge = document.getElementById('patientAge').value;
            const reasonForVisit = document.getElementById('reasonForVisit').value;
            
            // Create appointment data object
            // Make sure you're sending the Name property correctly
            // Add this before sending the request
            console.log("Patient name being sent:", patientName);
            
            const appointmentData = {
                DoctorId: doctorId,
                Date: date,
                Time: time,
                Name: patientName, // Make sure this is not empty
                Age: parseInt(patientAge),
                ReasonForVisit: reasonForVisit,
                Type: "Regular"
            };
            
            console.log('Sending appointment data:', appointmentData);
            
            // Send API request - check if this is the correct endpoint
            fetch('/api/user/appointments', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                body: JSON.stringify(appointmentData)
            })
            .then(response => {
                if (!response.ok) {
                    return response.json().then(data => {
                        throw new Error(data.message || 'Failed to book appointment');
                    });
                }
                return response.json();
            })
            .then(data => {
                // Show success message
                alert('Appointment booked successfully!');
                // Optionally redirect to appointments page
                window.location.href = '/appointments';
            })
            .catch(error => {
                console.error('Error:', error);
                alert(error.message || 'Failed to book appointment. Please try again.');
            })
            .finally(() => {
                // Reset button state
                bookingButton.innerHTML = originalButtonText;
                bookingButton.disabled = false;
            });
        });
    }
});

// Make sure your form is capturing the name correctly
// Example code (adjust to match your actual implementation):
$("#bookAppointmentForm").submit(function(e) {
    e.preventDefault();
    
    var appointmentData = {
        Date: $("#appointmentDate").val(),
        Time: $("#appointmentTime").val(),
        Name: $("#patientName").val(), // Make sure this field exists and is populated
        Age: parseInt($("#patientAge").val()),
        ReasonForVisit: $("#reasonForVisit").val(),
        DoctorId: $("#doctorSelect").val(),
        Type: "Regular"
    };
    
    // Ajax call to submit the form
    // ...
});