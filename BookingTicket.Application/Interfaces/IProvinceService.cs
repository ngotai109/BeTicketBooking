using BookingTicket.Application.DTOs.Province;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces
{
    public interface IProvinceService
    {
        Task<IEnumerable<ProvinceDto>> GetAllActiveProvincesAsync();
        Task<IEnumerable<ProvinceDto>> GetAllProvinceAsync();
        Task<ProvinceDto?> GetByIdAsync(int id);
        Task<ProvinceDto?> ToggleActiveProvinceAsync(int id);
    }
}
