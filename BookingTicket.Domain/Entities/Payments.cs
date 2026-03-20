using BookingTicket.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Domain.Entities
{
    public class Payments
    {
        [Key]
        public int PaymentId { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public int PaymentMethodId { get; set; }
        public PaymentMethods PaymentMethod { get; set; }

        public int BookingId { get; set; }
        public Bookings Booking { get; set; }
    }
}
