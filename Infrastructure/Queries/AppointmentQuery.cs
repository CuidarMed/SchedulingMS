using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                .OrderByDescending(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> SearchAsync(
            long? doctorId,
            long? patientId,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            string? status,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Appointments.AsQueryable();

            if (doctorId.HasValue)
                query = query.Where(a => a.DoctorId == doctorId.Value);

            if (patientId.HasValue)
                query = query.Where(a => a.PatientId == patientId.Value);

            if (startDate.HasValue)
                query = query.Where(a => a.StartTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(a => a.EndTime <= endDate.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == status);

            return await query
                .OrderBy(a => a.StartTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> HasConflictAsync(
            long doctorId,
            DateTimeOffset startTime,
            DateTimeOffset endTime,
            long? excludeAppointmentId = null)
        {
            var query = _context.Appointments
                .Where(a => a.DoctorId == doctorId &&
                           a.Status != "CANCELLED" &&
                           ((a.StartTime < endTime && a.EndTime > startTime)));

            if (excludeAppointmentId.HasValue)
                query = query.Where(a => a.AppointmentId != excludeAppointmentId.Value);

            return await query.AnyAsync();
        }
    }
}
