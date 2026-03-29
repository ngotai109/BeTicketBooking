using System;

namespace BookingTicket.Application.DTOs.Trip
{
    public class AutoGenerateTripDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class CreateTripInputDto
    {
        public int ScheduleId { get; set; }
        public DateTime DepartureDate { get; set; }
    }
}
