using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class AppointmentReschedule
    {
        [Required]
        public DateTimeOffset NewStartTime { get; set; }
        [Required]
        public DateTimeOffset NewEndTime { get; set; }
    }
}
