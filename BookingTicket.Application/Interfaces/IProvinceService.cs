using BookingTicket.Application.DTOs.Province;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces
{
    public interface IProvinceService
    {
        Task<IEnumerable<ProvinceDto>> GetAllActiveProvincesAsync();
        Task<ProvinceDto?> GetByIdAsync(int id);
    }
}
