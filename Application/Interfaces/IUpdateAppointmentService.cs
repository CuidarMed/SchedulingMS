using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUpdateAppointmentService
    {
        Task<AppointmentResponse> RescheduleAsync(long id, AppointmentReschedule request);
        Task<AppointmentResponse> CancelAsync(long id, AppointmentCancel request);
        Task<AppointmentResponse> UpdateAttendanceAsync(long id, AppointmentAttendance request);
    }
}
