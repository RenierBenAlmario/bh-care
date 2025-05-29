# Barangay Health Care System - Admin Reports Database Integration

This document outlines the database integration for the Admin Reports page in the Barangay Health Care System web application.

## Overview

The Admin Reports page now uses real database data for the following charts:

1. **Staff Distribution by Role (Pie Chart)**
   - Shows distribution of staff members by role (Doctors, Nurses, Admins, etc.)
   - Data source: `dbo.StaffMembers` table

2. **Patient Registrations (Line Chart)**
   - Shows patient registrations over time (last 7, 30, 90 days or 1 year)
   - Data source: `dbo.Patients` table, using `CreatedAt` date

3. **Consultations by Type (Bar Chart)** 
   - Shows count of consultations by type (Check-up, Vaccination, etc.)
   - Data source: `dbo.MedicalRecords` table, using `Type` field

4. **Barangay Health Index (Line Chart)**
   - Shows health index metric calculated from vital signs
   - Data source: `dbo.VitalSigns` table

## Database Schema

The implementation uses the following database tables:

- `dbo.StaffMembers` - Staff information including role, department
- `dbo.Patients` - Patient registration data with creation dates
- `dbo.MedicalRecords` - Consultation records with types and dates
- `dbo.VitalSigns` - Patient vital signs data used for health index calculation

## Technical Implementation

The implementation consists of three main components:

1. **C# API Endpoints** (`Pages/Admin/Reports.cshtml.cs`)
   - Each chart has a corresponding API endpoint that queries the database
   - Endpoints support filtering by date range
   - Each endpoint returns properly formatted JSON data for charts

2. **JavaScript Client** (`js/reports.js`)
   - Fetches data from the API endpoints
   - Updates chart visualizations with real data
   - Handles filtering and refreshing of data

3. **HTML/CSS Presentation** (`Pages/Admin/Reports.cshtml`)
   - Chart containers and UI elements for filtering
   - Chart.js initialization

## Health Index Calculation

The Health Index is calculated based on vital signs data, specifically:

- SpO2 levels (60% weight)
- Heart rate (40% weight)

The algorithm normalizes these values to a 0-100 scale where 100 represents optimal health. This is a simplified health metric for demonstration purposes.

## Testing the Integration

To test the reports database connectivity:

1. **Verify data presence:**
   - Ensure you have data in `StaffMembers` table
   - Ensure you have patient registration records with various dates
   - Ensure you have medical records with different consultation types
   - Ensure you have vital signs data for the health index

2. **Access the reports page:**
   - Navigate to `/Admin/Reports` (must be logged in as Admin)

3. **Test filters:**
   - Try different time ranges (7 days, 30 days, 90 days, 1 year)
   - Try custom date range
   - Filter by report type

4. **Verify API endpoints directly:**
   - `/Admin/Reports?handler=StaffDistribution`
   - `/Admin/Reports?handler=PatientRegistrations&timeRange=30days`
   - `/Admin/Reports?handler=ConsultationsByType&timeRange=30days`
   - `/Admin/Reports?handler=HealthIndex&timeRange=30days`

## Troubleshooting

If charts show "No data available":

1. Check that the database contains appropriate data for the selected time period
2. Inspect browser console for JavaScript errors
3. Check server logs for any exceptions in the API endpoints
4. Verify database connection string in `appsettings.json`

## Future Enhancements

Potential future improvements:

1. Add more sophisticated health index calculation
2. Add export functionality for reports (PDF, Excel, CSV)
3. Add more detailed drill-down reports
4. Add caching for expensive database queries
5. Add comparison with previous time periods (e.g., this month vs. last month)

## Last Updated

May 21, 2025 