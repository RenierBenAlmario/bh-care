using System;

namespace Barangay.Models
{
    /// <summary>
    /// Represents the possible states of an appointment in the system
    /// </summary>
    public enum AppointmentStatus
    {
        /// <summary>
        /// Initial state when appointment is first created
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Appointment has been confirmed by the doctor
        /// </summary>
        Confirmed = 1,

        /// <summary>
        /// Doctor is currently seeing the patient
        /// </summary>
        InProgress = 2,

        /// <summary>
        /// Appointment has been completed successfully
        /// </summary>
        Completed = 3,

        /// <summary>
        /// Appointment was cancelled by either party
        /// </summary>
        Cancelled = 4,

        /// <summary>
        /// Appointment requires immediate attention
        /// </summary>
        Urgent = 5,

        /// <summary>
        /// Patient did not show up for the appointment
        /// </summary>
        NoShow = 6
    }

    /// <summary>
    /// Represents the status of a prescription
    /// </summary>
    public enum PrescriptionStatus
    {
        /// <summary>
        /// Prescription has been created but not yet processed
        /// </summary>
        Created = 0,
        
        /// <summary>
        /// Prescription is waiting to be processed
        /// </summary>
        Pending = 1,
        
        /// <summary>
        /// Prescription has been filled by pharmacy
        /// </summary>
        Filled = 2,
        
        /// <summary>
        /// Prescription has been dispensed to patient
        /// </summary>
        Dispensed = 3,
        
        /// <summary>
        /// Prescription has been completed
        /// </summary>
        Completed = 4,
        
        /// <summary>
        /// Prescription has been cancelled
        /// </summary>
        Cancelled = 5
    }
}