using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingTicket.Domain.Enums;

namespace BookingTicket.Domain.Entities
{
    public class TripSeats
    {
        [Key]
        public int TripSeatId { get; set; }

        public int TripId { get; set; }
        public Trips Trip { get; set; }

        public int SeatId { get; set; }
        public Seats Seat { get; set; }

        public SeatStatus Status { get; set; }
        
        public ICollection<Tickets> Tickets { get; set; }
    }
}
