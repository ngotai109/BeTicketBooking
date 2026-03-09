using BookingTicket.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces
{
    public interface ILocationRepository : IGenericRepository<Locations>
    {
        Task<bool> ExistsByNameAsync(string name);
    }
}
