/**
 * Admin Reports JavaScript File
 * This file contains functions to interact with the database through API endpoints
 * and update the report charts with real data.
 */

document.addEventListener('DOMContentLoaded', function() {
    // Initialize charts
    initializeReportCharts();
    
    // Load initial data
    fetchAllReportsData();
    
    // Setup event listeners for report filters
    setupEventListeners();
});

// Set up all necessary event listeners
function setupEventListeners() {
    // Show custom date range when "Custom Range" is selected
    const timeRangeSelect = document.getElementById('timeRange');
    const customDateRange = document.getElementById('customDateRange');
    
    timeRangeSelect.addEventListener('change', function() {
        if (this.value === 'custom') {
            customDateRange.style.display = 'block';
        } else {
            customDateRange.style.display = 'none';
        }
    });
    
    // Apply filters button click handler
    document.getElementById('applyFilters').addEventListener('click', function() {
        fetchAllReportsData();
    });
    
    // Refresh data button click handler
    document.getElementById('refresh-data').addEventListener('click', function() {
        this.classList.add('rotating');
        fetchAllReportsData();
        setTimeout(() => {
            this.classList.remove('rotating');
            showMessage('Reports refreshed successfully', 'success');
        }, 1000);
    });
}

// Initialize all report charts with empty data
function initializeReportCharts() {
    // Staff Distribution Chart
    const staffCtx = document.getElementById('staffDistributionChart').getContext('2d');
    window.staffDistributionChart = new Chart(staffCtx, {
        type: 'pie',
        data: {
            labels: [],
            datasets: [{
                data: [],
                backgroundColor: [
                    '#4285F4', // Doctor
                    '#34A853', // Nurse
                    '#EA4335', // Admin
                    '#FBBC05', // Pharmacist
                    '#7B8794'  // Other
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                }
            }
        }
    });
    
    // Patient Registrations Chart
    const patientCtx = document.getElementById('patientRegistrationsChart').getContext('2d');
    window.patientRegistrationsChart = new Chart(patientCtx, {
        type: 'line',
        data: {
            labels: [],
            datasets: [{
                label: 'New Registrations',
                data: [],
                borderColor: '#1976D2',
                backgroundColor: 'rgba(25, 118, 210, 0.1)',
                fill: true,
                tension: 0.4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    precision: 0
                }
            }
        }
    });
    
    // Consultations by Type Chart
    const consultationsCtx = document.getElementById('consultationsByTypeChart').getContext('2d');
    window.consultationsByTypeChart = new Chart(consultationsCtx, {
        type: 'bar',
        data: {
            labels: [],
            datasets: [{
                label: 'Consultations',
                data: [],
                backgroundColor: [
                    'rgba(25, 118, 210, 0.7)',
                    'rgba(38, 166, 154, 0.7)',
                    'rgba(255, 160, 0, 0.7)',
                    'rgba(211, 47, 47, 0.7)',
                    'rgba(123, 31, 162, 0.7)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    precision: 0
                }
            }
        }
    });
    
    // Health Index Chart
    const healthCtx = document.getElementById('healthIndexChart').getContext('2d');
    window.healthIndexChart = new Chart(healthCtx, {
        type: 'line',
        data: {
            labels: [],
            datasets: [{
                label: 'Health Index',
                data: [],
                borderColor: '#26A69A',
                backgroundColor: 'rgba(38, 166, 154, 0.1)',
                fill: true,
                tension: 0.4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    min: 0,
                    max: 100
                }
            }
        }
    });
}

// Fetch data for all reports
function fetchAllReportsData() {
    const timeRange = document.getElementById('timeRange').value;
    let startDate = null;
    let endDate = null;
    
    if (timeRange === 'custom') {
        startDate = document.getElementById('startDate').value;
        endDate = document.getElementById('endDate').value;
    }
    
    // Fetch all report data in parallel
    Promise.all([
        fetchStaffDistribution(),
        fetchPatientRegistrations(timeRange, startDate, endDate),
        fetchConsultationsByType(timeRange, startDate, endDate),
        fetchHealthIndex(timeRange, startDate, endDate)
    ]).then(() => {
        // All data has been fetched and charts updated
        showMessage('All reports data has been updated', 'success');
    }).catch(error => {
        console.error('Error fetching report data:', error);
        showMessage('Error loading report data. Please try again.', 'error');
    });
}

// Fetch staff distribution data
function fetchStaffDistribution() {
    document.getElementById('staffDistributionNoData').style.display = 'none';
    
    return fetch('/Admin/Reports?handler=StaffDistribution')
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch staff distribution data');
            }
            return response.json();
        })
        .then(data => {
            if (!data || data.length === 0) {
                document.getElementById('staffDistributionNoData').style.display = 'flex';
                return;
            }
            
            // Update staff distribution chart
            const labels = data.map(item => item.role);
            const counts = data.map(item => item.count);
            
            window.staffDistributionChart.data.labels = labels;
            window.staffDistributionChart.data.datasets[0].data = counts;
            window.staffDistributionChart.update();
            
            // Update chart legend if needed
            updateStaffLegend(data);
        })
        .catch(error => {
            console.error('Error fetching staff distribution:', error);
            document.getElementById('staffDistributionNoData').style.display = 'flex';
        });
}

