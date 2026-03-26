using System.Collections.Generic;
using System.Threading.Tasks;
using BookingTicket.Domain.Entities;

namespace BookingTicket.Application.Interfaces.IRepositories
{
    public interface IOfficeRepository : IGenericRepository<Office>
    {
        Task<IEnumerable<Office>> GetAllWithDetailsAsync();
        Task<IEnumerable<Office>> GetAllActiveWithDetailsAsync();
        Task<Office?> GetByIdWithDetailsAsync(int id);
        Task<Office?> ToggleActiveStatusAsync(int id);
    }
}
