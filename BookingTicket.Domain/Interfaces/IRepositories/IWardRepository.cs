using BookingTicket.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Domain.Interfaces.IRepositories
{
    public interface IWardRepository : IGenericRepository<Ward>
    {
        Task<IEnumerable<Ward>> GetAllWardAsync();
        Task<IEnumerable<Ward>> GetWardsByProvinceIdAsync(int provinceId);
        Task<IEnumerable<Ward>> GetAllActiveWardAsync();
        Task<Ward?> ToggleActiveWardAsync(int id);
    }
}
