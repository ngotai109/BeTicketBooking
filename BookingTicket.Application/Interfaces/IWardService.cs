using BookingTicket.Application.DTOs.Province;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces
{
    public interface IWardService
    {
        Task<IEnumerable<WardDto>> GetWardsByProvinceIdAsync(int provinceId);
    }
}
