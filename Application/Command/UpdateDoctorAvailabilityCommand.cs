using Application.DTOs;
using MediatR;

namespace Application.Command.DoctorAvailability
{
    public sealed record UpdateDoctorAvailabilityCommand(
        long DoctorId,
        long AvailabilityId,
        DoctorAvailabilityUpdate Body
    ) : IRequest<DoctorAvailabilityResponse>;
}
