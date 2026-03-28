using BookingTicket.Application.DTOs.User;
using BookingTicket.Application.Interfaces.IRepositories;
using BookingTicket.Application.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingTicket.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBookingRepository _bookingRepository;

        public UserService(
            IUserRepository userRepository,
            IBookingRepository bookingRepository)
        {
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<IEnumerable<PassengerDto>> GetAllPassengersAsync()
        {
            var passengers = await _userRepository.GetUsersInRoleAsync("User");
            var result = new List<PassengerDto>();

            foreach (var user in passengers)
            {
                var userBookings = (await _bookingRepository.GetBookingsByUserIdAsync(user.Id)).ToList();

                result.Add(new PassengerDto
                {
                    Id = user.Id,
                    FullName = user.FullName ?? user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    TotalBookings = userBookings.Count,
                    TotalSpent = userBookings.Sum(b => b.TotalPrice),
                    LastBooking = userBookings.OrderByDescending(b => b.BookingDate).FirstOrDefault()?.BookingDate,
                    Status = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.Now ? "Locked" : "Active"
                });
            }

            return result;
        }

        public async Task<PassengerDto> GetPassengerByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            var userBookings = (await _bookingRepository.GetBookingsByUserIdAsync(user.Id)).ToList();

            return new PassengerDto
            {
                Id = user.Id,
                FullName = user.FullName ?? user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                TotalBookings = userBookings.Count,
                TotalSpent = userBookings.Sum(b => b.TotalPrice),
                LastBooking = userBookings.OrderByDescending(b => b.BookingDate).FirstOrDefault()?.BookingDate,
                Status = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.Now ? "Locked" : "Active"
            };
        }

        public async Task<IEnumerable<PassengerHistoryDto>> GetPassengerHistoryAsync(string id)
        {
            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(id);
            
            return bookings.Select(b => new PassengerHistoryDto
            {
                BookingId = b.BookingId,
                BookingDate = b.BookingDate,
                Amount = b.TotalPrice,
                Status = b.Status.ToString(),
                TicketCode = $"LH-{b.BookingId:D5}",
                RouteName = "Thông tin chuyến đi"
            }).ToList();
        }

        public async Task<bool> ToggleLockStatusAsync(string id)
        {
            return await _userRepository.ToggleLockStatusAsync(id);
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            return await _userRepository.DeleteAsync(id);
        }
    }
}
