using MediatR;

namespace Application.Command.DoctorAvailability;

public record DeleteDoctorAvailabilityCommand(long DoctorId, long AvailabilityId) : IRequest<bool>;
