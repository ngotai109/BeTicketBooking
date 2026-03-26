using BookingTicket.Application.DTOs.Bus;
using BookingTicket.Application.DTOs.Route;
using BookingTicket.Application.Interfaces.IRepositories;
using BookingTicket.Domain.Enums;
using BookingTicket.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Repositories
{
    public class VehicalRepository : GenericRepository<BusDTO>, IVehicalRepository
    {
        private readonly BusStatus busStatus;
        public VehicalRepository(ApplicationDbContext context) : base(context)
        {

        }

        public Task<IEnumerable<BusDTO>> GetAllActiveBusesAsync()
        {
            throw new NotImplementedException();
        }


        public async Task<IEnumerable<BusDTO>> GetAllBusesAsync()
        {
         
            throw new NotImplementedException();
        }
        public Task<BusDTO?> GetBusByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<BusDTO?> ToggleActiveRouteAsync(int id, string status)
        {
            var bus = await _dbSet.FindAsync(id);
            if (bus!=null)
            {
                if(status == "InActive")
                {
                    bus.Status = BusStatus.Inactive;
                }
                else if(status == "Active")
                {
                    bus.Status = BusStatus.Active;
                }
                else if(status == "Maintenance")
                {
                    bus.Status = BusStatus.Maintenance;
                }
                await _context.SaveChangesAsync();
            }
            return bus;
        }

    }
}
