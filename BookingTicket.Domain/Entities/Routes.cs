using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingTicket.Domain.Entities
{
    public class Routes
    {
        [Key]
        public int RouteId { get; set; }
        [Required]
        [MaxLength(200)]
        public string RouteName { get; set; }
        public int DepartureLocationId { get; set; }
        public int ArrivalLocationId { get; set; }
        [Precision(18,2)]
        public decimal BasePrice { set; get; }
        [Required]
        [Precision(10,2)]
        public decimal DistanceKm { set; get; }
        [Required]
        public int EstimatedTimeHours { set; get; }
        public bool IsActive { set; get; }
        [ForeignKey("DepartureLocationId")]
        public Locations DepartureLocation { get; set; }

        [ForeignKey("ArrivalLocationId")]
        public Locations ArrivalLocation { get; set; }

        public ICollection<Trips> Trips { get; set; } = new List<Trips>();
    }
}
