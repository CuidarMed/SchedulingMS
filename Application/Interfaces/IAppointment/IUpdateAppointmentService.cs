using Application.DTOs;

namespace Application.Interfaces.IAppointment
{
    public interface IUpdateAppointmentService
    {
        Task<AppointmentResponse> RescheduleAsync(long id, AppointmentReschedule request);
        Task<AppointmentResponse> CancelAsync(long id, AppointmentCancel request);
        Task<AppointmentResponse> UpdateAttendanceAsync(long id, AppointmentAttendance request);
        Task<AppointmentResponse> UpdateAsync(long id, AppointmentUpdate dto);
    }
}
