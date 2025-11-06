using Domain.Enum;

namespace Application.DTOs
{
    public class DoctorAvailabilityResponse
    {
        public long AvailabilityId { get; set; }

        public long DoctorId { get; set; }

        public WeekDay DayOfWeek { get; set; } 

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int DurationMinutes { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
