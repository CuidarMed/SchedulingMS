using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class AppointmentReschedule
    {
        public DateTimeOffset NewStartTime { get; set; }
        public DateTimeOffset NewEndTime { get; set; }
        public string? Reason { get; set; }
    }
}
