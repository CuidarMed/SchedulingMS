using Application.Interfaces.IVideo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Application.Services
{
    public class VideoService : IVideoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<VideoService> _logger;
        private readonly string _apiKey;
        private readonly string _apiUrl;

        public VideoService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<VideoService> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            
            _apiKey = configuration["Daily:ApiKey"] ?? throw new InvalidOperationException("Daily:ApiKey no está configurado en appsettings.json");
            _apiUrl = configuration["Daily:ApiUrl"] ?? "https://api.daily.co";
            
            // Configurar headers para Daily.co API
            _httpClient.BaseAddress = new Uri(_apiUrl);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            // Content-Type se agrega automáticamente por PostAsJsonAsync
        }

        public async Task<VideoRoomResponse> CreateOrGetRoomAsync(long appointmentId, long doctorId, long patientId)
        {
            try
            {
                var roomName = $"appointment-{appointmentId}";
                
                // Intentar obtener la sala existente
                var existingRoom = await GetRoomAsync(roomName);
                
                if (existingRoom != null)
                {
                    _logger.LogInformation("Sala existente encontrada: {RoomName}", roomName);
                    
                    // Generar token para el doctor (owner)
                    var token = await GetTokenAsync(roomName, $"doctor-{doctorId}", true);
                    
                    return new VideoRoomResponse
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
                        exp = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds(), // Expira en 24 horas
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
                    _logger.LogError("Error al crear sala en Daily.co: {StatusCode} - {Error}. URL: {Url}", response.StatusCode, errorContent, $"{_apiUrl}/v1/rooms");
                    
                    // Si es 401, probablemente la API Key es inválida
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
                
                // Generar token para el doctor (owner)
                var doctorToken = await GetTokenAsync(roomName, $"doctor-{doctorId}", true);
                
                _logger.LogInformation("Sala creada exitosamente: {RoomName}", roomName);
                
                return new VideoRoomResponse
                {
                    RoomUrl = room.Url,
                    RoomName = roomName,
                    Token = doctorToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en CreateOrGetRoomAsync para appointment {AppointmentId}", appointmentId);
                throw;
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
                        user_id = userId,
                        is_owner = isOwner,
                        exp = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds() // Expira en 24 horas
                    }
                };
                
                var response = await _httpClient.PostAsJsonAsync("/v1/meeting-tokens", tokenRequest);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error al generar token en Daily.co: {StatusCode} - {Error}", response.StatusCode, errorContent);
                    
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new Exception("API Key de Daily.co inválida. Por favor, verifica la configuración.");
                    }
                    
                    throw new Exception($"Error al generar token en Daily.co: {response.StatusCode} - {errorContent}");
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
                _logger.LogError(ex, "Error en GetTokenAsync para room {RoomName}, user {UserId}", roomName, userId);
                throw;
            }
        }

        private async Task<DailyRoomResponse?> GetRoomAsync(string roomName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/v1/rooms/{roomName}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Error al obtener sala de Daily.co: {StatusCode} - {Error}", response.StatusCode, errorContent);
                    return null;
                }
                
                return await response.Content.ReadFromJsonAsync<DailyRoomResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al obtener sala {RoomName} de Daily.co", roomName);
                return null;
            }
        }

        private class DailyRoomResponse
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
            
            [JsonPropertyName("url")]
            public string Url { get; set; } = string.Empty;
            
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;
        }

        private class DailyTokenResponse
        {
            [JsonPropertyName("token")]
            public string Token { get; set; } = string.Empty;
        }
    }
}

