using BookingTicket.Domain.Enums;
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
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public bool IsReminderSent { get; set; } = false;

        // Thông tin khách hàng (Dành cho khách vãng lai)
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }

        public string? UserId { get; set; } // Cho phép null nếu không đăng nhập
        public ApplicationUser? User { get; set; }

        public ICollection<Tickets> Tickets { get; set; } = new List<Tickets>();
        public ICollection<Payments> Payments { get; set; } = new List<Payments>();
    }
}
