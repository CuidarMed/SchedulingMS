using Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class AppointmentAttendance
    {
        public AppointmentStatus? Status { get; set; }
        public string? Reason { get; set; }
    }
}
