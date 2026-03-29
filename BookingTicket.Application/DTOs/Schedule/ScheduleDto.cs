using System;

namespace BookingTicket.Application.DTOs.Schedule
{
    public class ScheduleDto
    {
        public int ScheduleId { get; set; }
        public int RouteId { get; set; }
        public string RouteName { get; set; }
        public int BusId { get; set; }
        public string BusPlate { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public decimal TicketPrice { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateScheduleDto
    {
        public int RouteId { get; set; }
        public int BusId { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public decimal TicketPrice { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
