using Application.DTOs;
using Application.Interfaces.IVideoCall;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Application.Services
{
    public class VideoCallService : IVideoCallService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<VideoCallService> _logger;
        private readonly string _apiKey;
        private readonly string _apiUrl;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public VideoCallService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<VideoCallService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _logger = logger;
            _apiKey = configuration["Daily:ApiKey"] ?? throw new InvalidOperationException("Daily:ApiKey no está configurado en appsettings.json");
            _apiUrl = configuration["Daily:ApiUrl"] ?? "https://api.daily.co";

            // Configurar headers para Daily.co API
            _httpClient.BaseAddress = new Uri(_apiUrl);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<VideoCallRoomResponse> CreateOrGetRoomAsync(
            long appointmentId,
            long doctorId,
            long patientId,
            string? userName = null,
            string? userType = null)
        {
            try
            {
                var roomName = $"appointment-{appointmentId}";

                // ✅ Determinar el nombre de usuario y si es owner
                string participantName;
                bool isOwner;

                if (!string.IsNullOrEmpty(userName))
                {
                    participantName = userName;
                }
                else
                {
                    participantName = userType?.ToLower() == "patient"
                        ? $"Paciente-{patientId}"
                        : $"Doctor-{doctorId}";
                }

                isOwner = userType?.ToLower() == "doctor";

                _logger.LogInformation(
                    "Procesando sala {RoomName} para {ParticipantName} (tipo: {UserType}, owner: {IsOwner})",
                    roomName, participantName, userType ?? "desconocido", isOwner);

                // Intentar obtener la sala existente
                var existingRoom = await GetRoomAsync(roomName);

                if (existingRoom != null)
                {
                    _logger.LogInformation("Sala existente encontrada: {RoomName}", roomName);

                    // ✅ Generar token con el nombre correcto
                    var token = await GetTokenAsync(roomName, participantName, isOwner);

                    // 🔔 Si es el doctor uniéndose, notificar al paciente
                    if (userType?.ToLower() == "doctor")
                    {
                        await NotifyPatientDoctorJoined(appointmentId, doctorId, patientId, userName);
                    }

                    return new VideoCallRoomResponse
                    {
                        RoomUrl = existingRoom.Url,
                        RoomName = roomName,
                        Token = token
                    };
                }

                // Crear nueva sala
                var createRoomRequest = new
                {
                    name = roomName,
                    privacy = "private",
                    properties = new
                    {
                        exp = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds(),
                        enable_chat = true,
                        enable_screenshare = true,
                        enable_recording = false
                    }
                };

                _logger.LogInformation("Creando nueva sala: {RoomName}", roomName);

                var response = await _httpClient.PostAsJsonAsync("/v1/rooms", createRoomRequest);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error al crear sala en Daily.co: {StatusCode} - {Error}", response.StatusCode, errorContent);

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new Exception("API Key de Daily.co inválida. Por favor, verifica la configuración.");
                    }

                    throw new Exception($"Error al crear sala en Daily.co: {response.StatusCode} - {errorContent}");
                }

                var room = await response.Content.ReadFromJsonAsync<DailyRoomResponse>();

                if (room == null || string.IsNullOrEmpty(room.Url))
                {
                    throw new Exception("No se recibió una respuesta válida de Daily.co");
                }

                var newToken = await GetTokenAsync(roomName, participantName, isOwner);

                // 🔔 Si es el doctor creando la sala, notificar al paciente
                if (userType?.ToLower() == "doctor")
                {
                    await NotifyPatientDoctorJoined(appointmentId, doctorId, patientId, userName);
                }

                _logger.LogInformation("Sala creada exitosamente: {RoomName}", roomName);

                return new VideoCallRoomResponse
                {
                    RoomUrl = room.Url,
                    RoomName = roomName,
                    Token = newToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en CreateOrGetRoomAsync para appointment {AppointmentId}", appointmentId);
                throw;
            }
        }

        //  NUEVO: Enviar notificación al paciente cuando el doctor se une
        private async Task NotifyPatientDoctorJoined(long appointmentId, long doctorId, long patientId, string? doctorName)
        {
            try
            {
                var authApiUrl = _configuration["Services:AuthMS"] ?? "http://localhost:8082/api/v1";
                var directoryApiUrl = _configuration["Services:DirectoryMS"] ?? "http://localhost:8081/api";

                using var authClient = _httpClientFactory.CreateClient();
                using var directoryClient = _httpClientFactory.CreateClient();

                // 1️⃣ Obtener el userId del paciente desde DirectoryMS
                var patientResponse = await directoryClient.GetAsync($"{directoryApiUrl}/v1/Patient/{patientId}");

                if (!patientResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("No se pudo obtener información del paciente {PatientId}", patientId);
                    return;
                }

                var patient = await patientResponse.Content.ReadFromJsonAsync<PatientResponse>();

                if (patient == null || patient.UserId == 0)
                {
                    _logger.LogWarning("No se encontró userId para el paciente {PatientId}", patientId);
                    return;
                }

                // 2️⃣ Crear notificación
                var notificationPayload = new
                {
                    userId = patient.UserId.ToString(),
                    eventType = "DoctorJoinedVideoCall",
                    payload = new
                    {
                        appointmentId = appointmentId,
                        doctorId = doctorId,
                        doctorName = doctorName ?? "El doctor",
                        message = $"{doctorName ?? "El doctor"} se ha unido a la videollamada"
                    }
                };

                var notificationResponse = await authClient.PostAsJsonAsync(
                    $"{authApiUrl}/notifications/events",
                    notificationPayload);

                if (notificationResponse.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Notificación enviada exitosamente al paciente");
                }
                else
                {
                    var errorContent = await notificationResponse.Content.ReadAsStringAsync();
                    _logger.LogWarning("Error al enviar notificación: {Error}", errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar notificación al paciente");
                // No lanzar excepción para no interrumpir el flujo de la videollamada
            }
        }

        public async Task<string> GetTokenAsync(string roomName, string userId, bool isOwner)
        {
            try
            {
                var tokenRequest = new
                {
                    properties = new
                    {
                        room_name = roomName,
                        user_name = userId,
                        is_owner = isOwner,
                        exp = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds()
                    }
                };

                _logger.LogInformation("Generando token para sala {RoomName}, usuario {UserId}, owner: {IsOwner}",
                    roomName, userId, isOwner);

                var response = await _httpClient.PostAsJsonAsync("/v1/meeting-tokens", tokenRequest);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error al generar token: {StatusCode} - {Error}", response.StatusCode, errorContent);
                    throw new Exception($"Error al generar token: {response.StatusCode}");
                }

                var tokenResponse = await response.Content.ReadFromJsonAsync<DailyTokenResponse>();

                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.Token))
                {
                    throw new Exception("No se recibió un token válido de Daily.co");
                }

                return tokenResponse.Token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar token para {RoomName}", roomName);
                throw;
            }
        }

        private async Task<DailyRoomResponse?> GetRoomAsync(string roomName)
        {
            try
            {
                _logger.LogInformation("Verificando si existe la sala: {RoomName}", roomName);

                var response = await _httpClient.GetAsync($"/v1/rooms/{roomName}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("Sala no encontrada: {RoomName}", roomName);
                    return null;
                }

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Error al obtener sala {RoomName}: {StatusCode} - {Error}",
                        roomName, response.StatusCode, errorContent);
                    return null;
                }

                var room = await response.Content.ReadFromJsonAsync<DailyRoomResponse>();
                return room;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al verificar sala existente {RoomName}", roomName);
                return null;
            }
        }
    }

    internal class DailyTokenResponse
    {
        public string Token { get; set; } = string.Empty;
    }
    internal class PatientResponse
    {
        public long UserId { get; set; }
    }
}