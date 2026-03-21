using BookingTicket.Application.DTOs.Bus;
using BookingTicket.Application.DTOs.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces
{
    public interface IBusRepository
    {
        Task<IEnumerable<BusDTO>> GetAllBusesAsync();
        Task<IEnumerable<BusDTO>> GetAllActiveBusesAsync();
        Task<BusDTO?> GetBusByIdAsync(int id);
        Task<BusDTO?> ToggleActiveRouteAsync(int id);
    }
}
