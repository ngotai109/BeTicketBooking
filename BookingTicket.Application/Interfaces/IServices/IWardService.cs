using BookingTicket.Application.DTOs.Province;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IServices
{
    public interface IWardService
    {
        Task<IEnumerable<WardDto>> GetWardsByProvinceIdAsync(int provinceId);
        Task<IEnumerable<WardDto>> GetAllActiveWardsAsync();
        Task<IEnumerable<WardDto>> GetAllWardsAsync();
        Task<WardDto?> GetWardByIdAsync(int id);
        Task<WardDto> CreateWardAsync(WardDto wardDto);
        Task<WardDto?> UpdateWardAsync(int id, WardDto wardDto);
        Task<bool> DeleteWardAsync(int id);
        Task<WardDto?> ToggleActiveWardAsync(int id);
    }
}
