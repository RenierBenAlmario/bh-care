document.addEventListener('DOMContentLoaded', function() {
    initializeCalendar();
    setupEventListeners();
});

function initializeCalendar() {
    const calendarEl = document.getElementById('appointmentCalendar');
    const calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
        events: '/api/appointments/calendar',
        eventClick: function(info) {
            viewAppointment(info.event.id);
        },
        dateClick: function(info) {
            openNewAppointmentModal(info.date);
        }
    });
    calendar.render();
}

function setupEventListeners() {
    const newAppointmentModal = document.getElementById('newAppointmentModal');
    if (newAppointmentModal) {
        newAppointmentModal.addEventListener('shown.bs.modal', function() {
            const firstInput = newAppointmentModal.querySelector('select, input');
            if (firstInput) firstInput.focus();
        });
    }
}

function refreshAppointments() {
    location.reload();
}

function viewAppointment(id) {
    // TODO: Implement appointment view logic
    console.log('Viewing appointment:', id);
}

function startConsultation(id) {
    // TODO: Implement consultation start logic
    window.location.href = `/Doctor/Consultation/${id}`;
}

function cancelAppointment(id) {
    if (confirm('Are you sure you want to cancel this appointment?')) {
        // TODO: Implement cancellation logic
        fetch(`/api/appointments/${id}/cancel`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showToast('Appointment cancelled successfully', 'success');
                refreshAppointments();
            } else {
                showToast('Failed to cancel appointment', 'error');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showToast('An error occurred', 'error');
        });
    }
}

function saveAppointment() {
    const form = document.getElementById('newAppointmentForm');
    if (form.checkValidity()) {
        // TODO: Implement save logic
        const formData = new FormData(form);
        fetch('/api/appointments', {
            method: 'POST',
            body: formData
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showToast('Appointment saved successfully', 'success');
                $('#newAppointmentModal').modal('hide');
                refreshAppointments();
            } else {
                showToast('Failed to save appointment', 'error');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showToast('An error occurred', 'error');
        });
    } else {
        form.reportValidity();
    }
}

function showToast(message, type = 'info') {
    const toastContainer = document.querySelector('.toast-container');
    const toast = document.createElement('div');
    toast.className = `toast align-items-center text-white bg-${type === 'error' ? 'danger' : type}`;
    toast.setAttribute('role', 'alert');
    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">${message}</div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
    `;
    toastContainer.appendChild(toast);
    const bsToast = new bootstrap.Toast(toast);
    bsToast.show();
    toast.addEventListener('hidden.bs.toast', () => toast.remove());
}