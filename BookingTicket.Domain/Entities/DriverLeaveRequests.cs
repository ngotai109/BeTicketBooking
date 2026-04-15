using BookingTicket.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingTicket.Domain.Entities
{
    public class DriverLeaveRequests
    {
        [Key]
        public int LeaveRequestId { get; set; }

        public int DriverId { get; set; }

        [ForeignKey("DriverId")]
        public virtual Drivers Driver { get; set; }

        public DateTime LeaveDate { get; set; }

        public LeaveType Type { get; set; }

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; }

        public LeaveRequestStatus Status { get; set; }

        [MaxLength(500)]
        public string? AdminNote { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
