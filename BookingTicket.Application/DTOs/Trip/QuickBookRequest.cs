namespace BookingTicket.Application.DTOs.Trip
{
    public class QuickBookRequest
    {
        public int TripSeatId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int Status { get; set; }
    }
}
