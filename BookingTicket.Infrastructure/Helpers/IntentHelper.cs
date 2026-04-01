using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingTicket.Infrastructure.Helpers
{
    public static class IntentHelper
    {
        public static string DetectIntent(string message)
        {
            if (string.IsNullOrEmpty(message))
                return "out_of_scope";

            message = message.ToLower();

            if (message.Contains("d?t vé") || message.Contains("mua vé"))
                return "book_ticket";

            if (message.Contains("di") || message.Contains("chuy?n"))
                return "search_trip";

            if (message.Contains("van phòng") || message.Contains("d?a ch?"))
                return "office_info";

            if (message.Contains("h?y"))
                return "cancel_ticket";

            if (message.Contains("chào") || message.Contains("hello"))
                return "small_talk";

            return "out_of_scope";
        }
    }
}
