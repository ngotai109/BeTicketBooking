namespace BookingTicket.Application.DTOs.Office
{
    public class OfficeDto
    {
        public int OfficeId { get; set; }
        public string OfficeName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        
        public int WardId { get; set; }
        public string WardName { get; set; } = string.Empty;
        public string ProvinceName { get; set; } = string.Empty;
        
        public bool IsActive { get; set; }
    }

    public class CreateOfficeDto
    {
        public string OfficeName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int WardId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
