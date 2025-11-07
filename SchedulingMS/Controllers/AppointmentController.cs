using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SchedulingMS.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly ICreateAppointmentService _createAppointmentService;
        private readonly ISearchAppointmentService _searchAppointmentService;
        private readonly IUpdateAppointmentService _updateAppointmentService;

        public AppointmentController(
            ICreateAppointmentService createAppointmentService,
            ISearchAppointmentService searchAppointmentService,
            IUpdateAppointmentService updateAppointmentService)
        {
            _createAppointmentService = createAppointmentService;
            _searchAppointmentService = searchAppointmentService;
            _updateAppointmentService = updateAppointmentService;
        }

        /// <summary>
        /// POST /api/v1/appointments - Crear turno
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AppointmentResponse>> Create([FromBody] AppointmentCreate request)
        {
            try
            {
                var result = await _createAppointmentService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.AppointmentId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/v1/appointments/{id} - Obtener turno por ID
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentResponse>> GetById(long id)
        {
            try
            {
                var result = await _searchAppointmentService.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { message = "Turno no encontrado" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/v1/appointments - Buscar turnos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentResponse>>> Search([FromQuery] AppointmentSearch search)
        {
            try
            {
                var result = await _searchAppointmentService.SearchAsync(search);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// PATCH /api/v1/appointments/{id}/cancel - Cancelar turno
        /// </summary>
        [HttpPatch("{id}/cancel")]
        public async Task<ActionResult<AppointmentResponse>> Cancel(long id, [FromBody] AppointmentCancel request)
        {
            try
            {
                var result = await _updateAppointmentService.CancelAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// PATCH /api/v1/appointments/{id}/attendance - Marcar asistencia
        /// </summary>
        [HttpPatch("{id}/attendance")]
        public async Task<ActionResult<AppointmentResponse>> UpdateAttendance(long id, [FromBody] AppointmentAttendance request)
        {
            try
            {
                var result = await _updateAppointmentService.UpdateAttendanceAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// PATCH /api/v1/appointments/{id}/reschedule - Reprogramar turno
        /// </summary>
        [HttpPatch("{id}/reschedule")]
        public async Task<ActionResult<AppointmentResponse>> Reschedule(long id, [FromBody] AppointmentReschedule request)
        {
            try
            {
                var result = await _updateAppointmentService.RescheduleAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
