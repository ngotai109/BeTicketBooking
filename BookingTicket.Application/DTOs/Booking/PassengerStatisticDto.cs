using System;

namespace BookingTicket.Application.DTOs.Booking
{
    public class PassengerStatisticDto
    {
        public string Id { get; set; } = string.Empty; 
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int TotalBookings { get; set; }
        public int TotalTickets { get; set; }
        public decimal TotalSpent { get; set; }
        public string? LastBooking { get; set; }
        public string Status { get; set; } = "Active";
    }
}
