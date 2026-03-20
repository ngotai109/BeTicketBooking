using BookingTicket.Domain.Entities;
using BookingTicket.Domain.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

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

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
