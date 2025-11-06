using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class AppointmentCreate
    {
        [Required]
        public long DoctorId { get; set; }

        [Required]
        public long PatientId { get; set; }

        [Required]
        public DateTimeOffset StartTime { get; set; }

        [Required]
        public DateTimeOffset EndTime { get; set; }

    }
}
