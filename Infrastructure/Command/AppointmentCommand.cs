using Application.Interfaces.IAppointment;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Command
{
    public class AppointmentCommand : IAppointmentCommand
    {
        private readonly AppDbContext _context;

        public AppointmentCommand(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null) return;
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }

    }
}
