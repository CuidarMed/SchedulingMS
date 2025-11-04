using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Appointment
    {
        [Key]
        public long AppointmentId { get; set; }

        [Required]
        public long DoctorId { get; set; } // Referencia externa al doctor (DirectoryMS)

        [Required]
        public long PatientId { get; set; }  // Referencia externa al paciente (DirectoryMS)

        [Required]
        public DateTimeOffset StartTime { get; set; }

        [Required]
        public DateTimeOffset EndTime { get; set; }

        [Required]
        [MaxLength(20)]
        public string? Status { get; set; } // CONFIRMED, CANCELLED, RESCHEDULED, COMPLETED, NO_SHOW

        public string? Reason { get; set; }

        public string? MeetingURL { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Reprogramación
        public long? OriginalAppointmentId { get; set; }
        public Appointment? OriginalAppointment { get; set; }
    }
}
