using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class AppointmentAttendance
    {
        [Required]
        [RegularExpression("COMPLETED|NO_SHOW", ErrorMessage = "El estado debe ser COMPLETED o NO_SHOW")]
        public string? Status { get; set; }
    }
}
