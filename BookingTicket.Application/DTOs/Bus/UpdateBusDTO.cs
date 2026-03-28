using BookingTicket.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Application.DTOs.Bus
{
    public class UpdateBusDTO
    {
        public int BusId { set; get; }

        public string BusName { set; get; }

        public int TotalSeats { set; get; }

        public int BusTypeId { set; get; }

        public string PlateNumber { set; get; }

        public BusStatus Status { get; set; } 
    }
}
