using Application.Interfaces.IClinical;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Application.Services
{
    public class ClinicalService : IClinicalService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ClinicalService> _logger;

        public ClinicalService(
            IHttpClientFactory httpClientFactory,
            ILogger<ClinicalService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ClinicalMS");
            _logger = logger;
        }

        public async Task<bool> HasEncounterForAppointmentAsync(long appointmentId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/v1/Encounter?appointmentId={appointmentId}");

                if (response.IsSuccessStatusCode)
                {
                    var encounters = await response.Content.ReadFromJsonAsync<IEnumerable<object>>();
                    return encounters != null && encounters.Any();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }

                _logger.LogWarning(
                    "Error al verificar encounter para appointment {AppointmentId}. StatusCode: {StatusCode}",
                    appointmentId, response.StatusCode);
                return false;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al verificar encounter para appointment {AppointmentId}", appointmentId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al verificar encounter para appointment {AppointmentId}", appointmentId);
                return false;
            }
        }

        public async Task<object?> GetEncounterByAppointmentIdAsync(long appointmentId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/v1/Encounter?appointmentId={appointmentId}");

                if (response.IsSuccessStatusCode)
                {
                    var encounters = await response.Content.ReadFromJsonAsync<IEnumerable<object>>();
                    return encounters?.FirstOrDefault();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                _logger.LogWarning(
                    "Error al obtener encounter para appointment {AppointmentId}. StatusCode: {StatusCode}",
                    appointmentId, response.StatusCode);
                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al obtener encounter para appointment {AppointmentId}", appointmentId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener encounter para appointment {AppointmentId}", appointmentId);
                return null;
            }
        }
    }
}

