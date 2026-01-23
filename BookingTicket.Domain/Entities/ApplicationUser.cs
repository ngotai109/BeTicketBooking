using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace BookingTicket.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
   
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public ICollection<Bookings> Bookings { get; set; } = new List<Bookings>();
    }
}
