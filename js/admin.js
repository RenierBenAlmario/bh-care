document.addEventListener('DOMContentLoaded', function() {
    const adminLoginForm = document.getElementById('adminLoginForm');
    
    adminLoginForm.addEventListener('submit', function(e) {
        e.preventDefault();
        
        const username = document.getElementById('adminUsername').value;
        const password = document.getElementById('adminPassword').value;
        
        // Verify admin credentials
        if (DB.verifyAdmin(username, password)) {
            // Redirect to admin dashboard
            window.location.href = 'dashboard.html';
        } else {
            alert('Invalid username or password!');
        }
    });
}); 