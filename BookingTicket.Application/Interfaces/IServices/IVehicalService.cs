using BookingTicket.Application.DTOs.Bus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IServices
{
    public interface IVehicalService
    {
        Task<BusDTO> ToggleActiveVehicalAsync(int id, string status);
        Task<IEnumerable<BusDTO>> GetAllBusesAsync();
        Task<IEnumerable<BusDTO>> GetAllActiveBusesAsync();

        Task<BusDTO> CreateBus(CreateBusDTO createBusDTO);
        Task<BusDTO> UpdateBus(int id, UpdateBusDTO updateBusDTO);
        Task<BusDTO?> GetBusByIdAsync(int id);
        Task<bool> DeleteBusAsync(int id);



    }
}
