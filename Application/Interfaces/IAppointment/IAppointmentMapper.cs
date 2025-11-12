using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces.IAppointment
{
    public interface IAppointmentMapper
    {
        AppointmentResponse ToResponse(Appointment entity);
        List<AppointmentResponse> ToResponseList(List<Appointment> entities);
    }
}
