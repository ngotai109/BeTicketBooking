using BookingTicket.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces
{
    public interface IRouteRepository : IGenericRepository<Routes>
    {
        Task<bool> ExitsByAsyncName(string name);
    }
}
