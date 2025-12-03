using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace SchedulingMS.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class VideoController : ControllerBase
    {
        private readonly VideoService _videoService;

        public VideoController(VideoService videoService)
        {
            _videoService = videoService;
        }

        // POST: api/v1/Video/room/{appointmentId}
        [HttpPost("room/{appointmentId:long}")]
        public async Task<IActionResult> CreateOrGetRoom(
            long appointmentId,
            [FromQuery] long doctorId,
            [FromQuery] long patientId)
        {
            try
            {
                Console.WriteLine($"📹 [VideoController] Creando/obteniendo sala para appointment {appointmentId}, doctor {doctorId}, patient {patientId}");
                Console.WriteLine($"📹 [VideoController] Sala que se creará/obtendrá: appointment-{appointmentId}");
                var roomName = $"appointment-{appointmentId}";
                var room = await _videoService.CreateOrGetRoomAsync(roomName);

                if (room == null || string.IsNullOrEmpty(room.url))
                {
                    Console.WriteLine($"❌ Sala creada pero sin URL: {room?.name}");
                    return StatusCode(500, new { error = "No se pudo obtener la URL de la sala" });
                }

                Console.WriteLine($"✅ Sala creada/obtenida: {room.url}");
                return Ok(new
                {
                    roomUrl = room.url,
                    roomName = room.name,
                    roomId = room.id
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al crear/obtener sala: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/v1/Video/token/{appointmentId}
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
                    Console.WriteLine($"❌ userId no proporcionado para appointment {appointmentId}");
                    return BadRequest(new { error = "userId es requerido" });
                }

                Console.WriteLine($"📹 Creando token para appointment {appointmentId}, userId {userId}, isOwner {isOwner}");
                var roomName = $"appointment-{appointmentId}";
                var token = await _videoService.CreateTokenAsync(roomName, userId, isOwner);

                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine($"❌ Token creado pero vacío para appointment {appointmentId}");
                    return StatusCode(500, new { error = "No se pudo generar el token" });
                }

                Console.WriteLine($"✅ Token creado para appointment {appointmentId} (longitud: {token.Length})");
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al crear token: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/v1/Video/test
        [HttpGet("test")]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult Test()
        {
            Console.WriteLine($"═══════════════════════════════════════════════════════════");
            Console.WriteLine($"🧪 [VideoController] TEST ENDPOINT LLAMADO");
            Console.WriteLine($"═══════════════════════════════════════════════════════════");
            return Ok(new { message = "VideoController está funcionando", timestamp = DateTime.UtcNow });
        }

        // GET: api/v1/Video/check-doctor/{appointmentId}
        [HttpGet("check-doctor/{appointmentId:long}")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> CheckDoctorInRoom(long appointmentId)
        {
            try
            {
                Console.WriteLine($"═══════════════════════════════════════════════════════════");
                Console.WriteLine($"📹 [VideoController] ⚠️ REQUEST RECIBIDO ⚠️");
                Console.WriteLine($"📹 [VideoController] Verificando si hay doctor en la sala para appointment {appointmentId}");
                Console.WriteLine($"📹 [VideoController] Request recibido en: {Request.Path}");
                Console.WriteLine($"📹 [VideoController] Request Method: {Request.Method}");
                Console.WriteLine($"📹 [VideoController] Request QueryString: {Request.QueryString}");
                Console.WriteLine($"📹 [VideoController] Request Headers: {string.Join(", ", Request.Headers.Select(h => $"{h.Key}={h.Value}"))}");
                Console.WriteLine($"═══════════════════════════════════════════════════════════");
                
                if (_videoService == null)
                {
                    Console.WriteLine($"❌ [VideoController] VideoService es null!");
                    return StatusCode(500, new { error = "VideoService no está disponible", hasDoctor = false });
                }
                
                var roomName = $"appointment-{appointmentId}";
                var hasDoctor = await _videoService.HasDoctorInRoomAsync(roomName);

                Console.WriteLine($"✅ [VideoController] Verificación completada: doctor presente = {hasDoctor}");
                return Ok(new { hasDoctor });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [VideoController] Error al verificar doctor en sala: {ex.Message}");
                Console.WriteLine($"❌ [VideoController] Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { error = ex.Message, hasDoctor = false });
            }
        }
    }
}
