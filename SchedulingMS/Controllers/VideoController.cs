using Application.Interfaces.IVideo;
using Microsoft.AspNetCore.Mvc;

namespace SchedulingMS.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;
        private readonly ILogger<VideoController> _logger;

        public VideoController(IVideoService videoService, ILogger<VideoController> logger)
        {
            _videoService = videoService;
            _logger = logger;
        }

        /// <summary>
        /// Crea o obtiene una sala de videollamada para un appointment
        /// </summary>
        /// <param name="appointmentId">ID del appointment</param>
        /// <param name="doctorId">ID del doctor</param>
        /// <param name="patientId">ID del paciente</param>
        /// <returns>Información de la sala incluyendo URL y token</returns>
        [HttpPost("room/{appointmentId:long}")]
        public async Task<IActionResult> CreateOrGetRoom(
            long appointmentId,
            [FromQuery] long doctorId,
            [FromQuery] long patientId)
        {
            try
            {
                if (doctorId <= 0 || patientId <= 0)
                {
                    return BadRequest(new { error = "doctorId y patientId son requeridos y deben ser mayores a 0" });
                }

                _logger.LogInformation("Creando/obteniendo sala para appointment {AppointmentId}, doctor {DoctorId}, patient {PatientId}", 
                    appointmentId, doctorId, patientId);

                var roomResponse = await _videoService.CreateOrGetRoomAsync(appointmentId, doctorId, patientId);

                return Ok(new
                {
                    roomUrl = roomResponse.RoomUrl,
                    roomName = roomResponse.RoomName,
                    token = roomResponse.Token
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error de configuración: {Message}", ex.Message);
                return StatusCode(500, new { error = "El servicio de videollamadas no está configurado correctamente. Por favor, contacta al administrador." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear/obtener sala para appointment {AppointmentId}", appointmentId);
                return StatusCode(500, new { error = $"Error al crear la sala de videollamada: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obtiene un token para unirse a una sala existente
        /// </summary>
        /// <param name="appointmentId">ID del appointment</param>
        /// <param name="userId">ID del usuario (doctor o paciente)</param>
        /// <param name="isOwner">Indica si el usuario es el dueño de la sala (doctor)</param>
        /// <returns>Token para unirse a la sala</returns>
        [HttpGet("token/{appointmentId:long}")]
        public async Task<IActionResult> GetToken(
            long appointmentId,
            [FromQuery] string userId,
            [FromQuery] bool isOwner = false)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new { error = "userId es requerido" });
                }

                var roomName = $"appointment-{appointmentId}";
                var token = await _videoService.GetTokenAsync(roomName, userId, isOwner);

                return Ok(new { token, roomName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener token para appointment {AppointmentId}, user {UserId}", appointmentId, userId);
                return StatusCode(500, new { error = $"Error al obtener token: {ex.Message}" });
            }
        }
    }
}


