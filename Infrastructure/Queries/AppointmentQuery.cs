using Application.DTOs;
using Application.Interfaces.IAppointment;
using Domain.Entities;
using Domain.Enum;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Queries
{
    public class AppointmentQuery : IAppointmentQuery
    {
        private readonly AppDbContext _context;

        public AppointmentQuery(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Appointment?> GetByIdAsync(long appointmentId)
        {
            return await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
        }

        public async Task<IReadOnlyList<Appointment>> GetAll()
        {
            return await _context.Appointments
                .OrderByDescending(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Appointment>> SearchAsync(AppointmentSearch search)
        {
            var query = _context.Appointments.AsQueryable();

            if (search.DoctorId.HasValue)
                query = query.Where(a => a.DoctorId == search.DoctorId.Value);

            if (search.PatientId.HasValue)
                query = query.Where(a => a.PatientId == search.PatientId.Value);

            if (search.StartTime.HasValue)
                query = query.Where(a => a.StartTime >= search.StartTime.Value);

            if (search.EndTime.HasValue)
                query = query.Where(a => a.EndTime <= search.EndTime.Value);

            if (search.Status.HasValue)
                query = query.Where(a => a.Status == search.Status.Value);

            return await query.OrderBy(a => a.StartTime).ToListAsync();
        }


        public async Task<bool> HasConflictAsync(long doctorId, DateTimeOffset startTime, DateTimeOffset endTime, long? excludeAppointmentId = null)
        {
            var query = _context.Appointments
                .Where(a => a.DoctorId == doctorId &&
                            a.Status != AppointmentStatus.CANCELLED &&
                            a.StartTime < endTime &&
                            a.EndTime > startTime);

            if (excludeAppointmentId.HasValue)
                query = query.Where(a => a.AppointmentId != excludeAppointmentId.Value);

            return await query.AnyAsync();
        }

        public Task<bool> HasConflictAsync(long doctorId, DateTimeOffset start, DateTimeOffset end)
        {
            throw new NotImplementedException();
        }
    }
}
