using Application.DTOs;
using Application.Interfaces.IAppointment;

namespace Application.Services.AppointmentService
{
    public class SearchAppointmentService : ISearchAppointmentService
    {
        private readonly IAppointmentQuery _query;
        private readonly IAppointmentMapper _mapper;

        public SearchAppointmentService(IAppointmentQuery query, IAppointmentMapper mapper)
        {
            _query = query;
            _mapper = mapper;
        }

        public async Task<AppointmentResponse> GetByIdAsync(long id)
        {
            var appointment = await _query.GetByIdAsync(id);
            if (appointment == null)
                return null; 
            return _mapper.ToResponse(appointment);
        }

        public async Task<List<AppointmentResponse>> GetAllAsync()
        {
            var appointments = await _query.GetAll();
            return _mapper.ToResponseList(appointments.ToList());
        }

        public async Task<List<AppointmentResponse>> SearchAsync(AppointmentSearch search)
        {
            // Llamamos al query con el DTO de búsqueda
            var appointments = await _query.SearchAsync(search);

            // Mapear a DTOs
            return _mapper.ToResponseList(appointments.ToList());
        }

        public async Task<List<AppointmentResponse>> GetPatientsByDoctorIdAsync(long doctorId)
        {
            var appointments = await _query.GetByDoctorIdAsync(doctorId);

            if (appointments == null || !appointments.Any())
                return new List<AppointmentResponse>();

            return _mapper.ToResponseList(appointments);
        }
    }
}
