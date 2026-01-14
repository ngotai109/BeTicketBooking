using BookingTicket.Domain.Entities;
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

    public string Status { get; set; } = "Booked";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
