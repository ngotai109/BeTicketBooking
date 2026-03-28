namespace BookingTicket.Application.DTOs.Bus
{
    public class BusTypeDto
    {
        public int BusTypeId { get; set; }
        public string TypeName { get; set; }
        public int DefaultSeats { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateBusTypeDto
    {
        public string TypeName { get; set; }
        public int DefaultSeats { get; set; }
        public string Description { get; set; }
    }
}
