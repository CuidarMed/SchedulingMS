using Domain.Enum;

namespace Domain.Entities
{
    public class Appointment
    {
        public long AppointmentId { get; set; }
        public long DoctorId { get; set; } // Referencia externa al doctor (DirectoryMS)
        public long PatientId { get; set; }  // Referencia externa al paciente (DirectoryMS)
        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }
        public AppointmentStatus Status { get; set; } // CONFIRMED, CANCELLED, RESCHEDULED, COMPLETED, NO_SHOW

        public string? Reason { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Reprogramación
        public long? OriginalAppointmentId { get; set; }
        public Appointment? OriginalAppointment { get; set; }
    }
}
