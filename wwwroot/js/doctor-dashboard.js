// Doctor Dashboard JavaScript

document.addEventListener('DOMContentLoaded', function() {
    // Sidebar toggle functionality
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.getElementById('mainContent');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const isMobile = window.innerWidth <= 768;

    function toggleSidebar() {
        sidebar.classList.toggle('show');
        mainContent.classList.toggle('expanded');
        sidebarToggle.classList.toggle('show');
    }

    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', toggleSidebar);
    }

    // Handle window resize
    window.addEventListener('resize', function() {
        if (window.innerWidth > 768) {
            sidebar.classList.remove('show');
            mainContent.classList.remove('expanded');
            sidebarToggle.classList.remove('show');
        }
    });

    // Close sidebar when clicking outside on mobile
    if (isMobile) {
        document.addEventListener('click', function(event) {
            if (!sidebar.contains(event.target) && 
                !sidebarToggle.contains(event.target) && 
                sidebar.classList.contains('show')) {
                toggleSidebar();
            }
        });
    }

    // Auto-refresh dashboard data
    const refreshInterval = 30000; // 30 seconds
    let refreshTimer;

    function startRefreshTimer() {
        const indicator = document.createElement('div');
        indicator.className = 'refresh-indicator';
        indicator.innerHTML = '<i class="fas fa-sync-alt fa-spin"></i> Auto-refreshing...';
        document.body.appendChild(indicator);

        refreshTimer = setInterval(function() {
            location.reload();
        }, refreshInterval);
    }

    startRefreshTimer();

    // Handle availability toggle
    const availabilityToggle = document.getElementById('availabilityToggle');
    if (availabilityToggle) {
        availabilityToggle.addEventListener('change', function() {
            this.closest('form').submit();
        });
    }

    // Working days selection
    const workingDays = document.querySelectorAll('.working-days-group input[type="checkbox"]');
    workingDays.forEach(day => {
        day.addEventListener('change', function() {
            const form = this.closest('form');
            if (form) {
                form.submit();
            }
        });
    });

    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Handle notification clicks
    const notifications = document.querySelectorAll('.notification-item');
    notifications.forEach(notification => {
        notification.addEventListener('click', function() {
            this.classList.add('read');
            // Add your notification handling logic here
        });
    });
}); 