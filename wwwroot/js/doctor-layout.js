document.addEventListener('DOMContentLoaded', function() {
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.getElementById('mainContent');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebarOverlay = document.getElementById('sidebarOverlay');
    const isMobile = window.innerWidth <= 768;
    let isCollapsed = localStorage.getItem('sidebarCollapsed') === 'true';

    function updateSidebarState() {
        if (isCollapsed) {
            sidebar.classList.add('collapsed');
        } else {
            sidebar.classList.remove('collapsed');
        }
        localStorage.setItem('sidebarCollapsed', isCollapsed);
    }

    // Initialize sidebar state
    if (!isMobile) {
        updateSidebarState();
    }

    function toggleSidebar() {
        if (isMobile) {
            sidebar.classList.toggle('show');
            sidebarToggle.classList.toggle('show');
            sidebarOverlay.classList.toggle('show');
        } else {
            isCollapsed = !isCollapsed;
            updateSidebarState();
        }
    }

    function closeSidebar() {
        sidebar.classList.remove('show');
        sidebarToggle.classList.remove('show');
        sidebarOverlay.classList.remove('show');
    }

    sidebarToggle.addEventListener('click', function(e) {
        e.stopPropagation();
        toggleSidebar();
    });

    // Close sidebar when clicking outside on mobile
    if (isMobile) {
        sidebarOverlay.addEventListener('click', closeSidebar);
        document.addEventListener('click', function(event) {
            if (!sidebar.contains(event.target) && 
                !sidebarToggle.contains(event.target) && 
                sidebar.classList.contains('show')) {
                closeSidebar();
            }
        });
    }

    // Handle window resize
    window.addEventListener('resize', function() {
        const newIsMobile = window.innerWidth <= 768;
        if (newIsMobile !== isMobile) {
            location.reload();
        }
    });
});
