using Application.Converters;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public class AppointmentCreate
    {
        public long DoctorId { get; set; }
        public long PatientId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

    }
}
