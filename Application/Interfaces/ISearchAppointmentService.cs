using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISearchAppointmentService
    {
        Task<AppointmentResponse?> GetByIdAsync(long id);
        
        Task<IEnumerable<AppointmentResponse>> SearchAsync(AppointmentSearch search);
    }
}
