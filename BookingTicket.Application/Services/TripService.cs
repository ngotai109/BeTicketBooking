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
        private readonly ITripSeatRepository _tripSeatRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ISeatRepository _seatRepository;

        public TripService(
            ITripRepository tripRepository,
            ITripSeatRepository tripSeatRepository,
            IScheduleRepository scheduleRepository,
            ISeatRepository seatRepository)
        {
            _tripRepository = tripRepository;
            _tripSeatRepository = tripSeatRepository;
            _scheduleRepository = scheduleRepository;
            _seatRepository = seatRepository;
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

        public async Task<bool> AutoGenerateTripsAsync(DateTime startDate, DateTime endDate)
        {
            var allSchedules = await _scheduleRepository.GetAllAsync();
            var activeSchedules = allSchedules.Where(s => s.IsActive).ToList();
            if (!activeSchedules.Any()) return false;

            var allSeats = await _seatRepository.GetAllAsync();
            var allTrips = await _tripRepository.GetAllAsync();

            bool tripsAdded = false;

            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                foreach (var schedule in activeSchedules)
                {
                    bool exists = allTrips.Any(t => t.ScheduleId == schedule.ScheduleId 
                                                 && t.DepartureTime.Date == date.Date);

                    if (!exists)
                    {
                        await CreateNewTripFromSchedule(schedule, date, allSeats);
                        tripsAdded = true;
                    }
                }
            }

            return tripsAdded;
        }

        public async Task<bool> CreateTripAsync(int scheduleId, DateTime departureDate)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null || !schedule.IsActive) return false;

            var allTrips = await _tripRepository.GetAllAsync();
            bool exists = allTrips.Any(t => t.ScheduleId == scheduleId && t.DepartureTime.Date == departureDate.Date);
            if (exists) return false;

            var allSeats = await _seatRepository.GetAllAsync();
            await CreateNewTripFromSchedule(schedule, departureDate, allSeats);

            return true;
        }

        public async Task<bool> UpdateTripStatusAsync(int tripId, int status)
        {
            var trip = await _tripRepository.GetByIdAsync(tripId);
            if (trip == null) return false;

            trip.Status = (TripStatus)status;
            await _tripRepository.UpdateAsync(trip);
            return true;
        }

        public async Task<bool> DeleteTripAsync(int tripId)
        {
            var trip = await _tripRepository.GetByIdAsync(tripId);
            if (trip == null) return false;

            await _tripRepository.DeleteAsync(trip);
            return true;
        }

        private async Task CreateNewTripFromSchedule(Schedules schedule, DateTime date, IEnumerable<Seats> allSeats)
        {
            var departureTime = date.Add(schedule.DepartureTime);
            var arrivalTime = date.Add(schedule.ArrivalTime);
            
            // Handle cross-day trips
            if (schedule.ArrivalTime < schedule.DepartureTime)
            {
                arrivalTime = arrivalTime.AddDays(1);
            }

            var newTrip = new Trips
            {
                ScheduleId = schedule.ScheduleId,
                TicketPrice = schedule.TicketPrice,
                DepartureTime = departureTime,
                ArrivalTime = arrivalTime,
                Status = TripStatus.Scheduled,
                BusId = schedule.BusId,
                RouteId = schedule.RouteId,
                TripSeats = new List<TripSeats>()
            };

            var busSeats = allSeats.Where(s => s.BusId == schedule.BusId && s.IsActive).ToList();
            foreach (var seat in busSeats)
            {
                newTrip.TripSeats.Add(new TripSeats
                {
                    SeatId = seat.SeatId,
                    Status = SeatStatus.Available
                });
            }

            await _tripRepository.AddAsync(newTrip);
        }
    }
}
