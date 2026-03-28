using BookingTicket.Application.DTOs.Bus;
using BookingTicket.Application.DTOs.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingTicket.Domain.Entities;
namespace BookingTicket.Application.Interfaces.IRepositories
{
    public interface IVehicalRepository : IGenericRepository<Buses>
    {
        Task<IEnumerable<Buses>> GetAllWithDetailsAsync();
        Task<IEnumerable<Buses>> GetAllActiveBusesAsync();
        Task<Buses?> GetByIdWithDetailsAsync(int id);
        Task<Buses?> ToggleActiveVehicalAsync(int id,string status);
    }
}
