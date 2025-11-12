using Application.Interfaces.IAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ServiceTokenProvider : IServiceTokenProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServiceTokenProvider> _logger;
        private string? _cachedToken;
        private DateTime _tokenExpiry = DateTime.MinValue;

        public ServiceTokenProvider(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<ServiceTokenProvider> logger)
        {
            _httpClient = httpClientFactory.CreateClient("AuthMS");
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GetServiceTokenAsync()
        {
            // Si el token está en caché y no ha expirado, devolverlo
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
            {
                return _cachedToken;
            }

            try
            {
                // Obtener credenciales de servicio desde configuración
                var serviceEmail = _configuration["ServiceAuth:Email"] ?? "scheduling-service@cuidarmed.com";
                var servicePassword = _configuration["ServiceAuth:Password"] ?? "ServicePassword123!";

                // Llamar a AuthMS para obtener token
                var loginRequest = new
                {
                    Email = serviceEmail,
                    Password = servicePassword
                };

                var response = await _httpClient.PostAsJsonAsync("/api/v1/Auth/Login", loginRequest);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al obtener token de servicio. StatusCode: {StatusCode}", response.StatusCode);
                    throw new Exception("No se pudo obtener token de servicio");
                }

                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (loginResponse == null || string.IsNullOrEmpty(loginResponse.AccessToken))
                {
                    throw new Exception("No se recibió accessToken del servicio de autenticación");
                }

                _cachedToken = loginResponse.AccessToken;
                // Asumir que el token expira en 55 minutos (renovar antes de que expire)
                _tokenExpiry = DateTime.UtcNow.AddMinutes(55);

                _logger.LogInformation("Token de servicio obtenido exitosamente");
                return _cachedToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener token de servicio");
                throw;
            }
        }

        private class LoginResponse
        {
            public string? AccessToken { get; set; }
            public string? RefreshToken { get; set; }
        }
    }
}

