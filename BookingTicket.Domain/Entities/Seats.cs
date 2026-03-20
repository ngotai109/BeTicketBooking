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
            public int SeatId { get; set; }

            [Required]
            [MaxLength(10)]
            public string SeatNumber { get; set; } // A1, B1, C1...
            [Required]
            public int Floor { get; set; } // 1: Tầng dưới, 2: Tầng trên

            [Required]
            public int Row { get; set; } // Vị trí hàng (0, 1, 2, 3...)

            [Required]
            public int Column { get; set; } // Vị trí cột (0, 1, 2...)

            public bool IsActive { get; set; } = true; // Trạng thái ghế (VD: ghế hỏng thì false)

            public int BusId { get; set; }
            public Buses Bus { get; set; }

            public ICollection<TripSeats> TripSeats { get; set; } = new List<TripSeats>();
        }

}
