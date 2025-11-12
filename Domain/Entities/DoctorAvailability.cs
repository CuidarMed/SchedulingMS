using Domain.Enum;
namespace Domain.Entities
{
    public class DoctorAvailability
    {
        public long AvailabilityId { get; set; }
        public long DoctorId { get; set; } // Referencia externa al doctor (DirectoryMS)
        public WeekDay DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
