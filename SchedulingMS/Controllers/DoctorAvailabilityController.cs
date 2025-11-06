using Application.Command.DoctorAvailability;
using Application.DTOs;
using Application.Queries.DoctorAvailability;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SchedulingMS.Controllers;

[ApiController]
[Route("api/v1/doctors/{doctorId:long}/availability")]
public sealed class DoctorAvailabilityController : ControllerBase
{
    private readonly IMediator _mediator;
    public DoctorAvailabilityController(IMediator mediator) => _mediator = mediator;

    // GET /api/v1/doctors/{doctorId}/availability
    [HttpGet]
    public async Task<ActionResult<List<DoctorAvailabilityResponse>>> Get(long doctorId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetDoctorAvailabilityByDoctorQuery(doctorId), ct);
        return Ok(result);
    }

    // POST /api/v1/doctors/{doctorId}/availability
    [HttpPost]
    public async Task<ActionResult<DoctorAvailabilityResponse>> Create(long doctorId, [FromBody] DoctorAvailabilityCreate body, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateDoctorAvailabilityCommand(doctorId, body), ct);
        return CreatedAtAction(nameof(Get), new { doctorId }, result);
    }

    // PATCH /api/v1/doctors/{doctorId}/availability/{id}
    [HttpPatch("{id:long}")]
    public async Task<ActionResult<DoctorAvailabilityResponse>> Update(long doctorId, long id, [FromBody] DoctorAvailabilityUpdate body, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateDoctorAvailabilityCommand(doctorId, id, body), ct);
        return Ok(result);
    }

    // DELETE /api/v1/doctors/{doctorId}/availability/{id}
    [HttpDelete("{id:long}")]
    public async Task<ActionResult> Delete(long doctorId, long id, CancellationToken ct)
    {
        var ok = await _mediator.Send(new DeleteDoctorAvailabilityCommand(doctorId, id), ct);
        return ok ? NoContent() : NotFound();
    }
}
