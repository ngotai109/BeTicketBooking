using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingTicket.Domain.Entities
{
    public class Bookings
    {
        [Key]
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.Now;
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending";

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<Tickets> Tickets { get; set; } = new List<Tickets>();
        public ICollection<Payments> Payments { get; set; } = new List<Payments>();
    }
}
