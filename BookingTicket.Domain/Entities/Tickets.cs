using BookingTicket.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace BookingTicket.Domain.Entities
{
    public class Tickets
    {
        [Key]
        public int TicketId { get; set; }

        public int TripSeatId { get; set; }
        public TripSeats TripSeat { get; set; }

        public int BookingId { get; set; }
        public Bookings Booking { get; set; }

        public decimal Price { get; set; }

        public TicketStatus Status { get; set; } = TicketStatus.Booked;
        
        public bool IsBoarded { get; set; } = false; // Tài xế xác nhận khách đã lên xe
        public bool IsDroppedOff { get; set; } = false; // Tài xế xác nhận khách đã xuống xe

        public string? ActualDropOffLocation { get; set; } // Điểm xuống xe giữa dọc đường (do tài xế nhập)
        public DateTime? ActualDropOffTime { get; set; } // Thời gian khách xuống xe giữa dọc đường

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
