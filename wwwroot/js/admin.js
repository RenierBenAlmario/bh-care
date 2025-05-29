// Admin Dashboard JavaScript

document.addEventListener('DOMContentLoaded', function() {
    // Initialize tooltips
    initTooltips();
    
    // Make tables responsive
    makeTablesResponsive();
    
    // Add confirmation dialogs to delete/deactivate actions
    addConfirmationDialogs();

    // Initialize notification system
    initNotificationSystem();
    
    // Set up logout button
    setupLogoutButton();
    
    // Mark all notifications as read
    const markAllAsReadButton = document.getElementById('markAllAsRead');
    if (markAllAsReadButton) {
        markAllAsReadButton.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Send request to mark all as read
            fetch('/api/Notification/markAllAsRead', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
                }
            })
            .then(response => {
                if (response.ok) {
                    // Remove unread class from all notifications
                    document.querySelectorAll('.notification-item.unread').forEach(item => {
                        item.classList.remove('unread');
                    });
                    
                    // Remove notification badge
                    const badge = document.querySelector('#notificationDropdown .badge');
                    if (badge) {
                        badge.remove();
                    }
                }
            })
            .catch(error => {
                console.error('Error marking notifications as read:', error);
            });
        });
    }
    
    // Mark individual notification as read when clicked
    document.querySelectorAll('.notification-item').forEach(item => {
        item.addEventListener('click', function() {
            const notificationId = this.dataset.id;
            if (!notificationId) return;
            
            // Send request to mark as read
            fetch(`/api/Notification/markAsRead/${notificationId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
                }
            })
            .catch(error => {
                console.error('Error marking notification as read:', error);
            });
            
            // Remove unread class
            this.classList.remove('unread');
            
            // Update badge count
            const badge = document.querySelector('#notificationDropdown .badge');
            if (badge) {
                const count = parseInt(badge.textContent) - 1;
                if (count <= 0) {
                    badge.remove();
                } else {
                    badge.textContent = count;
                }
            }
        });
    });
});

// Initialize notification system for pending users
function initNotificationSystem() {
    // Get the notification dropdown and bell icon
    const notificationDropdown = document.getElementById('notificationDropdown');
    
    if (notificationDropdown) {
        // Make sure the bell icon is wrapped in a container for the badge
        wrapBellIconWithContainer();
        
        // Check for pending users
        updatePendingNotifications();
        
        // Set up interval to periodically check for pending users
        setInterval(updatePendingNotifications, 60000); // Check every minute
    }
    
    // Add pending alert on User Management page
    if (document.getElementById('pendingUsersAlertPlaceholder')) {
        showPendingUsersAlert();
    }
    
    // Expose functions to window for cross-script usage
    window.updatePendingNotifications = updatePendingNotifications;
    window.showPendingUsersAlert = showPendingUsersAlert;
}

// Wrap the bell icon in a container element if it's not already
function wrapBellIconWithContainer() {
    const notificationDropdown = document.getElementById('notificationDropdown');
    if (!notificationDropdown) return;
    
    const bellIcon = notificationDropdown.querySelector('i.fas.fa-bell');
    
    if (bellIcon && !bellIcon.closest('.bell-icon-container')) {
        // Create wrapper element
        const wrapper = document.createElement('span');
        wrapper.className = 'bell-icon-container';
        
        // Insert wrapper before bell icon
        if (bellIcon.parentNode) {
            bellIcon.parentNode.insertBefore(wrapper, bellIcon);
        }
        
        // Move bell icon into wrapper
        wrapper.appendChild(bellIcon);
        
        // Add the badge element
        const badge = document.createElement('span');
        badge.className = 'notification-badge';
        badge.id = 'notificationBadge';
        wrapper.appendChild(badge);
    }
}

// Update notifications for pending users
function updatePendingNotifications() {
    try {
        // Get pending users from localStorage
        const users = getPendingUsers();
        const pendingCount = users.length;
        
        // Update notification badge
        updateNotificationBadge(pendingCount);
        
        // Update notification dropdown content
        updateNotificationDropdownContent(users);
        
        return pendingCount;
    } catch (error) {
        console.error('Error updating pending notifications:', error);
        return 0;
    }
}

// Get pending users from localStorage
function getPendingUsers() {
    try {
        const usersData = localStorage.getItem('bhcUsers');
        if (!usersData) return [];
        
        const users = JSON.parse(usersData);
        return users.filter(user => user.status === 'pending');
    } catch (error) {
        console.error('Error getting pending users:', error);
        return [];
    }
}

// Update the notification badge with the count of pending users
function updateNotificationBadge(count) {
    const badge = document.getElementById('notificationBadge');
    if (!badge) return;
    
    if (count > 0) {
        badge.textContent = count > 9 ? '9+' : count;
        badge.classList.add('show');
        
        // Add animation to bell icon if not already animated
        if (badge.parentElement) {
            const bellIcon = badge.parentElement.querySelector('i.fas.fa-bell');
            if (bellIcon && !bellIcon.classList.contains('bell-animation')) {
                bellIcon.classList.add('bell-animation');
                
                // Remove animation class after animation completes
                setTimeout(() => {
                    bellIcon.classList.remove('bell-animation');
                }, 800);
            }
        }
    } else {
        badge.textContent = '';
        badge.classList.remove('show');
    }
}

// Update notification dropdown content with pending users
function updateNotificationDropdownContent(pendingUsers) {
    const dropdownMenu = document.querySelector('.notification-dropdown .notification-list');
    if (!dropdownMenu) return;
    
    if (pendingUsers.length === 0) {
        dropdownMenu.innerHTML = `
            <div class="dropdown-item text-center">
                <span class="text-muted">No new notifications</span>
            </div>
        `;
        return;
    }
    
    let notificationsHTML = '';
    
    // Add pending users notifications
    pendingUsers.forEach(user => {
        const fullName = [user.firstName, user.middleName, user.lastName, user.suffix].filter(Boolean).join(' ');
        const registrationDate = new Date(user.registrationDate);
        const formattedDate = registrationDate.toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
        });
        
        notificationsHTML += `
            <a class="dropdown-item notification-item unread" href="/Admin/UserManagement" data-username="${user.username}">
                <div class="notification-icon">
                    <i class="fas fa-user-clock text-warning"></i>
                </div>
                <div class="notification-content">
                    <div class="notification-title">New User Registration</div>
                    <div class="notification-message">${fullName} is waiting for approval</div>
                    <div class="notification-time">${formattedDate}</div>
                </div>
            </a>
        `;
    });
    
    dropdownMenu.innerHTML = notificationsHTML;
    
    // Add click event to items to redirect to User Management page
    dropdownMenu.querySelectorAll('.notification-item').forEach(item => {
        item.addEventListener('click', function(e) {
            e.preventDefault();
            window.location.href = '/Admin/UserManagement';
        });
    });
}

// Show pending users alert on the User Management page
function showPendingUsersAlert() {
    const alertPlaceholder = document.getElementById('pendingUsersAlertPlaceholder');
    if (!alertPlaceholder || !window.location.pathname.includes('/Admin/UserManagement')) { 
        return;
    }
    
    // Check if we're on the User Management page
    if (window.location.pathname.includes('/Admin/UserManagement')) {
        const pendingUsers = getPendingUsers();
        
        if (pendingUsers.length > 0) {
            // Create alert if it doesn't exist
            let alertElement = document.getElementById('pendingUsersAlert');
            
            if (!alertElement) {
                alertElement = document.createElement('div');
                alertElement.id = 'pendingUsersAlert';
                alertElement.className = 'pending-alert';
                alertElement.innerHTML = `
                    <div>
                        <i class="fas fa-exclamation-circle"></i>
                        You have <strong>${pendingUsers.length}</strong> pending user ${pendingUsers.length === 1 ? 'approval' : 'approvals'}
                    </div>
                    <button type="button" class="close-alert" aria-label="Close">
                        <i class="fas fa-times"></i>
                    </button>
                `;
                
                // Find the container to insert the alert
                const container = document.querySelector('.user-management-container');
                if (container) {
                    container.insertBefore(alertElement, container.firstChild);
                    
                    // Add close button functionality
                    const closeButton = alertElement.querySelector('.close-alert');
                    if (closeButton) {
                        closeButton.addEventListener('click', function() {
                            alertElement.classList.remove('show');
                            setTimeout(() => {
                                alertElement.remove();
                            }, 300);
                        });
                    }
                    
                    // Show the alert with animation
                    setTimeout(() => {
                        alertElement.classList.add('show');
                    }, 100);
                }
            }
        }
    }
}

// Set up logout button
function setupLogoutButton() {
    const logoutButton = document.getElementById('logoutButton');
    
    if (logoutButton) {
        // Update the button to a proper button with the right classes
        logoutButton.className = 'logout-btn';
        
        // Update the logout button event
        logoutButton.addEventListener('click', function(e) {
            e.preventDefault();
            
            if (confirm('Are you sure you want to log out?')) {
                // Show loading indicator
                this.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Logging out...';
                this.disabled = true;
                
                // Redirect to login page after a short delay
                setTimeout(() => {
                    window.location.href = '/Account/Login';
                }, 500);
            }
        });
    }
}

// Initialize Bootstrap tooltips
function initTooltips() {
    // Check if Bootstrap 5 tooltip is available
    if (typeof bootstrap !== 'undefined' && typeof bootstrap.Tooltip !== 'undefined') {
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    } 
    // Fallback for jQuery tooltips
    else if (typeof $ !== 'undefined' && typeof $.fn.tooltip !== 'undefined') {
        $('[data-toggle="tooltip"]').tooltip();
    }
}

// Make tables responsive
function makeTablesResponsive() {
    const tables = document.querySelectorAll('table');
    tables.forEach(table => {
        if (!table.parentElement.classList.contains('table-responsive')) {
            const wrapper = document.createElement('div');
            wrapper.classList.add('table-responsive');
            table.parentNode.insertBefore(wrapper, table);
            wrapper.appendChild(table);
        }
    });
}

// Add confirmation dialogs
function addConfirmationDialogs() {
    const dangerButtons = document.querySelectorAll('.btn-danger');
    dangerButtons.forEach(button => {
        if (!button.hasAttribute('data-confirmed')) {
            button.setAttribute('data-confirmed', 'false');
            
            // Only add event listener if it doesn't already have onclick
            if (!button.hasAttribute('onclick')) {
                button.addEventListener('click', function(e) {
                    const isConfirmed = button.getAttribute('data-confirmed');
                    if (isConfirmed === 'false') {
                        e.preventDefault();
                        const action = button.textContent.trim();
                        if (confirm(`Are you sure you want to ${action.toLowerCase()} this item?`)) {
                            button.setAttribute('data-confirmed', 'true');
                            button.click();
                        }
                    }
                });
            }
        }
    });
}

// Add hover effects to table rows
document.addEventListener('mouseover', function(e) {
    if (e.target.tagName === 'TR' || e.target.parentElement.tagName === 'TR') {
        const row = e.target.tagName === 'TR' ? e.target : e.target.parentElement;
        if (row.parentElement.tagName === 'TBODY') {
            row.classList.add('row-hover');
        }
    }
});

document.addEventListener('mouseout', function(e) {
    if (e.target.tagName === 'TR' || e.target.parentElement.tagName === 'TR') {
        const row = e.target.tagName === 'TR' ? e.target : e.target.parentElement;
        if (row.parentElement.tagName === 'TBODY') {
            row.classList.remove('row-hover');
        }
    }
}); 