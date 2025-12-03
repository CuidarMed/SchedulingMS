using System.Text.Json.Serialization;

namespace Application.DTOs
{
    // Respuesta de Daily.co al crear/obtener una sala
    public class DailyRoomResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Privacy { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public string? Created_at { get; set; } // Daily.co lo envía como string

        public DailyRoomConfig? Config { get; set; }
    }
}
