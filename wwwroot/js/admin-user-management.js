// Admin User Management JavaScript

// Simulated database - we'll load from localStorage
let usersDatabase = [];
let filteredUsers = [];
let currentPage = 1;
const usersPerPage = 10;

// Check if document.addEventListener exists to avoid errors
if (typeof document !== 'undefined' && document.addEventListener) {
    document.addEventListener('DOMContentLoaded', function() {
        console.log('Admin User Management script initialized');
        
        // Initialize the user management functionality
        initUserManagement();
    });
}

function initUserManagement() {
    try {
        console.log('Initializing user management');
        
        // Load users initially
        loadUsers('all', 1);
        
        // Setup event handlers for filter, search, etc.
        setupEventHandlers();
        
    } catch (error) {
        console.error('Error initializing user management:', error);
    }
}

function loadUsers(filter, page, search = '') {
    console.log(`Loading users with filter: ${filter}, page: ${page}, search: '${search}'`);
    
    // Show loading indicator
    const tableBody = document.getElementById('userTableBody');
    if (tableBody) {
        tableBody.innerHTML = '<tr><td colspan="8" class="text-center"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></td></tr>';
    }
    
    // Get users from API with additional debugging information
    fetch(`/Admin/UserManagement?handler=Users&filter=${filter}&page=${page}&search=${encodeURIComponent(search)}&_=${new Date().getTime()}`)
        .then(response => {
            console.log(`Response status: ${response.status}`);
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            console.log(`Received ${data.users ? data.users.length : 0} users from server`);
            console.log('User data sample:', data.users && data.users.length > 0 ? data.users[0] : 'No users found');
            
            // Update the user table
            updateUserTable(data.users);
            
            // Update pagination
            updatePagination(data.totalPages, page, filter, search);
            
            // Update pending count if available
            if (data.pendingCount !== undefined) {
                updatePendingCount(data.pendingCount);
            }
        })
        .catch(error => {
            console.error('Error loading users:', error);
            
            // Show error message in table
            if (tableBody) {
                tableBody.innerHTML = `<tr><td colspan="8" class="text-center text-danger">
                    <i class="fas fa-exclamation-circle"></i> 
                    Error loading users: ${error.message}. 
                    <button class="btn btn-sm btn-outline-primary ms-3" onclick="location.reload()">Reload Page</button>
                </td></tr>`;
            }
        });
}

function updateUserTable(users) {
    const tableBody = document.getElementById('userTableBody');
    if (!tableBody) {
        console.error('User table body element not found');
        return;
    }
    
    if (!users || users.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="8" class="text-center">No users found</td></tr>';
        console.log('No users found or empty users array');
        return;
    }
    
    // Clear existing table
    tableBody.innerHTML = '';
    
    // Add each user to the table
    users.forEach(user => {
        const row = document.createElement('tr');
        
        // Log user data for debugging
        console.log(`Processing user: ${user.username}, hasDocument: ${user.hasDocument}, documentId: ${user.documentId || 'none'}`);
        
        // Create the document column content
        let documentContent = '';
        if (user.hasDocument) {
            documentContent = `
                <div class="document-actions">
                    <span class="document-name">${escapeHtml(user.documentName)}</span>
                    <div class="document-buttons">
                        <a href="${user.documentPath || user.filePath || '#'}" target="_blank" class="btn btn-sm btn-outline-primary" title="Download">
                            <i class="fas fa-download"></i>
                        </a>
                        <button type="button" class="btn btn-sm btn-outline-info preview-document" 
                                data-bs-toggle="modal" data-bs-target="#viewFileModal"
                                data-id="${user.documentId}" data-path="${user.documentPath || user.filePath || '#'}" 
                                data-name="${escapeHtml(user.documentName)}" data-type="${user.documentType || 'application/octet-stream'}"
                                data-user="${escapeHtml(user.fullName)}" title="Preview">
                            <i class="fas fa-eye"></i>
                        </button>
                    </div>
                </div>`;
        } else if (user.profilePicture && user.profilePicture.includes('/uploads/residency_proofs/')) {
            // Legacy document in ProfilePicture
            documentContent = `
                <div class="document-actions">
                    <span class="document-name">Legacy Document</span>
                    <div class="document-buttons">
                        <a href="${user.profilePicture}" target="_blank" class="btn btn-sm btn-outline-primary" title="Download">
                            <i class="fas fa-download"></i>
                        </a>
                        <button type="button" class="btn btn-sm btn-warning migrate-document"
                                data-id="${user.id}" data-path="${user.profilePicture}" title="Migrate">
                            <i class="fas fa-exchange-alt"></i> Migrate
                        </button>
                    </div>
                </div>`;
        } else {
            documentContent = '<span class="text-danger">No document</span>';
        }
        
        // Create action buttons based on status
        let actionButtons = '';
        if (user.status === 'Pending') {
            actionButtons = `
                <div class="d-flex gap-2">
                    <button class="btn btn-success approve-user" data-id="${user.id}">
                        <i class="fas fa-check me-1"></i> Approve
                    </button>
                    <button class="btn btn-danger reject-user" data-id="${user.id}">
                        <i class="fas fa-times me-1"></i> Reject
                    </button>
                </div>`;
        } else {
            actionButtons = `
                <button class="btn btn-outline-secondary btn-sm" disabled>
                    Status: ${user.status}
                </button>`;
        }
        
        // Build the row content
        row.innerHTML = `
            <td data-label="Username">${escapeHtml(user.username)}</td>
            <td data-label="Full Name">${escapeHtml(user.fullName)}</td>
            <td data-label="Email">${escapeHtml(user.email)}</td>
            <td data-label="Contact Number">${escapeHtml(user.contactNumber)}</td>
            <td data-label="Residency Proof">${documentContent}</td>
            <td data-label="Status">${user.status}</td>
            <td data-label="Registered On">${formatDate(user.registeredOn)}</td>
            <td data-label="Actions">${actionButtons}</td>
        `;
        
        tableBody.appendChild(row);
    });
    
    // Attach event listeners to new buttons
    attachEventListeners();
}

