document.addEventListener('DOMContentLoaded', function() {
    // Update user counts
    function updateCounts() {
        try {
            if (typeof DB !== 'undefined' && DB.getCounts) {
                const counts = DB.getCounts();
                
                const totalCount = document.getElementById('totalCount');
                const pendingCount = document.getElementById('pendingCount');
                const verifiedCount = document.getElementById('verifiedCount');
                const rejectedCount = document.getElementById('rejectedCount');
                
                if (totalCount) totalCount.textContent = counts.total || 0;
                if (pendingCount) pendingCount.textContent = counts.pending || 0;
                if (verifiedCount) verifiedCount.textContent = counts.verified || 0;
                if (rejectedCount) rejectedCount.textContent = counts.rejected || 0;
            } else {
                console.error('DB or DB.getCounts is not available');
            }
        } catch (error) {
            console.error('Error updating counts:', error);
        }
    }
    
    // Load recent users
    function loadRecentUsers() {
        const tableBody = document.getElementById('recentUsersTableBody');
        if (!tableBody) {
            console.error('Recent users table body not found');
            return;
        }
        
        let recentUsers = [];
        try {
            // Check if DB is defined and has the required method
            if (typeof DB !== 'undefined' && DB.getRecentUsers) {
                recentUsers = DB.getRecentUsers() || [];
            } else {
                console.error('DB or DB.getRecentUsers is not available');
                showErrorMessage(tableBody);
                retryDataLoad(3000); // Retry after 3 seconds
                return;
            }
        } catch (error) {
            console.error('Error fetching recent users:', error);
            showErrorMessage(tableBody);
            retryDataLoad(3000);
            return;
        }
        
        tableBody.innerHTML = '';
        
        if (!Array.isArray(recentUsers) || recentUsers.length === 0) {
            const row = document.createElement('tr');
            row.innerHTML = '<td colspan="6" style="text-align: center;">No users found</td>';
            tableBody.appendChild(row);
            return;
        }
        
        recentUsers.forEach(user => {
            const row = document.createElement('tr');
            
            // Format date safely
            let formattedDate = 'N/A';
            try {
                if (user.registrationDate) {
                    const date = new Date(user.registrationDate);
                    formattedDate = `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
                }
            } catch (e) {
                console.warn('Error formatting date:', e);
            }
            
            // Determine status class
            let statusClass = '';
            switch (user.status) {
                case 'pending': statusClass = 'status-pending'; break;
                case 'verified': statusClass = 'status-verified'; break;
                case 'rejected': statusClass = 'status-rejected'; break;
            }
            
            row.innerHTML = `
                <td>${user.username || 'N/A'}</td>
                <td>${user.fullName || 'N/A'}</td>
                <td>${user.email || 'N/A'}</td>
                <td><span class="${statusClass}">${user.status || 'unknown'}</span></td>
                <td>${formattedDate}</td>
                <td>
                    <div class="action-buttons">
                        <a href="userManagement.html?id=${user.id}" class="btn-primary btn-sm">View</a>
                    </div>
                </td>
            `;
            
            tableBody.appendChild(row);
        });
    }
    
    // Display error message in the table
    function showErrorMessage(tableBody) {
        tableBody.innerHTML = `
            <tr>
                <td colspan="6" style="text-align: center;">
                    <div class="error-message">
                        <i class="fas fa-exclamation-circle"></i>
                        <p>Failed to load data. <button id="retry-load" class="retry-btn">Retry</button></p>
                    </div>
                </td>
            </tr>
        `;
        
        const retryBtn = document.getElementById('retry-load');
        if (retryBtn) {
            retryBtn.addEventListener('click', () => {
                tableBody.innerHTML = '<tr><td colspan="6" style="text-align: center;">Loading...</td></tr>';
                setTimeout(loadRecentUsers, 500);
            });
        }
    }
    
    // Retry loading data after a delay
    function retryDataLoad(delay) {
        setTimeout(loadRecentUsers, delay);
    }
    
    // Helper function to show notifications
    function showMessage(message, type = 'info') {
        // Create message container if it doesn't exist
        let messageContainer = document.querySelector('.message-container');
        if (!messageContainer) {
            messageContainer = document.createElement('div');
            messageContainer.className = 'message-container';
            document.body.appendChild(messageContainer);
        }
        
        // Create message element
        const messageElement = document.createElement('div');
        messageElement.className = `message message-${type}`;
        messageElement.innerHTML = `
            <i class="fas ${type === 'success' ? 'fa-check-circle' : type === 'error' ? 'fa-exclamation-circle' : 'fa-info-circle'}"></i>
            <span>${message}</span>
        `;
        
        // Add to container
        messageContainer.appendChild(messageElement);
        
        // Animate and remove after timeout
        setTimeout(() => {
            messageElement.classList.add('fade-out');
            setTimeout(() => {
                messageElement.remove();
            }, 300);
        }, 5000);
    }
    
    // Initialize dashboard
    updateCounts();
    loadRecentUsers();
}); 