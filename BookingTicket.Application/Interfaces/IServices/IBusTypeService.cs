using BookingTicket.Application.DTOs.Bus;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IServices
{
    public interface IBusTypeService
    {
        Task<IEnumerable<BusTypeDto>> GetAllBusTypesAsync();
        Task<IEnumerable<BusTypeDto>> GetAllActiveBusTypesAsync();
        Task<BusTypeDto?> GetBusTypeByIdAsync(int id);
        Task<BusTypeDto> CreateBusTypeAsync(CreateBusTypeDto createBusTypeDto);
        Task<BusTypeDto?> UpdateBusTypeAsync(int id, CreateBusTypeDto updateBusTypeDto);
        Task<bool> DeleteBusTypeAsync(int id);
        Task<BusTypeDto?> ToggleActiveStatusAsync(int id);
    }
}
