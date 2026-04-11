using BookingTicket.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingTicket.Domain.Entities
{
    public class Drivers
    {
        [Key]
        public int DriverId { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public string LicenseNumber { get; set; }

        public string? LicenseType { get; set; } // Ví dụ: Hạng E, Hạng D...

        public int ExperienceYears { get; set; }

        public DriverStatus Status { get; set; } = DriverStatus.Available;

        public DateTime JoinedDate { get; set; } = DateTime.Now;

        public ICollection<Trips> Trips { get; set; } = new List<Trips>();
    }
}
