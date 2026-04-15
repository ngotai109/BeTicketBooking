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
        private readonly IBookingRepository _bookingRepository;
        private readonly ITicketRepository _ticketRepository;

        public TripService(
            ITripRepository tripRepository,
            ITripSeatRepository tripSeatRepository,
            IScheduleRepository scheduleRepository,
            ISeatRepository seatRepository,
            IBookingRepository bookingRepository,
            ITicketRepository ticketRepository)
        {
            _tripRepository = tripRepository;
            _tripSeatRepository = tripSeatRepository;
            _scheduleRepository = scheduleRepository;
            _seatRepository = seatRepository;
            _bookingRepository = bookingRepository;
            _ticketRepository = ticketRepository;
        }

        public async Task<IEnumerable<TripMonitoringDto>> GetTripsForMonitoringAsync(DateTime? date, int? routeId)
        {
            var trips = await _tripRepository.GetTripsWithDetailsAsync(date, routeId);

            var tripIds = trips.Select(t => t.TripId).Distinct().ToList();
            var seatCounts = await _tripSeatRepository.GetSeatCountsForTripsAsync(tripIds);

            var result = new List<TripMonitoringDto>();
            foreach (var trip in trips)
            {
                seatCounts.TryGetValue(trip.TripId, out var counts);
                result.Add(MapToMonitoringDtoSync(trip, counts.total, counts.booked));
            }

            return result;
        }

        public async Task<IEnumerable<TripSeatDetailDto>> GetTripSeatDetailsAsync(int tripId)
        {
            // Ensure seats exist before returning them
            await EnsureTripSeatsExistAsync(tripId);

            var tripSeats = await _tripSeatRepository.GetSeatsByTripIdAsync(tripId);

            return tripSeats.Select(ts => {
                var firstTicket = ts.Tickets?.FirstOrDefault();
                return new TripSeatDetailDto
                {
                    TripSeatId = ts.TripSeatId,
                    SeatNumber = ts.Seat?.SeatNumber ?? $"#{ts.SeatId}",
                    Status = (int)ts.Status,
                    Floor = ts.Seat?.Floor ?? 1,
                    Row = ts.Seat?.Row ?? 0,
                    Column = ts.Seat?.Column ?? 0,
                    CustomerName = firstTicket?.Booking?.CustomerName ?? string.Empty,
                    PhoneNumber = firstTicket?.Booking?.CustomerPhone ?? string.Empty
                };
            }).ToList();
        }

        public async Task<bool> QuickBookSeatAsync(int tripSeatId, string customerName, string phoneNumber, int status)
        {
            var tripSeat = await _tripSeatRepository.GetByIdAsync(tripSeatId);
            if (tripSeat == null) return false;

            // 1. Cập nhật trạng thái ghế
            tripSeat.Status = SeatStatus.Booked;
            await _tripSeatRepository.UpdateAsync(tripSeat);

            // 2. Tạo Booking "thô" để lưu thông tin hành khách nếu được cung cấp
            if (!string.IsNullOrEmpty(customerName) || !string.IsNullOrEmpty(phoneNumber))
            {
                var trip = await _tripRepository.GetByIdAsync(tripSeat.TripId);
                
                var booking = new Bookings
                {
                    CustomerName = customerName ?? "Khách lẻ",
                    CustomerPhone = phoneNumber ?? "N/A",
                    BookingDate = DateTime.Now,
                    TotalPrice = trip?.TicketPrice ?? 0,
                    Status = status == 1 ? BookingStatus.Pending : BookingStatus.Confirmed,
                    AdminNote = "Đặt vé nhanh từ Admin"
                };
                await _bookingRepository.AddAsync(booking);

                var ticket = new Tickets
                {
                    BookingId = booking.BookingId,
                    TripSeatId = tripSeatId,
                    Price = trip?.TicketPrice ?? 0
                };
                await _ticketRepository.AddAsync(ticket);
            }

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
                        var depTime = date.Date.Add(schedule.DepartureTime);
                        var arrTime = date.Date.Add(schedule.ArrivalTime);
                        if (schedule.ArrivalTime < schedule.DepartureTime) arrTime = arrTime.AddDays(1);

                        var isOccupied = await _tripRepository.IsBusOccupiedAsync(schedule.BusId, depTime, arrTime);
                        if (isOccupied) continue;

                        if (schedule.DriverId.HasValue)
                        {
                            var isDriverOccupied = await _tripRepository.IsDriverOccupiedAsync(schedule.DriverId.Value, depTime, arrTime);
                            if (isDriverOccupied) continue;
                        }

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

            var depTime = departureDate.Date.Add(schedule.DepartureTime);
            var arrTime = departureDate.Date.Add(schedule.ArrivalTime);
            if (schedule.ArrivalTime < schedule.DepartureTime) arrTime = arrTime.AddDays(1);

            var isOccupied = await _tripRepository.IsBusOccupiedAsync(schedule.BusId, depTime, arrTime);
            if (isOccupied) return false;

            if (schedule.DriverId.HasValue)
            {
                var isDriverOccupied = await _tripRepository.IsDriverOccupiedAsync(schedule.DriverId.Value, depTime, arrTime);
                if (isDriverOccupied) return false;
            }

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

        public async Task<bool> AssignDriverAsync(int tripId, int driverId)
        {
            var trip = await _tripRepository.GetByIdAsync(tripId);
            if (trip == null) return false;

            // Check if driver is occupied
            var isOccupied = await _tripRepository.IsDriverOccupiedAsync(driverId, trip.DepartureTime, trip.ArrivalTime, tripId);
            if (isOccupied) return false;

            trip.DriverId = driverId;
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

            var tripIds = filteredTrips.Select(t => t.TripId).Distinct().ToList();
            var seatCounts = await _tripSeatRepository.GetSeatCountsForTripsAsync(tripIds);

            var result = new List<TripMonitoringDto>();
            foreach (var trip in filteredTrips)
            {
                seatCounts.TryGetValue(trip.TripId, out var counts);
                result.Add(MapToMonitoringDtoSync(trip, counts.total, counts.booked));
            }

            return result;
        }

        private TripMonitoringDto MapToMonitoringDtoSync(Trips trip, int totalSeats, int bookedSeats)
        {
            var busType = trip.Bus?.BusType?.TypeName ?? "N/A";

            return new TripMonitoringDto
            {
                TripId = trip.TripId,
                RouteId = trip.RouteId,
                RouteName = trip.Route?.RouteName,
                DepartureTime = trip.DepartureTime.ToString(@"HH\:mm"),
                DepartureDate = trip.DepartureTime.ToString("yyyy-MM-dd"),
                ArrivalTime = trip.ArrivalTime.ToString(@"HH\:mm"),
                BusId = trip.BusId,
                BusPlate = trip.Bus?.PlateNumber,
                BusType = busType,
                TotalSeats = totalSeats,
                AvailableSeats = totalSeats - bookedSeats,
                Status = (int)trip.Status,
                TicketPrice = trip.TicketPrice,
                DepartureOfficeName = trip.Route?.DepartureOffice?.OfficeName,
                ArrivalOfficeName = trip.Route?.ArrivalOffice?.OfficeName,
                DriverId = trip.DriverId,
                DriverName = trip.Driver?.User?.FullName
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
                DriverId = schedule.DriverId,
                TripSeats = new List<TripSeats>(),
                Status = TripStatus.Scheduled
            };

            // UPFRONT SEAT GENERATION: Prepare seats for "Real Booking"
            var allSeats = await _seatRepository.GetAllAsync();
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
            return newTrip;
        }

        private async Task EnsureTripSeatsExistAsync(int tripId)
        {
            var currentSeats = await _tripSeatRepository.GetSeatsByTripIdAsync(tripId);
            var trip = await _tripRepository.GetByIdAsync(tripId);
            
            if (trip == null || trip.BusId <= 0) return;

            var allSeats = await _seatRepository.GetAllAsync();
            var busSeats = allSeats.Where(s => s.BusId == trip.BusId && s.IsActive).ToList();

            // TRƯỜNG HỢP 1: Chưa có ghế nào -> Sinh mới
            if (!currentSeats.Any())
            {
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
            // TRƯỜNG HỢP 2: Đã có ghế nhưng số lượng hoặc cấu trúc sai (do lỗi logic cũ)
            // Chỉ thực hiện sửa lại nếu CHƯA CÓ AI ĐẶT GHẾ (để an toàn)
            else if (currentSeats.Count() != busSeats.Count && !currentSeats.Any(s => s.Status == SeatStatus.Booked))
            {
                // Xóa hết ghế cũ bị sai
                foreach (var oldSeat in currentSeats)
                {
                    await _tripSeatRepository.DeleteAsync(oldSeat);
                }

                // Sinh lại ghế theo đúng cấu trúc Bus đã chuẩn hóa
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
