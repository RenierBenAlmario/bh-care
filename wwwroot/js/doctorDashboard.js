// Initialize tooltips and popovers
document.addEventListener('DOMContentLoaded', function() {
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

    // Initialize search functionality
    initSearch();

    // Initialize charts
    initCharts();
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

// Search functionality
function initSearch() {
    const searchInput = document.getElementById('modalPatientSearch');
    if (searchInput) {
        searchInput.addEventListener('input', debounce(function(e) {
            const searchTerm = e.target.value.toLowerCase();
            const rows = document.querySelectorAll('#modalPatientRecordsTable tr');
            
            rows.forEach(row => {
                const text = row.textContent.toLowerCase();
                row.style.display = text.includes(searchTerm) ? '' : 'none';
            });
        }, 300));
    }
}

// Utility function for debouncing
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
}

// Charts initialization
function initCharts() {
    const ctx = document.getElementById('consultationMetricsChart');
    if (ctx) {
        new Chart(ctx, {
            type: 'line',
            data: {
                labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
                datasets: [{
                    label: 'Consultations',
                    data: window.chartData?.consultations || [0, 0, 0, 0, 0, 0, 0],
                    borderColor: 'rgb(75, 192, 192)',
                    tension: 0.1
                }]
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }
}

// Alert acknowledgment
function acknowledgeAlert(recordNo) {
    fetch('/api/doctor/acknowledgeAlert', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ recordNo: recordNo })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            const alertElement = document.querySelector(`[data-record-no="${recordNo}"]`);
            if (alertElement) {
                alertElement.remove();
            }
            updateAlertCount();
            showToast('Alert acknowledged successfully');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        showToast('Error acknowledging alert', 'error');
    });
}

// Toast notification
function showToast(message, type = 'info') {
    const toastContainer = document.querySelector('.toast-container');
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
    
    toastContainer.appendChild(toast);
    const bsToast = new bootstrap.Toast(toast);
    bsToast.show();
    
    toast.addEventListener('hidden.bs.toast', () => {
        toast.remove();
    });
}

// Start consultation
async function startConsultation(recordNo) {
    try {
        const response = await fetch('/Doctor/DoctorDashboard?handler=StartConsultation', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify({ recordNo })
        });

        const data = await response.json();
        
        if (data.success) {
            showToast(data.message, 'success');
            startTimer(data.patientName);
            updateQueueDisplay();
        } else {
            showToast(data.message, 'danger');
        }
    } catch (error) {
        showToast('Error starting consultation', 'danger');
        console.error('Error:', error);
    }
}

// End consultation
async function endConsultation(recordNo) {
    try {
        const response = await fetch('/Doctor/DoctorDashboard?handler=EndConsultation', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify({ recordNo })
        });

        const data = await response.json();
        
        if (data.success) {
            showToast(data.message, 'success');
            stopTimer();
            updateQueueDisplay();
        } else {
            showToast(data.message, 'danger');
        }
    } catch (error) {
        showToast('Error ending consultation', 'danger');
        console.error('Error:', error);
    }
}

// View patient history
async function viewHistory(recordNo) {
    try {
        const response = await fetch(`/Doctor/DoctorDashboard?handler=PatientHistory&recordNo=${recordNo}`);
        const data = await response.json();
        
        if (data) {
            const historyModal = new bootstrap.Modal(document.getElementById('historyModal'));
            const historyContainer = document.getElementById('patientHistoryContainer');
            
            historyContainer.innerHTML = data.map(record => `
                <div class="history-item border-start border-4 border-primary p-3 mb-3">
                    <div class="d-flex justify-content-between">
                        <strong>${record.date}</strong>
                        <span class="badge bg-${getStatusBadgeColor(record.status)}">${record.status}</span>
                    </div>
                    <p class="mb-1"><strong>Condition:</strong> ${record.condition || 'N/A'}</p>
                    <p class="mb-0"><strong>Notes:</strong> ${record.notes || 'No notes available'}</p>
                </div>
            `).join('');
            
            historyModal.show();
        } else {
            showToast('Error fetching patient history', 'danger');
        }
    } catch (error) {
        showToast('Error fetching patient history', 'danger');
        console.error('Error:', error);
    }
}

// Add prescription
async function addPrescription(recordNo) {
    const form = document.getElementById('prescriptionForm');
    const formData = new FormData(form);
    
    try {
        const response = await fetch('/Doctor/DoctorDashboard?handler=AddPrescription', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify(Object.fromEntries(formData))
        });

        const data = await response.json();
        
        if (data.success) {
            showToast(data.message, 'success');
            const prescriptionModal = bootstrap.Modal.getInstance(document.getElementById('prescriptionModal'));
            prescriptionModal.hide();
            form.reset();
        } else {
            showToast(data.message, 'danger');
        }
    } catch (error) {
        showToast('Error adding prescription', 'danger');
        console.error('Error:', error);
    }
}

// Update queue display
function updateQueueDisplay() {
    const queueCount = document.getElementById('queueCount');
    const currentCount = parseInt(queueCount.textContent);
    queueCount.textContent = Math.max(0, currentCount - 1);
}

// Helper function to get status badge color
function getStatusBadgeColor(status) {
    switch (status.toLowerCase()) {
        case 'completed':
            return 'success';
        case 'inprogress':
            return 'primary';
        case 'urgent':
            return 'danger';
        case 'pending':
            return 'warning';
        default:
            return 'secondary';
    }
}

// Export functions for testing
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        initSessionTimeout,
        initOfflineDetection,
        initSearch,
        debounce,
        acknowledgeAlert,
        startConsultation,
        viewHistory,
        updateAlertCount,
        showToast
    };
} 