// Fetch patient registrations data
function fetchPatientRegistrations(timeRange, startDate, endDate) {
    document.getElementById('patientRegistrationsNoData').style.display = 'none';
    document.getElementById('totalRegistrations').textContent = 'Loading...';
    
    let url = `/Admin/Reports?handler=PatientRegistrations&timeRange=${timeRange}`;
    if (timeRange === 'custom' && startDate && endDate) {
        url += `&startDate=${startDate}&endDate=${endDate}`;
    }
    
    return fetch(url)
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch patient registrations data');
            }
            return response.json();
        })
        .then(data => {
            if (!data || !data.registrations || data.registrations.length === 0) {
                document.getElementById('patientRegistrationsNoData').style.display = 'flex';
                document.getElementById('totalRegistrations').textContent = '0';
                return;
            }
            
            // Update patient registrations chart
            const registrations = data.registrations;
            
            const labels = registrations.map(item => formatDate(item.date));
            const counts = registrations.map(item => item.count);
            
            window.patientRegistrationsChart.data.labels = labels;
            window.patientRegistrationsChart.data.datasets[0].data = counts;
            window.patientRegistrationsChart.update();
            
            // Update total registrations
            document.getElementById('totalRegistrations').textContent = data.total || 0;
        })
        .catch(error => {
            console.error('Error fetching patient registrations:', error);
            document.getElementById('patientRegistrationsNoData').style.display = 'flex';
            document.getElementById('totalRegistrations').textContent = '0';
        });
}

// Fetch consultations by type data
function fetchConsultationsByType(timeRange, startDate, endDate) {
    document.getElementById('consultationsByTypeNoData').style.display = 'none';
    document.getElementById('totalConsultations').textContent = 'Loading...';
    
    let url = `/Admin/Reports?handler=ConsultationsByType&timeRange=${timeRange}`;
    if (timeRange === 'custom' && startDate && endDate) {
        url += `&startDate=${startDate}&endDate=${endDate}`;
    }
    
    return fetch(url)
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch consultations by type data');
            }
            return response.json();
        })
        .then(data => {
            if (!data || !data.consultations || data.consultations.length === 0) {
                document.getElementById('consultationsByTypeNoData').style.display = 'flex';
                document.getElementById('totalConsultations').textContent = '0';
                return;
            }
            
            // Update consultations by type chart
            const consultations = data.consultations;
            
            const labels = consultations.map(item => item.type);
            const counts = consultations.map(item => item.count);
            
            window.consultationsByTypeChart.data.labels = labels;
            window.consultationsByTypeChart.data.datasets[0].data = counts;
            window.consultationsByTypeChart.update();
            
            // Update total consultations
            document.getElementById('totalConsultations').textContent = data.total || 0;
        })
        .catch(error => {
            console.error('Error fetching consultations by type:', error);
            document.getElementById('consultationsByTypeNoData').style.display = 'flex';
            document.getElementById('totalConsultations').textContent = '0';
        });
}

