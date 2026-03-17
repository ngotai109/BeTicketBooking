using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingTicket.Domain.Entities
{
    public class Office
    {
        [Key]
        public int OfficeId { get; set; }

        [Required]
        [MaxLength(200)]
        public string OfficeName { get; set; }

        [Required]
        [MaxLength(500)]
        public string Address { get; set; }

        [Phone]
        [MaxLength(10)]
        public string PhoneNumber { get; set; }

        public int WardId { get; set; }
        public Ward Ward { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation ngược từ Office → Routes
        public ICollection<Routes> DepartureRoutes { get; set; } = new List<Routes>();
        public ICollection<Routes> ArrivalRoutes { get; set; } = new List<Routes>();
    }
}
