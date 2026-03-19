namespace BookingTicket.Application.DTOs.Route
{
    public class RouteDto
    {
        public int RouteId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public int DepartureOfficeId { get; set; }
        public string DepartureOfficeName { get; set; } = string.Empty;
        public int ArrivalOfficeId { get; set; }
        public string ArrivalOfficeName { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public decimal DistanceKm { get; set; }
        public int EstimatedTimeHours { get; set; }
        public bool IsActive { get; set; }
    }
}
