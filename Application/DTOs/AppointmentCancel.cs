using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class AppointmentCancel
    {
        [Required]
        public string? Reason { get; set; }
    }
}
