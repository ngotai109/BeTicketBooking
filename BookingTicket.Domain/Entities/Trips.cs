using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingTicket.Domain.Entities
{
    public class Trips
    {
        [Key]
        public int TripId { get; set; }

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        /// <summary>Scheduled | InProgress | Completed | Cancelled</summary>
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Scheduled";

        public int BusId { get; set; }
        public Buses Bus { get; set; }

        public int RouteId { get; set; }
        public Routes Route { get; set; }

        public ICollection<TripSeats> TripSeats { get; set; } = new List<TripSeats>();
        public ICollection<Tickets> Tickets { get; set; } = new List<Tickets>();
    }
}
