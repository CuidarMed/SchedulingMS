using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAppointmentQuery
    {
        Task<Appointment?> GetByIdAsync(long appointmentId);
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<IEnumerable<Appointment>> SearchAsync(
            long? doctorId,
            long? patientId,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            string? status,
            int pageNumber,
            int pageSize);
        Task<bool> HasConflictAsync(
            long doctorId,
            DateTimeOffset startTime,
            DateTimeOffset endTime,
            long? excludeAppointmentId = null);
    }
}
