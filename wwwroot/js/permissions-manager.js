/**
 * Permissions Manager JavaScript
 * Handles UI interactions for the permissions management interface
 */
document.addEventListener('DOMContentLoaded', function() {
    // Cache DOM elements
    const permissionsForm = document.getElementById('permissionsForm');
    const saveBtn = document.querySelector('.save-btn');
    const selectAllBtn = document.getElementById('selectAllPermissions');
    const deselectAllBtn = document.getElementById('deselectAllPermissions');
    const staffSearch = document.getElementById('staffSearch');
    const staffItems = document.querySelectorAll('.staff-item');
    const permissionCheckboxes = document.querySelectorAll('.permission-checkbox');
    
    // Initialize permission counts
    updatePermissionCounts();
    
    // Form submission with validation
    if (permissionsForm) {
        permissionsForm.addEventListener('submit', function(e) {
            // Show loading state
            if (saveBtn) {
                saveBtn.disabled = true;
                saveBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-1"></i> Saving...';
            }
            
            // Ensure unchecked permissions are properly handled
            const allCheckboxes = document.querySelectorAll('.permission-checkbox');
            const selectedPermissions = [];
            
            allCheckboxes.forEach(checkbox => {
                if (checkbox.checked) {
                    selectedPermissions.push(checkbox.value);
                }
            });
            
            // Log the form data being submitted
            console.log('Submitting permissions form with selected permissions:', selectedPermissions);
            
            // Continue with form submission
            return true;
        });
    }
    
    // Select all permissions
    if (selectAllBtn) {
        selectAllBtn.addEventListener('click', function(e) {
            e.preventDefault();
            permissionCheckboxes.forEach(checkbox => {
                checkbox.checked = true;
            });
            updatePermissionCounts();
        });
    }
    
    // Deselect all permissions
    if (deselectAllBtn) {
        deselectAllBtn.addEventListener('click', function(e) {
            e.preventDefault();
            permissionCheckboxes.forEach(checkbox => {
                checkbox.checked = false;
            });
            updatePermissionCounts();
        });
    }
    
    // Staff search functionality
    if (staffSearch) {
        staffSearch.addEventListener('input', function() {
            const searchTerm = this.value.toLowerCase();
            
            staffItems.forEach(item => {
                const name = item.querySelector('h6').textContent.toLowerCase();
                const role = item.querySelector('.badge').textContent.toLowerCase();
                
                if (name.includes(searchTerm) || role.includes(searchTerm)) {
                    item.style.display = '';
                } else {
                    item.style.display = 'none';
                }
            });
        });
    }
    
    // Update permission counts when checkboxes change
    permissionCheckboxes.forEach(checkbox => {
        checkbox.addEventListener('change', updatePermissionCounts);
    });
    
    // Function to update the permission counts in each category
    function updatePermissionCounts() {
        const categories = document.querySelectorAll('.accordion-item');
        
        categories.forEach(category => {
            const checkboxes = category.querySelectorAll('.permission-checkbox');
            const checkedCount = Array.from(checkboxes).filter(cb => cb.checked).length;
            const countBadge = category.querySelector('.category-count');
            
            if (countBadge) {
                countBadge.textContent = `${checkedCount}/${checkboxes.length}`;
                
                // Update badge color based on selection
                if (checkedCount === 0) {
                    countBadge.className = 'badge bg-secondary ms-2 category-count';
                } else if (checkedCount === checkboxes.length) {
                    countBadge.className = 'badge bg-success ms-2 category-count';
                } else {
                    countBadge.className = 'badge bg-primary ms-2 category-count';
                }
            }
        });
    }
    
    // Add highlighting to permission items on hover
    const permissionItems = document.querySelectorAll('.permission-check');
    permissionItems.forEach(item => {
        item.addEventListener('mouseenter', function() {
            this.classList.add('bg-light');
        });
        
        item.addEventListener('mouseleave', function() {
            this.classList.remove('bg-light');
        });
    });
}); 