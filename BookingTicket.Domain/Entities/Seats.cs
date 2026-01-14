using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Domain.Entities
{
    public class Seats
    {
        [Key]
        public int SeatId { set; get; }
        
        [Required]
        [MaxLength(10)]
        public string SeatNumber { set; get; }

        public int BusId { get; set; }
        public Buses Bus { get; set; }

        public ICollection<TripSeats> TripSeats { get; set; }
        public ICollection<Tickets> Tickets { get; set; }
    }
}
