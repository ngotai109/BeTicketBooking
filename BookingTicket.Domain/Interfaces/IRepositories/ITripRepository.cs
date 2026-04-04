using BookingTicket.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Domain.Interfaces.IRepositories
{
    public interface ITripRepository : IGenericRepository<Trips>
    {
        // Add specific methods for Trips if needed
        Task<IEnumerable<Trips>> GetTripsWithDetailsAsync(DateTime? date, int? routeId);
        Task<Trips?> GetTripByIdWithDetailsAsync(int id);
        Task<bool> IsBusOccupiedAsync(int busId, DateTime departureTime, DateTime arrivalTime, int? excludedTripId = null);
    }
}
