using BookingTicket.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IRepositories
{
    public interface IBusTypeRepository : IGenericRepository<BusTypes>
    {
        Task<IEnumerable<BusTypes>> GetAllActiveAsync();
        Task<BusTypes?> ToggleActiveStatusAsync(int id);
    }
}
