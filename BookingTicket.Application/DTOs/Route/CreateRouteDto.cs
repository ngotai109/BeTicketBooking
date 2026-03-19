namespace BookingTicket.Application.DTOs.Route
{
    public class CreateRouteDto
    {
        public string RouteName { get; set; } = string.Empty;
        public int DepartureOfficeId { get; set; }
        public int ArrivalOfficeId { get; set; }
        public decimal BasePrice { get; set; }
        public decimal DistanceKm { get; set; }
        public int EstimatedTimeHours { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
