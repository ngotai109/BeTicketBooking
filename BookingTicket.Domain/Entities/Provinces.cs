using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingTicket.Domain.Entities
{
    public class Provinces
    {
        [Key]
        public int ProvinceId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProvinceName { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Ward> Wards { get; set; } = new List<Ward>();
    }
}
