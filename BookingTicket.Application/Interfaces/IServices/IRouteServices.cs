using BookingTicket.Application.DTOs.Route;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IServices
{
    public interface IRouteServices
    {
        Task<IEnumerable<RouteDto>> GetAllRoutesAsync();
        Task<IEnumerable<RouteDto>> GetAllActiveRoutesAsync();
        Task<RouteDto?> GetRouteByIdAsync(int id);
        Task<RouteDto> CreateRouteAsync(CreateRouteDto createRouteDto);
        Task<RouteDto?> UpdateRouteAsync(int id, CreateRouteDto updateRouteDto);
        Task<bool> DeleteRouteAsync(int id);
        Task<RouteDto?> ToggleActiveRouteAsync(int id);
    }
}
