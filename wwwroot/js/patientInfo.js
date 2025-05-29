// Function to search for patients
async function searchPatient(query) {
    try {
        console.log('Searching for patients with query:', query);
        const response = await fetch(`/api/nurse/search-patient?query=${encodeURIComponent(query)}`);
        if (!response.ok) {
            throw new Error('Failed to search patients');
        }
        
        const patients = await response.json();
        console.log('Found patients:', patients);
        displaySearchResults(patients);
    } catch (error) {
        console.error('Error searching patients:', error);
        showToast('Failed to search patients', 'error');
    }
}

// Function to display search results
function displaySearchResults(patients) {
    const searchResultsContainer = document.getElementById('searchResults');
    if (!searchResultsContainer) {
        console.error('Search results container not found');
        return;
    }

    if (patients.length === 0) {
        searchResultsContainer.innerHTML = '<p class="text-muted">No patients found</p>';
        return;
    }

    const html = patients.map(patient => `
        <div class="patient-card" onclick="loadPatientInformation('${patient.userId}')">
            <div class="d-flex justify-content-between align-items-center">
                <h5 class="mb-0">${patient.name || 'No name'}</h5>
                <span class="badge bg-${getStatusBadgeClass(patient.status)}">${patient.status || 'Unknown'}</span>
            </div>
            <div class="patient-info">
                <p class="mb-1">
                    <span class="text-muted">Gender:</span> ${patient.gender || 'Not specified'} | 
                    <span class="text-muted">Age:</span> ${patient.age || 'Not specified'}
                </p>
                <p class="mb-1">
                    <span class="text-muted">Last Visit:</span> ${formatDate(patient.lastVisit) || 'No visits yet'}
                </p>
                <p class="mb-0">
                    <span class="text-muted">Contact:</span> ${patient.contactNumber || 'Not specified'}
                </p>
            </div>
        </div>
    `).join('');

    searchResultsContainer.innerHTML = html;
}

// Function to get badge class based on status
function getStatusBadgeClass(status) {
    switch (status?.toLowerCase()) {
        case 'active': return 'success';
        case 'pending': return 'warning';
        case 'inactive': return 'danger';
        default: return 'secondary';
    }
}

// Function to load and display patient information
async function loadPatientInformation(userId) {
    try {
        console.log('Loading patient information for userId:', userId);
        const response = await fetch(`/api/doctor/patient/${userId}`);
        if (!response.ok) {
            throw new Error('Failed to fetch patient information');
        }
        
        const patientInfo = await response.json();
        console.log('Received patient info:', patientInfo);
        
        // Update personal information
        updateElementText('fullName', patientInfo.name);
        updateElementText('gender', patientInfo.gender);
        updateElementText('age', `${patientInfo.age} years`);
        updateElementText('lastVisit', formatDate(patientInfo.lastVisit));
        updateElementText('address', patientInfo.address);
        updateElementText('contactNumber', patientInfo.contactNumber);
        
        // Update medical information
        updateTextAreaValue('allergies', patientInfo.allergies);
        updateTextAreaValue('currentMedications', patientInfo.currentMedications);
        updateTextAreaValue('medicalHistory', patientInfo.medicalHistory);
        
        // Update status
        const statusElement = document.getElementById('patientStatus');
        if (statusElement) {
            statusElement.textContent = patientInfo.status || 'Unknown';
            statusElement.className = `badge bg-${getStatusBadgeClass(patientInfo.status)}`;
        }

        // Update vital signs if available
        if (patientInfo.vitalSigns && patientInfo.vitalSigns.length > 0) {
            const latestVitals = patientInfo.vitalSigns[0];
            updateElementText('temperature', `${latestVitals.temperature || 'N/A'} °C`);
            updateElementText('bloodPressure', latestVitals.bloodPressure || 'N/A');
            updateElementText('heartRate', `${latestVitals.heartRate || 'N/A'} bpm`);
            updateElementText('respiratoryRate', `${latestVitals.respiratoryRate || 'N/A'} /min`);
            updateElementText('spO2', `${latestVitals.spO2 || 'N/A'} %`);
            updateElementText('vitalsDate', formatDate(latestVitals.recordedAt));
        }
        
    } catch (error) {
        console.error('Error loading patient information:', error);
        showToast('Failed to load patient information', 'error');
    }
}

