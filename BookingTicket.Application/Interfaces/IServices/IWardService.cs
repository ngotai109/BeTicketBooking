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
        Task<WardDto?> ToggleActiveWardAsync(int id);
    }
}
