using Domain.Enum;

namespace Application.DTOs
{
    public class AppointmentUpdate
    {
        public long DoctorId { get; set; }
        public long PatientId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public AppointmentStatus? Status { get; set; } 
        public string? Reason { get; set; }
    }
}
