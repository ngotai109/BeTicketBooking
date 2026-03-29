using BookingTicket.Application.DTOs.Trip;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingTicket.Application.Interfaces.IServices
{
    public interface ITripService
    {
        Task<IEnumerable<TripMonitoringDto>> GetTripsForMonitoringAsync(DateTime? date, int? routeId);
        Task<IEnumerable<TripSeatDetailDto>> GetTripSeatDetailsAsync(int tripId);
        Task<bool> QuickBookSeatAsync(int tripSeatId, string customerName, string phoneNumber, int status);
        Task<bool> AutoGenerateTripsAsync(DateTime startDate, DateTime endDate);
        Task<bool> CreateTripAsync(int scheduleId, DateTime departureDate);
    }
}
