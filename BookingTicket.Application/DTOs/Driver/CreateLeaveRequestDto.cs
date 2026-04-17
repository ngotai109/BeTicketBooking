using BookingTicket.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace BookingTicket.Application.DTOs.Driver
{
    public class CreateLeaveRequestDto
    {
        [Required]
        public DateTime LeaveDate { get; set; }

        [Required]
        public LeaveType Type { get; set; }

        [MaxLength(500)]
        public string Reason { get; set; }

        public int? TripId { get; set; }
    }

    public class ProcessLeaveRequestDto
    {
        public int Status { get; set; }
        public string AdminNote { get; set; }
    }
}
