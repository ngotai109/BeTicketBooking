using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingTicket.Domain.Entities
{
    public class BusTypes
    {
        [Key]
        public int BusTypeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TypeName { get; set; } 

        public int DefaultSeats { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Buses> Buses { get; set; } = new List<Buses>();
    }
}
