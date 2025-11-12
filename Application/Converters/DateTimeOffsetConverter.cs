using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Converters
{
    public class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        private const string Format = "yyyy-MM-dd HH:mm";

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (DateTimeOffset.TryParse(value, out var date))
                return date;

            throw new JsonException($"Formato de fecha inválido: {value}");
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }
    }
}
