using Application.Interfaces.Command;
using Application.Interfaces.Queries;
using MediatR;

namespace Application.Command.DoctorAvailability;

public sealed class DeleteDoctorAvailabilityHandler
    : IRequestHandler<DeleteDoctorAvailabilityCommand, bool>
{
    private readonly IDoctorAvailabilityCommandRepository _cmd;
    private readonly IDoctorAvailabilityQueryRepository _qry;

    public DeleteDoctorAvailabilityHandler(
        IDoctorAvailabilityCommandRepository cmd,
        IDoctorAvailabilityQueryRepository qry)
    {
        _cmd = cmd; _qry = qry;
    }

    public async Task<bool> Handle(DeleteDoctorAvailabilityCommand rq, CancellationToken ct)
    {
        var entity = await _qry.GetByIdAsync(rq.DoctorId, rq.AvailabilityId, ct);
        if (entity is null) return false;

        entity.IsActive = false; // desactivación (soft delete para agenda)
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        await _cmd.UpdateAsync(entity, ct);
        await _cmd.SaveChangesAsync(ct);
        return true;
    }
}
