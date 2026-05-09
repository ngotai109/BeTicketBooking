namespace BookingTicket.Domain.Projections
{
    public class DriverLookupProjection
    {
        public int DriverId { get; set; }
        public string FullName { get; set; } = string.Empty;
    }

    public class ScheduleProjection
    {
        public int ScheduleId { get; set; }
        public int RouteId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public int BusId { get; set; }
        public string BusPlate { get; set; } = string.Empty;
        public string DepartureTime { get; set; } = string.Empty;
        public string ArrivalTime { get; set; } = string.Empty;
        public decimal TicketPrice { get; set; }
        public bool IsActive { get; set; }
        public int? DriverId { get; set; }
        public string? DriverName { get; set; }
    }
}
