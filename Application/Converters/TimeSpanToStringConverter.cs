using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace Application.Converters
{
    public class TimeSpanToStringConverter : JsonConverter<TimeSpan>
    {
        private static readonly string[] AllowedFormats =
        {
            @"h\:mm", @"hh\:mm", @"h\:mm\:ss", @"hh\:mm\:ss"
        };

        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (string.IsNullOrWhiteSpace(value))
                throw new JsonException("El campo de hora no puede estar vacío.");

            // Intentamos parsear con distintos formatos
            foreach (var format in AllowedFormats)
            {
                if (TimeSpan.TryParseExact(value, format, CultureInfo.InvariantCulture, out var time))
                    return time;
            }

            throw new JsonException($"Formato de hora inválido: '{value}'. Usa formatos como '8:30' o '17:00'.");
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            // Serializamos con formato corto (sin segundos)
            writer.WriteStringValue(value.ToString(@"hh\:mm"));
        }
    }
}
