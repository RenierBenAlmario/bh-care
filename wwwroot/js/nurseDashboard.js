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

    // Initialize variables
    let autoRefreshEnabled = true;
    let autoRefreshInterval;
    const REFRESH_INTERVAL = 30000; // 30 seconds

    // Initialize components
    initializeAutoRefresh();
    initializeMessageSystem();
    initializeButtons();
    initializeTooltips();

    function initializeAutoRefresh() {
        const toggleButton = document.querySelector('[data-toggle="auto-refresh"]');
        if (toggleButton) {
            toggleButton.addEventListener('click', function() {
                autoRefreshEnabled = !autoRefreshEnabled;
                toggleButton.textContent = `Toggle Auto-Refresh (${autoRefreshEnabled ? 'On' : 'Off'})`;
                if (autoRefreshEnabled) {
                    startAutoRefresh();
                } else {
                    stopAutoRefresh();
                }
            });
        }
        startAutoRefresh();
    }

    $(document).ready(function() {
        let autoRefreshInterval;
        const AUTO_REFRESH_INTERVAL = 30000; // 30 seconds
    
        function startAutoRefresh() {
            autoRefreshInterval = setInterval(refreshDashboard, AUTO_REFRESH_INTERVAL);
            $('#toggleAutoRefresh').text('Toggle Auto-Refresh (On)');
        }
    
        function stopAutoRefresh() {
            clearInterval(autoRefreshInterval);
            $('#toggleAutoRefresh').text('Toggle Auto-Refresh (Off)');
        }
    
        function refreshDashboard() {
            location.reload();
        }
    
        $('#toggleAutoRefresh').click(function() {
            if (autoRefreshInterval) {
                stopAutoRefresh();
            } else {
                startAutoRefresh();
            }
        });
    
        $('#nextPatient').click(function() {
            // Implement next patient logic
            $.post('?handler=NextPatient', function(response) {
                refreshDashboard();
            });
        });
    
        $('#sendMessageBtn').click(function() {
            const message = $('#messageInput').val();
            const recipient = $('#recipientSelect').val();
            
            if (message.trim()) {
                $.post('?handler=SendMessage', { 
                    message: message, 
                    recipient: recipient 
                }, function(response) {
                    $('#messageInput').val('');
                    refreshDashboard();
                });
            }
        });
    
        // Start auto-refresh by default
        startAutoRefresh();
    });

    function updateDashboardUI(data) {
        // Update metrics
        const totalPatientsElement = document.querySelector('#totalPatients');
        const inProgressPatientsElement = document.querySelector('#inProgressPatients');
        const waitingPatientsElement = document.querySelector('#waitingPatients');
        const completedPatientsElement = document.querySelector('#completedPatients');

        if (totalPatientsElement) totalPatientsElement.textContent = data.metrics.totalPatients;
        if (inProgressPatientsElement) inProgressPatientsElement.textContent = data.metrics.inProgressPatients;
        if (waitingPatientsElement) waitingPatientsElement.textContent = data.metrics.waitingPatients;
        if (completedPatientsElement) completedPatientsElement.textContent = data.metrics.completedPatients;

        // Update treatment cards
        const treatmentContainer = document.querySelector('#treatmentCards');
        if (treatmentContainer && data.appointments) {
            console.log('Appointments before filtering:', data.appointments); // Add this debug line
            const filteredAppointments = data.appointments.filter(apt => apt.status !== 'Completed');
            console.log('Appointments after filtering:', filteredAppointments); // Add this debug line
            
            if (filteredAppointments.length === 0) {
                treatmentContainer.innerHTML = '<div class="alert alert-info">No active appointments found.</div>';
            } else {
                treatmentContainer.innerHTML = filteredAppointments
                    .map(apt => createTreatmentCardHTML(apt))
                    .join('');
            }
        }
    }

    function createTreatmentCardHTML(appointment) {
        return `
            <div class="treatment-card mb-3 ${appointment.status === 'Urgent' ? 'border-danger' : ''}">
                <div class="card">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-center mb-2">
                            <h5 class="mb-0">${appointment.patientName}</h5>
                            <span class="badge bg-${getStatusBadgeClass(appointment.status)}">${appointment.status}</span>
                        </div>
                        ${appointment.allergies ? `
                        <p class="text-muted mb-2">
                            <i class="bi bi-exclamation-triangle-fill text-warning me-1"></i>
                            ${appointment.allergies}
                        </p>
                        ` : ''}
                        <p class="mb-2">
                            <i class="bi bi-clock me-1"></i>
                            ${appointment.startTime}
                        </p>
                        <p class="mb-2">
                            <i class="bi bi-clipboard2-pulse me-1"></i>
                            ${appointment.condition}
                        </p>
                        <div class="d-flex justify-content-between align-items-center">
                            <button class="btn btn-success btn-sm" onclick="completeAppointment('${appointment.id}')">Complete</button>
                            <button class="btn btn-warning btn-sm" onclick="prioritizeAppointment('${appointment.id}')">Prioritize</button>
                        </div>
                    </div>
                </div>
            </div>
        `;
    }

    function getStatusBadgeClass(status) {
        switch (status) {
            case 'Waiting': return 'warning';
            case 'InProgress': return 'success';
            case 'Urgent': return 'danger';
            default: return 'secondary';
        }
    }

    function initializeMessageSystem() {
        const messageForm = document.querySelector('#messageForm');
        const messageInput = document.querySelector('#messageInput');
        const recipientSelect = document.querySelector('#recipientSelect');

        if (messageForm) {
            messageForm.addEventListener('submit', async (e) => {
                e.preventDefault();
                const message = messageInput.value.trim();
                const recipient = recipientSelect.value;

                if (!message) return;

                try {
                    const response = await fetch('/api/nurse/send-message', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                        },
                        body: JSON.stringify({
                            message,
                            recipient,
                        }),
                    });

                    if (!response.ok) throw new Error('Failed to send message');

                    messageInput.value = '';
                    await refreshMessages();
                } catch (error) {
                    console.error('Error sending message:', error);
                    showToast('Error sending message', 'error');
                }
            });
        }
    }

    async function refreshMessages() {
        try {
            const response = await fetch('/api/nurse/messages');
            if (!response.ok) throw new Error('Failed to fetch messages');
            const messages = await response.json();
            updateMessagesUI(messages);
        } catch (error) {
            console.error('Error refreshing messages:', error);
        }
    }

    function updateMessagesUI(messages) {
        const messagesContainer = document.querySelector('#messages');
        if (messagesContainer) {
            messagesContainer.innerHTML = messages
                .map(msg => `
                    <div class="message mb-2">
                        <strong>${msg.sender} (to ${msg.recipient}):</strong>
                        <p class="mb-1">${msg.content}</p>
                    </div>
                `)
                .join('');
        }
    }

    function initializeButtons() {
        // Next Patient button
        document.querySelector('#nextPatient')?.addEventListener('click', async () => {
            try {
                const response = await fetch('/api/nurse/next-patient');
                if (!response.ok) throw new Error('Failed to get next patient');
                const data = await response.json();
                showToast(`Next patient: ${data.patientName}`, 'success');
            } catch (error) {
                console.error('Error getting next patient:', error);
                showToast('Error getting next patient', 'error');
            }
        });

        // Add Note button
        document.querySelector('#addNote')?.addEventListener('click', () => {
            const noteModal = new bootstrap.Modal(document.querySelector('#noteModal'));
            noteModal.show();
        });

        // Assign Task button
        document.querySelector('#assignTask')?.addEventListener('click', () => {
            const taskModal = new bootstrap.Modal(document.querySelector('#taskModal'));
            taskModal.show();
        });

        // Patient Search button
        document.querySelector('#patientSearch')?.addEventListener('click', () => {
            const searchModal = new bootstrap.Modal(document.querySelector('#searchModal'));
            searchModal.show();
        });
    }

    function initializeTooltips() {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }

    // Expose necessary functions globally
    window.completeAppointment = async function(appointmentId) {
        try {
            const response = await fetch('/api/nurse/complete-appointment', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ appointmentId }),
            });

            if (!response.ok) throw new Error('Failed to complete appointment');
            showToast('Appointment completed successfully', 'success');
            await refreshDashboard();
        } catch (error) {
            console.error('Error completing appointment:', error);
            showToast('Error completing appointment', 'error');
        }
    };

    window.prioritizeAppointment = async function(appointmentId) {
        try {
            const response = await fetch('/api/nurse/prioritize-appointment', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ appointmentId }),
            });

            if (!response.ok) throw new Error('Failed to prioritize appointment');
            showToast('Appointment prioritized successfully', 'success');
            await refreshDashboard();
        } catch (error) {
            console.error('Error prioritizing appointment:', error);
            showToast('Error prioritizing appointment', 'error');
        }
    };
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
        const modal = document.getElementById('sessionTimeoutModal');
        if (modal) {
            modal.classList.remove('show');
        }
    }

    function startTimeoutCountdown() {
        timeoutInterval = setInterval(() => {
            timeoutDuration--;
            if (timeoutDuration <= 0) {
                window.location.href = '/Account/Logout';
            } else {
                const countdown = document.getElementById('timeoutCountdown');
                if (countdown) {
                    countdown.textContent = timeoutDuration;
                }
            }
        }, 1000);
    }

    document.addEventListener('mousemove', () => {
        if (!warningShown) {
            resetTimeout();
        }
    });

    const stayLoggedInBtn = document.getElementById('stayLoggedIn');
    if (stayLoggedInBtn) {
        stayLoggedInBtn.addEventListener('click', () => {
            resetTimeout();
        });
    }

    const logoutNowBtn = document.getElementById('logoutNow');
    if (logoutNowBtn) {
        logoutNowBtn.addEventListener('click', () => {
            window.location.href = '/Account/Logout';
        });
    }
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
    const ctx = document.getElementById('patientMetricsChart');
    if (ctx) {
        new Chart(ctx, {
            type: 'line',
            data: {
                labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
                datasets: [{
                    label: 'Patients Seen',
                    data: window.chartData?.patientsSeen || [0, 0, 0, 0, 0, 0, 0],
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
function acknowledgeAlert(alertId) {
    fetch('/api/nurse/acknowledgeAlert', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ alertId: alertId })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            const alertElement = document.querySelector(`[data-alert-id="${alertId}"]`);
            if (alertElement) {
                alertElement.remove();
            }
            updateAlertCount();
        }
    })
    .catch(error => console.error('Error:', error));
}

// Update alert count
function updateAlertCount() {
    const alertsList = document.getElementById('alertsList');
    const alertCount = alertsList.children.length;
    const alertBadge = document.getElementById('alertBadge');
    if (alertBadge) {
        alertBadge.textContent = alertCount;
        alertBadge.style.display = alertCount > 0 ? 'inline' : 'none';
    }
}

// View patient history
function viewHistory(patientId) {
    fetch(`/api/nurse/patientHistory/${patientId}`)
        .then(response => response.json())
        .then(data => {
            // Implementation for displaying patient history
            console.log('Patient history:', data);
        })
        .catch(error => console.error('Error:', error));
}

// Handle form submissions
document.getElementById('noteForm')?.addEventListener('submit', function(e) {
    e.preventDefault();
    const formData = new FormData(this);
    
    fetch('/api/nurse/addNote', {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            bootstrap.Modal.getInstance(document.getElementById('noteModal')).hide();
            this.reset();
            // Show success message
            showToast('Note added successfully');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        showToast('Error adding note', 'error');
    });
});

// Toast notification
function showToast(message, type = 'info') {
    const toastContainer = document.querySelector('#toastContainer');
    if (!toastContainer) return;

    const toast = document.createElement('div');
    toast.className = `toast align-items-center text-white bg-${type} border-0`;
    toast.setAttribute('role', 'alert');
    toast.setAttribute('aria-live', 'assertive');
    toast.setAttribute('aria-atomic', 'true');

    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">${message}</div>
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
        initSearch,
        debounce,
        acknowledgeAlert,
        updateAlertCount,
        viewHistory,
        showToast
    };
}
document.addEventListener('DOMContentLoaded', function() {
    // Initialize buttons
    const completeBtn = document.getElementById('completeBtn');
    const prioritizeBtn = document.getElementById('prioritizeBtn');
    
    if (completeBtn) {
        completeBtn.addEventListener('click', function() {
            if (confirm('Are you sure you want to mark this appointment as completed?')) {
                document.getElementById('completeForm').submit();
            }
        });
    }
    
    if (prioritizeBtn) {
        prioritizeBtn.addEventListener('click', function() {
            if (confirm('Are you sure you want to prioritize this appointment?')) {
                document.getElementById('prioritizeForm').submit();
            }
        });
    }
    
    // Auto-refresh functionality
    const toggleAutoRefreshBtn = document.getElementById('toggleAutoRefresh');
    let autoRefreshEnabled = true;
    let refreshInterval;
    
    function startAutoRefresh() {
        refreshInterval = setInterval(function() {
            // Refresh the current appointment data
            fetch('/api/appointments/current')
                .then(response => response.json())
                .then(data => {
                    updateTreatmentPlan(data);
                })
                .catch(error => console.error('Error fetching current appointment:', error));
        }, 30000); // Refresh every 30 seconds
    }
    
    function stopAutoRefresh() {
        clearInterval(refreshInterval);
    }
    
    if (toggleAutoRefreshBtn) {
        toggleAutoRefreshBtn.addEventListener('click', function() {
            autoRefreshEnabled = !autoRefreshEnabled;
            if (autoRefreshEnabled) {
                toggleAutoRefreshBtn.textContent = 'Toggle Auto-Refresh (On)';
                startAutoRefresh();
            } else {
                toggleAutoRefreshBtn.textContent = 'Toggle Auto-Refresh (Off)';
                stopAutoRefresh();
            }
        });
    }
    
    // Start auto-refresh by default
    if (autoRefreshEnabled) {
        startAutoRefresh();
    }
    
    function updateTreatmentPlan(data) {
        const patientNameElement = document.getElementById('currentPatientName');
        const appointmentTimeElement = document.getElementById('appointmentTime');
        const treatmentPlanElement = document.querySelector('.treatment-plan-content');
        const appointmentIdInput = document.getElementById('appointmentId');
        
        if (patientNameElement && data.patientName) {
            patientNameElement.textContent = data.patientName;
        }
        
        if (appointmentTimeElement && data.appointmentTime) {
            appointmentTimeElement.textContent = data.appointmentTime;
        }
        
        if (treatmentPlanElement && data.description) {
            treatmentPlanElement.innerHTML = `<p>${data.description || 'No description provided'}</p>`;
        }
        
        if (appointmentIdInput && data.id) {
            appointmentIdInput.value = data.id;
        }
    }
});