// Fetch health index data
function fetchHealthIndex(timeRange, startDate, endDate) {
    document.getElementById('healthIndexNoData').style.display = 'none';
    document.getElementById('currentHealthIndex').textContent = 'Loading...';
    
    let url = `/Admin/Reports?handler=HealthIndex&timeRange=${timeRange}`;
    if (timeRange === 'custom' && startDate && endDate) {
        url += `&startDate=${startDate}&endDate=${endDate}`;
    }
    
    return fetch(url)
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch health index data');
            }
            return response.json();
        })
        .then(data => {
            if (!data || !data.healthData || data.healthData.length === 0) {
                document.getElementById('healthIndexNoData').style.display = 'flex';
                document.getElementById('currentHealthIndex').textContent = 'N/A';
                return;
            }
            
            // Update health index chart
            const healthData = data.healthData;
            
            const labels = healthData.map(item => formatDate(item.date));
            const values = healthData.map(item => item.index);
            
            window.healthIndexChart.data.labels = labels;
            window.healthIndexChart.data.datasets[0].data = values;
            window.healthIndexChart.update();
            
            // Update current health index
            const currentIndex = values[values.length - 1];
            document.getElementById('currentHealthIndex').textContent = currentIndex.toFixed(1);
        })
        .catch(error => {
            console.error('Error fetching health index:', error);
            document.getElementById('healthIndexNoData').style.display = 'flex';
            document.getElementById('currentHealthIndex').textContent = 'N/A';
        });
}

// Update staff legend based on data
function updateStaffLegend(staffData) {
    const legendContainer = document.querySelector('.chart-legend');
    if (!legendContainer) return;
    
    // Clear existing legend items
    legendContainer.innerHTML = '';
    
    // Define colors for different roles
    const roleColors = {
        'Doctor': 'doctor',
        'Nurse': 'nurse',
        'Admin': 'admin',
        'Pharmacist': 'pharmacist',
        'Other': 'other'
    };
    
    // Add legend items for each role in the data
    staffData.forEach((item, index) => {
        const role = item.role;
        const colorClass = roleColors[role] || 'other';
        
        const legendItem = document.createElement('div');
        legendItem.className = 'legend-item';
        legendItem.innerHTML = `
            <span class="legend-color ${colorClass}"></span>
            <span>${role} (${item.count})</span>
        `;
        
        legendContainer.appendChild(legendItem);
    });
}

// Format date for chart labels
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { month: 'short', day: '2-digit' });
}

// Format month and year for chart labels
function formatMonthYear(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { month: 'short', year: 'numeric' });
}

// Show message to the user
function showMessage(message, type = 'info') {
    const messageContainer = document.querySelector('.message-container') || createMessageContainer();
    
    const messageElement = document.createElement('div');
    messageElement.className = `message message-${type}`;
    
    // Add icon based on message type
    let icon = 'info-circle';
    if (type === 'success') icon = 'check-circle';
    if (type === 'error') icon = 'exclamation-circle';
    if (type === 'warning') icon = 'exclamation-triangle';
    
    messageElement.innerHTML = `
        <i class="fas fa-${icon}"></i>
        <span>${message}</span>
    `;
    
    messageContainer.appendChild(messageElement);
    
    // Auto remove after 5 seconds
    setTimeout(() => {
        messageElement.classList.add('fade-out');
        setTimeout(() => {
            messageElement.remove();
            
            // Remove the container if it's empty
            if (messageContainer.children.length === 0) {
                messageContainer.remove();
            }
        }, 300);
    }, 5000);
}

function createMessageContainer() {
    const container = document.createElement('div');
    container.className = 'message-container';
    document.body.appendChild(container);
    return container;
} 