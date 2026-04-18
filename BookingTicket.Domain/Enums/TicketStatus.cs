using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Domain.Enums
{
    public enum TicketStatus
    {
        Booked = 0,
        Cancelled = 1,
        WaittingDropOffConfirm = 2,
        MidTripEmailSent = 3,
        MidTripRejected = 4
    }
}
