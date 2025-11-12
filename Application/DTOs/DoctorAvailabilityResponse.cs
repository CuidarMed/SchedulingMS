using System.Text.Json.Serialization;
using Application.Converters;
using Domain.Enum;

namespace Application.DTOs
{
    public class DoctorAvailabilityResponse
    {
        public long AvailabilityId { get; set; }
        public long DoctorId { get; set; }
        public WeekDay DayOfWeek { get; set; }

        [JsonConverter(typeof(Converters.TimeSpanToStringConverter))]
        public TimeSpan StartTime { get; set; }

        [JsonConverter(typeof(Converters.TimeSpanToStringConverter))]
        public TimeSpan EndTime { get; set; }

        public int DurationMinutes { get; set; }
        public bool IsActive { get; set; }
        [JsonConverter(typeof(DateTimeOffsetConverter))]
        public DateTimeOffset CreatedAt { get; set; }
        [JsonConverter(typeof(DateTimeOffsetConverter))]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
