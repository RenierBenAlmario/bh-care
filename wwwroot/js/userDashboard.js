// Initialize components when the document is ready
document.addEventListener('DOMContentLoaded', function() {
    // Initialize tooltips and popovers
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function(tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function(popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Initialize session timeout
    initSessionTimeout();

    // Initialize offline detection
    initOfflineDetection();

    // Initialize appointment form handlers
    initAppointmentForm();

    // Initialize medical history chart
    initMedicalHistoryChart();

    // Initialize feedback form
    initFeedbackForm();
});

// Session timeout handling
function initSessionTimeout() {
    let timeoutDuration = 30; // seconds
    let warningShown = false;
    let timeoutInterval;

    function resetTimeout() {
        clearInterval(timeoutInterval);
        timeoutDuration = 30;
        warningShown = false;
        document.getElementById('sessionTimeoutModal').classList.remove('show');
    }

    function startTimeoutCountdown() {
        timeoutInterval = setInterval(() => {
            timeoutDuration--;
            if (timeoutDuration <= 0) {
                window.location.href = '/Account/Logout';
            } else {
                document.getElementById('timeoutCountdown').textContent = timeoutDuration;
            }
        }, 1000);
    }

    document.addEventListener('mousemove', () => {
        if (!warningShown) {
            resetTimeout();
        }
    });

    document.getElementById('stayLoggedIn').addEventListener('click', () => {
        resetTimeout();
    });

    document.getElementById('logoutNow').addEventListener('click', () => {
        window.location.href = '/Account/Logout';
    });
}

// Offline detection
function initOfflineDetection() {
    function updateOnlineStatus() {
        const indicator = document.getElementById('offlineIndicator');
        if (!navigator.onLine) {
            indicator.style.display = 'block';
        } else {
            indicator.style.display = 'none';
        }
    }

    window.addEventListener('online', updateOnlineStatus);
    window.addEventListener('offline', updateOnlineStatus);
}

// Appointment form handling
function initAppointmentForm() {
    const appointmentForm = document.getElementById('appointmentForm');
    const dateInput = document.getElementById('appointmentDate');
    const timeSelect = document.getElementById('appointmentTime');
    const doctorSelect = document.getElementById('doctorSelect');

    if (dateInput) {
        dateInput.addEventListener('change', () => {
            fetchAvailableTimeSlots(dateInput.value);
        });
    }

    // Fetch available doctors on page load
    fetchDoctors();

    if (appointmentForm) {
        appointmentForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            const formData = new FormData(appointmentForm);
            
            try {
                const response = await fetch('/api/user/bookAppointment', {
                    method: 'POST',
                    body: formData
                });
                
                const data = await response.json();
                if (data.success) {
                    showToast('Appointment booked successfully');
                    bootstrap.Modal.getInstance(document.getElementById('appointmentModal')).hide();
                    appointmentForm.reset();
                    // Refresh the appointments list
                    location.reload();
                } else {
                    showToast(data.message || 'Error booking appointment', 'error');
                }
            } catch (error) {
                console.error('Error:', error);
                showToast('Error booking appointment', 'error');
            }
        });
    }
}

// Fetch available time slots for selected date
async function fetchAvailableTimeSlots(date) {
    const timeSelect = document.getElementById('appointmentTime');
    const doctorId = document.getElementById('doctorSelect').value;
    
    try {
        const response = await fetch(`/api/user/availableTimeSlots?date=${date}&doctorId=${doctorId}`);
        const data = await response.json();
        
        timeSelect.innerHTML = '<option value="">Select a time slot</option>';
        data.timeSlots.forEach(slot => {
            const option = document.createElement('option');
            option.value = slot;
            option.textContent = slot;
            timeSelect.appendChild(option);
        });
    } catch (error) {
        console.error('Error:', error);
        showToast('Error fetching available time slots', 'error');
    }
}

// Fetch available doctors
async function fetchDoctors() {
    const doctorSelect = document.getElementById('doctorSelect');
    
    try {
        const response = await fetch('/api/user/availableDoctors');
        const data = await response.json();
        
        doctorSelect.innerHTML = '<option value="">Choose a doctor</option>';
        data.doctors.forEach(doctor => {
            const option = document.createElement('option');
            option.value = doctor.id;
            option.textContent = `Dr. ${doctor.name} - ${doctor.specialization}`;
            doctorSelect.appendChild(option);
        });
    } catch (error) {
        console.error('Error:', error);
        showToast('Error fetching doctors list', 'error');
    }
}