// Helper function to escape HTML to prevent XSS
function escapeHtml(unsafe) {
    if (unsafe === null || unsafe === undefined) return '';
    return String(unsafe)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

// Helper function to format dates
function formatDate(dateString) {
    try {
        const date = new Date(dateString);
        return date.toLocaleDateString('en-US', { 
            month: 'short', 
            day: 'numeric', 
            year: 'numeric' 
        });
    } catch (error) {
        console.error('Error formatting date:', error);
        return dateString || 'N/A';
    }
}

function setupEventHandlers() {
    // Pagination click
    $(document).on('click', '.page-link', function(e) {
        e.preventDefault();
        const page = $(this).data('page');
        const filter = $('.filter-item.active').data('filter');
        const searchTerm = $('#userSearchInput').val();
        loadUsers(filter, page, searchTerm);
    });
    
    // Filter click
    $(document).on('click', '.filter-item', function(e) {
        e.preventDefault();
        const filter = $(this).data('filter');
        
        // Update filter button text
        $('#filterDropdown').html(`<i class="fas fa-filter me-1"></i> ${$(this).text()}`);
        
        // Update active state
        $('.filter-item').removeClass('active');
        $(this).addClass('active');
        
        // Reset to page 1 and reload
        const searchTerm = $('#userSearchInput').val();
        loadUsers(filter, 1, searchTerm);
    });
    
    // Search button click
    $('#searchButton').on('click', function() {
        const searchTerm = $('#userSearchInput').val();
        const filter = $('.filter-item.active').data('filter');
        loadUsers(filter, 1, searchTerm);
    });
    
    // Search on enter key
    $('#userSearchInput').on('keypress', function(e) {
        if (e.which === 13) {
            const searchTerm = $(this).val();
            const filter = $('.filter-item.active').data('filter');
            loadUsers(filter, 1, searchTerm);
        }
    });
    
    // Handler for migrate-document buttons
    $(document).on('click', '.migrate-document', function() {
        const userId = $(this).data('id');
        const filePath = $(this).data('path');
        
        console.log(`Migrate document clicked for user: ${userId}, path: ${filePath}`);
        
        if (confirm('Do you want to migrate this legacy document to the new system?')) {
            // Disable button to prevent multiple clicks
            $(this).prop('disabled', true);
            $(this).html('<i class="fas fa-spinner fa-spin"></i> Migrating...');
            
            // Get anti-forgery token
            const token = $('input[name="__RequestVerificationToken"]').val();
            
            $.ajax({
                url: '?handler=MigrateDocument',
                type: 'POST',
                data: { id: userId },
                headers: {
                    'RequestVerificationToken': token
                },
                success: function(response) {
                    console.log('Migration response:', response);
                    
                    if (response.success) {
                        // Show success message
                        alert('Document migrated successfully');
                        
                        // Reload the current page
                        const filter = $('.filter-item.active').data('filter');
                        const page = parseInt($('.page-item.active .page-link').data('page') || 1);
                        loadUsers(filter, page);
                    } else {
                        alert(`Migration failed: ${response.message || 'Unknown error'}`);
                        // Re-enable the button
                        $(this).prop('disabled', false);
                        $(this).html('<i class="fas fa-exchange-alt"></i> Migrate');
                    }
                },
                error: function(xhr, status, error) {
                    console.error('Error migrating document:', error);
                    alert('Error migrating document. Please try again.');
                    
                    // Re-enable the button
                    $(this).prop('disabled', false);
                    $(this).html('<i class="fas fa-exchange-alt"></i> Migrate');
                }
            });
        }
    });
    
    // Document view button click
    $(document).on('click', '.btn-view-document', function() {
        const userId = $(this).data('user-id');
        const documentId = $(this).data('document-id');
        const documentName = $(this).data('document-name');
        const documentType = $(this).data('document-type');
        
        console.log('View document clicked for:', userId, documentId, documentName);
        openDocumentPreview(userId, documentId, documentName, documentType);
    });
    
    // View file button click
    $('#viewFileBtn').on('click', function() {
        const fileName = $('#fileName').text();
        const ownerName = $('#fileOwner').text();
        viewFile(fileName, ownerName);
    });
    
    // Notification bell click
    $('#notificationBell').on('click', function(e) {
        e.stopPropagation();
        $('.notification-dropdown').toggle();
    });
    
    // Close notification dropdown when clicking elsewhere
    $(document).on('click', function(e) {
        if (!$(e.target).closest('#notificationBell').length) {
            $('.notification-dropdown').hide();
        }
    });
    
    // Mark all notifications as read
    $(document).on('click', '#markAllAsReadBtn', function() {
        $.ajax({
            url: '/api/Notification/MarkAllAsRead',
            type: 'POST',
            headers: {
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
            success: function() {
                // Update UI
                $('.notification-item').removeClass('unread');
                $('#navbarNotificationBadge').text('0').hide();
            },
            error: function(error) {
                console.error("Error marking notifications as read:", error);
            }
        });
    });
    
    // Direct approve button click from user table
    $(document).on('click', '.btn-approve', function() {
        const userId = $(this).data('user-id');
        console.log("Approve clicked for user ID:", userId);
        
        if (!userId) {
            console.error("User ID is missing from approve button");
            showAlert('error', "Could not approve user: Missing user ID");
            return;
        }
        
        updateUserStatus(userId, 'Verified');
    });
    
    // Direct reject button click from user table
    $(document).on('click', '.btn-reject', function() {
        const userId = $(this).data('user-id');
        console.log("Reject clicked for user ID:", userId);
        
        if (!userId) {
            console.error("User ID is missing from reject button");
            showAlert('error', "Could not reject user: Missing user ID");
            return;
        }
        
        updateUserStatus(userId, 'Rejected');
    });
}

function openDocumentPreview(userId, documentId, documentName, documentType) {
    // Set modal title and document info
    $('#viewFileModalLabel').text('View Residency Proof');
    $('#fileName').text(documentName);
    $('#fileOwner').text(`User ${userId}`);
    $('#viewFileBtn').data('document-id', documentId);
    
    // Determine file type and set appropriate badge and preview
    let fileTypeText = 'Document';
    let previewHtml = '';
    
    if (documentType && documentType.toLowerCase().includes('pdf')) {
        fileTypeText = 'PDF Document';
        
        // For testing purposes, use the placeholder PDF
        previewHtml = `
            <div class="pdf-container">
                <iframe src="/images/sample-document-preview.pdf" width="100%" height="400" class="pdf-preview"></iframe>
                <div class="placeholder-overlay">
                    <div class="placeholder-content">
                        <i class="fas fa-file-pdf fa-3x mb-3 text-danger"></i>
                        <h4>Live Preview of ${documentName}</h4>
                        <p class="mb-0">Click the View button below to see the full document</p>
                    </div>
                </div>
            </div>
        `;
    } else if (documentType && documentType.toLowerCase().match(/image\/jpeg|image\/png|image\/gif/)) {
        fileTypeText = 'Image';
        
        // For testing purposes, use the placeholder image
        previewHtml = `
            <div class="image-container">
                <img src="/images/preview-placeholder.jpg" alt="Document preview" class="image-preview">
                <div class="placeholder-overlay">
                    <div class="placeholder-content">
                        <i class="fas fa-image fa-3x mb-3 text-primary"></i>
                        <h4>Live Preview of ${documentName}</h4>
                        <p class="mb-0">Click the View button below to see the full image</p>
                    </div>
                </div>
            </div>
        `;
    } else {
        // Handle unknown document types or missing type info
        const fileExt = documentName.split('.').pop().toLowerCase();
        if (['pdf'].includes(fileExt)) {
            fileTypeText = 'PDF Document';
            previewHtml = `
                <div class="pdf-container">
                    <iframe src="/images/sample-document-preview.pdf" width="100%" height="400" class="pdf-preview"></iframe>
                    <div class="placeholder-overlay">
                        <div class="placeholder-content">
                            <i class="fas fa-file-pdf fa-3x mb-3 text-danger"></i>
                            <h4>Live Preview of ${documentName}</h4>
                            <p class="mb-0">Click the View button below to see the full document</p>
                        </div>
                    </div>
                </div>
            `;
        } else if (['jpg', 'jpeg', 'png', 'gif'].includes(fileExt)) {
            fileTypeText = 'Image';
            previewHtml = `
                <div class="image-container">
                    <img src="/images/preview-placeholder.jpg" alt="Document preview" class="image-preview">
                    <div class="placeholder-overlay">
                        <div class="placeholder-content">
                            <i class="fas fa-image fa-3x mb-3 text-primary"></i>
                            <h4>Live Preview of ${documentName}</h4>
                            <p class="mb-0">Click the View button below to see the full image</p>
                        </div>
                    </div>
                </div>
            `;
        } else {
            fileTypeText = 'Unknown Document';
            previewHtml = `
                <div class="placeholder-content">
                    <i class="fas fa-file fa-3x mb-3 text-secondary"></i>
                    <h4>Cannot Preview ${documentName}</h4>
                    <p class="mb-0">This file type cannot be previewed. Click the View button below to download it.</p>
                </div>
            `;
        }
    }
    
    $('#fileType').text(fileTypeText);
    $('#documentPreviewArea').html(previewHtml);
    
    // Show the modal
    $('#viewFileModal').modal('show');
}

function updateUserStatus(userId, status) {
    // Validate user ID
    if (!userId) {
        console.error("Invalid user ID:", userId);
        showAlert('error', "Could not update user status: Missing user ID.");
        return;
    }
    
    // Ensure userId is a string
    const userIdString = userId.toString();
    
    // Get anti-forgery token
    const token = $('input[name="__RequestVerificationToken"]').val();
    
    // Check if token exists
    if (!token) {
        console.error("Anti-forgery token not found - this is required for the request to work");
        showAlert('error', "Security token missing. Please refresh the page and try again.");
        return;
    }
    
    console.log("Updating user status:", userIdString, status);
    console.log("Token found:", token ? "Yes" : "No");
    
    // Debug the data being sent
    const postData = { userId: userIdString, status: status };
    console.log("Sending data:", postData);
    
    // Show loading indicator
    showAlert('info', `Processing ${status.toLowerCase()} request...`);
    
    $.ajax({
        url: '/Admin/UserManagement?handler=UpdateUserStatus',
        type: 'POST',
        data: JSON.stringify(postData),
        contentType: 'application/json',
        headers: {
            "RequestVerificationToken": token
        },
        success: function(response) {
            console.log("Update response:", response);
            if (response.success) {
                // Show email notification
                showEmailNotification(userIdString, status);
                
                // Show success message in modal
                const action = status === 'Verified' ? 'approved' : 'rejected';
                $('#successMessage').text(`User has been successfully ${action}. The user will be notified via email.`);
                $('#successModal').modal('show');
                
                const filter = $('.filter-item.active').data('filter');
                const page = parseInt($('.page-item.active .page-link').data('page') || 1);
                
                // Reload users to update table
                loadUsers(filter, page);
                
                // Update notification badges with new count
                updateNotificationBadges(response.pendingCount);
            } else {
                console.error("Error updating user status:", response.error);
                showAlert('error', `Failed to ${status.toLowerCase()} user: ${response.error || 'Unknown error'}`);
            }
        },
        error: function(error) {
            console.error(`Error ${status.toLowerCase()} user:`, error);
            console.error("Response text:", error.responseText);
            showAlert('error', `Failed to ${status.toLowerCase()} user. Please try again.`);
        }
    });
}

function showEmailNotification(userId, status) {
    $('#emailActionText').text(`User has been ${status.toLowerCase()} successfully!`);
    
    if (status.toLowerCase() === 'verified') {
        $('#emailSubject').text('Barangay Health Center - Account Verified');
        $('#emailBody').html(`
            <p>Dear <span id="recipientName">User</span>,</p>
            <p>Congratulations! Your account has been verified! You can now log in to the Barangay Health Center system.</p>
            <p>Thank you for your patience during the verification process.</p>
            <p>Best regards,<br>Barangay Health Center Admin</p>
        `);
    } else {
        $('#emailSubject').text('Barangay Health Center - Account Rejected');
        $('#emailBody').html(`
            <p>Dear <span id="recipientName">User</span>,</p>
            <p>We regret to inform you that your account registration has been rejected. This may be due to insufficient or invalid residency proof documentation.</p>
            <p>You may reapply with the correct documentation if you wish to gain access to our system.</p>
            <p>Best regards,<br>Barangay Health Center Admin</p>
        `);
    }
    
    $('#emailNotificationModal').modal('show');
}

function showAlert(type, message) {
    const alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
    const icon = type === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle';
    
    const alertHtml = `
        <div class="alert ${alertClass} alert-dismissible fade show mt-3" role="alert">
            <i class="fas ${icon} me-2"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
    
    // Insert alert after the heading
    $(alertHtml).insertAfter('.user-management-container h2').delay(5000).fadeOut(500, function() {
        $(this).remove();
    });
}

function formatDate(dateString) {
    if (!dateString) return 'N/A';
    const options = { year: 'numeric', month: 'short', day: 'numeric' };
    return new Date(dateString).toLocaleDateString('en-US', options);
}

function updateNotificationBadges(pendingCount) {
    // If pendingCount is not provided, use any value from the page
    if (pendingCount === undefined) {
        // Try to get the count from the badge or hidden field
        const badgeText = $('#navbarNotificationBadge').text();
        pendingCount = badgeText && !isNaN(parseInt(badgeText)) ? parseInt(badgeText) : 0;
    }
    
    // Update the navbar notification badge
    const badge = $('#navbarNotificationBadge');
    if (badge.length) {
        if (pendingCount > 0) {
            badge.text(pendingCount > 99 ? '99+' : pendingCount);
            badge.addClass('show');
        } else {
            badge.text('');
            badge.removeClass('show');
        }
    }
    
    // Update page-level alert if exists
    const pendingAlert = $('#pendingUsersAlert');
    if (pendingAlert.length) {
        if (pendingCount > 0) {
            pendingAlert.find('.pending-count').text(pendingCount);
            pendingAlert.show();
        } else {
            pendingAlert.hide();
        }
    }
}

function viewFile(fileName, username) {
    const fileExt = fileName.split('.').pop().toLowerCase();
    let fileType = 'Document';
    let previewContent = '';
    
    // Determine file type and prepare appropriate preview content
    if (['pdf'].includes(fileExt)) {
        fileType = 'PDF Document';
        // For real implementation, use the actual file path
        const placeholderPath = '/uploads/residency_proofs/preview-placeholder.pdf';
        
        // Create iframe for PDF preview (in production, this would be the actual file path)
        previewContent = `
            <div class="pdf-container">
                <iframe 
                    src="/images/sample-document-preview.pdf" 
                    width="100%" 
                    height="400" 
                    frameborder="0" 
                    class="pdf-preview">
                </iframe>
                <div class="placeholder-overlay">
                    <div class="placeholder-content">
                        <i class="fas fa-file-pdf fa-3x mb-3 text-danger"></i>
                        <h5>PDF Preview Placeholder</h5>
                        <p>This is a simulated preview of: ${fileName}</p>
                    </div>
                </div>
            </div>
        `;
    } else if (['jpg', 'jpeg', 'png', 'gif'].includes(fileExt)) {
        fileType = 'Image';
        // For real implementation, use the actual file path
        // Here we use a placeholder image
        previewContent = `
            <div class="image-container">
                <img 
                    src="/images/preview-placeholder.jpg" 
                    alt="${fileName}" 
                    class="img-fluid image-preview">
                <div class="placeholder-overlay">
                    <div class="placeholder-content">
                        <i class="fas fa-image fa-3x mb-3 text-primary"></i>
                        <h5>Image Preview Placeholder</h5>
                        <p>This is a simulated preview of: ${fileName}</p>
                    </div>
                </div>
            </div>
        `;
    } else if (['doc', 'docx'].includes(fileExt)) {
        fileType = 'Word Document';
        previewContent = `
            <div class="document-placeholder">
                <i class="fas fa-file-word fa-5x text-primary mb-3"></i>
                <h5>Word Document Preview</h5>
                <p>Preview not available for Word documents</p>
            </div>
        `;
    } else {
        // Generic file display for other types
        previewContent = `
            <div class="document-placeholder">
                <i class="fas fa-file fa-5x mb-3"></i>
                <h5>File Preview</h5>
                <p>Preview not available for this file type</p>
            </div>
        `;
    }
    
    $('#fileType').text(fileType);
    $('#documentPreviewArea').html(previewContent);
    
    // Show the modal
    $('#viewFileModal').modal('show');
}

// Load users from localStorage
function loadUsersFromLocalStorage() {
    const storedUsers = localStorage.getItem('bhcUsers');
    if (storedUsers) {
        usersDatabase = JSON.parse(storedUsers);
        
        // Add some demo data if no users exist
        if (usersDatabase.length === 0) {
            addDemoData();
        }
    } else {
        // Add demo data if no data exists
        addDemoData();
    }
}

// Save users to localStorage
function saveUsersToLocalStorage() {
    localStorage.setItem('bhcUsers', JSON.stringify(usersDatabase));
}

// Add demo data for testing
function addDemoData() {
    const demoUsers = [
        {
            username: 'johndoe',
            email: 'john.doe@example.com',
            firstName: 'John',
            lastName: 'Doe',
            middleName: 'Smith',
            suffix: '',
            contactNumber: '09171234567',
            age: '35',
            residencyProof: 'john_doe_barangay_clearance.pdf',
            status: 'pending',
            registrationDate: '2023-07-15T08:30:00Z'
        },
        {
            username: 'mariagarcia',
            email: 'maria.garcia@example.com',
            firstName: 'Maria',
            lastName: 'Garcia',
            middleName: 'C',
            suffix: '',
            contactNumber: '09187654321',
            age: '28',
            residencyProof: 'maria_garcia_id.jpg',
            status: 'verified',
            registrationDate: '2023-06-20T14:15:00Z'
        },
        {
            username: 'robertsantos',
            email: 'robert.santos@example.com',
            firstName: 'Robert',
            lastName: 'Santos',
            middleName: 'B',
            suffix: 'Jr',
            contactNumber: '09123456789',
            age: '42',
            residencyProof: 'robert_santos_proof.pdf',
            status: 'pending',
            registrationDate: '2023-07-18T10:45:00Z'
        }
    ];
    
    usersDatabase = demoUsers;
    saveUsersToLocalStorage();
}

// Update notification badge
function updateNotificationBadge() {
    // Get pending users count
    const pendingUsers = getPendingUsers();
    const pendingCount = pendingUsers.length;
    
    // Update badge if exists
    const badge = document.getElementById('notificationBadge');
    if (badge) {
        if (pendingCount > 0) {
            badge.textContent = pendingCount > 9 ? '9+' : pendingCount;
            badge.classList.add('show');
        } else {
            badge.textContent = '';
            badge.classList.remove('show');
        }
    }
    
    // Update alert if exists
    const alertElement = document.getElementById('pendingUsersAlert');
    if (alertElement) {
        if (pendingCount > 0) {
            const countElement = alertElement.querySelector('strong');
            if (countElement) {
                countElement.textContent = pendingCount;
            }
            
            // Update text for singular/plural
            const textNode = alertElement.querySelector('div').childNodes[1];
            if (textNode) {
                textNode.nodeValue = ` pending user ${pendingCount === 1 ? 'approval' : 'approvals'}`;
            }
            
            alertElement.classList.add('show');
        } else {
            // Hide and remove alert if no pending users
            alertElement.classList.remove('show');
            setTimeout(() => {
                alertElement.remove();
            }, 300);
        }
    }
}

// Get pending users
function getPendingUsers() {
    return usersDatabase.filter(user => user.status === 'pending');
}

// View file function
function viewFile(fileName, username) {
    const fileExt = fileName.split('.').pop().toLowerCase();
    let fileType = 'Document';
    let previewContent = '';
    
    // Determine file type and prepare appropriate preview content
    if (['pdf'].includes(fileExt)) {
        fileType = 'PDF Document';
        // For real implementation, use the actual file path
        const placeholderPath = '/uploads/residency_proofs/preview-placeholder.pdf';
        
        // Create iframe for PDF preview (in production, this would be the actual file path)
        previewContent = `
            <div class="pdf-container">
                <iframe 
                    src="/images/sample-document-preview.pdf" 
                    width="100%" 
                    height="400" 
                    frameborder="0" 
                    class="pdf-preview">
                </iframe>
                <div class="placeholder-overlay">
                    <div class="placeholder-content">
                        <i class="fas fa-file-pdf fa-3x mb-3 text-danger"></i>
                        <h5>PDF Preview Placeholder</h5>
                        <p>This is a simulated preview of: ${fileName}</p>
                    </div>
                </div>
            </div>
        `;
    } else if (['jpg', 'jpeg', 'png', 'gif'].includes(fileExt)) {
        fileType = 'Image';
        // For real implementation, use the actual file path
        // Here we use a placeholder image
        previewContent = `
            <div class="image-container">
                <img 
                    src="/images/preview-placeholder.jpg" 
                    alt="${fileName}" 
                    class="img-fluid image-preview">
                <div class="placeholder-overlay">
                    <div class="placeholder-content">
                        <i class="fas fa-image fa-3x mb-3 text-primary"></i>
                        <h5>Image Preview Placeholder</h5>
                        <p>This is a simulated preview of: ${fileName}</p>
                    </div>
                </div>
            </div>
        `;
    } else if (['doc', 'docx'].includes(fileExt)) {
        fileType = 'Word Document';
        previewContent = `
            <div class="document-placeholder">
                <i class="fas fa-file-word fa-5x text-primary mb-3"></i>
                <h5>Word Document Preview</h5>
                <p>Preview not available for Word documents</p>
            </div>
        `;
    } else {
        // Generic file display for other types
        previewContent = `
            <div class="document-placeholder">
                <i class="fas fa-file fa-5x mb-3"></i>
                <h5>File Preview</h5>
                <p>Preview not available for this file type</p>
            </div>
        `;
    }
    
    $('#fileType').text(fileType);
    $('#documentPreviewArea').html(previewContent);
    
    // Show the modal
    $('#viewFileModal').modal('show');
}

// Attach event listeners to dynamic elements
function attachEventListeners() {
    // Preview document buttons
    $('.preview-document').off('click').on('click', function() {
        const docId = $(this).data('id');
        const docPath = $(this).data('path');
        const docName = $(this).data('name');
        const docType = $(this).data('type');
        const docUser = $(this).data('user');
        
        console.log('Preview document clicked:', docId, docPath, docName, docType);
        
        // Update modal content
        $('#fileName').text(docName);
        $('#fileType').text(getReadableFileType(docType));
        $('#fileOwner').text(docUser);
        
        // Create preview content based on file type
        const previewArea = $('#documentPreviewArea');
        previewArea.empty();
        
        if (docType.startsWith('image/')) {
            // Image preview
            previewArea.html(`
                <div class="image-container">
                    <img src="${docPath}" alt="${docName}" class="img-fluid">
                </div>
            `);
        } else if (docType === 'application/pdf') {
            // PDF preview
            previewArea.html(`
                <div class="pdf-container">
                    <iframe src="${docPath}" width="100%" height="400" frameborder="0"></iframe>
                </div>
            `);
        } else {
            // Generic file icon for other types
            previewArea.html(`
                <div class="document-placeholder">
                    <i class="fas fa-file fa-5x mb-3"></i>
                    <p>This file type cannot be previewed</p>
                </div>
            `);
        }
        
        // Update view file button
        $('#viewFileBtn').off('click').on('click', function() {
            window.open(docPath, '_blank');
        });
    });
    
    // Approve user buttons
    $('.approve-user').off('click').on('click', function() {
        const userId = $(this).data('id');
        if (confirm('Are you sure you want to approve this user?')) {
            updateUserStatus(userId, 'Verified');
        }
    });
    
    // Reject user buttons
    $('.reject-user').off('click').on('click', function() {
        const userId = $(this).data('id');
        if (confirm('Are you sure you want to reject this user?')) {
            updateUserStatus(userId, 'Rejected');
        }
    });
}

// Helper function to get a readable file type description
function getReadableFileType(mimeType) {
    if (!mimeType) return 'Unknown File Type';
    
    if (mimeType.startsWith('image/')) {
        const type = mimeType.split('/')[1].toUpperCase();
        return `Image (${type})`;
    } else if (mimeType === 'application/pdf') {
        return 'PDF Document';
    } else if (mimeType.includes('word') || mimeType.includes('doc')) {
        return 'Word Document';
    } else if (mimeType.includes('excel') || mimeType.includes('spreadsheet')) {
        return 'Excel Spreadsheet';
    } else if (mimeType.includes('zip') || mimeType.includes('compressed')) {
        return 'Compressed Archive';
    } else {
        return mimeType;
    }
}

// Function to update pagination
function updatePagination(totalPages, currentPage, filter, search) {
    const paginationElement = document.getElementById('userPagination');
    if (!paginationElement) return;
    
    // Clear existing pagination
    paginationElement.innerHTML = '';
    
    // Don't show pagination if there's only one page or no pages
    if (totalPages <= 1) return;
    
    // Add Previous button
    const prevLi = document.createElement('li');
    prevLi.className = `page-item ${currentPage <= 1 ? 'disabled' : ''}`;
    
    const prevLink = document.createElement('a');
    prevLink.className = 'page-link';
    prevLink.href = '#';
    prevLink.innerHTML = '&laquo;';
    prevLink.setAttribute('aria-label', 'Previous');
    if (currentPage > 1) {
        prevLink.dataset.page = currentPage - 1;
    }
    
    prevLi.appendChild(prevLink);
    paginationElement.appendChild(prevLi);
    
    // Determine which page numbers to show
    let startPage = Math.max(1, currentPage - 2);
    let endPage = Math.min(totalPages, startPage + 4);
    
    // Adjust if we're near the end
    if (endPage - startPage < 4) {
        startPage = Math.max(1, endPage - 4);
    }
    
    // Add page numbers
    for (let i = startPage; i <= endPage; i++) {
        const pageLi = document.createElement('li');
        pageLi.className = `page-item ${i === currentPage ? 'active' : ''}`;
        
        const pageLink = document.createElement('a');
        pageLink.className = 'page-link';
        pageLink.href = '#';
        pageLink.textContent = i;
        pageLink.dataset.page = i;
        
        pageLi.appendChild(pageLink);
        paginationElement.appendChild(pageLi);
    }
    
    // Add Next button
    const nextLi = document.createElement('li');
    nextLi.className = `page-item ${currentPage >= totalPages ? 'disabled' : ''}`;
    
    const nextLink = document.createElement('a');
    nextLink.className = 'page-link';
    nextLink.href = '#';
    nextLink.innerHTML = '&raquo;';
    nextLink.setAttribute('aria-label', 'Next');
    if (currentPage < totalPages) {
        nextLink.dataset.page = currentPage + 1;
    }
    
    nextLi.appendChild(nextLink);
    paginationElement.appendChild(nextLi);
    
    // Attach click events to pagination links
    document.querySelectorAll('#userPagination .page-link').forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const page = this.dataset.page;
            if (page) {
                loadUsers(filter, parseInt(page), search);
            }
        });
    });
}

// Function to update pending count in UI
function updatePendingCount(pendingCount) {
    if (pendingCount === undefined) return;
    
    // Update pending count in the alert
    const pendingAlert = document.getElementById('pendingUsersAlert');
    if (pendingAlert) {
        const countElement = pendingAlert.querySelector('.pending-count');
        if (countElement) {
            countElement.textContent = pendingCount;
        }
        
        // Show or hide the alert based on pending count
        if (pendingCount > 0) {
            pendingAlert.style.display = 'block';
        } else {
            pendingAlert.style.display = 'none';
        }
    }
    
    // Update notification badge
    const badge = document.getElementById('navbarNotificationBadge');
    if (badge) {
        if (pendingCount > 0) {
            badge.textContent = pendingCount > 99 ? '99+' : pendingCount;
            badge.classList.add('show');
        } else {
            badge.textContent = '0';
            badge.classList.remove('show');
        }
    }
}