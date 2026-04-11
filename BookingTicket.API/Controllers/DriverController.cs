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
        private readonly UserManager<ApplicationUser> _userManager;

        public DriverController(IDriverRepository driverRepository, UserManager<ApplicationUser> userManager)
        {
            _driverRepository = driverRepository;
            _userManager = userManager;
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
        public async Task<IActionResult> DeleteDriver(int id)
        {
            var driver = await _driverRepository.GetByIdAsync(id);
            if (driver == null) return NotFound();

            // Ở đây có thể chọn xóa User luôn hoặc chỉ xóa bản ghi Driver
            // Để an toàn, ta chỉ xóa Driver hoặc đánh dấu trạng thái
            await _driverRepository.DeleteAsync(driver);
            return NoContent();
        }

        [HttpGet("my-schedule")]
        [Authorize(Roles = "Driver")]
        public async Task<ActionResult<IEnumerable<object>>> GetMySchedule()
        {
            var userId = _userManager.GetUserId(User);
            var driver = await _driverRepository.GetByUserIdAsync(userId);
            if (driver == null) return NotFound("Không tìm thấy thông tin tài xế.");

            var trips = await _driverRepository.GetByIdWithDetailsAsync(driver.DriverId);
            // Ở đây mình trả về dữ liệu tối giản cho tài xế
            var result = trips.Trips.OrderBy(t => t.DepartureTime).Select(t => new
            {
                t.TripId,
                RouteName = t.Route?.RouteName ?? "N/A",
                DepartureTime = t.DepartureTime,
                BusPlateNumber = t.Bus?.PlateNumber ?? "N/A",
                Status = t.Status.ToString(),
                PassengerCount = t.Tickets.Count
            });

            return Ok(result);
        }

        [HttpGet("my-trips/{tripId}/passengers")]
        [Authorize(Roles = "Driver")]
        public async Task<ActionResult<IEnumerable<object>>> GetTripPassengers(int tripId)
        {
            // Kiểm tra xem trip này có đúng của tài xế này không (bảo mật)
            var userId = _userManager.GetUserId(User);
            var driver = await _driverRepository.GetByUserIdAsync(userId);
            
            // Tìm trip và bao gồm vé + thông tin khách
            // (Giả sử có ITripRepository hoặc dùng DbContext trực tiếp cho nhanh)
            var passengers = await _driverRepository.GetByIdWithDetailsAsync(driver.DriverId);
            var trip = passengers.Trips.FirstOrDefault(t => t.TripId == tripId);
            
            if (trip == null) return Forbidden();

            var result = trip.Tickets.Select(tk => new
            {
                SeatNumber = tk.TripSeat?.Seat?.SeatNumber ?? "N/A",
                CustomerName = tk.Booking?.CustomerName ?? "N/A",
                PhoneNumber = tk.Booking?.CustomerPhone ?? "N/A",
                Status = tk.Booking?.Status.ToString(),
                BookingId = tk.BookingId
            });

            return Ok(result);
        }

        private ActionResult Forbidden() => StatusCode(403, "Bạn không có quyền truy cập chuyến xe này.");
    }
}
