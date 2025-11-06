using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class DoctorAvailabilityUpdate
    {
        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        [Range(5, 45, ErrorMessage = "La duración debe estar entre 5 y 45 minutos.")]
        public int DurationMinutes { get; set; }

        // Opcional: permitir desactivar disponibilidad sin borrarla
        public bool? IsActive { get; set; }
    }
}
