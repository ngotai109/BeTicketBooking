using BookingTicket.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingTicket.Domain.Entities
{
    public class Trips
    {
        [Key]
        public int TripId { get; set; }
        public int ScheduleId { get; set; }
        public Schedules Schedule { get; set; }
        public decimal TicketPrice { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public TripStatus Status { get; set; } = TripStatus.Scheduled;
        public int BusId { get; set; }
        public Buses Bus { get; set; }
        public int RouteId { get; set; }
        public Routes Route { get; set; }
        public int? DriverId { get; set; }
        public Drivers? Driver { get; set; }
        public ICollection<TripSeats> TripSeats { get; set; } = new List<TripSeats>();
        public ICollection<Tickets> Tickets { get; set; } = new List<Tickets>();

    }
}
