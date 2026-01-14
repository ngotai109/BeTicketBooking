using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Domain.Entities
{
    public class PaymentMethods
    {
        [Key]
        public int PaymentMethodId { set; get; }

        public string Paymentype { set; get; }

        public ICollection<Payments> Payments { get; set; }
    }
}
