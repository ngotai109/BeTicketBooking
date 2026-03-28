using BookingTicket.Application.DTOs.Trip;
using BookingTicket.Application.Interfaces.IRepositories;
using BookingTicket.Application.Interfaces.IServices;
using BookingTicket.Domain.Entities;
using BookingTicket.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingTicket.Application.Services
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly IGenericRepository<TripSeats> _tripSeatRepository;

        public TripService(
            ITripRepository tripRepository,
            IGenericRepository<TripSeats> tripSeatRepository)
        {
            _tripRepository = tripRepository;
            _tripSeatRepository = tripSeatRepository;
        }

        public async Task<IEnumerable<TripMonitoringDto>> GetTripsForMonitoringAsync(DateTime? date, int? routeId)
        {
            var trips = await _tripRepository.GetTripsWithDetailsAsync(date, routeId);
            var result = new List<TripMonitoringDto>();

            foreach (var trip in trips)
            {
                // We'll use the Repository to query TripSeats
                var allTripSeats = await _tripSeatRepository.GetAllAsync();
                var specificTripSeats = allTripSeats.Where(ts => ts.TripId == trip.TripId).ToList();
                
                var totalSeats = specificTripSeats.Count;
                var bookedSeats = specificTripSeats.Count(ts => ts.Status == SeatStatus.Booked);

                result.Add(new TripMonitoringDto
                {
                    TripId = trip.TripId,
                    RouteId = trip.RouteId,
                    RouteName = trip.Route?.RouteName,
                    DepartureTime = trip.DepartureTime.ToString(@"hh\:mm"),
                    ArrivalTime = trip.ArrivalTime.ToString(@"hh\:mm"),
                    BusId = trip.BusId,
                    BusPlate = trip.Bus?.PlateNumber,
                    BusType = trip.Bus?.BusType?.TypeName,
                    TotalSeats = totalSeats,
                    AvailableSeats = totalSeats - bookedSeats,
                    Status = (int)trip.Status
                });
            }

            return result;
        }

        public async Task<IEnumerable<TripSeatDetailDto>> GetTripSeatDetailsAsync(int tripId)
        {
            var allTripSeats = await _tripSeatRepository.GetAllAsync();
            return allTripSeats
                .Where(ts => ts.TripId == tripId)
                .Select(ts => new TripSeatDetailDto
                {
                    TripSeatId = ts.TripSeatId,
                    SeatNumber = ts.Seat?.SeatNumber ?? "N/A",
                    Status = (int)ts.Status,
                    Floor = ts.Seat?.Floor ?? 1,
                    Row = ts.Seat?.Row ?? 0,
                    Column = ts.Seat?.Column ?? 0
                }).ToList();
        }

        public async Task<bool> QuickBookSeatAsync(int tripSeatId, string customerName, string phoneNumber, int status)
        {
            var tripSeat = await _tripSeatRepository.GetByIdAsync(tripSeatId);
            if (tripSeat == null) return false;

            tripSeat.Status = status == 1 ? SeatStatus.Booked : SeatStatus.Booked;
            
            await _tripSeatRepository.UpdateAsync(tripSeat);
            return true;
        }
    }
}
