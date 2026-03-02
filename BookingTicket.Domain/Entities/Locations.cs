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
        [Required]
        public byte Level { get; set;}
        [Required]
        public int? ParentLocationId { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public ICollection<Routes> DepartureRoutes { get; set; }
        public ICollection<Routes> ArrivalRoutes { get; set; }
    }
}
