using BookingTicket.Application.DTOs.Driver;
using BookingTicket.Application.Interfaces.IServices;
using BookingTicket.Domain.Entities;
using BookingTicket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingTicket.Application.Services
{
    public class DriverLeaveRequestService : IDriverLeaveRequestService
    {
        private readonly IApplicationDbContext _context;

        public DriverLeaveRequestService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DriverLeaveRequestDto> SubmitLeaveRequestAsync(string userId, CreateLeaveRequestDto requestDto)
        {
            var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == userId);
            if (driver == null) throw new Exception("Không tìm thấy thông tin tài xế.");

            var request = new DriverLeaveRequests
            {
                DriverId = driver.DriverId,
                LeaveDate = requestDto.LeaveDate,
                Type = requestDto.Type,
                Reason = requestDto.Reason,
                Status = LeaveRequestStatus.Pending,
                TripId = requestDto.TripId,
                CreatedAt = DateTime.Now
            };

            _context.DriverLeaveRequests.Add(request);
            await _context.SaveChangesAsync();

            return MapToDto(request, driver);
        }

        public async Task<IEnumerable<DriverLeaveRequestDto>> GetMyLeaveRequestsAsync(string userId)
        {
            var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == userId);
            if (driver == null) return new List<DriverLeaveRequestDto>();

            var requests = await _context.DriverLeaveRequests
                .Include(r => r.Driver)
                .ThenInclude(d => d.User)
                .Include(r => r.Trip)
                .ThenInclude(t => t.Route)
                .Where(r => r.DriverId == driver.DriverId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return requests.Select(r => MapToDto(r, r.Driver));
        }

        public async Task<IEnumerable<DriverLeaveRequestDto>> GetAllLeaveRequestsAsync()
        {
            var requests = await _context.DriverLeaveRequests
                .AsNoTracking()
                .Include(r => r.Driver)
                .ThenInclude(d => d.User)
                .Include(r => r.Trip)
                .ThenInclude(t => t.Route)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return requests.Select(r => MapToDto(r, r.Driver));
        }

        public async Task<bool> ProcessLeaveRequestAsync(int requestId, ProcessLeaveRequestDto processDto)
        {
            var request = await _context.DriverLeaveRequests.FindAsync(requestId);
            if (request == null || request.Status != LeaveRequestStatus.Pending) return false;

            request.Status = (LeaveRequestStatus)processDto.Status;
            request.AdminNote = processDto.AdminNote;

            // Optional future logic: If Approved, find Trips assigned to this driver on LeaveDate and unassign or notify Admin.
            if (request.Status == LeaveRequestStatus.Approved)
            {
                var trips = await _context.Trips
                    .Where(t => t.DriverId == request.DriverId && t.DepartureTime.Date == request.LeaveDate.Date)
                    .ToListAsync();
                
                foreach (var trip in trips)
                {
                    trip.DriverId = null; // Unassign driver
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetPendingLeaveRequestCountAsync()
        {
            return await _context.DriverLeaveRequests
                .CountAsync(r => r.Status == LeaveRequestStatus.Pending);
        }

        private DriverLeaveRequestDto MapToDto(DriverLeaveRequests request, Drivers driver)
        {
            return new DriverLeaveRequestDto
            {
                LeaveRequestId = request.LeaveRequestId,
                DriverId = request.DriverId,
                DriverName = driver?.User?.FullName ?? "Unknown",
                LicenseNumber = driver?.LicenseNumber ?? "",
                LeaveDate = request.LeaveDate,
                Type = request.Type,
                Reason = request.Reason,
                Status = request.Status,
                AdminNote = request.AdminNote,
                TripId = request.TripId,
                TripInfo = request.Trip != null ? $"[{request.Trip.DepartureTime:HH:mm}] {request.Trip.Route?.RouteName}" : null,
                CreatedAt = request.CreatedAt
            };
        }
    }
}
