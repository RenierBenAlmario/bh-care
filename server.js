
const express = require('express');
const app = express();
const port = 3000;

// Middleware to parse JSON bodies, which is needed for updating permissions.
app.use(express.json());

// --- Mock Data ---
// In a real application, this data would come from a database.

// Mock staff data, including a doctor who initially lacks "Dashboard" permission.
const staff = [
    { email: 'doctor1@example.com', role: 'doctor', joinDate: '2023-01-15', permissions: ['Reports', 'Appointments'] },
    { email: 'nurse1@example.com', role: 'nurse', joinDate: '2023-02-20', permissions: ['Dashboard', 'PatientRecords'] },
    { email: 'admin1@example.com', role: 'admin', joinDate: '2023-01-01', permissions: ['Dashboard', 'Reports', 'Settings', 'UserManagement'] }
];

// Mock user data with different statuses.
const users = [
    { userId: 1, email: 'doctor1@example.com', status: 'VERIFIED', birthDate: '1985-05-20', guardianConsent: 'N/A' },
    { userId: 2, email: 'nurse1@example.com', status: 'APPROVED', birthDate: '1990-08-12', guardianConsent: 'N/A' },
    { userId: 3, email: 'admin1@example.com', status: 'VERIFIED', birthDate: '1980-11-01', guardianConsent: 'N/A' },
    { userId: 4, email: 'newuser@example.com', status: 'PENDING', birthDate: '2010-04-04', guardianConsent: 'PENDING' }
];


// --- Permission Logic ---

/**
 * Middleware to check user permissions for a specific page.
 * This is a factory function: it takes the required page and returns a middleware handler.
 * @param {string} requiredPage - The page the user is trying to access (e.g., "Dashboard").
 */
const checkPermission = (requiredPage) => {
    return (req, res, next) => {
        // In a real app, user identity would come from a secure token (e.g., JWT).
        // For this example, we'll use a header to specify the user.
        const userEmail = req.headers['x-user-email'];

        if (!userEmail) {
            return res.status(401).json({ message: 'Unauthorized: x-user-email header is required.' });
        }

        // 1. Find the user and their corresponding staff profile.
        const user = users.find(u => u.email === userEmail);
        const staffMember = staff.find(s => s.email === userEmail);

        if (!user || !staffMember) {
            return res.status(404).json({ message: 'User or staff profile not found.' });
        }

        // 2. Ensure the user has a "VERIFIED" or "APPROVED" status.
        const isStatusApproved = ['VERIFIED', 'APPROVED'].includes(user.status);
        if (!isStatusApproved) {
            return res.status(403).json({ message: `Access Denied: User status is '${user.status}'. Approval required.` });
        }

        // 3. Check if the staff member's role has the required permission.
        // The permissions array is dynamic and can be changed by an admin.
        const hasPermission = staffMember.permissions.includes(requiredPage);
        if (!hasPermission) {
            return res.status(403).json({ message: `Access Denied: Your role ('${staffMember.role}') does not have permission to access '${requiredPage}'.` });
        }

        // If all checks pass, grant access to the requested route.
        console.log(`SUCCESS: Access granted to ${userEmail} for page '${requiredPage}'.`);
        next();
    };
};


// --- API Routes ---

// A protected route for the dashboard. Only users with "Dashboard" permission can access it.
app.get('/dashboard', checkPermission('Dashboard'), (req, res) => {
    res.status(200).json({ message: 'Welcome to the Dashboard!' });
});

// A protected route for reports.
app.get('/reports', checkPermission('Reports'), (req, res) => {
    res.status(200).json({ message: 'Here are your reports.' });
});

/**
 * An admin-only route to update permissions for a staff member.
 * This demonstrates how an admin can dynamically assign page access.
 */
app.put('/admin/staff/:email/permissions', (req, res) => {
    // Note: A real application should have an additional `checkPermission('UserManagement')`
    // or similar middleware here to ensure only admins can perform this action.

    const { email } = req.params;
    const { permissions } = req.body; // Expects a JSON body like { "permissions": ["Dashboard", "Reports"] }

    const staffMember = staff.find(s => s.email === email);

    if (!staffMember) {
        return res.status(404).json({ message: 'Staff member not found.' });
    }

    if (!Array.isArray(permissions)) {
        return res.status(400).json({ message: 'Invalid input: permissions must be an array of strings.' });
    }

    // Update the permissions for the specified staff member.
    staffMember.permissions = permissions;

    console.log(`ADMIN ACTION: Updated permissions for ${email} to: [${permissions.join(', ')}]`);
    res.status(200).json({
        message: `Successfully updated permissions for ${email}.`,
        staffMember
    });
});

// A public route to view the current state of staff permissions for demonstration.
app.get('/staff-permissions', (req, res) => {
    res.status(200).json(staff);
});


// --- Server Initialization ---
app.listen(port, () => {
    console.log(`Permission fix example server running at http://localhost:${port}`);
    console.log('\nTo test, use a tool like curl or Postman. See instructions in the documentation.');
});
