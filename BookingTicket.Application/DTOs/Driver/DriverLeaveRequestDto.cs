using BookingTicket.Domain.Enums;
using System;

namespace BookingTicket.Application.DTOs.Driver
{
    public class DriverLeaveRequestDto
    {
        public int LeaveRequestId { get; set; }
        public int DriverId { get; set; }
        public string DriverName { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime LeaveDate { get; set; }
        public LeaveType Type { get; set; }
        public string Reason { get; set; }
        public LeaveRequestStatus Status { get; set; }
        public string AdminNote { get; set; }
        public int? TripId { get; set; }
        public string? TripInfo { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
