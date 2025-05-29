document.addEventListener('DOMContentLoaded', function() {
    setupSearchFilter();
});

function setupSearchFilter() {
    const searchInput = document.getElementById('searchPatients');
    if (searchInput) {
        searchInput.addEventListener('input', function(e) {
            const searchTerm = e.target.value.toLowerCase();
            const patientList = document.querySelectorAll('.list-group-item');
            
            patientList.forEach(item => {
                const patientName = item.querySelector('h6').textContent.toLowerCase();
                const patientRecord = item.querySelector('small').textContent.toLowerCase();
                
                if (patientName.includes(searchTerm) || patientRecord.includes(searchTerm)) {
                    item.style.display = '';
                } else {
                    item.style.display = 'none';
                }
            });
        });
    }
}

function loadPatientDetails(patientId) {
    fetch(`/api/patients/${patientId}`)
        .then(response => response.json())
        .then(patient => {
            updatePatientInfo(patient);
            updateVitalSigns(patient.vitalSigns);
            updateMedicalHistory(patient.medicalHistory);
            enableActionButtons();
        })
        .catch(error => {
            console.error('Error loading patient details:', error);
            showToast('Failed to load patient details', 'error');
        });
}

function updatePatientInfo(patient) {
    document.getElementById('patientName').textContent = patient.name;
    document.getElementById('patientBasicInfo').innerHTML = `
        Age: ${patient.age}<br>
        Gender: ${patient.gender}<br>
        Record No: ${patient.recordNo}
    `;
}

function updateVitalSigns(vitalSigns) {
    if (vitalSigns) {
        document.getElementById('bp').textContent = vitalSigns.bloodPressure;
        document.getElementById('heartRate').textContent = vitalSigns.heartRate + ' bpm';
        document.getElementById('temperature').textContent = vitalSigns.temperature + '°C';
        document.getElementById('weight').textContent = vitalSigns.weight + ' kg';
    }
}

function updateMedicalHistory(history) {
    const tbody = document.getElementById('medicalHistory');
    tbody.innerHTML = '';
    
    if (history && history.length > 0) {
        history.forEach(record => {
            tbody.innerHTML += `
                <tr>
                    <td>${new Date(record.date).toLocaleDateString()}</td>
                    <td>${record.type}</td>
                    <td>${record.diagnosis}</td>
                    <td>${record.treatment}</td>
                    <td>${record.doctor}</td>
                </tr>
            `;
        });
    } else {
        tbody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">No medical history available</td></tr>';
    }
}

function enableActionButtons() {
    const buttons = document.querySelectorAll('.btn-group button');
    buttons.forEach(button => button.disabled = false);
}

function startConsultation() {
    const patientId = getCurrentPatientId();
    if (patientId) {
        window.location.href = `/Doctor/Consultation/${patientId}`;
    }
}

function scheduleAppointment() {
    const patientId = getCurrentPatientId();
    if (patientId) {
        window.location.href = `/Doctor/Appointments?patientId=${patientId}`;
    }
}

function writePrescription() {
    const patientId = getCurrentPatientId();
    if (patientId) {
        window.location.href = `/Doctor/Prescriptions/New?patientId=${patientId}`;
    }
}

function getCurrentPatientId() {
    // TODO: Implement getting current patient ID
    return document.querySelector('.list-group-item.active')?.dataset.patientId;
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