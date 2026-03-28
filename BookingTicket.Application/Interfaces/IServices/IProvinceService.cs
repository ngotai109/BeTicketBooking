using BookingTicket.Application.DTOs.Province;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IServices
{
    public interface IProvinceService
    {
        Task<IEnumerable<ProvinceDto>> GetAllActiveProvincesAsync();
        Task<IEnumerable<ProvinceDto>> GetAllProvinceAsync();
        Task<ProvinceDto?> GetByIdAsync(int id);
        Task<ProvinceDto> CreateProvinceAsync(ProvinceDto provinceDto);
        Task<ProvinceDto?> UpdateProvinceAsync(int id, ProvinceDto provinceDto);
        Task<bool> DeleteProvinceAsync(int id);
        Task<ProvinceDto?> ToggleActiveProvinceAsync(int id);
    }
}
