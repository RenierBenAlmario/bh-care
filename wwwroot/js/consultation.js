document.addEventListener('DOMContentLoaded', function() {
    setupFormHandlers();
    setupAutoSave();
});

function setupFormHandlers() {
    const vitalSignsForm = document.getElementById('vitalSignsForm');
    const consultationForm = document.getElementById('consultationForm');

    vitalSignsForm?.addEventListener('submit', function(e) {
        e.preventDefault();
        saveVitalSigns();
    });

    consultationForm?.addEventListener('submit', function(e) {
        e.preventDefault();
        saveConsultationNotes();
    });
}

function setupAutoSave() {
    const textareas = document.querySelectorAll('textarea');
    textareas.forEach(textarea => {
        textarea.addEventListener('blur', function() {
            autoSaveConsultation();
        });
    });
}

function saveVitalSigns() {
    const form = document.getElementById('vitalSignsForm');
    const formData = new FormData(form);
    
    fetch('/api/consultation/vitals', {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            showToast('Vital signs saved', 'success');
        } else {
            showToast('Failed to save vital signs', 'error');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        showToast('An error occurred', 'error');
    });
}

function saveConsultationNotes() {
    const form = document.getElementById('consultationForm');
    const formData = new FormData(form);
    
    fetch('/api/consultation/notes', {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            showToast('Consultation notes saved', 'success');
        } else {
            showToast('Failed to save consultation notes', 'error');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        showToast('An error occurred', 'error');
    });
}

function autoSaveConsultation() {
    const consultationForm = document.getElementById('consultationForm');
    const formData = new FormData(consultationForm);
    
    fetch('/api/consultation/autosave', {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            console.log('Auto-saved consultation notes');
        }
    })
    .catch(error => {
        console.error('Auto-save error:', error);
    });
}

function completeConsultation() {
    if (confirm('Are you sure you want to complete this consultation?')) {
        fetch('/api/consultation/complete', {
            method: 'POST'
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showToast('Consultation completed', 'success');
                setTimeout(() => {
                    window.location.href = '/Doctor/Appointments';
                }, 1500);
            } else {
                showToast('Failed to complete consultation', 'error');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showToast('An error occurred', 'error');
        });
    }
}

function writePrescription() {
    const consultationId = new URLSearchParams(window.location.search).get('id');
    window.location.href = `/Doctor/Prescriptions/New?consultationId=${consultationId}`;
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