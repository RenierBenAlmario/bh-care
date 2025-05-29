document.addEventListener('DOMContentLoaded', function() {
    // DOM Elements
    const userTableBody = document.getElementById('userTableBody');
    const searchInput = document.getElementById('searchInput');
    const searchButton = document.getElementById('searchButton');
    const statusFilter = document.getElementById('statusFilter');
    const pagination = document.getElementById('pagination');
    
    // File View Modal Elements
    const fileViewModal = document.getElementById('fileViewModal');
    const filePreview = document.getElementById('filePreview');
    const closeFileModalBtns = fileViewModal.querySelectorAll('.close-modal');
    
    // Email Preview Modal Elements
    const emailPreviewModal = document.getElementById('emailPreviewModal');
    const emailTo = document.getElementById('emailTo');
    const emailSubject = document.getElementById('emailSubject');
    const emailBody = document.getElementById('emailBody');
    const sendEmailBtn = document.getElementById('sendEmailBtn');
    const emailSendingStatus = document.getElementById('emailSendingStatus');
    const closeEmailModalBtns = emailPreviewModal.querySelectorAll('.close-modal');
    
    // State variables
    let currentPage = 1;
    const itemsPerPage = 5;
    let filteredUsers = [];
    let currentStatus = 'all';
    let searchTerm = '';
    let selectedUser = null;
    let emailAction = '';
    
    // Load users with pagination
    function loadUsers() {
        // Get filtered users
        filteredUsers = getFilteredUsers();
        
        // Calculate pagination
        const totalPages = Math.ceil(filteredUsers.length / itemsPerPage);
        const startIndex = (currentPage - 1) * itemsPerPage;
        const endIndex = Math.min(startIndex + itemsPerPage, filteredUsers.length);
        const paginatedUsers = filteredUsers.slice(startIndex, endIndex);
        
        // Update table
        updateTable(paginatedUsers);
        
        // Update pagination
        updatePagination(totalPages);
    }
    
    // Get filtered users based on search and status filter
    function getFilteredUsers() {
        let users = [];
        try {
            // Check if DB is initialized and getUsers is available
            if (typeof DB !== 'undefined' && DB.getUsers) {
                users = DB.getUsers() || [];
            } else {
                console.error('DB or DB.getUsers is not available');
                showMessage('Database connection error. Please refresh the page or contact support.', 'error');
                return [];
            }
        } catch (error) {
            console.error('Error fetching users:', error);
            showMessage('Error loading user data: ' + error.message, 'error');
            return [];
        }
        
        // Handle empty users array
        if (!Array.isArray(users) || users.length === 0) {
            return [];
        }
        
        // Filter by status
        if (currentStatus !== 'all') {
            users = users.filter(user => user.status === currentStatus);
        }
        
        // Filter by search term
        if (searchTerm) {
            users = users.filter(user => 
                user.username?.toLowerCase().includes(searchTerm.toLowerCase()) ||
                user.fullName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
                user.email?.toLowerCase().includes(searchTerm.toLowerCase())
            );
        }
        
        // Sort by registration date (newest first)
        return users.sort((a, b) => new Date(b.registrationDate) - new Date(a.registrationDate));
    }
    
    // Update the table with users
    function updateTable(users) {
        userTableBody.innerHTML = '';
        
        if (users.length === 0) {
            const row = document.createElement('tr');
            row.innerHTML = '<td colspan="8" style="text-align: center;">No users found</td>';
            userTableBody.appendChild(row);
            return;
        }
        
        users.forEach(user => {
            const row = document.createElement('tr');
            
            // Format date
            const date = new Date(user.registrationDate);
            const formattedDate = `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
            
            // Determine status class
            let statusClass = '';
            switch (user.status) {
                case 'pending': statusClass = 'status-pending'; break;
                case 'verified': statusClass = 'status-verified'; break;
                case 'rejected': statusClass = 'status-rejected'; break;
            }
            
            // Create action buttons based on status
            let actionButtons = '';
            if (user.status === 'pending') {
                actionButtons = `
                    <button class="btn-success btn-sm" data-action="approve" data-id="${user.id}">Approve</button>
                    <button class="btn-danger btn-sm" data-action="reject" data-id="${user.id}">Reject</button>
                `;
            } else {
                actionButtons = `
                    <button class="btn-primary btn-sm" data-action="view" data-id="${user.id}">View</button>
                `;
            }
            
            row.innerHTML = `
                <td>${user.username}</td>
                <td>${user.fullName}</td>
                <td>${user.email}</td>
                <td>${user.contactNumber}</td>
                <td><span class="file-link" data-file="${user.residencyProof}">${user.residencyProof}</span></td>
                <td><span class="${statusClass}">${user.status}</span></td>
                <td>${formattedDate}</td>
                <td>
                    <div class="action-buttons">
                        ${actionButtons}
                    </div>
                </td>
            `;
            
            userTableBody.appendChild(row);
        });
        
        // Add event listeners to buttons
        addActionButtonListeners();
        
        // Add event listeners to file links
        addFileViewListeners();
    }
    
    // Update pagination
    function updatePagination(totalPages) {
        pagination.innerHTML = '';
        
        if (totalPages <= 1) {
            return;
        }
        
        // Previous page
        const prevLi = document.createElement('li');
        const prevLink = document.createElement('a');
        prevLink.href = '#';
        prevLink.textContent = '«';
        if (currentPage === 1) {
            prevLink.style.opacity = '0.5';
            prevLink.style.pointerEvents = 'none';
        }
        prevLink.addEventListener('click', function(e) {
            e.preventDefault();
            if (currentPage > 1) {
                currentPage--;
                loadUsers();
            }
        });
        prevLi.appendChild(prevLink);
        pagination.appendChild(prevLi);
        
        // Page numbers
        for (let i = 1; i <= totalPages; i++) {
            const pageLi = document.createElement('li');
            const pageLink = document.createElement('a');
            pageLink.href = '#';
            pageLink.textContent = i;
            if (i === currentPage) {
                pageLink.classList.add('active');
            }
            pageLink.addEventListener('click', function(e) {
                e.preventDefault();
                currentPage = i;
                loadUsers();
            });
            pageLi.appendChild(pageLink);
            pagination.appendChild(pageLi);
        }
        
        // Next page
        const nextLi = document.createElement('li');
        const nextLink = document.createElement('a');
        nextLink.href = '#';
        nextLink.textContent = '»';
        if (currentPage === totalPages) {
            nextLink.style.opacity = '0.5';
            nextLink.style.pointerEvents = 'none';
        }
        nextLink.addEventListener('click', function(e) {
            e.preventDefault();
            if (currentPage < totalPages) {
                currentPage++;
                loadUsers();
            }
        });
        nextLi.appendChild(nextLink);
        pagination.appendChild(nextLi);
    }
    
    // Add event listeners to action buttons
    function addActionButtonListeners() {
        const actionButtons = userTableBody.querySelectorAll('button[data-action]');
        
        actionButtons.forEach(button => {
            button.addEventListener('click', function() {
                const action = this.getAttribute('data-action');
                const userId = parseInt(this.getAttribute('data-id'));
                const user = DB.getUserById(userId);
                
                if (!user) return;
                
                selectedUser = user;
                
                if (action === 'approve') {
                    showEmailPreview('approve', user);
                } else if (action === 'reject') {
                    showEmailPreview('reject', user);
                }
            });
        });
    }
    
    // Add event listeners to file links
    function addFileViewListeners() {
        const fileLinks = userTableBody.querySelectorAll('.file-link');
        
        fileLinks.forEach(link => {
            link.addEventListener('click', function() {
                const fileName = this.getAttribute('data-file');
                showFilePreview(fileName);
            });
        });
    }
    
    // Show file preview
    function showFilePreview(fileName) {
        // Determine file type
        const fileExtension = fileName.split('.').pop().toLowerCase();
        let previewHTML = '';
        
        if (['jpg', 'jpeg', 'png', 'gif'].includes(fileExtension)) {
            // Image preview
            previewHTML = `
                <img src="../../images/file-placeholder.png" alt="${fileName}" style="max-width: 100%; max-height: 400px;">
                <p style="margin-top: 15px;"><strong>${fileName}</strong></p>
                <p><em>Image preview simulation</em></p>
            `;
        } else if (fileExtension === 'pdf') {
            // PDF preview
            previewHTML = `
                <div style="background-color: #f8f9fa; padding: 20px; border-radius: 4px;">
                    <svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 24 24" fill="#dc3545">
                        <path d="M12 16L8 12h3V8h2v4h3l-4 4zm9-13h-6v2h5v11H4V5h5V3H3a1 1 0 00-1 1v14a1 1 0 001 1h18a1 1 0 001-1V4a1 1 0 00-1-1z"/>
                    </svg>
                    <p style="margin-top: 15px;"><strong>${fileName}</strong></p>
                    <p><em>PDF preview simulation</em></p>
                </div>
            `;
        } else {
            // Generic file preview
            previewHTML = `
                <div style="background-color: #f8f9fa; padding: 20px; border-radius: 4px;">
                    <svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 24 24" fill="#0056b3">
                        <path d="M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8l-6-6zm-1 1v5h5v10H6V4h7z"/>
                    </svg>
                    <p style="margin-top: 15px;"><strong>${fileName}</strong></p>
                    <p><em>File preview simulation</em></p>
                </div>
            `;
        }
        
        filePreview.innerHTML = previewHTML;
        fileViewModal.style.display = 'block';
    }
    
    // Show email preview
    function showEmailPreview(action, user) {
        emailAction = action;
        
        // Set email details
        emailTo.textContent = user.email;
        
        if (action === 'approve') {
            emailSubject.textContent = 'Your Barangay Health Center Account has been Approved';
            emailBody.innerHTML = `
                <p>Dear ${user.fullName},</p>
                <p>We are pleased to inform you that your account for the Barangay Health Center has been approved. You can now access our healthcare services.</p>
                <p>Username: ${user.username}</p>
                <p>Please log in to your account using your registered email and password.</p>
                <p>If you have any questions, please contact our support team.</p>
                <p>Thank you for registering with us!</p>
                <p>Best regards,<br>Barangay Health Center Team</p>
            `;
        } else {
            emailSubject.textContent = 'Your Barangay Health Center Account Application';
            emailBody.innerHTML = `
                <p>Dear ${user.fullName},</p>
                <p>Thank you for your interest in registering with the Barangay Health Center.</p>
                <p>After reviewing your application, we regret to inform you that we are unable to approve your account at this time. This could be due to incomplete or incorrect information provided.</p>
                <p>You are welcome to submit a new application with complete and accurate information.</p>
                <p>If you have any questions, please contact our support team.</p>
                <p>Best regards,<br>Barangay Health Center Team</p>
            `;
        }
        
        // Reset status
        emailSendingStatus.textContent = '';
        
        // Show modal
        emailPreviewModal.style.display = 'block';
    }
    
    // Send email and update user status
    function sendEmail() {
        if (!selectedUser) return;
        
        // Simulate email sending
        emailSendingStatus.textContent = 'Sending email...';
        
        setTimeout(() => {
            // Update user status
            const newStatus = emailAction === 'approve' ? 'verified' : 'rejected';
            DB.updateUserStatus(selectedUser.id, newStatus);
            
            // Show success message
            emailSendingStatus.textContent = 'Email sent successfully!';
            
            // Close modal after a delay
            setTimeout(() => {
                emailPreviewModal.style.display = 'none';
                loadUsers(); // Reload users table
            }, 1500);
            
            // Log to console
            console.log(`Email sent to ${selectedUser.email} with subject: ${emailSubject.textContent}`);
            console.log(`User status updated to ${newStatus}`);
        }, 1500);
    }
    
    // Search button click event
    searchButton.addEventListener('click', function() {
        searchTerm = searchInput.value.trim();
        currentPage = 1;
        loadUsers();
    });
    
    // Search input enter key event
    searchInput.addEventListener('keyup', function(e) {
        if (e.key === 'Enter') {
            searchTerm = searchInput.value.trim();
            currentPage = 1;
            loadUsers();
        }
    });
    
    // Status filter change event
    statusFilter.addEventListener('change', function() {
        currentStatus = this.value;
        currentPage = 1;
        loadUsers();
    });
    
    // Close file modal
    closeFileModalBtns.forEach(btn => {
        btn.addEventListener('click', function() {
            fileViewModal.style.display = 'none';
        });
    });
    
    // Close email modal
    closeEmailModalBtns.forEach(btn => {
        btn.addEventListener('click', function() {
            emailPreviewModal.style.display = 'none';
        });
    });
    
    // Send email button
    sendEmailBtn.addEventListener('click', sendEmail);
    
    // Close modals if clicked outside
    window.addEventListener('click', function(event) {
        if (event.target === fileViewModal) {
            fileViewModal.style.display = 'none';
        }
        if (event.target === emailPreviewModal) {
            emailPreviewModal.style.display = 'none';
        }
    });
    
    // Initial load
    loadUsers();
}); 