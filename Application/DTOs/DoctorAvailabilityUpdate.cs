using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class DoctorAvailabilityUpdate
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int DurationMinutes { get; set; }

        // Opcional: permitir desactivar disponibilidad sin borrarla
        public bool? IsActive { get; set; }
    }
}