// Helper function to update element text content
function updateElementText(elementId, value) {
    const element = document.getElementById(elementId);
    if (element) {
        element.textContent = value || 'Not specified';
    } else {
        console.warn(`Element with id '${elementId}' not found`);
    }
}

// Helper function to update textarea value
function updateTextAreaValue(elementId, value) {
    const element = document.getElementById(elementId);
    if (element) {
        element.value = value || '';
    } else {
        console.warn(`Textarea with id '${elementId}' not found`);
    }
}

// Helper function to format date
function formatDate(dateString) {
    if (!dateString) return 'Not specified';
    try {
        const date = new Date(dateString);
        const options = { 
            year: 'numeric', 
            month: 'short', 
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        };
        return date.toLocaleDateString('en-US', options);
    } catch (error) {
        console.warn('Error formatting date:', error);
        return 'Invalid date';
    }
}

// Function to show toast notifications
function showToast(message, type = 'info') {
    const toast = document.createElement('div');
    toast.className = `toast align-items-center text-white bg-${type} border-0`;
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
    
    const toastContainer = document.getElementById('toast-container');
    if (!toastContainer) {
        const container = document.createElement('div');
        container.id = 'toast-container';
        container.className = 'toast-container position-fixed bottom-0 end-0 p-3';
        document.body.appendChild(container);
    }
    
    document.getElementById('toast-container').appendChild(toast);
    const bsToast = new bootstrap.Toast(toast);
    bsToast.show();
    
    // Remove the toast after it's hidden
    toast.addEventListener('hidden.bs.toast', () => {
        toast.remove();
    });
}

// Function to update patient medical information
async function updateMedicalInformation() {
    try {
        const form = document.getElementById('medicalInfoForm');
        const userId = form.dataset.patientId;
        if (!userId) {
            throw new Error('Patient ID not found');
        }

        const medicalInfo = {
            medicalHistory: document.getElementById('medicalHistory')?.value?.trim(),
            allergies: document.getElementById('allergies')?.value?.trim(),
            currentMedications: document.getElementById('currentMedications')?.value?.trim()
        };

        const response = await fetch(`/api/doctor/patient/${userId}/update-medical-info`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify(medicalInfo)
        });

        if (!response.ok) {
            throw new Error('Failed to update medical information');
        }

        const result = await response.json();
        showToast(result.message || 'Medical information updated successfully', 'success');
        
        // Switch back to view mode
        document.getElementById('medicalInfoView').style.display = 'block';
        document.getElementById('medicalInfoEdit').style.display = 'none';
        document.getElementById('editMedicalInfoBtn').style.display = 'inline-block';
        
        // Refresh the view
        await loadPatientInformation(userId);
    } catch (error) {
        console.error('Error updating medical information:', error);
        showToast('Failed to update medical information', 'error');
    }
}

// Initialize patient search when the page loads
document.addEventListener('DOMContentLoaded', function() {
    console.log('Initializing patient search...');
    const searchInput = document.getElementById('patientSearch');
    if (searchInput) {
        searchInput.addEventListener('input', debounce((e) => {
            const query = e.target.value.trim();
            if (query.length >= 2) {
                searchPatient(query);
            } else {
                const searchResults = document.getElementById('searchResults');
                if (searchResults) {
                    searchResults.innerHTML = '';
                }
            }
        }, 300));
    } else {
        console.warn('Patient search input not found');
    }

    // Load initial patient if ID is available
    const patientId = document.querySelector('[data-patient-id]')?.dataset.patientId;
    if (patientId) {
        console.log('Loading initial patient:', patientId);
        loadPatientInformation(patientId);
    } else {
        console.log('No initial patient ID found');
    }

    // Add event listener for the update medical information button
    const updateMedicalInfoBtn = document.getElementById('updateMedicalInfoBtn');
    if (updateMedicalInfoBtn) {
        updateMedicalInfoBtn.addEventListener('click', updateMedicalInformation);
    } else {
        console.warn('Update medical information button not found');
    }
});

// Debounce function to limit API calls
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}; 