using Application.Interfaces.IVideoCall;
using Microsoft.AspNetCore.Mvc;

namespace SchedulingMS.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class VideoCallController: ControllerBase
    {
        private readonly IVideoCallService _videoService;
        private readonly ILogger<VideoCallController> _logger;

        public VideoCallController(IVideoCallService videoService, ILogger<VideoCallController> logger)
        {
            _videoService = videoService;
            _logger = logger;
        }

        [HttpPost("room/{appointmentId:long}")]
        public async Task<IActionResult> CreateOrGetRoom(long appointmentId,[FromQuery] long doctorId,[FromQuery] long patientId, [FromQuery] string? userName = null,[FromQuery] string? userType = null)
        {
            try
            {
                if (doctorId <= 0 || patientId <= 0)
                {
                    return BadRequest(new { error = "doctorId y patientId son requeridos y deben ser mayores a 0" });
                }

                _logger.LogInformation(
                    "Creando/obteniendo sala para appointment {AppointmentId}, doctor {DoctorId}, patient {PatientId}, user {UserName} ({UserType})",
                    appointmentId, doctorId, patientId, userName ?? "sin nombre", userType ?? "sin tipo");

                var roomResponse = await _videoService.CreateOrGetRoomAsync(appointmentId, doctorId, patientId, userName,userType);

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

        [HttpGet("token/{appointmentId:long}")]
        public async Task<IActionResult> GetToken(long appointmentId,[FromQuery] string userId,[FromQuery] bool isOwner = false)
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

