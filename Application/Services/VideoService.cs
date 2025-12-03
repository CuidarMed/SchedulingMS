using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class VideoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://api.daily.co/v1";

        public VideoService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Daily:ApiKey"] ?? throw new InvalidOperationException("Daily:ApiKey no configurada");
            Console.WriteLine($"🔑 Daily API Key configurada: {_apiKey.Substring(0, 10)}...");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<VideoRoomResponse> CreateOrGetRoomAsync(string roomName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/rooms/{roomName}");
                Console.WriteLine($"[Daily] Get room status: {(int)response.StatusCode}");
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[Daily] Get room body: {body}");
                    var existingRoom = await response.Content.ReadFromJsonAsync<VideoRoomResponse>() ?? throw new Exception("Error al leer respuesta");
                    
                    // Si la sala existe, actualizarla para asegurar que permita tokens
                    await UpdateRoomConfigAsync(roomName);
                    
                    return existingRoom;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Daily] Get room error: {ex.Message}");
            }
            
            // Daily.co API: estructura mínima para crear una sala
            // Solo usar parámetros que Daily.co acepta oficialmente
            var createRequest = new { 
                name = roomName, 
                privacy = "public"
            };
            var createResponse = await _httpClient.PostAsJsonAsync($"{_baseUrl}/rooms", createRequest);
            Console.WriteLine($"[Daily] Create room status: {(int)createResponse.StatusCode}");
            
            if (!createResponse.IsSuccessStatusCode)
            {
                var errorContent = await createResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"[Daily] Create room error body: {errorContent}");
                throw new Exception($"Daily.co error: {createResponse.StatusCode} - {errorContent}");
            }
            
            var bodyContent = await createResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"[Daily] Create room body: {bodyContent}");
            
            var room = await createResponse.Content.ReadFromJsonAsync<VideoRoomResponse>() ?? throw new Exception("Error al crear sala");
            if (string.IsNullOrEmpty(room.url)) throw new Exception("Daily.co no devolvió URL de la sala");
            return room;
        }
        
        private async Task UpdateRoomConfigAsync(string roomName)
        {
            try
            {
                // Cambiar la sala a "public" para permitir tokens
                // Usar estructura mínima
                var updateRequest = new { 
                    privacy = "public"
                };
                var updateResponse = await _httpClient.PostAsJsonAsync($"{_baseUrl}/rooms/{roomName}", updateRequest);
                Console.WriteLine($"[Daily] Update room status: {(int)updateResponse.StatusCode}");
                if (updateResponse.IsSuccessStatusCode)
                {
                    var body = await updateResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"[Daily] Update room body: {body}");
                    Console.WriteLine($"[Daily] ✅ Sala {roomName} actualizada a 'public' para permitir tokens");
                }
                else
                {
                    var errorBody = await updateResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"[Daily] ⚠️ Error al actualizar sala: {errorBody}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Daily] Update room error: {ex.Message}");
            }
        }

        public async Task<string> CreateTokenAsync(string roomName, string userId, bool isOwner)
        {
            var request = new { properties = new { room_name = roomName, user_id = userId, is_owner = isOwner } };
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/meeting-tokens", request);
            Console.WriteLine($"[Daily] Token request - isOwner={isOwner}, userId={userId}, roomName={roomName}");
            Console.WriteLine($"[Daily] Token response status: {(int)response.StatusCode}");
            
            response.EnsureSuccessStatusCode();
            
            var bodyContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Daily] Token response body: {bodyContent}");
            
            var result = await response.Content.ReadFromJsonAsync<VideoTokenResponse>() ?? throw new Exception("Error al crear token");
            Console.WriteLine($"[Daily] Token isOwner={isOwner}, len={result.token?.Length ?? 0}");
            return result.token;
        }

        public async Task<bool> HasActiveParticipantsAsync(string roomName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/rooms/{roomName}");
                if (!response.IsSuccessStatusCode) return false;
                var room = await response.Content.ReadFromJsonAsync<VideoRoomResponse>();
                return room != null && !string.IsNullOrEmpty(room.url);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica si hay un doctor (participante con userId que empieza con "doctor-") en la sala
        /// usando la API de Daily.co para obtener sesiones activas
        /// </summary>
        public async Task<bool> HasDoctorInRoomAsync(string roomName)
        {
            try
            {
                // SOLUCIÓN ALTERNATIVA: Como el endpoint /sessions no está disponible en la API pública,
                // y el endpoint /presence tiene un delay, usamos una estrategia diferente:
                // 1. Verificar si la sala existe y está activa
                // 2. Si la sala existe, asumir que si hay actividad reciente, hay un doctor
                // 3. Usar el endpoint /presence con más intentos y mayor tiempo de espera
                
                Console.WriteLine($"[Daily] ═══════════════════════════════════════════════════════════");
                Console.WriteLine($"[Daily] Verificando sala {roomName}...");
                
                // Primero verificar que la sala existe
                var roomResponse = await _httpClient.GetAsync($"{_baseUrl}/rooms/{roomName}");
                Console.WriteLine($"[Daily] Get room status: {(int)roomResponse.StatusCode}");
                
                if (!roomResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[Daily] ⚠️ La sala {roomName} no existe");
                    return false;
                }
                
                var roomBody = await roomResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"[Daily] Room body: {roomBody}");
                
                // Daily.co API: Intentar múltiples métodos para detectar participantes
                // Método 1: Usar /presence con más intentos y mayor tiempo de espera (ya que tiene delay)
                Console.WriteLine($"[Daily] MÉTODO 1: Intentando obtener presence para sala {roomName} (con más intentos)...");
                
                // Intentar diferentes formatos de URL para /sessions
                string[] sessionsUrls = new[]
                {
                    $"{_baseUrl}/sessions?room_name={roomName}&active=true",
                    $"{_baseUrl}/sessions?room={roomName}&active=true",
                    $"{_baseUrl}/sessions?room_name={roomName}",
                    $"{_baseUrl}/sessions?room={roomName}",
                    $"{_baseUrl}/sessions"
                };
                
                bool sessionsSuccess = false;
                foreach (var sessionsUrl in sessionsUrls)
                {
                    try
                    {
                        Console.WriteLine($"[Daily] Intentando URL: {sessionsUrl}");
                        var sessionsResponse = await _httpClient.GetAsync(sessionsUrl);
                        Console.WriteLine($"[Daily] Get sessions status: {(int)sessionsResponse.StatusCode}");
                    
                    if (sessionsResponse.IsSuccessStatusCode)
                    {
                        var sessionsBody = await sessionsResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"[Daily] Sessions body: {sessionsBody}");
                        
                        var sessionsData = await sessionsResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
                        
                        // Verificar si hay sesiones activas
                        if (sessionsData.TryGetProperty("data", out var sessionsArray) && sessionsArray.ValueKind == System.Text.Json.JsonValueKind.Array)
                        {
                            Console.WriteLine($"[Daily] Sesiones activas encontradas: {sessionsArray.GetArrayLength()}");
                            foreach (var session in sessionsArray.EnumerateArray())
                            {
                                // Obtener el session_id para consultar participantes
                                string? sessionId = null;
                                if (session.TryGetProperty("id", out var id))
                                {
                                    sessionId = id.GetString();
                                }
                                else if (session.TryGetProperty("session_id", out var sessionIdProp))
                                {
                                    sessionId = sessionIdProp.GetString();
                                }
                                
                                Console.WriteLine($"[Daily] Sesión encontrada: sessionId={sessionId}");
                                
                                if (!string.IsNullOrEmpty(sessionId))
                                {
                                    // Consultar participantes de la sesión usando el endpoint específico
                                    Console.WriteLine($"[Daily] Consultando participantes de sesión {sessionId}...");
                                    var sessionParticipantsResponse = await _httpClient.GetAsync($"{_baseUrl}/sessions/{sessionId}/participants");
                                    Console.WriteLine($"[Daily] Get session participants status: {(int)sessionParticipantsResponse.StatusCode}");
                                    
                                    if (sessionParticipantsResponse.IsSuccessStatusCode)
                                    {
                                        var participantsBody = await sessionParticipantsResponse.Content.ReadAsStringAsync();
                                        Console.WriteLine($"[Daily] Session participants body: {participantsBody}");
                                        
                                        var participantsData = await sessionParticipantsResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
                                        
                                        System.Text.Json.JsonElement? participantsArray = null;
                                        if (participantsData.TryGetProperty("data", out var dataArray) && dataArray.ValueKind == System.Text.Json.JsonValueKind.Array)
                                        {
                                            participantsArray = dataArray;
                                        }
                                        else if (participantsData.TryGetProperty("participants", out var participantsProp) && participantsProp.ValueKind == System.Text.Json.JsonValueKind.Array)
                                        {
                                            participantsArray = participantsProp;
                                        }
                                        
                                        if (participantsArray.HasValue)
                                        {
                                            Console.WriteLine($"[Daily] Participantes encontrados en sesión {sessionId}: {participantsArray.Value.GetArrayLength()}");
                                            foreach (var participant in participantsArray.Value.EnumerateArray())
                                            {
                                                string? userNameStr = null;
                                                string? userIdStr = null;
                                                
                                                if (participant.TryGetProperty("userName", out var userNameCamel))
                                                {
                                                    userNameStr = userNameCamel.GetString();
                                                }
                                                if (participant.TryGetProperty("userId", out var userIdCamel))
                                                {
                                                    userIdStr = userIdCamel.GetString();
                                                }
                                                
                                                if (string.IsNullOrEmpty(userNameStr) && participant.TryGetProperty("user_name", out var userNameSnake))
                                                {
                                                    userNameStr = userNameSnake.GetString();
                                                }
                                                if (string.IsNullOrEmpty(userIdStr) && participant.TryGetProperty("user_id", out var userIdSnake))
                                                {
                                                    userIdStr = userIdSnake.GetString();
                                                }
                                                
                                                Console.WriteLine($"[Daily] Participante encontrado: user_name={userNameStr}, user_id={userIdStr}");
                                                
                                                if ((!string.IsNullOrEmpty(userNameStr) && userNameStr.StartsWith("doctor-", StringComparison.OrdinalIgnoreCase)) ||
                                                    (!string.IsNullOrEmpty(userIdStr) && userIdStr.StartsWith("doctor-", StringComparison.OrdinalIgnoreCase)))
                                                {
                                                    Console.WriteLine($"[Daily] ✅✅✅ DOCTOR ENCONTRADO EN LA SALA USANDO /sessions/{sessionId}/participants! ✅✅✅");
                                                    return true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine($"[Daily] ⚠️ No se encontraron participantes en el formato esperado para sesión {sessionId}");
                                        }
                                    }
                                    else
                                    {
                                        var errorContent = await sessionParticipantsResponse.Content.ReadAsStringAsync();
                                        Console.WriteLine($"[Daily] Get session participants error ({sessionParticipantsResponse.StatusCode}): {errorContent}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"[Daily] ⚠️ Sesión sin sessionId válido");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[Daily] ⚠️ No se encontraron sesiones activas o formato de respuesta inesperado");
                        }
                    }
                    else
                    {
                        var errorContent = await sessionsResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"[Daily] Get sessions error ({sessionsResponse.StatusCode}): {errorContent}");
                    }
                        if (sessionsResponse.IsSuccessStatusCode)
                        {
                            sessionsSuccess = true;
                            break; // Salir del loop si encontramos una URL que funciona
                        }
                        else
                        {
                            var errorContent = await sessionsResponse.Content.ReadAsStringAsync();
                            Console.WriteLine($"[Daily] Get sessions error ({sessionsResponse.StatusCode}): {errorContent}");
                        }
                    }
                    catch (Exception urlEx)
                    {
                        Console.WriteLine($"[Daily] Error al intentar URL {sessionsUrl}: {urlEx.Message}");
                        continue; // Intentar siguiente URL
                    }
                }
                
                if (!sessionsSuccess)
                {
                    Console.WriteLine($"[Daily] ⚠️ Ninguna URL de /sessions funcionó. El endpoint puede no estar disponible en esta versión de Daily.co API.");
                }
                
                // Método 2: Intentar con /meetings/{roomName} que es el endpoint estándar
                Console.WriteLine($"[Daily] Intentando obtener información de meeting para sala {roomName}...");
                
                // Método 1: Intentar con /meetings/{roomName} (este endpoint reporta participantes activos)
                try
                {
                    var meetingResponse = await _httpClient.GetAsync($"{_baseUrl}/meetings/{roomName}");
                    Console.WriteLine($"[Daily] Get meeting status: {(int)meetingResponse.StatusCode}");
                    
                    if (meetingResponse.IsSuccessStatusCode)
                    {
                        var meetingBody = await meetingResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"[Daily] Meeting body: {meetingBody}");
                        
                        // Parsear la respuesta para ver si hay participantes
                        var meetingData = await meetingResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
                        
                        // Verificar si tiene "participants" (formato antiguo) o "data" (formato nuevo)
                        System.Text.Json.JsonElement? participantsArray = null;
                        if (meetingData.TryGetProperty("participants", out var participantsOld))
                        {
                            participantsArray = participantsOld;
                        }
                        else if (meetingData.TryGetProperty("data", out var dataArray) && dataArray.ValueKind == System.Text.Json.JsonValueKind.Object)
                        {
                            if (dataArray.TryGetProperty("participants", out var participantsNew))
                            {
                                participantsArray = participantsNew;
                            }
                        }
                        
                        if (participantsArray.HasValue && participantsArray.Value.ValueKind == System.Text.Json.JsonValueKind.Array)
                        {
                            foreach (var participant in participantsArray.Value.EnumerateArray())
                            {
                                string? userNameStr = null;
                                string? userIdStr = null;
                                
                                // Intentar con camelCase primero
                                if (participant.TryGetProperty("userName", out var userNameCamel))
                                {
                                    userNameStr = userNameCamel.GetString();
                                }
                                if (participant.TryGetProperty("userId", out var userIdCamel))
                                {
                                    userIdStr = userIdCamel.GetString();
                                }
                                
                                // Fallback a snake_case
                                if (string.IsNullOrEmpty(userNameStr) && participant.TryGetProperty("user_name", out var userNameSnake))
                                {
                                    userNameStr = userNameSnake.GetString();
                                }
                                if (string.IsNullOrEmpty(userIdStr) && participant.TryGetProperty("user_id", out var userIdSnake))
                                {
                                    userIdStr = userIdSnake.GetString();
                                }
                                
                                Console.WriteLine($"[Daily] Participante encontrado en /meetings: user_name={userNameStr}, user_id={userIdStr}");
                                
                                if ((!string.IsNullOrEmpty(userNameStr) && userNameStr.StartsWith("doctor-", StringComparison.OrdinalIgnoreCase)) ||
                                    (!string.IsNullOrEmpty(userIdStr) && userIdStr.StartsWith("doctor-", StringComparison.OrdinalIgnoreCase)))
                                {
                                    Console.WriteLine($"[Daily] ✅ Doctor encontrado en la sala usando /meetings!");
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[Daily] ⚠️ El endpoint /meetings no devolvió participantes en formato esperado");
                        }
                    }
                    else
                    {
                        var errorContent = await meetingResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"[Daily] Get meeting error ({meetingResponse.StatusCode}): {errorContent}");
                    }
                }
                catch (Exception meetingEx)
                {
                    Console.WriteLine($"[Daily] Error al obtener meeting: {meetingEx.Message}");
                    Console.WriteLine($"[Daily] Stack trace: {meetingEx.StackTrace}");
                }
                
                // Método 2: Intentar con /sessions para obtener sesiones activas (más confiable que /presence)
                Console.WriteLine($"[Daily] Intentando obtener sesiones activas para sala {roomName}...");
                try
                {
                    var sessionsResponse = await _httpClient.GetAsync($"{_baseUrl}/sessions?room_name={roomName}&active=true");
                    Console.WriteLine($"[Daily] Get sessions status: {(int)sessionsResponse.StatusCode}");
                    
                    if (sessionsResponse.IsSuccessStatusCode)
                    {
                        var sessionsBody = await sessionsResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"[Daily] Sessions body: {sessionsBody}");
                        
                        var sessionsData = await sessionsResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
                        
                        // Verificar si hay sesiones activas
                        if (sessionsData.TryGetProperty("data", out var sessionsArray) && sessionsArray.ValueKind == System.Text.Json.JsonValueKind.Array)
                        {
                            foreach (var session in sessionsArray.EnumerateArray())
                            {
                                // Obtener el session_id para consultar participantes
                                string? sessionId = null;
                                if (session.TryGetProperty("id", out var id))
                                {
                                    sessionId = id.GetString();
                                }
                                else if (session.TryGetProperty("session_id", out var sessionIdProp))
                                {
                                    sessionId = sessionIdProp.GetString();
                                }
                                
                                if (!string.IsNullOrEmpty(sessionId))
                                {
                                    Console.WriteLine($"[Daily] Sesión activa encontrada: {sessionId}");
                                    
                                    // Consultar participantes de la sesión
                                    var sessionParticipantsResponse = await _httpClient.GetAsync($"{_baseUrl}/sessions/{sessionId}/participants");
                                    Console.WriteLine($"[Daily] Get session participants status: {(int)sessionParticipantsResponse.StatusCode}");
                                    
                                    if (sessionParticipantsResponse.IsSuccessStatusCode)
                                    {
                                        var participantsBody = await sessionParticipantsResponse.Content.ReadAsStringAsync();
                                        Console.WriteLine($"[Daily] Session participants body: {participantsBody}");
                                        
                                        var participantsData = await sessionParticipantsResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
                                        
                                        if (participantsData.TryGetProperty("data", out var participantsArray) && participantsArray.ValueKind == System.Text.Json.JsonValueKind.Array)
                                        {
                                            foreach (var participant in participantsArray.EnumerateArray())
                                            {
                                                string? userNameStr = null;
                                                string? userIdStr = null;
                                                
                                                if (participant.TryGetProperty("userName", out var userNameCamel))
                                                {
                                                    userNameStr = userNameCamel.GetString();
                                                }
                                                if (participant.TryGetProperty("userId", out var userIdCamel))
                                                {
                                                    userIdStr = userIdCamel.GetString();
                                                }
                                                
                                                if (string.IsNullOrEmpty(userNameStr) && participant.TryGetProperty("user_name", out var userNameSnake))
                                                {
                                                    userNameStr = userNameSnake.GetString();
                                                }
                                                if (string.IsNullOrEmpty(userIdStr) && participant.TryGetProperty("user_id", out var userIdSnake))
                                                {
                                                    userIdStr = userIdSnake.GetString();
                                                }
                                                
                                                Console.WriteLine($"[Daily] Participante encontrado en /sessions: user_name={userNameStr}, user_id={userIdStr}");
                                                
                                                if ((!string.IsNullOrEmpty(userNameStr) && userNameStr.StartsWith("doctor-", StringComparison.OrdinalIgnoreCase)) ||
                                                    (!string.IsNullOrEmpty(userIdStr) && userIdStr.StartsWith("doctor-", StringComparison.OrdinalIgnoreCase)))
                                                {
                                                    Console.WriteLine($"[Daily] ✅ Doctor encontrado en la sala usando /sessions!");
                                                    return true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var errorContent = await sessionsResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"[Daily] Get sessions error ({sessionsResponse.StatusCode}): {errorContent}");
                    }
                }
                catch (Exception sessionsEx)
                {
                    Console.WriteLine($"[Daily] Error al obtener sessions: {sessionsEx.Message}");
                    Console.WriteLine($"[Daily] Stack trace: {sessionsEx.StackTrace}");
                }
                
                // Método 3: Intentar con /rooms/{roomName}/presence como último fallback
                // El endpoint /presence puede tener un delay, intentar hasta 3 veces con espera
                Console.WriteLine($"[Daily] Intentando obtener presence para sala {roomName}...");
                
                for (int attempt = 1; attempt <= 3; attempt++)
                {
                    if (attempt > 1)
                    {
                        // Espera progresiva: 3, 5, 7, 10 segundos
                        int waitSeconds = attempt == 2 ? 3 : attempt == 3 ? 5 : attempt == 4 ? 7 : 10;
                        Console.WriteLine($"[Daily] Intento {attempt}/5 - Esperando {waitSeconds} segundos antes de reintentar...");
                        await Task.Delay(waitSeconds * 1000);
                    }
                    
                    var response = await _httpClient.GetAsync($"{_baseUrl}/rooms/{roomName}/presence");
                    Console.WriteLine($"[Daily] Get room presence status (intento {attempt}): {(int)response.StatusCode}");
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"[Daily] Get room presence error ({response.StatusCode}): {errorContent}");
                        
                        // Si el endpoint no existe (404), el método no está disponible
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            Console.WriteLine($"[Daily] ⚠️ El endpoint /presence no está disponible en esta versión de Daily.co API");
                            Console.WriteLine($"[Daily] ⚠️ No se puede verificar participantes desde el backend");
                            return false;
                        }
                        
                        // Si no es el último intento, continuar
                        if (attempt < 3) continue;
                        return false;
                    }
                    
                    var bodyContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[Daily] Room presence body (intento {attempt}): {bodyContent}");
                    
                    // Daily.co API devuelve: {"total_count":0,"data":[]} o {"total_count":N,"data":[{...}]}
                    // Parsear la respuesta JSON usando JsonElement para mayor flexibilidad
                    var jsonDoc = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
                    
                    if (jsonDoc.ValueKind == System.Text.Json.JsonValueKind.Null)
                    {
                        Console.WriteLine($"[Daily] No se pudo parsear la respuesta de presence (intento {attempt})");
                        if (attempt < 3) continue;
                        return false;
                    }
                
                // Verificar si tiene la propiedad "data" (formato nuevo de Daily.co)
                // Daily.co puede devolver userId/userName (camelCase) o user_id/user_name (snake_case)
                List<DailyParticipant> participants = new List<DailyParticipant>();
                
                if (jsonDoc.TryGetProperty("data", out var dataArray) && dataArray.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    foreach (var item in dataArray.EnumerateArray())
                    {
                        var participant = new DailyParticipant();
                        
                        // Intentar con camelCase primero (formato actual de Daily.co)
                        if (item.TryGetProperty("userName", out var userNameCamel))
                        {
                            participant.user_name = userNameCamel.GetString();
                        }
                        if (item.TryGetProperty("userId", out var userIdCamel))
                        {
                            participant.user_id = userIdCamel.GetString();
                        }
                        
                        // Fallback a snake_case (formato antiguo)
                        if (string.IsNullOrEmpty(participant.user_name) && item.TryGetProperty("user_name", out var userNameSnake))
                        {
                            participant.user_name = userNameSnake.GetString();
                        }
                        if (string.IsNullOrEmpty(participant.user_id) && item.TryGetProperty("user_id", out var userIdSnake))
                        {
                            participant.user_id = userIdSnake.GetString();
                        }
                        
                        if (item.TryGetProperty("session_id", out var sessionId))
                        {
                            participant.session_id = sessionId.GetString();
                        }
                        if (item.TryGetProperty("id", out var id))
                        {
                            participant.session_id = id.GetString();
                        }
                        if (item.TryGetProperty("owner", out var owner))
                        {
                            participant.owner = owner.GetBoolean();
                        }
                        participants.Add(participant);
                    }
                }
                // Fallback: intentar con "participants" (formato antiguo)
                else if (jsonDoc.TryGetProperty("participants", out var participantsArray) && participantsArray.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    var presenceData = await response.Content.ReadFromJsonAsync<DailyPresenceResponse>();
                    participants = presenceData?.participants ?? new List<DailyParticipant>();
                    }
                    
                    Console.WriteLine($"[Daily] Participantes encontrados (intento {attempt}): {participants.Count}");
                    Console.WriteLine($"[Daily] Respuesta completa de Daily.co: {bodyContent}");
                    
                    if (participants.Count == 0)
                    {
                        Console.WriteLine($"[Daily] ⚠️ No hay participantes en la sala {roomName} (intento {attempt})");
                        if (attempt < 5)
                        {
                            int waitSeconds = attempt == 1 ? 3 : attempt == 2 ? 5 : attempt == 3 ? 7 : 10;
                            Console.WriteLine($"[Daily] ⚠️ Reintentando en {waitSeconds} segundos... (puede haber un delay en Daily.co)");
                            continue; // Reintentar
                        }
                        else
                        {
                            Console.WriteLine($"[Daily] ⚠️ No se encontraron participantes después de 5 intentos");
                            Console.WriteLine($"[Daily] ⚠️ Esto puede significar:");
                            Console.WriteLine($"[Daily] ⚠️   1. El doctor aún no se ha unido");
                            Console.WriteLine($"[Daily] ⚠️   2. El endpoint /presence tiene un delay mayor a 25 segundos");
                            Console.WriteLine($"[Daily] ⚠️   3. El doctor se unió a una sala diferente");
                            Console.WriteLine($"[Daily] ⚠️ NOTA: El dashboard de Daily.co muestra sesiones activas, pero el endpoint /presence no las reporta.");
                            return false;
                        }
                    }
                    
                    // Si hay participantes, procesarlos
                    foreach (var p in participants)
                    {
                        Console.WriteLine($"[Daily] - Participante: user_name={p.user_name ?? "null"}, user_id={p.user_id ?? "null"}, owner={p.owner}");
                        Console.WriteLine($"[Daily]   Verificando si es doctor: user_name empieza con 'doctor-' = {(!string.IsNullOrEmpty(p.user_name) && p.user_name.StartsWith("doctor-", StringComparison.OrdinalIgnoreCase))}");
                        Console.WriteLine($"[Daily]   Verificando si es doctor: user_id empieza con 'doctor-' = {(!string.IsNullOrEmpty(p.user_id) && p.user_id.StartsWith("doctor-", StringComparison.OrdinalIgnoreCase))}");
                    }
                    
                    // Verificar si hay participantes con userId o user_name que empiece con "doctor-"
                    // Daily.co puede usar user_id o user_name dependiendo de cómo se creó el token
                    var hasDoctor = participants.Any(p => 
                        (!string.IsNullOrEmpty(p.user_name) && p.user_name.StartsWith("doctor-", StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(p.user_id) && p.user_id.StartsWith("doctor-", StringComparison.OrdinalIgnoreCase))
                    );
                    
                    Console.WriteLine($"[Daily] Doctor en sala {roomName}: {hasDoctor}");
                    if (hasDoctor)
                    {
                        var doctorParticipants = participants.Where(p => 
                            (!string.IsNullOrEmpty(p.user_name) && p.user_name.StartsWith("doctor-", StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrEmpty(p.user_id) && p.user_id.StartsWith("doctor-", StringComparison.OrdinalIgnoreCase))
                        ).ToList();
                        Console.WriteLine($"[Daily] ✅ Participantes doctor encontrados: {string.Join(", ", doctorParticipants.Select(p => p.user_name ?? p.user_id ?? "unknown"))}");
                        return true; // Encontrado, salir del loop
                    }
                    else
                    {
                        Console.WriteLine($"[Daily] ⚠️ Hay participantes pero ninguno es doctor");
                        if (attempt < 3)
                        {
                            Console.WriteLine($"[Daily] ⚠️ Reintentando en 2 segundos...");
                            continue; // Reintentar
                        }
                    }
                }
                
                // Si llegamos aquí, no se encontró doctor después de 3 intentos
                Console.WriteLine($"[Daily] ❌ No se encontró doctor después de 3 intentos");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Daily] Error al verificar doctor en sala: {ex.Message}");
                Console.WriteLine($"[Daily] Stack trace: {ex.StackTrace}");
                return false;
            }
        }
    }

    public class VideoRoomResponse
    {
        public string? name { get; set; }
        public string? url { get; set; }
        public string? id { get; set; }
    }

    public class VideoTokenResponse
    {
        public string token { get; set; } = "";
    }

    public class DailyPresenceResponse
    {
        public List<DailyParticipant>? participants { get; set; }
    }

    public class DailyParticipant
    {
        public string? user_name { get; set; }
        public string? user_id { get; set; }
        public string? session_id { get; set; }
        public bool? owner { get; set; }
    }
}

