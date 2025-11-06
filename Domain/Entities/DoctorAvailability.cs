using Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class DoctorAvailability
    {
        [Key]
        public long AvailabilityId { get; set; }

        [Required]
        public long DoctorId { get; set; } // Referencia externa al doctor (DirectoryMS)

        [Required]
        public WeekDay DayOfWeek { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        [Range(1, 1440)]
        public int DurationMinutes { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
