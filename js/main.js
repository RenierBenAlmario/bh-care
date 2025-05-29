document.addEventListener('DOMContentLoaded', function() {
    // Check if database is initialized
    if (!localStorage.getItem('users')) {
        // Initialize database if not already done
        DB.init();
    }
}); 