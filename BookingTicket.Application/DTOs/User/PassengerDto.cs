using System;

namespace BookingTicket.Application.DTOs.User
{
    public class PassengerDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastBooking { get; set; }
        public string Status { get; set; } // "Active", "Locked"
    }

    public class PassengerHistoryDto
    {
        public int BookingId { get; set; }
        public string TicketCode { get; set; }
        public DateTime BookingDate { get; set; }
        public string RouteName { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }
}
