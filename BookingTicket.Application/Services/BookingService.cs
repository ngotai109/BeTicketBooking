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
                Status = BookingStatus.Pending // Bắt đầu ở trạng thái Chờ thanh toán (0)
            };

            await _bookingRepository.AddAsync(booking);

            // 3. Create Tickets and Update Seat Status
            foreach (var seat in tripSeats)
            {
                seat.Status = SeatStatus.Booked; // Vẫn giữ chỗ để tránh người khác đặt trùng trong khi chờ thanh toán
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

            // 4. Bỏ phần gửi Mail xác nhận ngay lập tức tại đây. 
            // Mail chỉ nên được gửi khi PaymentController nhận được Webhook thanh toán thành công từ PayOS/VNPay.

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
            var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);
            if (booking == null) return false;

            var oldStatus = booking.Status;
            booking.Status = (BookingStatus)status;
            await _bookingRepository.UpdateAsync(booking);

            // Giải phóng ghế nếu status là 2 (Cancelled)
            if (booking.Status == BookingStatus.Cancelled && oldStatus != BookingStatus.Cancelled)
            {
                foreach (var ticket in booking.Tickets)
                {
                    if (ticket.TripSeat != null)
                    {
                        ticket.TripSeat.Status = SeatStatus.Available;
                        await _tripSeatRepository.UpdateAsync(ticket.TripSeat);
                    }
                }
            }

            // Nếu chuyển sang trạng thái Confirmed (1) và trước đó chưa Confirmed
            if (booking.Status == BookingStatus.Confirmed && oldStatus != BookingStatus.Confirmed)
            {
                // Gửi mail xác nhận thành công (Background Task)
                _ = Task.Run(async () => {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        try
                        {
                            var scopedBookingRepo = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
                            var scopedTripRepo = scope.ServiceProvider.GetRequiredService<ITripRepository>();
                            var scopedEmailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                            // Lấy lại data full với context mới
                            var b = await scopedBookingRepo.GetByIdWithDetailsAsync(bookingId);
                            if (b == null || string.IsNullOrEmpty(b.CustomerEmail)) return;

                            var firstTicket = b.Tickets?.FirstOrDefault();
                            if (firstTicket?.TripSeat == null) return;

                            var trip = await scopedTripRepo.GetTripByIdWithDetailsAsync(firstTicket.TripSeat.TripId);
                            
                            string seats = string.Join(", ", b.Tickets.Select(t => t.TripSeat?.Seat?.SeatNumber ?? "N/A"));
                            string routeName = trip?.Route?.RouteName ?? "Đồng Hương Sông Lam";
                            string plateNumber = trip?.Bus?.PlateNumber ?? "N/A";
                            string depTime = trip != null 
                                ? $"{trip.DepartureTime:HH:mm} ngày {trip.DepartureTime:dd/MM/yyyy}" 
                                : "N/A";

                            await scopedEmailService.SendTicketConfirmationAsync(
                                b.CustomerEmail,
                                b.CustomerName,
                                $"DSL{b.BookingId:D6}",
                                routeName,
                                depTime,
                                seats,
                                b.TotalPrice,
                                plateNumber
                            );
                            Console.WriteLine($"[DEBUG] Đã gửi mail thành công sau thanh toán cho: {b.CustomerEmail}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[ERROR] Lỗi gửi mail sau thanh toán: {ex.Message}");
                        }
                    }
                });
            }

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
            var bookings = await _bookingRepository.GetAllWithDetailsAsync();
            var passengers = bookings
                .Where(b => !string.IsNullOrEmpty(b.CustomerPhone))
                .GroupBy(b => new { b.CustomerPhone, b.CustomerName })
                .Select(g => new PassengerStatisticDto
                {
                    Id = $"{g.Key.CustomerPhone}_{g.Key.CustomerName}",
                    PhoneNumber = g.Key.CustomerPhone,
                    Email = g.OrderByDescending(x => x.BookingDate).FirstOrDefault()?.CustomerEmail,
                    FullName = g.Key.CustomerName ?? "Khách hàng",
                    // Số lượng booking không phải đã hủy
                    TotalBookings = g.Count(x => x.Status != BookingStatus.Cancelled),
                    // Số lượng VÉ không phải đã hủy
                    TotalTickets = g.Where(x => x.Status != BookingStatus.Cancelled).Sum(x => x.Tickets?.Count ?? 0),
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

        public async Task<IEnumerable<object>> GetMidTripRequestsAsync()
        {
            var tickets = await _ticketRepository.GetAllAsync();
            var waitingTickets = tickets.Where(t => t.Status == TicketStatus.WaittingDropOffConfirm).ToList();

            var result = new List<object>();

            foreach (var t in waitingTickets)
            {
                var booking = await _bookingRepository.GetByIdWithDetailsAsync(t.BookingId);
                var seat = await _tripSeatRepository.GetByIdWithDetailsAsync(t.TripSeatId);
                var trip = seat != null ? await _tripRepository.GetTripByIdWithDetailsAsync(seat.TripId) : null;

                result.Add(new
                {
                    TicketId = t.TicketId,
                    BookingId = t.BookingId,
                    CustomerName = booking?.CustomerName ?? "N/A",
                    CustomerPhone = booking?.CustomerPhone ?? "N/A",
                    ActualDropOffLocation = t.ActualDropOffLocation ?? "Không rõ",
                    ActualDropOffTime = t.ActualDropOffTime,
                    RouteName = trip?.Route?.RouteName ?? "N/A",
                    SeatNumber = seat?.Seat?.SeatNumber ?? "N/A",
                    BusPlate = trip?.Bus?.PlateNumber ?? "N/A"
                });
            }

            return result;
        }

        public async Task<bool> ApproveMidTripRequestAndSendMailAsync(int ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null || ticket.Status != TicketStatus.WaittingDropOffConfirm) return false;

            var booking = await _bookingRepository.GetByIdWithDetailsAsync(ticket.BookingId);
            if (booking == null || string.IsNullOrEmpty(booking.CustomerEmail)) return false;

            var seat = await _tripSeatRepository.GetByIdWithDetailsAsync(ticket.TripSeatId);
            var trip = seat != null ? await _tripRepository.GetTripByIdWithDetailsAsync(seat.TripId) : null;

            string bookingCode = $"DSL{booking.BookingId:D6}";
            string approvalLink = $"http://localhost:3000/lookup/result?code={bookingCode}&phone={booking.CustomerPhone}&action=confirmDropOff&ticketId={ticketId}";
            
            await _emailService.SendMidTripDropOffConfirmationAsync(
                booking.CustomerEmail,
                booking.CustomerName ?? "Quý khách",
                bookingCode,
                trip?.Route?.RouteName ?? "Tuyến chưa xác định",
                ticket.ActualDropOffLocation ?? "Không rõ",
                approvalLink
            );

            // Tạm thời vẫn giữ status = WaittingDropOffConfirm, vì chờ khách click link
            // Nhưng có thể cần thêm field EmailSent.
            return true;
        }

        public async Task<bool> PassengerConfirmDropOffAsync(int ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null || ticket.Status != TicketStatus.WaittingDropOffConfirm) return false;

            // Khách xác nhận
            ticket.IsDroppedOff = true;
            ticket.Status = TicketStatus.Booked; // Hoặc nếu bạn muốn đánh dấu đã hoàn thành

            await _ticketRepository.UpdateAsync(ticket);
            return true;
        }
    }
}
