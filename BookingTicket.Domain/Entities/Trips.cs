using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Domain.Entities
{
    public class Trips
    {
       [Key]
       public int TripId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        public int BusId { get; set; }
        public Buses Bus { get; set; }

        public int RouteId { get; set; }
        public Routes Route { get; set; }

        public ICollection<TripSeats> TripSeats { get; set; }
        public ICollection<Tickets> Tickets { get; set; }
    }
}
