using Application.Interfaces.IAuth;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Handlers
{
    public class JwtServiceClientHandler : DelegatingHandler
    {
        private readonly IServiceTokenProvider _tokenProvider;
        private readonly ILogger<JwtServiceClientHandler> _logger;

        public JwtServiceClientHandler(
            IServiceTokenProvider tokenProvider,
            ILogger<JwtServiceClientHandler> logger)
        {
            _tokenProvider = tokenProvider;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Obtener token de servicio
                var token = await _tokenProvider.GetServiceTokenAsync();

                // Agregar token al header Authorization
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar token JWT a la solicitud");
                // Continuar sin token si falla (el servicio destino puede rechazar la petición)
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}


