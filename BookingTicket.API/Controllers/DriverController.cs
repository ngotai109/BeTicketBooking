using BookingTicket.Application.DTOs.Driver;
using BookingTicket.Domain.Entities;
using BookingTicket.Domain.Enums;
using BookingTicket.Domain.Interfaces.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingTicket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = "Admin")] // Mở khóa khi đã có xác thực
    public class DriverController : ControllerBase
    {
        private readonly IDriverRepository _driverRepository;
        private readonly ITripRepository _tripRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BookingTicket.Application.Interfaces.IServices.IDriverLeaveRequestService _leaveRequestService;

        public DriverController(IDriverRepository driverRepository, ITripRepository tripRepository, ITicketRepository ticketRepository, UserManager<ApplicationUser> userManager, BookingTicket.Application.Interfaces.IServices.IDriverLeaveRequestService leaveRequestService)
        {
            _driverRepository = driverRepository;
            _tripRepository = tripRepository;
            _ticketRepository = ticketRepository;
            _userManager = userManager;
            _leaveRequestService = leaveRequestService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DriverDto>>> GetDrivers()
        {
            var drivers = await _driverRepository.GetAllWithDetailsAsync();
            var dtos = drivers.Select(d => new DriverDto
            {
                DriverId = d.DriverId,
                UserId = d.UserId,
                FullName = d.User?.FullName ?? "N/A",
                Email = d.User?.Email ?? "N/A",
                AvatarUrl = d.User?.AvatarUrl,
                PhoneNumber = d.User?.PhoneNumber ?? "N/A",
                LicenseNumber = d.LicenseNumber,
                LicenseType = d.LicenseType,
                ExperienceYears = d.ExperienceYears,
                Status = d.Status,
                JoinedDate = d.JoinedDate
            });

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DriverDto>> GetDriver(int id)
        {
            var d = await _driverRepository.GetByIdWithDetailsAsync(id);
            if (d == null) return NotFound();

            var dto = new DriverDto
            {
                DriverId = d.DriverId,
                UserId = d.UserId,
                FullName = d.User?.FullName ?? "N/A",
                Email = d.User?.Email ?? "N/A",
                AvatarUrl = d.User?.AvatarUrl,
                PhoneNumber = d.User?.PhoneNumber ?? "N/A",
                LicenseNumber = d.LicenseNumber,
                LicenseType = d.LicenseType,
                ExperienceYears = d.ExperienceYears,
                Status = d.Status,
                JoinedDate = d.JoinedDate
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<DriverDto>> CreateDriver(CreateDriverDto createDto)
        {
            // 1. Kiểm tra email đã tồn tại chưa
            var existingUser = await _userManager.FindByEmailAsync(createDto.Email);
            if (existingUser != null) return BadRequest("Email đã được sử dụng.");

            // 2. Tạo User
            var user = new ApplicationUser
            {
                UserName = createDto.Email,
                Email = createDto.Email,
                FullName = createDto.FullName,
                AvatarUrl = createDto.AvatarUrl,
                PhoneNumber = createDto.PhoneNumber,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, createDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            // 3. Gán Role Driver
            await _userManager.AddToRoleAsync(user, "Driver");

            // 4. Tạo bản ghi Driver
            var driver = new Drivers
            {
                UserId = user.Id,
                LicenseNumber = createDto.LicenseNumber,
                LicenseType = createDto.LicenseType,
                ExperienceYears = createDto.ExperienceYears,
                Status = DriverStatus.Available,
                JoinedDate = DateTime.Now
            };

            await _driverRepository.AddAsync(driver);

            return CreatedAtAction(nameof(GetDriver), new { id = driver.DriverId }, createDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDriver(int id, UpdateDriverDto updateDto)
        {
            var driver = await _driverRepository.GetByIdWithDetailsAsync(id);
            if (driver == null) return NotFound();

            // Cập nhật thông tin User
            if (driver.User != null)
            {
                driver.User.FullName = updateDto.FullName;
                driver.User.AvatarUrl = updateDto.AvatarUrl;
                driver.User.PhoneNumber = updateDto.PhoneNumber;
                await _userManager.UpdateAsync(driver.User);
            }

            // Cập nhật thông tin Driver
            driver.LicenseNumber = updateDto.LicenseNumber;
            driver.LicenseType = updateDto.LicenseType;
            driver.ExperienceYears = updateDto.ExperienceYears;
            driver.Status = updateDto.Status;

            await _driverRepository.UpdateAsync(driver);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ToggleLockDriver(int id)
        {
            var driver = await _driverRepository.GetDriverWithUserAsync(id);
            if (driver == null) return NotFound();

            var user = driver.User;

            if (driver.Status == DriverStatus.Locked)
            {
                // Unlock
                driver.Status = DriverStatus.Available;
                if (user != null)
                {
                    await _userManager.SetLockoutEndDateAsync(user, null);
                }
            }
            else
            {
                // Lock
                driver.Status = DriverStatus.Locked;
                if (user != null)
                {
                    await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                }
            }

            await _driverRepository.UpdateAsync(driver);
            return Ok(new { message = driver.Status == DriverStatus.Locked ? "Đã khóa tài xế" : "Đã mở khóa tài xế", status = driver.Status });
        }

        [HttpGet("my-schedule")]
        [Authorize(Roles = "Driver,Admin")]
        public async Task<ActionResult<IEnumerable<object>>> GetMySchedule()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var driver = await _driverRepository.GetByUserIdAsync(userId);
            if (driver == null) return NotFound(new { message = "Không tìm thấy hồ sơ tài xế cho tài khoản này." });

            var trips = await _tripRepository.GetTripsByDriverIdAsync(driver.DriverId);

            // Trả về danh sách chuyến xe được gán cho tài xế này
            var result = trips?
                .OrderBy(t => t.DepartureTime)
                .Select(t => new
                {
                    t.TripId,
                    RouteName = t.Route?.RouteName ?? "N/A",
                    DepartureTime = t.DepartureTime,
                    ArrivalTime = t.ArrivalTime,
                    BusPlateNumber = t.Bus?.PlateNumber ?? "N/A",
                    Status = t.Status.ToString(),
                    PassengerCount = t.TripSeats?.Count(ts => ts.Status == SeatStatus.Booked) ?? 0
                }) ?? Enumerable.Empty<object>();

            return Ok(result);
        }

        [HttpGet("my-trips/{tripId}/passengers")]
        [Authorize(Roles = "Driver,Admin")]
        public async Task<ActionResult<IEnumerable<object>>> GetTripPassengers(int tripId)
        {
            var trip = await _tripRepository.GetTripWithPassengerDetailsAsync(tripId);

            if (trip == null) return NotFound(new { message = "Không tìm thấy chuyến xe." });
            
            // Nếu là Driver thì phải là chuyến của mình. Admin thì được xem hết.
            if (User.IsInRole("Driver") && !User.IsInRole("Admin"))
            {
                var userId = _userManager.GetUserId(User);
                var driver = await _driverRepository.GetByUserIdAsync(userId);
                if (driver == null || trip.DriverId != driver.DriverId) return Forbidden();
            }

            var result = trip.TripSeats?
                .Where(ts => ts.Status == SeatStatus.Booked || ts.Status == SeatStatus.Reserved)
                .Select(ts => {
                    var tk = ts.Tickets?.FirstOrDefault();
                    return new
                    {
                        SeatNumber = ts.Seat?.SeatNumber ?? "N/A",
                        CustomerName = tk?.Booking?.CustomerName ?? "Khách lẻ (Admin đặt)",
                        PhoneNumber = tk?.Booking?.CustomerPhone ?? "N/A",
                        Status = tk?.Status.ToString() ?? "Booked",
                        BookingId = tk?.BookingId,
                        TicketId = tk?.TicketId,
                        IsBoarded = tk?.IsBoarded ?? false,
                        IsDroppedOff = tk?.IsDroppedOff ?? false
                    };
                }) ?? Enumerable.Empty<object>();

            return Ok(result);
        }

 
        [HttpPatch("tickets/{ticketId}/toggle-board")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> ToggleBoard(int ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null) return NotFound(new { message = "Không tìm thấy vé." });
            
            ticket.IsBoarded = !ticket.IsBoarded;
            await _ticketRepository.UpdateAsync(ticket);
            
            return Ok(new { isBoarded = ticket.IsBoarded });
        }
 
        [HttpPatch("tickets/{ticketId}/toggle-dropoff")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> ToggleDropOff(int ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null) return NotFound(new { message = "Không tìm thấy vé." });
            
            ticket.IsDroppedOff = !ticket.IsDroppedOff;
            await _ticketRepository.UpdateAsync(ticket);
            
            return Ok(new { isDroppedOff = ticket.IsDroppedOff });
        }

        [HttpPost("tickets/{ticketId}/request-mid-trip-dropoff")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> RequestMidTripDropOff(int ticketId, [FromBody] MidTripDropOffRequestDto request)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null) return NotFound(new { message = "Không tìm thấy vé." });

            if (!ticket.IsBoarded) return BadRequest(new { message = "Khách chưa lên xe, không thể cho xuống." });

            ticket.Status = TicketStatus.WaittingDropOffConfirm;
            ticket.ActualDropOffLocation = request.ActualDropOffLocation;
            ticket.ActualDropOffTime = DateTime.Now;

            await _ticketRepository.UpdateAsync(ticket);

            // TODO: Gửi Email cho admin hoặc người dùng ở đây
            // var _emailService = ...

            return Ok(new { message = "Đã gửi yêu cầu xác nhận xuống xe.", status = ticket.Status.ToString() });
        }
        [HttpPost("leave-requests")]
        [Authorize(Roles = "Driver,Admin")]
        public async Task<IActionResult> SubmitLeaveRequest([FromBody] CreateLeaveRequestDto request)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try {
                var result = await _leaveRequestService.SubmitLeaveRequestAsync(userId, request);
                return Ok(result);
            } catch (Exception ex) {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("leave-requests")]
        [Authorize(Roles = "Driver,Admin")]
        public async Task<IActionResult> GetMyLeaveRequests()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _leaveRequestService.GetMyLeaveRequestsAsync(userId);
            return Ok(result);
        }

        [HttpGet("all-leave-requests")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllLeaveRequests()
        {
            var result = await _leaveRequestService.GetAllLeaveRequestsAsync();
            return Ok(result);
        }

        [HttpPost("leave-requests/{requestId}/process")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProcessLeaveRequest(int requestId, [FromBody] ProcessLeaveRequestDto processDto)
        {
            var success = await _leaveRequestService.ProcessLeaveRequestAsync(requestId, processDto);
            if (!success) return BadRequest(new { message = "Không thể xử lý yêu cầu. Yêu cầu có thể không tồn tại hoặc đã được xử lý." });
            return Ok(new { message = "Xử lý thành công." });
        }
 
        private ActionResult Forbidden() => StatusCode(403, "Bạn không có quyền truy cập chuyến xe này.");
    }
}
