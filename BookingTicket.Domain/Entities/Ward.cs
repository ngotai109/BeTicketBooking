using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Domain.Entities
{
    public class Ward
    {
        [Key]
        public int WardId { get; set; }

        public string WardName { get; set; }

        public int ProvinceId { get; set; }

        public Provinces Province { get; set; }

        public ICollection<Office> Offices { get; set; } = new List<Office>();

        public bool IsActive { set; get;}
    }
}