// Medical History Chart initialization
function initMedicalHistoryChart() {
    const ctx = document.getElementById('medicalHistoryChart');
    if (ctx) {
        new Chart(ctx, {
            type: 'line',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
                datasets: [{
                    label: 'Visits',
                    data: window.chartData?.visits || [0, 0, 0, 0, 0, 0],
                    borderColor: 'rgb(75, 192, 192)',
                    tension: 0.1
                }]
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                }
            }
        });
    }
}

// Feedback form handling
function initFeedbackForm() {
    const feedbackForm = document.getElementById('feedbackForm');
    
    if (feedbackForm) {
        feedbackForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            const formData = new FormData(feedbackForm);
            
            try {
                const response = await fetch('/api/user/submitFeedback', {
                    method: 'POST',
                    body: formData
                });
                
                const data = await response.json();
                if (data.success) {
                    showToast('Thank you for your feedback!');
                    bootstrap.Modal.getInstance(document.getElementById('feedbackModal')).hide();
                    feedbackForm.reset();
                } else {
                    showToast(data.message || 'Error submitting feedback', 'error');
                }
            } catch (error) {
                console.error('Error:', error);
                showToast('Error submitting feedback', 'error');
            }
        });
    }
}

// View appointment details
function viewDetails(appointmentId) {
    fetch(`/api/user/appointmentDetails/${appointmentId}`)
        .then(response => response.json())
        .then(data => {
            const detailsDiv = document.getElementById('appointmentDetails');
            detailsDiv.innerHTML = `
                <div class="mb-3">
                    <strong>Date:</strong> ${data.date}<br>
                    <strong>Time:</strong> ${data.time}<br>
                    <strong>Doctor:</strong> Dr. ${data.doctorName}<br>
                    <strong>Status:</strong> <span class="badge bg-${getStatusColor(data.status)}">${data.status}</span><br>
                    <strong>Reason:</strong> ${data.reason}<br>
                    ${data.diagnosis ? `<strong>Diagnosis:</strong> ${data.diagnosis}<br>` : ''}
                    ${data.prescription ? `<strong>Prescription:</strong> ${data.prescription}<br>` : ''}
                </div>
            `;
            
            const modal = new bootstrap.Modal(document.getElementById('appointmentDetailsModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showToast('Error fetching appointment details', 'error');
        });
}

// Cancel appointment
function cancelAppointment(appointmentId) {
    if (confirm('Are you sure you want to cancel this appointment?')) {
        fetch(`/api/user/cancelAppointment/${appointmentId}`, {
            method: 'POST'
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showToast('Appointment cancelled successfully');
                location.reload();
            } else {
                showToast(data.message || 'Error cancelling appointment', 'error');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showToast('Error cancelling appointment', 'error');
        });
    }
}

// Utility function to get status color
function getStatusColor(status) {
    switch (status.toLowerCase()) {
        case 'confirmed':
            return 'success';
        case 'pending':
            return 'warning';
        case 'cancelled':
            return 'danger';
        case 'completed':
            return 'info';
        default:
            return 'secondary';
    }
}

// Toast notification
function showToast(message, type = 'success') {
    const toastContainer = document.getElementById('toastContainer');
    const toast = document.createElement('div');
    toast.className = `toast align-items-center text-white bg-${type === 'success' ? 'success' : 'danger'} border-0`;
    toast.setAttribute('role', 'alert');
    toast.setAttribute('aria-live', 'assertive');
    toast.setAttribute('aria-atomic', 'true');
    
    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">
                ${message}
            </div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
    `;
    
    toastContainer.appendChild(toast);
    const bsToast = new bootstrap.Toast(toast);
    bsToast.show();
    
    toast.addEventListener('hidden.bs.toast', () => {
        toast.remove();
    });
}

// Export functions for testing
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        initSessionTimeout,
        initOfflineDetection,
        initAppointmentForm,
        initMedicalHistoryChart,
        initFeedbackForm,
        viewDetails,
        cancelAppointment,
        getStatusColor,
        showToast
    };
} 