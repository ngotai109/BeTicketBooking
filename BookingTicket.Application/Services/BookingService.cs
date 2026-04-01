using AutoMapper;
using BookingTicket.Application.DTOs.Booking;
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
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ITripSeatRepository _tripSeatRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
 
        public BookingService(
            IBookingRepository bookingRepository,
            ITicketRepository ticketRepository,
            ITripSeatRepository tripSeatRepository,
            ITripRepository tripRepository,
            IEmailService emailService,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _ticketRepository = ticketRepository;
            _tripSeatRepository = tripSeatRepository;
            _tripRepository = tripRepository;
            _emailService = emailService;
            _mapper = mapper;
        }


        public async Task<BookingDto?> CreateBookingAsync(CreateBookingDto request)
        {
            var tripSeats = new List<TripSeats>();
            decimal totalPrice = 0;

            // 1. Validate Seats
            foreach (var seatId in request.TripSeatIds)
            {
                var seat = await _tripSeatRepository.GetByIdWithDetailsAsync(seatId);
                if (seat == null || seat.Status != SeatStatus.Available)
                {
                    return null; // Seat is unavailable or already booked
                }
                tripSeats.Add(seat);
                
                // Fetch trip to get price
                var trip = await _tripRepository.GetByIdAsync(seat.TripId);
                if (trip != null) totalPrice += trip.TicketPrice;
            }

            // 2. Map booking data
            var booking = new Bookings
            {
                UserId = request.UserId,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                CustomerEmail = request.CustomerEmail,
                BookingDate = DateTime.Now,
                TotalPrice = totalPrice,
                Status = BookingStatus.Pending
            };

            await _bookingRepository.AddAsync(booking);

            // 3. Create Tickets and Update Seat Status
            foreach (var seat in tripSeats)
            {
                seat.Status = SeatStatus.Booked;
                await _tripSeatRepository.UpdateAsync(seat);

                var trip = await _tripRepository.GetByIdAsync(seat.TripId);
                
                var ticket = new Tickets
                {
                    BookingId = booking.BookingId,
                    TripSeatId = seat.TripSeatId,
                    Price = trip?.TicketPrice ?? 0
                };
                await _ticketRepository.AddAsync(ticket);
            }
            
            var result = await GetBookingByIdAsync(booking.BookingId);

            // 4. Send Email Confirmation
            if (result != null && !string.IsNullOrEmpty(booking.CustomerEmail))
            {
                // We use another Task to NOT block the checkout response
                _ = Task.Run(async () => {
                    try
                    {
                        var firstSeat = tripSeats.FirstOrDefault();
                        // Refresh trip data to get Route info if not loaded
                        var tripWithDetails = firstSeat != null ? await _tripRepository.GetByIdAsync(firstSeat.TripId) : null;
                        
                        string seats = string.Join(", ", result.Tickets.Select(t => t.SeatNumber));
                        string routeName = tripWithDetails?.Route?.RouteName ?? "Thông tin tuyến đang cập nhật";
                        string depTime = tripWithDetails != null 
                            ? $"{tripWithDetails.DepartureTime:HH:mm} ngày {tripWithDetails.DepartureTime:dd/MM/yyyy}" 
                            : "Đang cập nhật";

                        await _emailService.SendTicketConfirmationAsync(
                            booking.CustomerEmail,
                            booking.CustomerName,
                            $"DSL{booking.BookingId:D6}",
                            routeName,
                            depTime,
                            seats,
                            booking.TotalPrice
                        );
                    }

                    catch (Exception ex)
                    {
                        // Internal logging, won't affect HTTP response
                        Console.WriteLine("---- LỖI GỬI EMAIL: ----");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("-------------------------");
                    }
                });
            }

            return result;
        }

        public async Task<int> GetBookingCountByPhoneAsync(string phone)
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return bookings.Count(b => b.CustomerPhone == phone);
        }

        public async Task<BookingDto?> GetBookingByCodeAsync(string code, string phone)
        {
            if (string.IsNullOrEmpty(code) || !code.StartsWith("DSL")) return null;
            if (!int.TryParse(code.Substring(3), out int bookingId)) return null;
            
            var booking = await GetBookingByIdAsync(bookingId);
            if (booking == null) return null;
            if (booking.CustomerPhone != phone) return null;
            
            return booking;
        }

        public async Task<IEnumerable<BookingDto>> GetUserBookingsAsync(string userId)
        {
            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);
            return bookings.Select(b => new BookingDto 
            {
                BookingId = b.BookingId,
                BookingDate = b.BookingDate,
                TotalPrice = b.TotalPrice,
                Status = (int)b.Status,
                CustomerName = b.CustomerName,
                CustomerPhone = b.CustomerPhone,
                UserId = b.UserId
            });
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return bookings.OrderByDescending(b => b.BookingDate).Select(b => new BookingDto
            {
                BookingId = b.BookingId,
                BookingDate = b.BookingDate,
                TotalPrice = b.TotalPrice,
                Status = (int)b.Status,
                UserId = b.UserId,
                UserName = b.User?.FullName ?? "N/A"
            });
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, int status)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return false;

            booking.Status = (BookingStatus)status;
            await _bookingRepository.UpdateAsync(booking);
            return true;
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);
            if (booking == null) return null;

            var firstTicket = booking.Tickets?.FirstOrDefault();
            string routeName = "N/A";
            string depTime = "N/A";
            
            if (firstTicket != null)
            {
                var trip = await _tripRepository.GetByIdAsync(firstTicket.TripSeat.TripId);
                if (trip != null)
                {
                    routeName = trip.Route?.RouteName ?? "N/A";
                    depTime = $"{trip.DepartureTime:HH:mm} ngày {trip.DepartureTime:dd/MM/yyyy}";
                }
            }

            return new BookingDto
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                CustomerName = booking.CustomerName,
                CustomerPhone = booking.CustomerPhone,
                CustomerEmail = booking.CustomerEmail,
                BookingDate = booking.BookingDate,
                TotalPrice = booking.TotalPrice,
                Status = (int)booking.Status,
                UserName = booking.User?.FullName,
                RouteName = routeName,
                DepartureTime = depTime,
                Tickets = booking.Tickets?.Select(t => new TicketDto
                {
                    TicketId = t.TicketId,
                    TripSeatId = t.TripSeatId,
                    SeatNumber = t.TripSeat?.Seat?.SeatNumber ?? "N/A",
                    Price = t.Price
                }).ToList() ?? new List<TicketDto>()
            };
        }

        public async Task<IEnumerable<PassengerStatisticDto>> GetPassengersStatisticAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            var passengers = bookings
                .Where(b => !string.IsNullOrEmpty(b.CustomerPhone))
                .GroupBy(b => b.CustomerPhone)
                .Select(g => new PassengerStatisticDto
                {
                    Id = g.Key,
                    PhoneNumber = g.Key,
                    Email = g.OrderByDescending(x => x.BookingDate).FirstOrDefault()?.CustomerEmail,
                    FullName = g.OrderByDescending(x => x.BookingDate).FirstOrDefault()?.CustomerName ?? "Khách hàng",
                    TotalBookings = g.Count(),
                    TotalSpent = g.Where(x => x.Status != BookingStatus.Cancelled).Sum(x => x.TotalPrice),
                    LastBooking = g.Max(x => x.BookingDate).ToString("yyyy-MM-dd"),
                    Status = "Active"
                })
                .OrderByDescending(p => p.LastBooking)
                .ToList();

            return passengers;
        }
    }
}
