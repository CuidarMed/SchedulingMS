using Application.Converters;
using Domain.Enum;
using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public class DoctorAvailabilityCreate
    {
        public WeekDay DayOfWeek { get; set; }
        [JsonConverter(typeof(TimeSpanToStringConverter))]
        public TimeSpan StartTime { get; set; }
        [JsonConverter(typeof(TimeSpanToStringConverter))]
        public TimeSpan EndTime { get; set; }
        public int DurationMinutes { get; set; }
    }
}
