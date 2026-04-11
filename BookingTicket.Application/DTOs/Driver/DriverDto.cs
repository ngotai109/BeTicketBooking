using BookingTicket.Domain.Enums;
using System;

namespace BookingTicket.Application.DTOs.Driver
{
    public class DriverDto
    {
        public int DriverId { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? AvatarUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string LicenseNumber { get; set; }
        public string? LicenseType { get; set; }
        public int ExperienceYears { get; set; }
        public DriverStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public DateTime JoinedDate { get; set; }
    }

    public class CreateDriverDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? AvatarUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string LicenseNumber { get; set; }
        public string? LicenseType { get; set; }
        public int ExperienceYears { get; set; }
        public string Password { get; set; } = "Driver@123";
    }

    public class UpdateDriverDto
    {
        public string FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string LicenseNumber { get; set; }
        public string? LicenseType { get; set; }
        public int ExperienceYears { get; set; }
        public DriverStatus Status { get; set; }
    }
}
