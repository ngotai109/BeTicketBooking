using BookingTicket.Application.DTOs.Trip;
using BookingTicket.Domain.Interfaces.IRepositories;
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
                result.Add(await MapToMonitoringDto(trip));
            }

            return result;
        }

        public async Task<IEnumerable<TripSeatDetailDto>> GetTripSeatDetailsAsync(int tripId)
        {
            // Ensure seats exist before returning them
            await EnsureTripSeatsExistAsync(tripId);

            var tripSeats = await _tripSeatRepository.GetSeatsByTripIdAsync(tripId);

            return tripSeats.Select(ts => new TripSeatDetailDto
            {
                TripSeatId = ts.TripSeatId,
                SeatNumber = ts.Seat?.SeatNumber ?? $"#{ts.SeatId}",
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
                        await CreateNewTripFromSchedule(schedule, date);
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

            await CreateNewTripFromSchedule(schedule, departureDate);

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

        public async Task<IEnumerable<TripMonitoringDto>> SearchTripsAsync(string departure, string destination, DateTime date)
        {
            var trips = await _tripRepository.GetTripsWithDetailsAsync(date, null);

            var depSearch = RemoveDiacritics(departure ?? "").ToLower();
            var destSearch = RemoveDiacritics(destination ?? "").ToLower();

            var filteredTrips = trips.Where(t =>
            {
                var routeName = RemoveDiacritics(t.Route?.RouteName ?? "").ToLower();
                var depOfficeName = RemoveDiacritics(t.Route?.DepartureOffice?.OfficeName ?? "").ToLower();
                var depWardName = RemoveDiacritics(t.Route?.DepartureOffice?.Ward?.WardName ?? "").ToLower();
                var arrOfficeName = RemoveDiacritics(t.Route?.ArrivalOffice?.OfficeName ?? "").ToLower();
                var arrWardName = RemoveDiacritics(t.Route?.ArrivalOffice?.Ward?.WardName ?? "").ToLower();

                bool depMatch = string.IsNullOrWhiteSpace(depSearch) ||
                               routeName.Contains(depSearch) ||
                               depOfficeName.Contains(depSearch) ||
                               depWardName.Contains(depSearch);

                bool destMatch = string.IsNullOrWhiteSpace(destSearch) ||
                                routeName.Contains(destSearch) ||
                                arrOfficeName.Contains(destSearch) ||
                                arrWardName.Contains(destSearch);

                return depMatch && destMatch;
            }).ToList();

            var result = new List<TripMonitoringDto>();
            foreach (var trip in filteredTrips)
            {
                result.Add(await MapToMonitoringDto(trip));
            }

            return result;
        }

        private async Task<TripMonitoringDto> MapToMonitoringDto(Trips trip)
        {
            // Ensure seats exist for this trip
            await EnsureTripSeatsExistAsync(trip.TripId);
            
            var tripSeats = await _tripSeatRepository.GetSeatsByTripIdAsync(trip.TripId);
            var specificTripSeats = tripSeats.ToList();

            var totalSeats = specificTripSeats.Count;
            var bookedSeats = specificTripSeats.Count(ts => ts.Status == SeatStatus.Booked);
            var busType = trip.Bus?.BusType?.TypeName ?? "N/A";

            return new TripMonitoringDto
            {
                TripId = trip.TripId,
                RouteId = trip.RouteId,
                RouteName = trip.Route?.RouteName,
                DepartureTime = trip.DepartureTime.ToString(@"HH\:mm"),
                ArrivalTime = trip.ArrivalTime.ToString(@"HH\:mm"),
                BusId = trip.BusId,
                BusPlate = trip.Bus?.PlateNumber,
                BusType = busType,
                TotalSeats = totalSeats,
                AvailableSeats = totalSeats - bookedSeats,
                Status = (int)trip.Status,
                TicketPrice = trip.TicketPrice,
                DepartureOfficeName = trip.Route?.DepartureOffice?.OfficeName,
                ArrivalOfficeName = trip.Route?.ArrivalOffice?.OfficeName
            };
        }

        private string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }

        private async Task<Trips> CreateNewTripFromSchedule(Schedules schedule, DateTime date)
        {
            var departureTime = date.Add(schedule.DepartureTime);
            var arrivalTime = date.Add(schedule.ArrivalTime);


            if (schedule.ArrivalTime < schedule.DepartureTime)
            {
                arrivalTime = arrivalTime.AddDays(1);
            }

            var newTrip = new Trips
            {
                ScheduleId = schedule.ScheduleId,
                BusId = schedule.BusId,
                RouteId = schedule.RouteId,
                DepartureTime = departureTime,
                ArrivalTime = arrivalTime,
                TicketPrice = schedule.TicketPrice,
                TripSeats = new List<TripSeats>(),
                Status = TripStatus.Scheduled
            };

            // UPFRONT SEAT GENERATION: Prepare seats for "Real Booking"
            var allSeats = await _seatRepository.GetAllAsync();
            var busSeats = allSeats.Where(s => s.BusId == schedule.BusId && s.IsActive).ToList();

            // IF BUS HAS NO SEATS: Generate 40 default seats for this bus UPFRONT!
            if (!busSeats.Any())
            {
                var defaultSeats = new List<string> { 
                    "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "A10",
                    "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "B10",
                    "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "C10",
                    "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "D10"
                };

                foreach (var sNum in defaultSeats)
                {
                    var newSeat = new Seats
                    {
                        BusId = schedule.BusId,
                        SeatNumber = sNum,
                        Floor = sNum.StartsWith("A") || sNum.StartsWith("B") ? 1 : 2,
                        IsActive = true
                    };
                    await _seatRepository.AddAsync(newSeat);
                    busSeats.Add(newSeat);
                }
            }

            foreach (var seat in busSeats)
            {
                newTrip.TripSeats.Add(new TripSeats
                {
                    SeatId = seat.SeatId,
                    Status = SeatStatus.Available
                });
            }

            await _tripRepository.AddAsync(newTrip);
            return newTrip;
        }

        private async Task EnsureTripSeatsExistAsync(int tripId)
        {
            var currentSeats = await _tripSeatRepository.GetSeatsByTripIdAsync(tripId);
            if (!currentSeats.Any())
            {
                var trip = await _tripRepository.GetByIdAsync(tripId);
                if (trip != null && trip.BusId > 0)
                {
                    var allSeats = await _seatRepository.GetAllAsync();
                    var busSeats = allSeats.Where(s => s.BusId == trip.BusId && s.IsActive).ToList();
                    
                    // IF BUS HAS NO SEATS: Generate 40 default seats for this bus first!
                    if (!busSeats.Any())
                    {
                        var defaultSeats = new List<string> { 
                            "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "A10",
                            "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "B10",
                            "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "C10",
                            "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "D10"
                        };

                        var newSeats = new List<Seats>();
                        foreach (var sNum in defaultSeats)
                        {
                            var newSeat = new Seats
                            {
                                BusId = trip.BusId,
                                SeatNumber = sNum,
                                Floor = sNum.StartsWith("A") || sNum.StartsWith("B") ? 1 : 2,
                                IsActive = true
                            };
                            await _seatRepository.AddAsync(newSeat);
                            newSeats.Add(newSeat);
                        }
                        busSeats = newSeats;
                    }

                    if (busSeats.Any())
                    {
                        foreach (var seat in busSeats)
                        {
                            await _tripSeatRepository.AddAsync(new TripSeats
                            {
                                TripId = tripId,
                                SeatId = seat.SeatId,
                                Status = SeatStatus.Available
                            });
                        }
                    }
                }
            }
        }
    }
}
