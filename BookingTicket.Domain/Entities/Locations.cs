using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingTicket.Domain.Entities
{
    public class Locations
    {
        [Key]
        public int LocationId { get; set; }

        [Required]
        [MaxLength(100)]
        public string LocationName { get; set; }

        // Navigation Properties
        public ICollection<Routes> DepartureRoutes { get; set; }
        public ICollection<Routes> ArrivalRoutes { get; set; }
    }
}
