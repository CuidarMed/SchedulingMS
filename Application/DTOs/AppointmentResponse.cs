namespace Application.DTOs
{
    public class AppointmentResponse
    {
        public long AppointmentId { get; set; }
        public long DoctorId { get; set; }
        public long PatientId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public string? Status { get; set; }
        public string? Reason { get; set; }
        public string? MeetingURL { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public long? OriginalAppointmentId { get; set; }
    }
}
