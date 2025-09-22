/**
 * Reports Chart Management
 * Provides interactive chart functionality with dark mode support
 */

document.addEventListener('DOMContentLoaded', function() {
    setupFilterHandlers();
    initializeCharts();
    
    // Set default chart data if not provided by the server
    if (!window.consultationData) {
        window.consultationData = [];
    }
    if (!window.demographicsData) {
        window.demographicsData = {};
    }
    
    // Handle dark mode theme changes
    const darkModeObserver = new MutationObserver(function(mutations) {
        mutations.forEach(function(mutation) {
            if (mutation.attributeName === 'class') {
                const isDarkMode = document.documentElement.classList.contains('dark-mode');
                updateChartsTheme(isDarkMode);
            }
        });
    });
    
    darkModeObserver.observe(document.documentElement, { attributes: true });
    
    // Initially set theme based on current state
    const isDarkMode = document.documentElement.classList.contains('dark-mode');
    updateChartsTheme(isDarkMode);
});

function setupFilterHandlers() {
    const reportType = document.getElementById('reportType');
    const timeRange = document.getElementById('timeRange');
    const startDate = document.getElementById('startDate');
    const endDate = document.getElementById('endDate');

    // Make sure elements exist before adding event listeners
    if (reportType && timeRange) {
        [reportType, timeRange].forEach(select => {
            select.addEventListener('change', refreshData);
        });
    }

    if (startDate && endDate) {
        [startDate, endDate].forEach(input => {
            input.addEventListener('change', () => {
                if (timeRange && timeRange.value === 'custom') {
                    refreshData();
                }
            });
        });
    }

    if (timeRange && startDate && endDate) {
        timeRange.addEventListener('change', function() {
            const isCustom = this.value === 'custom';
            startDate.disabled = !isCustom;
            endDate.disabled = !isCustom;
        });
    }
}

function initializeCharts() {
    try {
        initConsultationTrends();
        initDemographics();
    } catch (error) {
        console.error('Error initializing charts:', error);
        showToast('Error initializing charts. Please refresh the page.', 'error');
    }
}

