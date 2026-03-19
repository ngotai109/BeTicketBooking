using BookingTicket.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces
{
    public interface IRouteRepository : IGenericRepository<Routes>
    {
        Task<IEnumerable<Routes>> GetAllWithDetailsAsync();
        Task<IEnumerable<Routes>> GetAllActiveWithDetailsAsync();
        Task<Routes?> GetByIdWithDetailsAsync(int id);
        Task<Routes?> ToggleActiveStatusAsync(int id);
    }
}
