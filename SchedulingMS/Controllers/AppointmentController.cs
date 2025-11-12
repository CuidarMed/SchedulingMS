using Application.DTOs;
using Application.Interfaces.IAppointment;
using Microsoft.AspNetCore.Mvc;

namespace SchedulingMS.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly ICreateAppointmentService _createService;
        private readonly ISearchAppointmentService _searchService;
        private readonly IUpdateAppointmentService _updateService;

        public AppointmentsController(
            ICreateAppointmentService createService,
            ISearchAppointmentService searchService,
            IUpdateAppointmentService updateService)
        {
            _createService = createService;
            _searchService = searchService;
            _updateService = updateService;
        }

        // POST: api/appointments
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AppointmentCreate request)
        {
            try
            {
                var result = await _createService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.AppointmentId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/appointments/{id}
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var appointment = await _searchService.GetByIdAsync(id);
            if (appointment == null) return NotFound();
            return Ok(appointment);
        }

        // GET: api/appointments
        // Soporta filtros opcionales
        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] long? doctorId,
            [FromQuery] long? patientId,
            [FromQuery] DateTimeOffset? startTime,
            [FromQuery] DateTimeOffset? endTime,
            [FromQuery] Domain.Enum.AppointmentStatus? status)
        {
            var searchDto = new AppointmentSearch
            {
                DoctorId = doctorId,
                PatientId = patientId,
                StartTime = startTime,
                EndTime = endTime,
                Status = status
            };

            var appointments = await _searchService.SearchAsync(searchDto);
            return Ok(appointments);
        }

        // PUT: api/appointments/{id}/reschedule
        [HttpPatch("{id:long}/reschedule")]
        public async Task<IActionResult> Reschedule(long id, [FromBody] AppointmentReschedule request)
        {
            try
            {
                var updated = await _updateService.RescheduleAsync(id, request);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // PUT: api/appointments/{id}/cancel
        [HttpPatch("{id:long}/cancel")]
        public async Task<IActionResult> Cancel(long id, [FromBody] AppointmentCancel request)
        {
            try
            {
                var updated = await _updateService.CancelAsync(id, request);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // PUT: api/appointments/{id}/attendance
        [HttpPatch("{id:long}/attendance")]
        public async Task<IActionResult> UpdateAttendance(long id, [FromBody] AppointmentAttendance request)
        {
            try
            {
                var updated = await _updateService.UpdateAttendanceAsync(id, request);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
