/**
 * Simulated Database for Barangay Health Center
 * This file provides database-like functionality using localStorage for persistence
 */

// Database object
const DB = {
    // Initialize the database
    init: function() {
        // Check if users exist in localStorage
        if (!localStorage.getItem('users')) {
            // Initialize with demo data if empty
            const demoUsers = [
                {
                    id: 1,
                    username: 'johndoe',
                    fullName: 'John Doe',
                    email: 'john.doe@example.com',
                    contactNumber: '09123456789',
                    address: '123 Main St, Barangay Health Center',
                    residencyProof: 'utility_bill.jpg',
                    status: 'pending',
                    registrationDate: '2023-05-15T08:30:00'
                },
                {
                    id: 2,
                    username: 'janedoe',
                    fullName: 'Jane Doe',
                    email: 'jane.doe@example.com',
                    contactNumber: '09198765432',
                    address: '456 Park Ave, Barangay Health Center',
                    residencyProof: 'id_card.pdf',
                    status: 'verified',
                    registrationDate: '2023-05-10T14:45:00'
                },
                {
                    id: 3,
                    username: 'mikeross',
                    fullName: 'Mike Ross',
                    email: 'mike.ross@example.com',
                    contactNumber: '09187654321',
                    address: '789 Elm St, Barangay Health Center',
                    residencyProof: 'electric_bill.jpg',
                    status: 'rejected',
                    registrationDate: '2023-05-08T11:20:00'
                },
                {
                    id: 4,
                    username: 'sarahjames',
                    fullName: 'Sarah James',
                    email: 'sarah.james@example.com',
                    contactNumber: '09165432187',
                    address: '234 Oak St, Barangay Health Center',
                    residencyProof: 'water_bill.pdf',
                    status: 'pending',
                    registrationDate: '2023-05-18T09:15:00'
                },
                {
                    id: 5,
                    username: 'robertsmith',
                    fullName: 'Robert Smith',
                    email: 'robert.smith@example.com',
                    contactNumber: '09176543219',
                    address: '567 Pine St, Barangay Health Center',
                    residencyProof: 'id_card.jpg',
                    status: 'pending',
                    registrationDate: '2023-05-19T16:40:00'
                }
            ];
            
            localStorage.setItem('users', JSON.stringify(demoUsers));
            localStorage.setItem('nextUserId', '6');
        }
        
        // Check if admins exist in localStorage
        if (!localStorage.getItem('admins')) {
            const admins = [
                {
                    username: 'admin',
                    password: 'admin123'
                }
            ];
            
            localStorage.setItem('admins', JSON.stringify(admins));
        }
        
        return this;
    },
    
    // Get all users
    getUsers: function() {
        return JSON.parse(localStorage.getItem('users') || '[]');
    },
    
    // Get user by ID
    getUserById: function(id) {
        const users = this.getUsers();
        return users.find(user => user.id === id);
    },
    
    // Get users by status
    getUsersByStatus: function(status) {
        const users = this.getUsers();
        if (status === 'all') {
            return users;
        }
        return users.filter(user => user.status === status);
    },
    
    // Search users by keyword
    searchUsers: function(keyword) {
        const users = this.getUsers();
        return users.filter(user => 
            user.username.toLowerCase().includes(keyword.toLowerCase()) ||
            user.fullName.toLowerCase().includes(keyword.toLowerCase()) ||
            user.email.toLowerCase().includes(keyword.toLowerCase())
        );
    },
    
    // Add a new user
    addUser: function(user) {
        const users = this.getUsers();
        const nextId = parseInt(localStorage.getItem('nextUserId') || '1');
        
        // Create user object
        const newUser = {
            id: nextId,
            ...user,
            status: 'pending',
            registrationDate: new Date().toISOString()
        };
        
        // Add to users array
        users.push(newUser);
        
        // Save to localStorage
        localStorage.setItem('users', JSON.stringify(users));
        localStorage.setItem('nextUserId', (nextId + 1).toString());
        
        return newUser;
    },
    
    // Update user status
    updateUserStatus: function(id, status) {
        const users = this.getUsers();
        const index = users.findIndex(user => user.id === id);
        
        if (index !== -1) {
            users[index].status = status;
            localStorage.setItem('users', JSON.stringify(users));
            return users[index];
        }
        
        return null;
    },
    
    // Get user counts
    getCounts: function() {
        const users = this.getUsers();
        
        return {
            total: users.length,
            pending: users.filter(user => user.status === 'pending').length,
            verified: users.filter(user => user.status === 'verified').length,
            rejected: users.filter(user => user.status === 'rejected').length
        };
    },
    
    // Get recent users (last 5)
    getRecentUsers: function() {
        const users = this.getUsers();
        return users
            .sort((a, b) => new Date(b.registrationDate) - new Date(a.registrationDate))
            .slice(0, 5);
    },
    
    // Verify admin credentials
    verifyAdmin: function(username, password) {
        const admins = JSON.parse(localStorage.getItem('admins') || '[]');
        return admins.some(admin => 
            admin.username === username && admin.password === password
        );
    }
};

// Initialize the database on script load
DB.init(); 