using BookingTicket.Application.DTOs.Bus;
using BookingTicket.Application.DTOs.Route;
using BookingTicket.Application.Interfaces;
using BookingTicket.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Repositories
{
    public class BusRepository : GenericRepository<BusDTO>, IBusRepository
    {
        public BusRepository(ApplicationDbContext context) : base(context)
        {

        }

        public Task<IEnumerable<RouteDto>> GetAllActiveBusesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BusDTO>> GetAllBusesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<RouteDto?> GetBusByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BusDTO?> ToggleActiveRouteAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<BusDTO>> IBusRepository.GetAllActiveBusesAsync()
        {
            throw new NotImplementedException();
        }

        Task<BusDTO?> IBusRepository.GetBusByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
