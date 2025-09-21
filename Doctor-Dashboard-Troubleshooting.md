# Doctor Dashboard Troubleshooting Guide

## Overview
This guide provides troubleshooting steps for the Doctor Dashboard that shows appointments for July 15, 2025, and allows doctors to view and edit patient results.

## Common Issues and Solutions

### No Appointments Showing Up

1. **Check Database Connection**
   - Verify that the application can connect to the database
   - Look for connection logs in the console output

2. **Check Date Format and Comparison**
   - The system is specifically looking for appointments on July 15, 2025
   - Make sure appointments exist in the database with this date
   - Run the `InsertTestAppointment.sql` script to add test data

3. **Check Doctor Association**
   - The system searches for appointments where:
     - DoctorId matches the current user's ID
     - Doctor name contains the current user's name
     - Doctor name contains the current user's last name
   - Check the logged-in user's information in the debug logs

4. **Run the Test Script**
   - Execute `insert-test-appointment.bat` to add test data
   - This will insert an appointment for July 15, 2025, with associated vital signs and assessments

### Editing Patient Results

1. **Check API Endpoints**
   - Ensure the `/api/update/results` endpoint is accessible
   - Verify it accepts PATCH requests

2. **Check Table Names**
   - Make sure the table names in the PATCH request match the database:
     - 'VitalSigns'
     - 'HEEADSSSAssessments'
     - 'NCDRiskAssessments'

3. **Check User Authentication**
   - Make sure the doctor is properly authenticated
   - Check if the doctor's ID is being properly passed to the queries

## Logging

The application now includes enhanced logging:
- Console logs for appointment queries
- Debug information about total appointments for the date
- Debug information about doctor information

## Test Data

To ensure there's test data for July 15, 2025:
1. Run the `insert-test-appointment.bat` file
2. This will add:
   - An appointment for July 15, 2025 at 1:00 PM
   - Vital signs with abnormal temperature (38.5°C)
   - HEADSSS assessment with "Yes" for suicidal thoughts
   - NCD risk assessment with both diabetes and chest pain

## Database Schema Check

Ensure the database has the following tables with the correct structure:
- `[Barangay].[dbo].[Appointments]`
- `[Barangay].[dbo].[VitalSigns]`
- `[Barangay].[dbo].[HEEADSSSAssessments]`
- `[Barangay].[dbo].[NCDRiskAssessments]`

## Debugging the Doctor Dashboard

Look for debug logs including:
- "Searching for appointments on 2025-07-15"
- "Doctor Dashboard: Using date 2025-07-15"
- "Total appointments for 2025-07-15" 