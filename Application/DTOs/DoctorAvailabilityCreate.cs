using Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class DoctorAvailabilityCreate
    {
        [Required]
        public WeekDay DayOfWeek { get; set; }
        [Required]
        public TimeSpan StartTime { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; }
        [Required]
        [Range(5, 45, ErrorMessage = "La duración debe ser entre 5 y 45 minutos.")]
        public int DurationMinutes { get; set; }
    }
}
