// Nurse Menu Functionality
document.addEventListener('DOMContentLoaded', function() {
    // Get the nurse menu dropdown button
    const nurseMenuBtn = document.getElementById('nurseNavDropdown');
    
    if (nurseMenuBtn) {
        // Get the dropdown menu
        const dropdownMenu = nurseMenuBtn.nextElementSibling;
        
        // Toggle dropdown on button click
        nurseMenuBtn.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            
            // Toggle show class
            dropdownMenu.classList.toggle('show');
        });
        
        // Close dropdown when clicking outside
        document.addEventListener('click', function(e) {
            if (!nurseMenuBtn.contains(e.target) && !dropdownMenu.contains(e.target)) {
                dropdownMenu.classList.remove('show');
            }
        });
        
        // Prevent dropdown from closing when clicking inside it
        dropdownMenu.addEventListener('click', function(e) {
            if (e.target.tagName !== 'A') {
                e.stopPropagation();
            }
        });
        
        // Add hover effect
        nurseMenuBtn.addEventListener('mouseenter', function() {
            dropdownMenu.classList.add('show');
        });
        
        // Handle dropdown mouseleave
        const nurseDropdown = document.querySelector('.nurse-menu-dropdown');
        nurseDropdown.addEventListener('mouseleave', function() {
            // Small delay to make the UX smoother
            setTimeout(function() {
                if (!nurseDropdown.matches(':hover')) {
                    dropdownMenu.classList.remove('show');
                }
            }, 300);
        });
    }
}); 