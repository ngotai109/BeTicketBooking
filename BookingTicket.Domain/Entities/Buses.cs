using BookingTicket.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Domain.Entities
{
    public class Buses
    {
        [Key]
        public int BusId { set; get; }

        public string BusName { set; get; }

        public int TotalSeats { set; get; }

        public int BusTypeId { get; set; }
        public BusTypes BusType { get; set; }

        public string PlateNumber { set; get; }

        public BusStatus Status { get; set; } = BusStatus.Active;

        public ICollection<Seats> Seats { get; set; }
        public ICollection<Trips> Trips { get; set; }
    }
}
