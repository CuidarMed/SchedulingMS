using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public class DailyRoomConfig
    {
        public long? Exp { get; set; }

        [JsonPropertyName("enable_chat")]
        public bool? Enable_chat { get; set; }

        [JsonPropertyName("enable_screenshare")]
        public bool? Enable_screenshare { get; set; }

        [JsonPropertyName("enable_recording")]
        public string? Enable_recording { get; set; } // Daily.co puede enviar "cloud", "local", o false
    }
}
