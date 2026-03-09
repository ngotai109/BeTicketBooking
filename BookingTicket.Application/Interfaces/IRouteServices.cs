using BookingTicket.Application.DTOs.Route;
using BookingTicket.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces
{
    public interface IRouteServices
    {
        Task<IEnumerable<Routes>> GetAllRouteAsync();
        Task<Routes?> GetRouteByIdAsync(int idRoute); 
        Task UpdateRouteAsync(int idRoute, UpdateRouteDto dto);
        Task CreateRouteAsync(CreateRouteDto dto);
        Task DeleteRouteAsync(int idRoute);
    }
}
