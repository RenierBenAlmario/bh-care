// Admin Notifications JavaScript
$(document).ready(function() {
    // Initialize notification functionality
    initializeNotifications();

    // Notification bell click handler
    $('#navbarNotificationBadge, #notificationBell').on('click', function(e) {
        e.preventDefault();
        const dropdown = $('.notification-dropdown');
        
        if (dropdown.is(':visible')) {
            dropdown.hide();
        } else {
            loadNotifications();
            dropdown.show();
        }
    });

    // Close dropdown when clicking outside
    $(document).on('click', function(e) {
        if (!$(e.target).closest('#notificationBell, .notification-dropdown').length) {
            $('.notification-dropdown').hide();
        }
    });

    // Update notifications every 2 minutes
    setInterval(updateNotificationBadge, 2 * 60 * 1000);
});

// Function to initialize notifications
function initializeNotifications() {
    updateNotificationBadge();
}

// Function to update the notification badge count
function updateNotificationBadge() {
    // Try to get pending count from the page
    const pendingCountElement = $('.pending-count');
    if (pendingCountElement.length) {
        const count = pendingCountElement.text();
        if (count && !isNaN(count)) {
            $('#navbarNotificationBadge').text(count);
            
            // Show/hide badge based on count
            if (parseInt(count) > 0) {
                $('#navbarNotificationBadge').show();
                $('#pendingUsersAlert').show();
            } else {
                $('#navbarNotificationBadge').hide();
                $('#pendingUsersAlert').hide();
            }
        }
    } else {
        // If no element on page, fetch from API
        fetch('/Admin/UserManagement?handler=PendingCount')
            .then(response => response.json())
            .then(data => {
                if (data.count !== undefined) {
                    $('#navbarNotificationBadge').text(data.count);
                    
                    // Show/hide badge based on count
                    if (parseInt(data.count) > 0) {
                        $('#navbarNotificationBadge').show();
                    } else {
                        $('#navbarNotificationBadge').hide();
                    }
                }
            })
            .catch(error => {
                console.error('Error updating notification badge:', error);
            });
    }
}

// Function to load notifications into the dropdown
function loadNotifications() {
    const notificationList = $('#notificationList');
    
    // Show loading indicator
    notificationList.html('<li class="notification-item loading"><div class="spinner-border spinner-border-sm text-primary" role="status"></div> Loading notifications...</li>');
    
    // Load pending users as notifications
    fetch('/Admin/UserManagement?handler=Users&filter=pending&page=1')
        .then(response => response.json())
        .then(data => {
            if (!data.users || data.users.length === 0) {
                notificationList.html('<li class="notification-item empty">No pending notifications</li>');
                return;
            }
            
            // Clear list
            notificationList.empty();
            
            // Add notifications for pending users
            data.users.forEach(user => {
                const item = $('<li class="notification-item"></li>');
                item.html(`
                    <div class="notification-content">
                        <div class="notification-title">New User Registration</div>
                        <div class="notification-message">
                            <strong>${user.fullName}</strong> has registered and is awaiting approval.
                        </div>
                        <div class="notification-time">${formatRelativeTime(user.registeredOn)}</div>
                    </div>
                    <div class="notification-actions">
                        <a href="/Admin/UserManagement" class="btn btn-sm btn-primary">
                            <i class="fas fa-user-check"></i> Review
                        </a>
                    </div>
                `);
                notificationList.append(item);
            });
        })
        .catch(error => {
            console.error('Error loading notifications:', error);
            notificationList.html('<li class="notification-item error"><i class="fas fa-exclamation-circle text-danger"></i> Failed to load notifications.</li>');
        });
}

// Helper function to format time relative to now
function formatRelativeTime(dateString) {
    try {
        const date = new Date(dateString);
        const now = new Date();
        const diffMs = now - date;
        const diffSec = Math.round(diffMs / 1000);
        const diffMin = Math.round(diffSec / 60);
        const diffHr = Math.round(diffMin / 60);
        const diffDays = Math.round(diffHr / 24);
        
        if (diffSec < 60) {
            return 'just now';
        } else if (diffMin < 60) {
            return `${diffMin} minute${diffMin > 1 ? 's' : ''} ago`;
        } else if (diffHr < 24) {
            return `${diffHr} hour${diffHr > 1 ? 's' : ''} ago`;
        } else if (diffDays < 7) {
            return `${diffDays} day${diffDays > 1 ? 's' : ''} ago`;
        } else {
            return date.toLocaleDateString('en-US', { 
                month: 'short', 
                day: 'numeric', 
                year: 'numeric' 
            });
        }
    } catch (error) {
        console.error('Error formatting date:', error);
        return dateString || 'N/A';
    }
} 