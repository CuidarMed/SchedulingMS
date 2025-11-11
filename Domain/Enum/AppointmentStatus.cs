using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum AppointmentStatus
    {
        SCHEDULED = 1,   // Turno creado, pendiente de confirmación
        CONFIRMED = 2,   // Paciente confirmó asistencia
        RESCHEDULED = 3, // Se reprogramó (genera nuevo turno)
        CANCELLED = 4,   // Cancelado por doctor o paciente
        COMPLETED = 5,   // Atendido exitosamente
        NO_SHOW = 6      // Paciente no asistió
    }
}
