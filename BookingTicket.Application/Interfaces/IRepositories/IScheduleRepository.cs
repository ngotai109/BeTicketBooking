using BookingTicket.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IRepositories
{
    public interface IScheduleRepository : IGenericRepository<Schedules>
    {
        Task<IEnumerable<Schedules>> GetAllWithDetailsAsync();
        Task<Schedules?> GetByIdWithDetailsAsync(int id);
    }
}
