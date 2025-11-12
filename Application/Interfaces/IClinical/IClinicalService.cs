namespace Application.Interfaces.IClinical
{
    public interface IClinicalService
    {
        Task<bool> HasEncounterForAppointmentAsync(long appointmentId);
        Task<object?> GetEncounterByAppointmentIdAsync(long appointmentId);
    }
}


