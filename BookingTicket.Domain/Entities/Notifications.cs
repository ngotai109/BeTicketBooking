using System;

namespace BookingTicket.Domain.Entities
{
    public class Notifications
    {
        public int NotificationId { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ApplicationUser User { get; set; }
    }
}
