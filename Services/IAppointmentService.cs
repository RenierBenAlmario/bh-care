using System;
using System.Linq;
using System.Threading.Tasks;
using Barangay.Models;

namespace Barangay.Services
{
    public interface IAppointmentService
    {
        Task<Appointment?> CreateAppointmentAsync(Appointment appointment);
        Task<bool> IsTimeSlotAvailableAsync(string doctorId, DateTime date, TimeSpan time);
        Task<bool> HasConflictingAppointmentAsync(string patientId, DateTime date);
        Task<bool> IsDoctorAvailableAsync(string doctorId, DateTime date);
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<IOrderedQueryable<Appointment>> GetAppointmentsForUserAsync(string userId, bool isDoctor);
        Task<bool> CheckTimeSlotAvailability(string doctorId, DateTime date, TimeSpan time);
    }
} 