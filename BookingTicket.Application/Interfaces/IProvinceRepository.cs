using BookingTicket.Application.DTOs.Province;
using BookingTicket.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces
{
    public interface IProvinceRepository : IGenericRepository<Provinces>
    {
        Task<IEnumerable<Provinces>> GetAllActiveProvincesAsync();
        Task<IEnumerable<Provinces>> GetAllProvinceAsync();
        Task<Provinces?> GetProvinceWithWardsAsync(int id);
        Task<Provinces?> ToggleActiveProvinceAsync(int id);
    }
}
