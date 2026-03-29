using System;
using System.Collections.Generic;

namespace BookingTicket.Application.DTOs.Booking
{
    public class CreateBookingDto
    {
        public string? UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public List<int> TripSeatIds { get; set; } = new List<int>();
    }

    public class BookingDto
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public List<TicketDto> Tickets { get; set; } = new List<TicketDto>();
    }

    public class TicketDto
    {
        public int TicketId { get; set; }
        public int TripSeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
