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
        private readonly IMapper _mapper;

        public BookingService(
            IBookingRepository bookingRepository,
            ITicketRepository ticketRepository,
            ITripSeatRepository tripSeatRepository,
            ITripRepository tripRepository,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _ticketRepository = ticketRepository;
            _tripSeatRepository = tripSeatRepository;
            _tripRepository = tripRepository;
            _mapper = mapper;
        }

        public async Task<BookingDto?> CreateBookingAsync(CreateBookingDto request)
        {
            var tripSeats = new List<TripSeats>();
            decimal totalPrice = 0;

            // 1. Validate Seats
            foreach (var seatId in request.TripSeatIds)
            {
                var seat = await _tripSeatRepository.GetByIdAsync(seatId);
                if (seat == null || seat.Status != SeatStatus.Available)
                {
                    return null; // One of the seats is not available anymore
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

            return await GetBookingByIdAsync(booking.BookingId);
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null) return null;

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
                Tickets = booking.Tickets?.Select(t => new TicketDto
                {
                    TicketId = t.TicketId,
                    TripSeatId = t.TripSeatId,
                    SeatNumber = t.TripSeat?.Seat?.SeatNumber ?? "N/A",
                    Price = t.Price
                }).ToList() ?? new List<TicketDto>()
            };
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

        public async Task<int> GetBookingCountByPhoneAsync(string phone)
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return bookings.Count(b => b.CustomerPhone == phone);
        }
    }
}
