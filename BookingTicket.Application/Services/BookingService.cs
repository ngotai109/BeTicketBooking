using AutoMapper;
using BookingTicket.Application.DTOs.Booking;
using BookingTicket.Domain.Interfaces.IRepositories;
using BookingTicket.Application.Interfaces.IServices;
using BookingTicket.Domain.Entities;
using BookingTicket.Domain.Enums;
using System;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;
 
        public BookingService(
            IBookingRepository bookingRepository,
            ITicketRepository ticketRepository,
            ITripSeatRepository tripSeatRepository,
            ITripRepository tripRepository,
            IEmailService emailService,
            IServiceScopeFactory scopeFactory,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _ticketRepository = ticketRepository;
            _tripSeatRepository = tripSeatRepository;
            _tripRepository = tripRepository;
            _emailService = emailService;
            _scopeFactory = scopeFactory;
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
                Status = BookingStatus.Confirmed // Set to Confirmed to trigger reminder service
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
                Console.WriteLine($"[DEBUG] Đang chuẩn bị gửi mail xác nhận đến: {booking.CustomerEmail}");
                
                // Cần capture các thông tin cần thiết trước khi bắt đầu Task mới
                var bookingId = booking.BookingId;
                var customerEmail = booking.CustomerEmail;
                var customerName = booking.CustomerName;
                var totalPriceVal = booking.TotalPrice;
                var firstTripSeatId = tripSeats.FirstOrDefault()?.TripSeatId;

                _ = Task.Run(async () => {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        try
                        {
                            var scopedTripRepo = scope.ServiceProvider.GetRequiredService<ITripRepository>();
                            var scopedEmailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                            var scopedTripSeatRepo = scope.ServiceProvider.GetRequiredService<ITripSeatRepository>();

                            // Lấy thông tin chuyến đi từ DbContext mới
                            int tripId = 0;
                            if (firstTripSeatId.HasValue)
                            {
                                var seat = await scopedTripSeatRepo.GetByIdWithDetailsAsync(firstTripSeatId.Value);
                                if (seat != null) tripId = seat.TripId;
                            }

                            var tripWithDetails = tripId > 0 ? await scopedTripRepo.GetTripByIdWithDetailsAsync(tripId) : null;
                            
                            string seats = string.Join(", ", result.Tickets.Select(t => t.SeatNumber));
                            string routeName = tripWithDetails?.Route?.RouteName ?? "Đồng Hương Sông Lam";
                            string plateNumber = tripWithDetails.Bus.PlateNumber;
                            string depTime = tripWithDetails != null 
                                ? $"{tripWithDetails.DepartureTime:HH:mm} ngày {tripWithDetails.DepartureTime:dd/MM/yyyy}" 
                                : "Đang cập nhật";
                           
                            await scopedEmailService.SendTicketConfirmationAsync(
                                customerEmail,
                                customerName,
                                $"DSL{bookingId:D6}",
                                routeName,
                                depTime,
                                seats,
                                totalPriceVal,
                                plateNumber
                            );
                            Console.WriteLine($"[DEBUG] Đã gửi mail thành công đến: {customerEmail}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("---- LỖI GỬI EMAIL (Background Task): ----");
                            Console.WriteLine(ex.ToString());
                            Console.WriteLine("-------------------------------------------");
                        }
                    }
                });
            }
            else
            {
                Console.WriteLine("[DEBUG] Bỏ qua gửi mail: Booking null hoặc thiếu Email khách hàng.");
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
            return bookings.Select(b => {
                var firstTicket = b.Tickets?.FirstOrDefault();
                string routeName = "N/A";
                string depTime = "N/A";
                
                if (firstTicket?.TripSeat?.Trip != null)
                {
                    var trip = firstTicket.TripSeat.Trip;
                    routeName = trip.Route?.RouteName ?? "N/A";
                    depTime = $"{trip.DepartureTime:HH:mm} ngày {trip.DepartureTime:dd/MM/yyyy}";
                }

                return new BookingDto 
                {
                    BookingId = b.BookingId,
                    BookingDate = b.BookingDate,
                    TotalPrice = b.TotalPrice,
                    Status = (int)b.Status,
                    CustomerName = b.CustomerName,
                    CustomerPhone = b.CustomerPhone,
                    UserId = b.UserId,
                    RouteName = routeName,
                    DepartureTime = depTime
                };
            });
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllWithDetailsAsync();
            return bookings.OrderByDescending(b => b.BookingDate).Select(b => {
                var firstTicket = b.Tickets?.FirstOrDefault();
                string routeName = "N/A";
                string depTime = "N/A";
                
                if (firstTicket?.TripSeat?.Trip != null)
                {
                    var trip = firstTicket.TripSeat.Trip;
                    routeName = trip.Route?.RouteName ?? "N/A";
                    depTime = $"{trip.DepartureTime:HH:mm} ngày {trip.DepartureTime:dd/MM/yyyy}";
                }

                return new BookingDto
                {
                    BookingId = b.BookingId,
                    BookingDate = b.BookingDate,
                    TotalPrice = b.TotalPrice,
                    Status = (int)b.Status,
                    CustomerName = b.CustomerName,
                    CustomerPhone = b.CustomerPhone,
                    CustomerEmail = b.CustomerEmail,
                    UserId = b.UserId,
                    UserName = b.User?.FullName,
                    RouteName = routeName,
                    DepartureTime = depTime,
                    Tickets = b.Tickets?.Select(t => new TicketDto
                    {
                        TicketId = t.TicketId,
                        TripSeatId = t.TripSeatId,
                        SeatNumber = t.TripSeat?.Seat?.SeatNumber ?? "N/A",
                        Price = t.Price
                    }).ToList() ?? new List<TicketDto>(),
                    CancellationReason = b.CancellationReason,
                    AdminNote = b.AdminNote
                };
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
            
            if (firstTicket?.TripSeat?.Trip != null)
            {
                var trip = firstTicket.TripSeat.Trip;
                routeName = trip.Route?.RouteName ?? "N/A";
                depTime = $"{trip.DepartureTime:HH:mm} ngày {trip.DepartureTime:dd/MM/yyyy}";
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
                }).ToList() ?? new List<TicketDto>(),
                CancellationReason = booking.CancellationReason,
                AdminNote = booking.AdminNote
            };
        }

        public async Task<IEnumerable<PassengerStatisticDto>> GetPassengersStatisticAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            var passengers = bookings
                .Where(b => !string.IsNullOrEmpty(b.CustomerPhone))
                .GroupBy(b => new { b.CustomerPhone, b.CustomerName })
                .Select(g => new PassengerStatisticDto
                {
                    Id = $"{g.Key.CustomerPhone}_{g.Key.CustomerName}",
                    PhoneNumber = g.Key.CustomerPhone,
                    Email = g.OrderByDescending(x => x.BookingDate).FirstOrDefault()?.CustomerEmail,
                    FullName = g.Key.CustomerName ?? "Khách hàng",
                    // Chỉ đếm các vé không phải đã hủy (Status != 2)
                    TotalBookings = g.Count(x => x.Status != BookingStatus.Cancelled),
                    // Tổng tiền cũng chỉ tính vé không hủy
                    TotalSpent = g.Where(x => x.Status != BookingStatus.Cancelled).Sum(x => x.TotalPrice),
                    LastBooking = g.Max(x => x.BookingDate).ToString("yyyy-MM-dd"),
                    Status = "Active"
                })
                .OrderByDescending(p => p.TotalSpent)
                .ToList();

            return passengers;
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByPhoneAsync(string phone, string name)
        {
            var bookings = await _bookingRepository.GetAllWithDetailsAsync();
            return bookings
                .Where(b => b.CustomerPhone == phone && b.CustomerName == name)
                .OrderByDescending(b => b.BookingDate)
                .Select(b => {
                    var firstTicket = b.Tickets?.FirstOrDefault();
                    string routeName = "N/A";
                    string depTime = "N/A";
                    
                    if (firstTicket?.TripSeat?.Trip != null)
                    {
                        var trip = firstTicket.TripSeat.Trip;
                        routeName = trip.Route?.RouteName ?? "N/A";
                        depTime = $"{trip.DepartureTime:HH:mm} ngày {trip.DepartureTime:dd/MM/yyyy}";
                    }

                    return new BookingDto
                    {
                        BookingId = b.BookingId,
                        BookingDate = b.BookingDate,
                        TotalPrice = b.TotalPrice,
                        Status = (int)b.Status,
                        CustomerName = b.CustomerName,
                        CustomerPhone = b.CustomerPhone,
                        CustomerEmail = b.CustomerEmail,
                        UserId = b.UserId,
                        UserName = b.User?.FullName,
                        RouteName = routeName,
                        DepartureTime = depTime,
                        Tickets = b.Tickets?.Select(t => new TicketDto
                        {
                            TicketId = t.TicketId,
                            TripSeatId = t.TripSeatId,
                            SeatNumber = t.TripSeat?.Seat?.SeatNumber ?? "N/A",
                            Price = t.Price
                        }).ToList() ?? new List<TicketDto>(),
                        CancellationReason = b.CancellationReason,
                        AdminNote = b.AdminNote
                    };
                })
                .ToList();
        }

        public async Task<bool> RequestCancellationAsync(int bookingId, string reason)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null || booking.Status != BookingStatus.Confirmed) return false;

            booking.Status = BookingStatus.RequestedCancellation;
            booking.CancellationReason = reason;
            await _bookingRepository.UpdateAsync(booking);
            return true;
        }

        public async Task<bool> ProcessCancellationAsync(int bookingId, bool approve, string adminNote)
        {
            var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);
            if (booking == null || booking.Status != BookingStatus.RequestedCancellation) return false;

            if (approve)
            {
                booking.Status = BookingStatus.Cancelled;
                booking.AdminNote = adminNote;

                // Giải phóng các chỗ ngồi
                foreach (var ticket in booking.Tickets)
                {
                    if (ticket.TripSeat != null)
                    {
                        ticket.TripSeat.Status = SeatStatus.Available;
                        await _tripSeatRepository.UpdateAsync(ticket.TripSeat);
                    }
                }
            }
            else
            {
                booking.Status = BookingStatus.Confirmed; // Trả lại trạng thái Đã thanh toán nếu từ chối
                booking.AdminNote = adminNote;
            }

            await _bookingRepository.UpdateAsync(booking);

            // Gửi mail thông báo kết quả cho khách hàng
            if (!string.IsNullOrEmpty(booking.CustomerEmail))
            {
                try
                {
                    string bookingCode = $"DSL{booking.BookingId.ToString().PadLeft(6, '0')}";
                    await _emailService.SendCancellationNotificationAsync(
                        booking.CustomerEmail,
                        booking.CustomerName,
                        bookingCode,
                        approve,
                        adminNote);
                }
                catch (Exception ex)
                {
                    // Chỉ log lỗi mail, không làm hỏng quy trình xử lý vé
                    Console.WriteLine($"Mail Error: {ex.Message}");
                }
            }

            return true;
        }
    }
}