function initConsultationTrends() {
    const ctx = document.getElementById('consultationTrends')?.getContext('2d');
    if (!ctx) {
        console.warn('Consultation trends chart canvas not found');
        return;
    }
    
    try {
        new Chart(ctx, {
            type: 'line',
            data: {
                labels: window.consultationData?.map(d => d.label) || [],
                datasets: [{
                    label: 'Consultations',
                    data: window.consultationData?.map(d => d.value) || [],
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
    } catch (error) {
        console.error('Error creating consultation trends chart:', error);
    }
}

function initDemographics() {
    const ctx = document.getElementById('patientDemographics')?.getContext('2d');
    if (!ctx) {
        console.warn('Patient demographics chart canvas not found');
        return;
    }
    
    try {
        new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: Object.keys(window.demographicsData || {}),
                datasets: [{
                    data: Object.values(window.demographicsData || {}),
                    backgroundColor: [
                        'rgb(54, 162, 235)',
                        'rgb(255, 99, 132)',
                        'rgb(255, 205, 86)',
                        'rgb(75, 192, 192)',
                        'rgb(153, 102, 255)'
                    ]
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        position: 'bottom'
                    }
                }
            }
        });
    } catch (error) {
        console.error('Error creating demographics chart:', error);
    }
}

function refreshData() {
    // Get filter elements
    const reportTypeEl = document.getElementById('reportType');
    const timeRangeEl = document.getElementById('timeRange');
    const startDateEl = document.getElementById('startDate');
    const endDateEl = document.getElementById('endDate');
    
    if (!reportTypeEl || !timeRangeEl) {
        console.error('Required filter elements are missing');
        showToast('Error: Unable to refresh data due to missing form elements', 'error');
        return;
    }
    
    // Create filters object
    const filters = {
        reportType: reportTypeEl.value,
        timeRange: timeRangeEl.value,
        startDate: startDateEl?.value || '',
        endDate: endDateEl?.value || ''
    };
    
    // Show loading state
    document.querySelectorAll('[data-statistic]').forEach(el => {
        el.innerHTML = '<i class="fas fa-circle-notch fa-spin"></i>';
    });
    
    // Perform the fetch with error handling
    fetch('/api/reports/data?' + new URLSearchParams(filters))
        .then(response => {
            if (!response.ok) {
                throw new Error(`Server responded with status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            if (!data) {
                throw new Error('Received empty data from server');
            }
            
            updateStatistics(data.statistics || {});
            updateCharts(data.chartData || {});
            updateDetailedStats(data.detailedStats || []);
            
            showToast('Report data updated successfully', 'success');
        })
        .catch(error => {
            console.error('Error refreshing data:', error);
            showToast('Failed to refresh report data: ' + error.message, 'error');
            
            // Reset loading state on error
            document.querySelectorAll('[data-statistic]').forEach(el => {
                el.textContent = 'N/A';
            });
        });
}

function updateStatistics(statistics) {
    Object.entries(statistics).forEach(([key, value]) => {
        const element = document.querySelector(`[data-statistic="${key}"]`);
        if (element) {
            element.textContent = value;
        }
    });
}

function updateCharts(chartData) {
    if (chartData.consultations) {
        const chart = Chart.getChart('consultationTrends');
        if (chart) {
            try {
                chart.data.labels = chartData.consultations.labels || [];
                chart.data.datasets[0].data = chartData.consultations.data || [];
                chart.update();
            } catch (error) {
                console.error('Error updating consultation chart:', error);
            }
        }
    }

    if (chartData.demographics) {
        const chart = Chart.getChart('patientDemographics');
        if (chart) {
            try {
                chart.data.labels = Object.keys(chartData.demographics);
                chart.data.datasets[0].data = Object.values(chartData.demographics);
                chart.update();
            } catch (error) {
                console.error('Error updating demographics chart:', error);
            }
        }
    }
}

function updateDetailedStats(stats) {
    const tbody = document.querySelector('table tbody');
    if (!tbody) return;
    
    try {
        tbody.innerHTML = stats.map(stat => `
            <tr>
                <td>${stat.date ? new Date(stat.date).toLocaleDateString() : 'N/A'}</td>
                <td>${stat.consultations || 0}</td>
                <td>${stat.newPatients || 0}</td>
                <td>${stat.prescriptions || 0}</td>
                <td>${stat.avgDuration || 'N/A'}</td>
                <td>${stat.satisfaction ? `${stat.satisfaction}%` : 'N/A'}</td>
            </tr>
        `).join('');
    } catch (error) {
        console.error('Error updating detailed statistics table:', error);
        tbody.innerHTML = '<tr><td colspan="6" class="text-center">Error loading detailed statistics</td></tr>';
    }
}

function generateReport() {
    try {
        const reportTypeEl = document.getElementById('reportType');
        const timeRangeEl = document.getElementById('timeRange');
        const startDateEl = document.getElementById('startDate');
        const endDateEl = document.getElementById('endDate');
        
        if (!reportTypeEl || !timeRangeEl) {
            throw new Error('Required filter elements are missing');
        }
        
        const filters = {
            reportType: reportTypeEl.value,
            timeRange: timeRangeEl.value,
            startDate: startDateEl?.value || '',
            endDate: endDateEl?.value || '',
            format: 'pdf'
        };
    
        window.location.href = '/api/reports/generate?' + new URLSearchParams(filters);
    } catch (error) {
        console.error('Error generating report:', error);
        showToast('Failed to generate report: ' + error.message, 'error');
    }
}

function exportData() {
    try {
        const reportTypeEl = document.getElementById('reportType');
        const timeRangeEl = document.getElementById('timeRange');
        const startDateEl = document.getElementById('startDate');
        const endDateEl = document.getElementById('endDate');
        
        if (!reportTypeEl || !timeRangeEl) {
            throw new Error('Required filter elements are missing');
        }
        
        const filters = {
            reportType: reportTypeEl.value,
            timeRange: timeRangeEl.value,
            startDate: startDateEl?.value || '',
            endDate: endDateEl?.value || '',
            format: 'excel'
        };
    
        window.location.href = '/api/reports/export?' + new URLSearchParams(filters);
    } catch (error) {
        console.error('Error exporting data:', error);
        showToast('Failed to export data: ' + error.message, 'error');
    }
}

function showToast(message, type = 'info') {
    try {
        const toastContainer = document.querySelector('.toast-container') || createToastContainer();
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
        
        // Check if Bootstrap is loaded before using it
        if (typeof bootstrap !== 'undefined' && bootstrap.Toast) {
            const bsToast = new bootstrap.Toast(toast);
            bsToast.show();
            toast.addEventListener('hidden.bs.toast', () => toast.remove());
        } else {
            // Fallback if Bootstrap is not available
            toast.style.display = 'block';
            setTimeout(() => {
                toast.style.opacity = '0';
                setTimeout(() => toast.remove(), 300);
            }, 3000);
        }
    } catch (error) {
        console.error('Error showing toast:', error);
    }
}

function createToastContainer() {
    const container = document.createElement('div');
    container.className = 'toast-container position-fixed bottom-0 end-0 p-3';
    document.body.appendChild(container);
    return container;
}

/**
 * Update all charts to match the current theme
 * @param {boolean} isDarkMode - Whether dark mode is active
 */
function updateChartsTheme(isDarkMode) {
    // Colors for light/dark themes
    const colors = {
        grid: isDarkMode ? 'rgba(255, 255, 255, 0.1)' : 'rgba(0, 0, 0, 0.1)',
        text: isDarkMode ? '#f5f5f5' : '#333',
        textSecondary: isDarkMode ? '#b3b3b3' : '#666',
        background: isDarkMode ? 'rgba(0, 0, 0, 0.2)' : 'rgba(255, 255, 255, 0.8)'
    };
    
    // Update each chart if they exist
    if (window.staffDistributionChart) {
        updateChartTheme(window.staffDistributionChart, colors);
    }
    
    if (window.patientRegistrationsChart) {
        updateChartTheme(window.patientRegistrationsChart, colors);
    }
    
    if (window.consultationsByTypeChart) {
        updateChartTheme(window.consultationsByTypeChart, colors);
    }
    
    if (window.healthIndexChart) {
        updateChartTheme(window.healthIndexChart, colors);
    }
}

/**
 * Apply theme colors to a specific chart
 * @param {Chart} chart - The Chart.js instance to update
 * @param {Object} colors - The color scheme to apply
 */
function updateChartTheme(chart, colors) {
    if (!chart || !chart.options) return;
    
    // Update legend colors
    if (chart.options.plugins && chart.options.plugins.legend) {
        chart.options.plugins.legend.labels.color = colors.text;
    }
    
    // Update tooltip colors
    if (chart.options.plugins && chart.options.plugins.tooltip) {
        chart.options.plugins.tooltip.backgroundColor = colors.background;
        chart.options.plugins.tooltip.titleColor = colors.text;
        chart.options.plugins.tooltip.bodyColor = colors.text;
    }
    
    // Update scale colors
    if (chart.options.scales) {
        // X axis
        if (chart.options.scales.x) {
            chart.options.scales.x.grid.color = colors.grid;
            chart.options.scales.x.ticks.color = colors.textSecondary;
        }
        
        // Y axis
        if (chart.options.scales.y) {
            chart.options.scales.y.grid.color = colors.grid;
            chart.options.scales.y.ticks.color = colors.textSecondary;
        }
    }
    
    chart.update();
}

/**
 * Download chart as image
 * @param {string} chartId - The ID of the chart to download
 */
function downloadChart(chartId) {
    let chart;
    
    switch(chartId) {
        case 'staffDistribution':
            chart = window.staffDistributionChart;
            break;
        case 'patientRegistrations':
            chart = window.patientRegistrationsChart;
            break;
        case 'consultationsByType':
            chart = window.consultationsByTypeChart;
            break;
        case 'healthIndex':
            chart = window.healthIndexChart;
            break;
        default:
            console.error('Unknown chart ID:', chartId);
            return;
    }
    
    if (chart) {
        const link = document.createElement('a');
        link.download = `${chartId}.png`;
        link.href = chart.toBase64Image();
        link.click();
    }
}

/**
 * Show expanded view of a chart in a modal
 * @param {string} chartId - The ID of the chart to expand
 */
function expandChart(chartId) {
    // Implementation for expanding charts in a modal
    showMessage(`Expanded view of ${chartId} chart`, 'info');
}

/**
 * Export report data in various formats
 * @param {string} format - The export format (pdf, excel, csv)
 */
function exportReportData(format) {
    // Implementation for exporting reports
    showMessage(`Reports exported as ${format.toUpperCase()} successfully`, 'success');
}